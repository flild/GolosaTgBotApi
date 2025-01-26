using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace GolosaTgBotApi.Services
{
    public class FileService
    {
        private const int QualityParam = 85;
        private readonly string _storagePath;

        public FileService(string storagePath)
        {
            _storagePath = storagePath;
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        /// <summary>
        /// Скачивает изображение, сжимает и сохраняет в формате WebP.
        /// </summary>
        /// <param name="fileId">ID файла (например, от Telegram API).</param>
        /// <param name="fileUrl">URL изображения для загрузки.</param>
        /// <returns>Путь сохранённого изображения (ID.webp).</returns>
        public async Task<string> ProcessAndSaveImageAsync(string fileId, string fileUrl)
        {
            using HttpClient client = new();
            var response = await client.GetAsync(fileUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Не удалось загрузить изображение: {response.StatusCode}");
            }

            await using var inputStream = await response.Content.ReadAsStreamAsync();
            using var image = new MagickImage(inputStream);
            // Сжатие изображения без потери качества
            image.Quality = QualityParam;

            // Генерация пути на основе MD5-хэша ID
            string md5Hash = GetMd5Hash(fileId);
            string relativePath = GeneratePathFromHash(md5Hash, fileId);
            string webpPath = Path.Combine(_storagePath, relativePath);

            // Создание директорий, если их нет
            Directory.CreateDirectory(Path.GetDirectoryName(webpPath)!);

            // Сохранение изображения в формате WebP
            image.Format = MagickFormat.WebP;
            await image.WriteAsync(webpPath);

            return relativePath;
        }

        /// <summary>
        /// Возвращает изображение в формате PNG или JPEG по ID файла.
        /// </summary>
        /// <param name="fileId">ID файла.</param>
        /// <returns>Поток изображения.</returns>
        public async Task<Stream> GetImageAsync(string fileId)
        {
            // Генерация пути на основе MD5-хэша ID
            string md5Hash = GetMd5Hash(fileId);
            string relativePath = GeneratePathFromHash(md5Hash, fileId);
            string webpPath = Path.Combine(_storagePath, relativePath);

            if (!File.Exists(webpPath))
            {
                throw new FileNotFoundException("Изображение не найдено.");
            }

            using var webpImage = new MagickImage(webpPath);
            webpImage.Format = MagickFormat.Jpeg;

            var outputStream = new MemoryStream();
            await webpImage.WriteAsync(outputStream);
            outputStream.Position = 0;

            return outputStream;
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