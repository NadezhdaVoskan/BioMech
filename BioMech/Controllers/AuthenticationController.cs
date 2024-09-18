using BioMech.Models;
using BioMech.Services;
using BioMech_API.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Web;
using User = BioMech.Models.User;

namespace BioMech.Controllers
{
    public class AuthenticationController : Controller
    {

        private readonly AuthenticationService authenticationService;
        private readonly PersonalAccountService personalAccountService;

        public AuthenticationController(ApiSettings apiSettings, AuthenticationService authenticationService, PersonalAccountService personalAccountService)
        {
            this.authenticationService = authenticationService;
            this.personalAccountService = personalAccountService;
        }

        /// <summary>
        /// Страница авторизации
        /// </summary>
        /// <returns></returns>
        public IActionResult Authorization()
        {
            return View();
        }

        /// <summary>
        /// Страница "Забыли пароль?"
        /// </summary>
        /// <returns></returns>
        public IActionResult Forgot()
        {
            return View();
        }

        /// <summary>
        /// Страница восстановления пароля
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IActionResult Recovery(string token)
        {
            ViewBag.Token = token;
            return View();
        }

        /// <summary>
        /// Страница регистрации
        /// </summary>
        /// <returns></returns>
        public IActionResult Registration()
        {
            return View();
        }


        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorization(string email, string password)
        {
            var user = new User
            {
                Email = email,
                Password = password
            };

            // Вызов метода сервиса для отправки запроса к API
            var isSuccess = await authenticationService.SignIn(user);

            if (isSuccess)
            {
                var userDeleted = await personalAccountService.GetUsersInfoByEmail(user.Email);
                if (userDeleted.DeletedUser != true) { 

                HttpContext.Session.Set("UserEmail", user.Email); // Сохранение почты пользователя в сессии
                return RedirectToAction("PersonalAccount", "PersonalData"); // Переход на страницу профиля

                }
                return RedirectToAction("Authorization");
            }
            else
            {
                return RedirectToAction("Authorization");
            }
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="surname"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Registration(string firstname, string surname, string email, string password)
        {
            var user = new User
            {
                FirstName = firstname,
                SecondName = surname,
                Email = email,
                Password = password,
                RoleId = 3
            };

            var userEmail = await personalAccountService.GetUsersInfoByEmail(user.Email);

            if (userEmail == null) // Проверка есть ли такой пользователь в системе
            {

                var isSuccess = await authenticationService.SignUp(user);

                if (isSuccess)
                {
                    return RedirectToAction("Authorization"); // Переход на страницу авторизации
                }
                else
                {
                    return RedirectToAction("Registration");
                }
            }
            else {
                return RedirectToAction("Registration");
            }
        }

        /// <summary>
        /// Выход из аккаунта пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ExitAccount()
        {
            HttpContext.Session.Clear();
            
            return RedirectToAction("Authorization"); 
        }

        /// <summary>
        /// Проверка статуса авторизованнного пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CheckAuthorization()
        {
            var isAuthorized = HttpContext.Session.GetString("UserEmail") != null;
            return Json(new { isAuthorized });
        }


        /// <summary>
        /// Отправление сообщения с ссылкой для восстановления пароля
        /// </summary>
        /// <param name="emailForRecovery"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendEmailForRecovery(string emailForRecovery)
        {

            var user = await personalAccountService.GetUsersInfoByEmail(emailForRecovery);

            if (user != null)
            {
                // Генерация уникального токена для пользователя
                 var token = authenticationService.GeneratePasswordResetToken(user.Email);

                var updateTokenRecovery = new UpdateTokenForRecoveryPasswordDTO
                {
                    TokenForRecoveryPassword = token,
                    TimeGenerateTokenForRecoveryPassword = DateTime.Now
                };

                // Сохранение токена с датой генерации в базу данных
                await authenticationService.UpdateTokenForRecoveryPassword(updateTokenRecovery, (int)user.IdUser);

                // Создание ссылки для восстановления пароля
                var recoveryLink = Url.Action("Recovery", "Authentication", new { token = token }, protocol: HttpContext.Request.Scheme);

                // Отправления письма с ссылкой на восстановление пароля
                await authenticationService.SendEmailRecoveryAsync(user.Email, $"Пожалуйста, перейдите по <a href=\"{recoveryLink}\">ссылке</a> для восстановления вашего пароля.");

                return View("Forgot");
            }
            else
            {
                // Пользователь не найден
                ViewBag.ErrorMessage = "Пользователь с таким адресом электронной почты не найден.";
                return View("Forgot");
            }
           
        }

        /// <summary>
        /// Отправление сообщения с ссылкой для восстановления пароля
        /// </summary>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <param name="repeatNewPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RecoveryPassword(string token, string newPassword, string repeatNewPassword)
        {
            if(newPassword != repeatNewPassword)
            {
                ViewBag.ErrorMessage = "Пароли не совпадают.";
                return View("Recovery");
            }
            var user = await authenticationService.GetUserByRecoveryToken(token);
            token = HttpUtility.UrlDecode(token);

            // Проверка "жизни" токена
            if (user.TimeGenerateTokenForRecoveryPassword.HasValue)
            {
                var expirationTime = user.TimeGenerateTokenForRecoveryPassword.Value.AddHours(24); 

                if (DateTime.Now > expirationTime)
                {
                    ViewBag.ErrorMessage = "Срок действия токена восстановления пароля истек.";
                    return View("Error");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Время генерации токена отсутствует.";
                return View("Error");
            }

            // Создание запроса на изменение пароля
            var changePasswordRequest = new ChangePasswordRequest
            {
                NewPassword = newPassword,
                RepeatedNewPassword = repeatNewPassword
            };

            // Вызов метода сервиса для изменения пароля
            var isSuccess = await authenticationService.RecoveryPassword(changePasswordRequest, (int)user.IdUser);

            if (isSuccess)
            {
                return RedirectToAction("Authorization");
            }
            else
            {
                ViewBag.ErrorMessage = "Ошибка при изменении пароля.";
                return View("Error");
            }

        }
    }
}
