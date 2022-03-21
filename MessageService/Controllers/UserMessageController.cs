using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageService.Services;
using MessageService.Models;
using System.Text;
using MessageService.Generation;
using System.IO;
using System.Text.RegularExpressions;

namespace MessageService.Controllers
{
    /// <summary>
    /// Класс для отображения запросов
    /// </summary>
    [Route("[controller]")]
    public class UserMessageController : Controller
    {
        // Список пользователей
        private static List<User> users;
        // Список сообщений
        private static List<Messages> messages;
        /// <summary>
        /// Конструктор для инициализации полей и свойств
        /// </summary>
        /// <param name="message">Сервис сообщений</param>
        /// <param name="user">Сервис пользователей</param>
        public UserMessageController(JsonFileMessageService message, JsonFileUserService user)
        {
            UserService = user;
            MessageService = message;
            users = (List<User>)UserService.GetUsers();
            messages = (List<Messages>)MessageService.GetMessages();
        }
        /// <summary>
        /// Хранение пользовательского сервиса
        /// </summary>
        public JsonFileUserService UserService { get; }
        /// <summary>
        /// Хранение сервиса сообщений
        /// </summary>
        public JsonFileMessageService MessageService { get; }
        /// <summary>
        /// Рандомная генерация пользователей
        /// </summary>
        /// <response code="200">Генерация прошла успешно</response>
        /// <returns>Ответ сервера</returns>
        [HttpPost]
        public IActionResult SetUsers()
        {
            users = Generate.GenerateUsers();
            messages = Generate.GenerateMessages(users);
            UserService.SetUsers(users);
            MessageService.SetMessages(messages);
            return Ok();
        }
        /// <summary>
        /// Добавление пользователя в список
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="email">Эмайл пользователя</param>
        /// <response code="400">Неверные данные</response>
        /// <response code="200">Добавление прошло успешно</response>
        /// <returns>Ответ сервера</returns>
        [HttpPost("user/add")]
        public IActionResult AddUser(string userName, string email)
        {
            string pattern = @"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$";
            if (!Regex.IsMatch(email, pattern))
                return BadRequest("Емайл не соответсвует. Пример емайла: qwerty@asd.com");
            var user = users.SingleOrDefault(x => x.Email == email || x.UserName == userName);
            if (user != null)
                return BadRequest("Пользователь с таким именем или эмайлом уже существует");
            users.Add(new User() { Email = email, UserName = userName });
            UserService.SetUsers(users);
            return Ok("Пользователь добавлен");
        }

        /// <summary>
        /// Добавление сообщения
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="receiver">Получатель</param>
        /// <param name="subject">Тема сообщения</param>
        /// <param name="message">Сообщение</param>
        /// <response code="400">Неверные данные об отправителе или получателе</response>
        /// <response code="404">Отправитель или получатель не найдены</response>
        /// <response code="200">Сообщение отправлено успешно</response>
        /// <returns>Ответ сервера</returns>
        [HttpPost("message/add")]
        public IActionResult AddMessage(string sender, string receiver, string subject, string message)
        {
            if (sender == receiver)
                return BadRequest("Отправитель и получатель совпадают");
            var userSender = users.SingleOrDefault(x => x.Email == sender);
            var userReceiver = users.SingleOrDefault(x => x.Email == receiver);
            if (userSender == null || userReceiver == null)
                return NotFound("Пользователь с таким эмайлом не найден");
            messages.Add(new Messages() { Message = message, ReceiverId = receiver, SenderId = sender, Subject = subject });
            MessageService.SetMessages(messages);
            return Ok("Сообщение успещно отправлено");
        }

