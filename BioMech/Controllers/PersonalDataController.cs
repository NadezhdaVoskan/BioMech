using BioMech.Models;
using BioMech.Services;
using BioMech_API.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService = BioMech.Services.AuthenticationService;

namespace BioMech.Controllers
{
    public class PersonalDataController : Controller
    {
        public IActionResult DeletePersonalAccount()
        {
            return View();
        }

        private string userEmail; // Почта пользователя, находящегося в сессии

        private readonly PersonalAccountService personalAccountService;
        private readonly AuthenticationService authenticationService;
        private readonly TrainingProgramsService trainingProgramsService;

        private string userID; // ID пользователя, находящегося в сессии

        private readonly IHttpContextAccessor _httpContextAccessor; // интерфейс для доступа к HttpContext


        public PersonalDataController(PersonalAccountService personalAccountService, AuthenticationService authenticationService, TrainingProgramsService trainingProgramsService, IHttpContextAccessor httpContextAccessor)
        {
            this.personalAccountService = personalAccountService;
            this.authenticationService = authenticationService;
            this.trainingProgramsService = trainingProgramsService;
            _httpContextAccessor = httpContextAccessor;
            userID = _httpContextAccessor.HttpContext.Session.GetString("UserID")?.Trim('"'); // ID пользователя, находящегося в сессии
        }

        /// <summary>
        /// Загрузка из сессии почты пользователя
        /// </summary>
        private void LoadUserEmail()
        {
            userEmail = HttpContext.Session.GetString("UserEmail");
            userEmail = userEmail?.Trim('"');
        }

        /// <summary>
        /// Личные данные вошедшего пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> PersonalAccount()
        {
            // Вызов метода загрузки Email из сессии
            LoadUserEmail();

            var user = await personalAccountService.GetUsersInfoByEmail(userEmail);

            if (user != null)
            {
                HttpContext.Session.Set("UserID", user.IdUser); // Сохранение ID пользователя в сессии
                HttpContext.Session.Set("Role", user.RoleId); // Сохранение роли пользователя в сессии
                return View(user);
            }
            else
            {
                return View("Error"); 
            }
        }

        /// <summary>
        /// Изменение имени и фамилии пользователя
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateName(string firstname, string surname)
        {
            var userName = new UpdateUserNameDTO
            {
                FirstName = firstname,
                SecondName = surname
            };

            // Вызов метода сервиса для отправки запроса к API
            var isSuccess = await personalAccountService.UpdateNameUser(userName);

            if (isSuccess)
            {
                return RedirectToAction("PersonalAccount", "PersonalData");
            }
            else
            {
                return BadRequest("Update Name failed");
            }
        }

        /// <summary>
        /// Изменение почты пользователя
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateEmail(string email)
        {
            var userEmail = new UpdateEmailDTO
            {
                Email= email
            };

            // Вызов метода сервиса для отправки запроса к API
            var isSuccess = await personalAccountService.UpdateEmailUser(userEmail);

            if (isSuccess)
            {
                HttpContext.Session.Set("UserEmail", email);
                return RedirectToAction("PersonalAccount", "PersonalData");
            }
            else
            {
                return BadRequest("Update Email failed");
            }
        }

