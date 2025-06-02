using GolosaTgBotApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GolosaTgBotApi.Services.FileService
{
    /// <summary>
    /// Фоновый сервис, который раз в нужный интервал проверяет таблицу FileRecords
    /// и удаляет те файлы, у которых LastAccessedAt старше 3 часов.
    /// </summary>
    public class FileCleanupService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FileCleanupService> _logger;
        private Timer? _timer;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(30); // раз в 30 минут можно проверять
        private readonly TimeSpan _maxAge = TimeSpan.FromHours(3);
        private readonly string _storagePath;

        public FileCleanupService(string storagePath, IServiceProvider serviceProvider, ILogger<FileCleanupService> logger)
        {
            _storagePath = storagePath;
            _serviceProvider = serviceProvider;
            _logger = logger;
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Запустим таймер: первый запуск - сразу, затем раз в _cleanupInterval
            _timer = new Timer(async _ => await DoWork(), null, TimeSpan.Zero, _cleanupInterval);
            _logger.LogInformation("FileCleanupService started, interval = {interval}", _cleanupInterval);
            return Task.CompletedTask;
        }

        private async Task DoWork()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<MariaContext>();

                DateTime threshold = DateTime.UtcNow - _maxAge;
                // Берём все записи, у которых LastAccessedAt < threshold
                var expiredRecords = await dbContext.FileRecords
                    .Where(fr => fr.LastAccessedAt < threshold)
                    .ToListAsync();

                if (!expiredRecords.Any())
                {
                    return;
                }

                foreach (var record in expiredRecords)
                {
                    string fullDiskPath = Path.Combine(_storagePath, record.FilePath);
                    try
                    {
                        if (File.Exists(fullDiskPath))
                        {
                            File.Delete(fullDiskPath);
                            _logger.LogInformation("Deleted file on disk: {path}", fullDiskPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при удалении файла {path}", fullDiskPath);
                    }

                    dbContext.FileRecords.Remove(record);
                }

                await dbContext.SaveChangesAsync();
                _logger.LogInformation("FileCleanupService удалил {count} записей из БД", expiredRecords.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в FileCleanupService.DoWork");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FileCleanupService is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}