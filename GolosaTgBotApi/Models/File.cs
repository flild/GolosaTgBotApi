using System.ComponentModel.DataAnnotations;

namespace GolosaTgBotApi.Models
{
    /// <summary>
    /// Таблица, которая хранит информацию о загруженных на сервер файлах.
    /// Поле FileId — идентификатор от Telegram (или иной), filePath — относительный путь на диске,
    /// CreatedAt — время первого сохранения, LastAccessedAt — время последнего запроса.
    /// </summary>
    public class DownloadedFile
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Идентификатор файла, который отдаёт Telegram API (например, "AgACAgUAAxkBAA...").
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        /// Относительный путь (от корня storagePath), где лежит webp-файл, например "af/15/12345.webp".
        /// Именно то, что возвращает ProcessAndSaveImageAsync.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Время первого сохранения (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Время последнего обращения (UTC). При первом сохранении его можно проинициализировать CreatedAt.
        /// </summary>
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Количество обращений (по желанию можно хранить статистику).
        /// </summary>
        public int AccessCount { get; set; } = 0;
    }
}