using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageService.Models
{
    /// <summary>
    /// Класс для пользователя
    /// </summary>
    public class User
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// Эмайл пользователя
        /// </summary>
        [Required]
        public string Email { get; set; }
    }
}