        /// <summary>
        /// Получить список всех пользователей
        /// </summary>
        /// <response code="404">Список пользователей пуст</response>
        /// <response code="200">Успешно выведен список пользователей</response>
        /// <returns>Ответ сервера</returns>
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            if (users == null)
                return NotFound("Массив не инициализирован");
            return Ok(users);
        }
        /// <summary>
        /// Получить список пользователей в диапазоне
        /// </summary>
        /// <response code="400">Неверные данные о границах</response>
        /// <response code="200">Успешно получен список пользователей</response>
        /// <param name="limit">Максимум пользователей для получения</param>
        /// <param name="offset">Начальный индекс</param>
        /// <returns>Ответ сервера</returns>
        [HttpGet("users/atRange")]
        public IActionResult GetUsersAtRange(int limit, int offset)
        {
            if (limit <= 0 || offset < 0)
                return BadRequest("Значения параметров меньше нуля");
            if (offset + limit > users.Count)
                return BadRequest("Недостаточное количество пользователей");
            //limit = offset + limit > users.Count ? users.Count - offset : limit;
            var usersAtRange = users.GetRange(offset, limit);
            return Ok(usersAtRange);
        }
        /// <summary>
        /// Вывести пользователя по эмайлу
        /// </summary>
        /// <response code="404">Список пользователей не инициализирован или не найден данный пользователь</response>
        /// <response code="200">Успешно найден пользователь</response>
        /// <param name="email">Эмайл</param>
        /// <returns>Ответ сервера</returns>
        [HttpGet("user/{email}")]
        public IActionResult GetUserByEmail(string email)
        {
            if (users == null)
                return NotFound("Массив не инициализирован");
            var user = users.SingleOrDefault(x => x.Email == email);
            if (user == null)
                return NotFound("Пользователь не найден");
            return Ok(user);
        }

        /// <summary>
        /// Получить список сообщений от конкретного пользователя конкретному
        /// </summary>
        /// <response code="404">Список пользователей не инициализирован или сообщение не найдено</response>
        /// <response code="200">Успешно получен список сообщений</response>
        /// <param name="sender">Отправитель</param>
        /// <param name="receiver">Получатель</param>
        /// <returns>Ответ сервера</returns>
        [HttpGet("message/senderReceiver")]
        public IActionResult GetMessageBySenderAndReceiver(string sender, string receiver)
        {
            if (users == null)
                return NotFound("Массив не инициализирован");
            var messageArray = messages.Where(x => x.ReceiverId == receiver && x.SenderId == sender);
            if (messageArray == null)
                return NotFound("Сообщение не найдено");
            return Ok(messageArray);
        }
        /// <summary>
        /// Получить список сообщений отправителя
        /// </summary>
        /// <response code="404">Список пользователей не инициализирован или сообщение не найдено</response>
        /// <response code="200">Успешно получен список сообщений</response>
        /// <param name="sender">Отправитель</param>
        /// <returns>Ответ сервера</returns>
        [HttpGet("message/sender")]
        public IActionResult GetMessageBySender(string sender)
        {
            if (users == null)
                return NotFound("Массив не инициализирован");
            var messageArray = messages.Where(x => x.SenderId == sender);
            if (messageArray == null)
                return NotFound("Сообщение не найдено");
            return Ok(messageArray);
        }
        /// <summary>
        /// Получить список сообщений получателя
        /// </summary>
        /// <response code="404">Список пользователей не инициализирован или сообщение не найдено</response>
        /// <response code="200">Успешно получен список сообщений</response>
        /// <param name="receiver">Получатель</param>
        /// <returns>Ответ сервера</returns>
        [HttpGet("message/receiver")]
        public IActionResult GetMessageByReceiver(string receiver)
        {
            if (users == null)
                return NotFound("Массив не инициализирован");
            var messageArray = messages.Where(x => x.ReceiverId == receiver);
            if (messageArray == null)
                return NotFound("Сообщение не найдено");
            return Ok(messageArray);
        }
        /// <summary>
        /// Получить xml комментарии к запросам
        /// </summary>
        /// <response code="200"></response>
        /// <returns>Ответ сервера</returns>
        [HttpGet("xml")]
        public IActionResult GetXml()
        {
            string path = @"MessageService.xml";
            using (var reader = new StreamReader(path))
            {
                var text = reader.ReadToEnd();
                return Ok(text);
            }
        }

    }
}
