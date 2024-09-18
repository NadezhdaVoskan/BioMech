using BioMech.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BioMech.Controllers
{
    public class SupportController : Controller
    {

        private readonly SupportService supportService;

        public SupportController(SupportService supportService)
        {
            this.supportService = supportService;
        }

        public IActionResult Support()
        {
            return View();
        }

        /// <summary>
        /// Отправление сообщения пользователя в поддержку
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userEmail"></param>
        /// <param name="userMessage"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendSupportRequest(string userName, string userEmail, string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(userMessage))
            {
                ViewBag.ErrorMessage = "Email и сообщение обязательны к заполнению.";
                return View("Support");
            }

            try
            {
                await supportService.SendEmailSupportAsync(userEmail, userName, userMessage);
                ViewBag.SuccessMessage = "Ваше сообщение успешно отправлено.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Произошла ошибка при отправке сообщения: {ex.Message}";
            }

            return View("Support");
        }
    }
}
