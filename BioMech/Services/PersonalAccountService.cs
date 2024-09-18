using BioMech.Models;
using BioMech.Repositories;
using BioMech_API.Models;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace BioMech.Services
{
    public class PersonalAccountService
    {
        private readonly PersonalAccountRepository personalAccountRepository;


        private string linkPhotoProfile; // Ссылка загруженной фотографии, сгенерируемая Firebase

        private string userID; // ID пользователя, находящегося в сессии

        private readonly IHttpContextAccessor _httpContextAccessor; // интерфейс для доступа к HttpContext

        private readonly IConfiguration _configuration; // Обращение к конфигурационному файлу

        public PersonalAccountService(PersonalAccountRepository personalAccountRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.personalAccountRepository = personalAccountRepository;
            _configuration = configuration; // Обращение к конфигурационному файлу
            _httpContextAccessor = httpContextAccessor;
            userID = _httpContextAccessor.HttpContext.Session.GetString("UserID")?.Trim('"'); // ID пользователя, находящегося в сессии

        }

        /// <summary>
        /// Получение личной информации вошедшего пользователя через Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Models.User> GetUsersInfoByEmail(string email)
        {
            var response = await personalAccountRepository.GetUsersInfoByEmail(email);
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();

                var user = JsonConvert.DeserializeObject<Models.User>(userJson); 

                return user;
            }
            return null;
        }

        /// <summary>
        /// Изменение имени и фамилии пользователя
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> UpdateNameUser(UpdateUserNameDTO userName)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await personalAccountRepository.UpdateNameUser(userName);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Изменение почты пользователя
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<bool> UpdateEmailUser(UpdateEmailDTO userEmail)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await personalAccountRepository.UpdateEmailUser(userEmail);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Загрузка фотографий профиля в облачное хранилище Firebase
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> UploadProfilePhoto(Stream stream, string fileName)
        {
            var firebaseSettings = _configuration.GetSection("FirebaseSettings");
            var auth = new FirebaseAuthProvider(new FirebaseConfig(firebaseSettings["ApiKey"]));
            var a = await auth.SignInWithEmailAndPasswordAsync(firebaseSettings["AuthEmail"], firebaseSettings["AuthPassword"]);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                firebaseSettings["Bucket"],
                new FirebaseStorageOptions
                {
                    ThrowOnCancel = true 
                })
                .Child("imagesProfile")
                .Child("User_ID_"+userID)
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);

            try
            {
                // ссылка на загруженную фотографию в Firebase
                linkPhotoProfile = await task;
                return linkPhotoProfile;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Удаление фотографии профиля
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<bool> DeleteProfilePhoto(string filePath)
        {
            try
            {
                var firebaseSettings = _configuration.GetSection("FirebaseSettings");
                var auth = new FirebaseAuthProvider(new FirebaseConfig(firebaseSettings["ApiKey"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(firebaseSettings["AuthEmail"], firebaseSettings["AuthPassword"]);

                // Создаем экземпляр FirebaseStorage, указывая Bucket
                var storage = new FirebaseStorage(firebaseSettings["Bucket"], new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                });

                // Удаляем файл
                await storage.Child("imagesProfile").Child("User_ID_" + userID).Child(filePath).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
                return false;
            }
        }

        /// <summary>
        /// Изменение изображения в профиле пользователя
        /// </summary>
        /// <param name="userPhotoProfile"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePhotoProfileUser(UpdatePhotoProfileDTO userPhotoProfile)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await personalAccountRepository.UpdatePhotoProfileUser(userPhotoProfile);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Удаление изображения из профиля пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RemovePhotoProfileUser()
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await personalAccountRepository.RemoveUserPhoto();

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="passwordRequest"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePasswordUser(ChangePasswordRequest passwordRequest)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await personalAccountRepository.UpdatePassword(passwordRequest);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Удаление личного аккаунта пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAccountUser(Models.User user)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await personalAccountRepository.DeleteAccountUser(user);

            return response.IsSuccessStatusCode;
        }


        /// <summary>
        /// Получение истории диагностики по категории
        /// </summary>
        /// <param name="idCategory"></param>
        /// <returns></returns>
        public async Task<List<PhotoDiagnostic>> GetDiagnosticsHistoryByCategory(int idCategory)
        {
            var response = await personalAccountRepository.GetDiagnosticsHistoryByCategory(idCategory);
            if (response.IsSuccessStatusCode)
            {
                var diagnosticsJson = await response.Content.ReadAsStringAsync();

                var allPhotoDiagnostics = JsonConvert.DeserializeObject<List<PhotoDiagnostic>>(diagnosticsJson);
                // Отфильтровать по пользователю и отсортировать по дате в обратном порядке
                var filteredDiagnostics = allPhotoDiagnostics
                    .Where(d => d.UserId == int.Parse(userID))
                    .OrderByDescending(d => d.DateDownload) // Сортировка по дате в убывающем порядке
                    .ToList();
                return filteredDiagnostics;
            }
            return null;
        }

        /// <summary>
        /// Получение всей истории диагностики пользователя
        /// </summary>
        /// <returns></returns>
        public async Task <List<PhotoDiagnostic>> GetDiagnosticsHistory()
        {
            var response = await personalAccountRepository.GetDiagnosticsHistory();
            if (response.IsSuccessStatusCode)
            {
                var diagnosticsJson = await response.Content.ReadAsStringAsync();
                var allPhotoDiagnostics = JsonConvert.DeserializeObject<List<PhotoDiagnostic>>(diagnosticsJson);
                // Отфильтровать по пользователю и отсортировать по дате в обратном порядке
                var filteredDiagnostics = allPhotoDiagnostics
                    .Where(d => d.UserId == int.Parse(userID))
                    .OrderByDescending(d => d.DateDownload) // Сортировка по дате в убывающем порядке
                    .ToList();
                return filteredDiagnostics;
            }
            return null;
        }

        /// <summary>
        /// Получение списка проблем диагностик
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<int, string>> GetDiagnosticsProblems()
        {
            var response = await personalAccountRepository.GetDiagnosticsProblems();
            if (response.IsSuccessStatusCode)
            {
                var diagnosticsProblemsJson = await response.Content.ReadAsStringAsync();
                var diagnosticsProblems = JsonConvert.DeserializeObject<List<ProblemCategory>>(diagnosticsProblemsJson);
                return diagnosticsProblems.ToDictionary(x => x.IdProblemCategory, x => x.NameProblemCategory);
            }
            return new Dictionary<int, string>();
        }

        /// <summary>
        /// Удаление конкретной фотографии из истории диагностик
        /// </summary>
        /// <param name="photoDiagnosticsId"></param>
        /// <returns></returns>
        public async Task<bool> DeletePhotoDiagnostic(int photoDiagnosticsId)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await personalAccountRepository.DeletePhotoDiagnostics(photoDiagnosticsId);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Удаление конкретной категории фотографии из истории диагностик
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<bool> DeletePhotoDiagnosticsByCategory(int categoryId)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await personalAccountRepository.DeletePhotoDiagnosticsByCategory(categoryId);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Удаление всех фотографий из истории диагностик
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAllPhotoDiagnostics()
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await personalAccountRepository.DeleteAllPhotoDiagnostics();

            return response.IsSuccessStatusCode;
        }

    }
}
