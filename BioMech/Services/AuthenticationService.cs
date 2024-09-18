using BioMech.Models;
using BioMech.Repositories;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using BioMech_API.Models;
using Newtonsoft.Json;

namespace BioMech.Services
{
    public class AuthenticationService
    {
        private readonly AuthenticationRepository authenticationRepository;

        private readonly IConfiguration _configuration; // Обращение к конфигурационному файлу

        public AuthenticationService(AuthenticationRepository authenticationRepository, IConfiguration configuration)
        {
            this.authenticationRepository = authenticationRepository;

            _configuration = configuration; // Обращение к конфигурационному файлу
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> SignIn(User user)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await authenticationRepository.SignIn(user);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> SignUp(User user)
        {
            var response = await authenticationRepository.SignUp(user);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Отправка электронного письма
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendEmailRecoveryAsync(string userEmail, string message)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings"); // Получение данных из конфигурационного файла
                string fromEmail = emailSettings["EmailAddress"]; // Почта отправителя 
                string emailPassword = emailSettings["EmailPassword"]; // Токен (шифр пароля) почты отправится
                string smtpServer = emailSettings["SmtpServer"]; // SMPT сервер
                int smtpPort = int.Parse(emailSettings["SmtpPort"]); // SMPT порт

                MailAddress from = new MailAddress(fromEmail, "BioMech");
                MailAddress to = new MailAddress(userEmail);
                MailMessage mailMessage = new MailMessage(from, to);

                mailMessage.Subject = "Восстановление пароля";
                mailMessage.Body = $"<a>{message}</a>";
                mailMessage.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);
                smtp.Credentials = new NetworkCredential(fromEmail, emailPassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке электронной почты: " + ex.Message);
            }
        }

        /// <summary>
        /// Генерация токена для изменения пароля
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public string GeneratePasswordResetToken(string userEmail)
        {
            // Использование RNGCryptoServiceProvider для генерации криптографически безопасного токена
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }

        /// <summary>
        /// Создание токена для восстановления пароля пользователя
        /// </summary>
        /// <param name="generateTokenForRecovery"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTokenForRecoveryPassword(UpdateTokenForRecoveryPasswordDTO generateTokenForRecovery, int userID)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await authenticationRepository.UpdateTokenForRecoveryPassword(generateTokenForRecovery, userID);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Восстановление пароля
        /// </summary>
        /// <param name="changePasswordRequest"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<bool> RecoveryPassword(ChangePasswordRequest changePasswordRequest, int userID)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await authenticationRepository.RecoveryPassword(changePasswordRequest, userID);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Получение личной информации вошедшего пользователя через Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Models.User> GetUserByRecoveryToken(string email)
        {
            var response = await authenticationRepository.GetUserByRecoveryToken(email);
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();

                var user = JsonConvert.DeserializeObject<Models.User>(userJson);

                return user;
            }
            return null;
        }
    }
}
