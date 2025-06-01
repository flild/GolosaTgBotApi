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

namespace GolosaTgBotApi.Services
{
    public class FileService
    {
        private const int QualityParam = 85;
        private readonly string _storagePath;
        private readonly IMariaService _mariaService;
        private readonly ITelegramService _telegram;

        public FileService(string storagePath, IMariaService mariaService, ITelegramService telegramService)
        {
            _storagePath = storagePath;
            _mariaService = mariaService;
            _telegram = telegramService;
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        /// <summary>
        /// Всегда возвращает Stream с изображением в формате JPEG:
        /// 1. Проверяет, есть ли файл в БД.
        /// 2. Если есть:
        ///    – читает WebP-изображение из хранилища, конвертирует его в JPEG,
        ///      обновляет в БД LastAccessedAt и AccessCount и возвращает поток.
        /// 3. Если нет:
        ///    – запрашивает файл у Telegram через GetFileById,
        ///      сжимает и сохраняет его как WebP в хранилище,
        ///      добавляет запись в таблицу FileRecords,
        ///      конвертирует тот же загруженный в памяти MagickImage в JPEG и возвращает поток.
        /// </summary>
        /// <param name="fileId">ID файла (например, из Telegram API).</param>
        /// <returns>Stream с изображением в формате JPEG.</returns>
        public async Task<Stream> GetOrDownloadAndGetImageStreamAsync(string fileId)
        {
            // 1. Проверяем, существует ли в БД запись о файле.
            var existingRecord = _mariaService.GetDownloadedFile(fileId);
            if (existingRecord != null)
            {
                // Файл уже есть в БД: existingRecord содержит относительный путь, например "af/15/12345.webp"
                string webpRelativePath = existingRecord;
                string webpFullPath = Path.Combine(_storagePath, webpRelativePath);

                if (!File.Exists(webpFullPath))
                {
                    // На всякий случай: если файл по указанному пути удалён/отсутствует,
                    // удалим запись из БД и пойдём вниз по алгоритму повторной загрузки.
                    _mariaService.DeleteFile(fileId);
                }
                else
                {
                    // 1.a. Обновляем статистику доступа:
                    var fileRecord = _mariaService.FindFileRecord(fileId);
                    fileRecord.LastAccessedAt = DateTime.UtcNow;
                    fileRecord.AccessCount += 1;
                    _mariaService.UpdateFile(fileRecord);

                    // 1.b. Открываем WebP и конвертируем в JPEG в MemoryStream:
                    using var webpImage = new MagickImage(webpFullPath);
                    webpImage.Format = MagickFormat.Jpeg;

                    var outputStream = new MemoryStream();
                    await webpImage.WriteAsync(outputStream);
                    outputStream.Position = 0;
                    return outputStream;
                }
            }

            // 2. Файл отсутствует в БД, или файл в хранилище отсутствует: загружаем из Telegram.
            // Получаем «сырую» картинку из Telegram:
            using var downloadedStream = await _telegram.GetFileById(fileId);
            downloadedStream.Position = 0;

            // 2.a. Загружаем изображение в MagickImage из скачанного потока:
            using var image = new MagickImage(downloadedStream);

            // 2.b. Сжимаем без потери качества (QualityParam – например, 85–90):
            image.Quality = QualityParam;

            // 2.c. Генерируем путь для сохранения WebP: MD5-хэш от fileId + вложенные папки:
            string md5Hash = GetMd5Hash(fileId);
            string relativePath = GeneratePathFromHash(md5Hash, fileId); // Например: "af/15/12345.webp"
            string webpFullSavePath = Path.Combine(_storagePath, relativePath);

            // 2.d. Убедимся, что директория существует:
            string? dir = Path.GetDirectoryName(webpFullSavePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // 2.e. Сохраняем изображение как WebP на диск:
            image.Format = MagickFormat.WebP;
            await image.WriteAsync(webpFullSavePath);

            // 2.f. Добавляем запись в БД:
            var newFileRecord = new DownloadedFile
            {
                FileId = fileId,
                FilePath = relativePath,
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                AccessCount = 1
            };
            _mariaService.AddFile(newFileRecord);

            // 2.g. Конвертируем тот же MagickImage (в памяти) в JPEG и возвращаем Stream:
            //    (для этого нужно сбросить формат обратно в JPEG)
            image.Format = MagickFormat.Jpeg;
            var jpegStream = new MemoryStream();
            await image.WriteAsync(jpegStream);
            jpegStream.Position = 0;
            return jpegStream;
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