using System.Net.Mail;
using System.Net;
using System;

namespace BioMech.Services
{
    public class SupportService
    {

        private readonly IConfiguration _configuration; // Обращение к конфигурационному файлу

        public SupportService(IConfiguration configuration)
        {
            _configuration = configuration; // Обращение к конфигурационному файлу
        }

        /// <summary>
        /// Отправка электронного письма
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendEmailSupportAsync(string userEmail, string userName, string message)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings"); // Получение данных из конфигурационного файла
                string fromEmail = emailSettings["EmailAddress"]; // Почта отправителя 
                string emailPassword = emailSettings["EmailPassword"]; // Токен (шифр пароля) почты отправится
                string smtpServer = emailSettings["SmtpServer"]; // SMPT сервер
                int smtpPort = int.Parse(emailSettings["SmtpPort"]); // SMPT порт

                MailAddress from = new MailAddress(fromEmail, userEmail+" "+userName);
                MailAddress to = new MailAddress("myrka.mur2014@yandex.ru");
                MailMessage mailMessage = new MailMessage(from, to);

                mailMessage.Subject = "Обратная связь. Пользователь - "+userEmail;
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
    
    }
}
