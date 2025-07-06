
namespace GolosaTgBotApi.Services.FileService
{
    public interface IFileService
    {
        /// <summary>
        /// Возвращает URL JPEG-изображения:
        /// 1. Проверяет БД на существование записи.
        /// 2. При наличии: обновляет статистику, конвертирует WebP->JPEG (сохраняет файл) и возвращает ссылку.
        /// 3. При отсутствии: загружает из Telegram, сохраняет WebP, записывает в БД,
        ///    конвертирует в JPEG (сохраняет файл) и возвращает ссылку.
        /// </summary>
        /// <param name="fileId">ID файла (например, из Telegram API).</param>
        /// <returns>URL JPEG-изображения.</returns>
        Task<string> GetOrDownloadAndGetImageUrlAsync(string fileId);
    }
}