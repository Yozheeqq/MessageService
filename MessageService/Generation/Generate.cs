using MessageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Generation
{
    /// <summary>
    /// Класс дял генерации строк
    /// </summary>
    public class Generate
    {
        // Поле рандома
        private static Random _rnd = new Random();
        // Массив доступных символов
        private static char[] _letters =
        {
            'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm',
            'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'Z', 'X', 'C', 'V', 'B', 'N', 'M',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
        /// <summary>
        /// Генерация сообщений
        /// </summary>
        /// <param name="users">Список пользователей</param>
        /// <returns>Список сообщений</returns>
        public static List<Messages> GenerateMessages(List<User> users)
        {
            var messages = new Messages[_rnd.Next(3, 16)];
            for (int i = 0; i < messages.Length; i++)
            {
                string subject = GenerateName(11);
                string message = GenerateName(40);
                string sender = users[_rnd.Next(users.Count)].Email;
                string receiver = sender;
                while (receiver == sender)
                    receiver = users[_rnd.Next(users.Count)].Email;
                messages[i] = new Messages() { Message = message, Subject = subject, SenderId = sender, ReceiverId = receiver };
            }
            return messages.ToList();
        }
        /// <summary>
        /// Генерация пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        public static List<User> GenerateUsers()
        {
            var users = new User[_rnd.Next(3, 11)];
            for (int i = 0; i < users.Length; i++)
            {
                users[i] = new User() { Email = GenerateEmail(), UserName = GenerateName(10) };
            }
            return users.ToList();
        }

        /// <summary>
        /// Генерация эмайлов
        /// </summary>
        /// <returns>Эмайл</returns>
        private static string GenerateEmail()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _rnd.Next(4, 11); i++)
            {
                sb.Append(_letters[_rnd.Next(0, _letters.Length)]);
            }
            sb.Append("@");
            for (int i = 0; i < _rnd.Next(2, 6); i++)
            {
                sb.Append(_letters[_rnd.Next(0, 26)]);
            }
            sb.Append('.');
            for (int i = 0; i < _rnd.Next(2, 6); i++)
            {
                sb.Append(_letters[_rnd.Next(0, 26)]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// Генерация строки
        /// </summary>
        /// <param name="max">Максимальная длина строки</param>
        /// <returns>Сгенерированная строка</returns>
        private static string GenerateName(int max)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _rnd.Next(3, max); i++)
            {
                sb.Append(_letters[_rnd.Next(_letters.Length)]);
            }
            return sb.ToString();
        }
    }
}
