using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageService.Models
{
    /// <summary>
    /// Класс для сообщения
    /// </summary>
    public class Messages
    {
        /// <summary>
        /// Тема сообщения
        /// </summary>
        [Required]
        public string Subject { get; set; }
        /// <summary>
        /// Сообщение
        /// </summary>
        [Required]
        public string Message { get; set; }
        /// <summary>
        /// Отправитель
        /// </summary>
        [Required]
        public string SenderId { get; set; }
        /// <summary>
        /// Получатель
        /// </summary>
        [Required]
        public string ReceiverId { get; set; }
    }
}
