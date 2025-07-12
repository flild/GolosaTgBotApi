using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.TelegramService;
using ImageMagick;

namespace GolosaTgBotApi.Services.FileService
{
    public class FileService : IFileService
    {
        private const int QualityParam = 85;
        private readonly string _storagePath = "files";
        private readonly IMariaService _mariaService;
        private readonly ITelegramService _telegram;
        private readonly string _baseUrl;

        public FileService(IMariaService mariaService, ITelegramService telegramService)
        {
            _mariaService = mariaService;
            _telegram = telegramService;
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }


        public async Task<string> GetOrDownloadAndGetImageUrlAsync(string fileId)
        {
            // Генерируем относительные пути
            string md5Hash = GetMd5Hash(fileId);
            string webpRelPath = GeneratePathFromHash(md5Hash, fileId);
            string jpegRelPath = Path.ChangeExtension(webpRelPath, ".jpg");

            string webpFullPath = Path.Combine(_storagePath, webpRelPath);
            string jpegFullPath = Path.Combine(_storagePath, jpegRelPath);

            DownloadedFile? record = _mariaService.GetDownloadedFile(fileId);

            if (record != null && File.Exists(webpFullPath))
            {
                // Обновляем статистику
                record.LastAccessedAt = DateTime.UtcNow;
                record.AccessCount += 1;
                _mariaService.UpdateFile(record);

                // Если JPEG уже существует, сразу возвращаем ссылку
                if (File.Exists(jpegFullPath))
                    return BuildUrl(jpegRelPath);

                // Конвертируем WebP в JPEG
                using var img = new MagickImage(webpFullPath);
                img.Format = MagickFormat.Jpeg;
                img.Quality = QualityParam;
                EnsureDirectoryExists(jpegFullPath);
                await img.WriteAsync(jpegFullPath);

                return BuildUrl(jpegRelPath);
            }

            // Файл не найден или отсутствует в хранилище: загружаем из Telegram
            using var downloaded = await _telegram.GetFileById(fileId);
            downloaded.Position = 0;

            using var image = new MagickImage(downloaded);
            image.Quality = QualityParam;

            // Сохраняем WebP
            EnsureDirectoryExists(webpFullPath);
            image.Format = MagickFormat.WebP;
            await image.WriteAsync(webpFullPath);

            // Сохраняем запись в БД
            var newRecord = new DownloadedFile
            {
                FileId = fileId,
                FilePath = webpRelPath,
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                AccessCount = 1
            };
            _mariaService.AddFile(newRecord);

            // Конвертируем и сохраняем JPEG
            image.Format = MagickFormat.Jpeg;
            EnsureDirectoryExists(jpegFullPath);
            await image.WriteAsync(jpegFullPath);

            return BuildUrl(jpegRelPath);
        }

        private string BuildUrl(string relativePath)
        {
            // Преобразуем пути к URL (замена обратных слэшей)
            string urlPath = relativePath.Replace(Path.DirectorySeparatorChar, '/');
            return $"{_baseUrl}/{urlPath}";
        }
        private static void EnsureDirectoryExists(string fullPath)
        {
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        private static string GetMd5Hash(string input)
        {
            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        private static string GeneratePathFromHash(string hash, string fileId)
        {
            // Разбиение на подпапки по первым двум символам MD5
            return Path.Combine(hash[..2], hash[2..4], $"{fileId}.webp");
        }
    }
}