        /// <summary>
        /// Загрузка фотографии профиля в FireBase
        /// </summary>
        /// <param name="userPhoto"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> UploadPhotoProfile(IFormFile userPhoto)
        {
            if (userPhoto != null && userPhoto.Length > 0)
            {
                var fileName = Path.GetFileName(userPhoto.FileName);
                using (var stream = userPhoto.OpenReadStream())
                {
                    var link = await personalAccountService.UploadProfilePhoto(stream, fileName);

                    if (!string.IsNullOrEmpty(link))
                    {
                        await UpdatePhotoProfile(link);
                        //return Ok();

                        return RedirectToAction("/PersonalData/PersonalAccount");
                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }
            else
            {
                return BadRequest("Invalid file");
            }
        }

        /// <summary>
        /// Изменение фотографии профиля
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePhotoProfile(string link)
        {
            var userPhoto = new UpdatePhotoProfileDTO
            {
                PhotoProfile = link
            };

            LoadUserEmail();

            // Вызов метода сервиса для отправки запроса к API
            var isSuccess = await personalAccountService.UpdatePhotoProfileUser(userPhoto);

            if (isSuccess)
            {
                return RedirectToAction("PersonalAccount", "PersonalData");
            }
            else
            {
                return BadRequest("Update photo failed");
            }

        }

        /// <summary>
        /// Удаление фотографии профиля
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemovePhotoProfile()
        {
            LoadUserEmail();

            var user = await personalAccountService.GetUsersInfoByEmail(userEmail);
            if (user != null && user.PhotoProfile != null)
            {
                // Извлечение имени файла из URL
                var fileName = ExtractFileName(user.PhotoProfile);

                // Удаление фотографии из Firebase
                var deleted = await personalAccountService.DeleteProfilePhoto(fileName);
                if (deleted)
                {
                    var isSuccess = await personalAccountService.RemovePhotoProfileUser();

                    if (isSuccess)
                    {
                        return RedirectToAction("PersonalAccount", "PersonalData");
                    }
                    else
                    {
                        return BadRequest("RemovePhoto failed");
                    }
                }
            }
            return View("Error");
          
        }

        /// <summary>
        /// Метод извлечения имени файла из URL
        /// </summary>
        /// <param name="photoUrl"></param>
        /// <returns></returns>
        private string ExtractFileName(string photoUrl)
        {
            if (string.IsNullOrWhiteSpace(photoUrl))
            {
                return string.Empty;
            }
            try
            {
                // Создание объекта Uri из строки URL
                Uri uri = new Uri(photoUrl);

                // Извлечение абсолютного пути (без Query String)
                string path = uri.AbsolutePath;

                // Декодирование URL, чтобы преобразовать %2F обратно в /
                string decodedPath = Uri.UnescapeDataString(path);

                // Извлечение имя файла из пути
                var segments = decodedPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 0)
                {
                    string fileName = segments.Last();
                    return fileName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting file name from URL: {ex.Message}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetUsersInfoByEmail()
        {
            // Вызов метода сервиса для отправки запроса к API
            var user = await personalAccountService.GetUsersInfoByEmail(userEmail);

            if (user != null)
            {
                return View("PersonalAccount", user); 
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="newPasswordRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordRequest newPasswordRequest)
        {
            
            // Вызов метода сервиса для отправки запроса к API
            var isSuccess = await personalAccountService.UpdatePasswordUser(newPasswordRequest);

            if (isSuccess)
            {
                return RedirectToAction("PersonalAccount", "PersonalData");
            }
            else
            {
                return BadRequest("ChangePassword failed");
            }
        }

        /// <summary>
        /// Удаление профиля
        /// </summary>
        /// <param name="checkPasswordForDelete"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteAccount(string checkPasswordForDelete)
        {
            LoadUserEmail();

            var user = await personalAccountService.GetUsersInfoByEmail(userEmail);

            var userForDelete = new Models.User
            {
                Email = user.Email,
                Password = checkPasswordForDelete
            };

            var isSuccessPassword = await authenticationService.SignIn(userForDelete);

            if (isSuccessPassword)
            {
                user.DeletedUser = true;

                var isSuccess = await personalAccountService.DeleteAccountUser(user);

                if (isSuccess)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Authorization", "authentication");
                }
                else
                {
                    return BadRequest("Delete account failed");
                }

            }
            return View("Error");
        }

        /// <summary>
        /// Вывод данных пользователя в профиле
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Profile()
        {
            LoadUserEmail();

            var user = await personalAccountService.GetUsersInfoByEmail(userEmail);

            if (user != null)
            {
                var diagnosticsProblems = await personalAccountService.GetDiagnosticsProblems();
                if (diagnosticsProblems != null)
                {
                    var diagnosticsHistory = await personalAccountService.GetDiagnosticsHistory();
                    if (diagnosticsHistory != null)
                    {
                        var existingPrograms = new List<PassingTrainingProgram> {};

                        foreach (var id in Enumerable.Range(1, 4))
                        {
                            existingPrograms.AddRange(await trainingProgramsService.GetPassingTrainingPrograms(id));
                        }

                        var model = new ProfileModel
                        {
                            User = user,
                            PhotoDiagnostic = diagnosticsHistory,
                            DiagnosticsProblems = diagnosticsProblems,
                            PassingTrainingPrograms = existingPrograms
                        };
                        model.TrainingProgram = new List<TrainingProgram> { };

                        foreach (var program in existingPrograms)
                        {
                            var trainingProgramInfo = await trainingProgramsService.GetTrainingProgramInfo((int)program.TrainingProgramId);

                            if (trainingProgramInfo != null)
                            {

                                model.TrainingProgram.Add(trainingProgramInfo);
                            }
                        }

                        return View(model);
                    }
                    else
                    {
                        // Обработка ошибки получения данных из сервиса
                        return View("Error");
                    }
                }
                else
                {
                    // Обработка ошибки получения категорий диагностики из сервиса
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }


        /// <summary>
        /// Все фотографии диагностики
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetAllDiagnostics()
        {

            var diagnosticsHistory = await personalAccountService.GetDiagnosticsHistory();
            return Json(diagnosticsHistory);
        }

        /// <summary>
        /// Фотографии диагностики в зависимости от категории
        /// </summary>
        /// <param name="idCategory"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDiagnosticsByCategory(int idCategory)
        {
            var diagnosticsByCategory = await personalAccountService.GetDiagnosticsHistoryByCategory(idCategory);
            if (diagnosticsByCategory != null)
            {
                return Json(diagnosticsByCategory);
            }
            else
            {
               
                return Json(new { error = "Данные недоступны" });
            }
        }

        /// <summary>
        /// Вывод истории диагностики в профиле
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ProfileResults(int categoryId)
        {
            LoadUserEmail();

            var user = await personalAccountService.GetUsersInfoByEmail(userEmail);

            if (user != null)
            {
                var diagnosticsProblems = await personalAccountService.GetDiagnosticsProblems();
                if (diagnosticsProblems != null)
                {
                    List<PhotoDiagnostic> diagnosticsHistory = null;
                    string categoryName = "Все диагностики";

                    if (categoryId == 0)
                    {
                        diagnosticsHistory = await personalAccountService.GetDiagnosticsHistory();
                    }
                    else
                    {
                        diagnosticsHistory = await personalAccountService.GetDiagnosticsHistoryByCategory(categoryId);

                        switch (categoryId)
                        {
                            case 1:
                                categoryName = "Лопатки";
                                break;
                            case 2:
                                categoryName = "Колени";
                                break;
                            case 3:
                                categoryName = "Стопы";
                                break;
                            case 4:
                                categoryName = "Грудной и поясничный отделы";
                                break;
                            case 5:
                                categoryName = "Положение головы и шеи";
                                break;
                            default:
                                categoryName = "Все диагностики";
                                break;
                        }
                    }

                    if (diagnosticsHistory != null)
                    {
                        var model = new ProfileModel
                        {
                            User = user,
                            PhotoDiagnostic = diagnosticsHistory,
                            DiagnosticsProblems = diagnosticsProblems,
                            CategoryName = categoryName
                        };
                        return View("ProfileResults", model);
                    }
                    else
                    {
                        // Обработка ошибки получения данных из сервиса
                        return View("Error");
                    }
                }
                else
                {
                    // Обработка ошибки получения категорий диагностики из сервиса
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }

        /// <summary>
        /// Удаление конкретной фотографии из истории диагностик
        /// </summary>
        /// <param name="photoDiagnosticsId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeletePhotoDiagnostic(int photoDiagnosticsId)
        {

            var isSuccess = await personalAccountService.DeletePhotoDiagnostic(photoDiagnosticsId);

            if (isSuccess)
            {
                return View();
            }
            else
            {
                return BadRequest("");
            }

        }

        /// <summary>
        /// Удаление всех фотографий из истории диагностик по категории
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeletePhotoDiagnosticsByCategory(int categoryId)
        {

            if (categoryId == 0)
            {
                return await DeleteAllPhotoDiagnostics();
            }
            else
            {
                var isSuccess = await personalAccountService.DeletePhotoDiagnosticsByCategory(categoryId);
                if (isSuccess)
                {
                    return View();
                }
                else
                {
                    return BadRequest("");
                }
            }

        }


        /// <summary>
        /// Удаление всех фотографий из истории диагностик
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteAllPhotoDiagnostics()
        {

            var isSuccess = await personalAccountService.DeleteAllPhotoDiagnostics();

            if (isSuccess)
            {
                return View();
            }
            else
            {
                return BadRequest("");
            }

        }

    }
}
