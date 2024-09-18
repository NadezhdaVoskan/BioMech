using BioMech.Models;
using BioMech_API.Models;
using FirebaseAdmin.Messaging;
using Newtonsoft.Json;
using System.Text;

namespace BioMech.Repositories
{
    public class PersonalAccountRepository
    {
        private readonly string apiUrl; // ip API

        private readonly IHttpContextAccessor _httpContextAccessor; // интерфейс для доступа к HttpContext

        private string userID; // ID пользователя, находящегося в сессии

        public PersonalAccountRepository(ApiSettings apiSettings, IHttpContextAccessor httpContextAccessor)
        {
            apiUrl = apiSettings.BaseUrl;
            _httpContextAccessor = httpContextAccessor;
            userID = _httpContextAccessor.HttpContext.Session.GetString("UserID")?.Trim('"'); // ID пользователя, находящегося в сессии
        }

        /// <summary>
        /// Запрос к API для получения личных данных вошедшего пользователя через Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetUsersInfoByEmail(string email)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}Users/ByEmail?email={email}");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для изменения имени и фамилии пользователя
        /// </summary>
        /// <param name="updateUserName"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateNameUser(UpdateUserNameDTO updateUserName)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(updateUserName), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}Users/UpdateName/{userID}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для изменения почты пользователя
        /// </summary>
        /// <param name="updateUserEmail"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateEmailUser(UpdateEmailDTO updateUserEmail)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(updateUserEmail), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}Users/UpdateEmail/{userID}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для изменения фотографии профиля пользователя
        /// </summary>
        /// <param name="updateUserPhoto"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdatePhotoProfileUser(UpdatePhotoProfileDTO updateUserPhoto)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(updateUserPhoto), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}Users/UpdatePhotoProfile/{userID}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для удаления фотографии профиля пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> RemoveUserPhoto()
        {
            using (var httpClient = new HttpClient())
            {
                string requestUri = $"{apiUrl}Users/RemovePhotoProfile/{userID}";

                var request = new HttpRequestMessage(HttpMethod.Put, requestUri);

                return await httpClient.SendAsync(request);
            }
        }

        /// <summary>
        /// Запрос к API для изменения пароля пользователя
        /// </summary>
        /// <param name="passwordRequest"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdatePassword(ChangePasswordRequest passwordRequest)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(passwordRequest), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}Users/ChangePassword/{userID}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для удаления аккаунта пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAccountUser(User user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}Users/{userID}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для получения истории диагностик по категории
        /// </summary>
        /// <param name="idCategory"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetDiagnosticsHistoryByCategory(int idCategory)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}PhotoDiagnostics/ByDiagnosticsCategoryID/{idCategory}");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения всей истории диагностики
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetDiagnosticsHistory()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}PhotoDiagnostics");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения всех названией категорий проблем диагностики
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetDiagnosticsProblems()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}ProblemCategories");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для удаления конкретной фотографии из истории диагностик
        /// </summary>
        /// <param name="photoDiagnosticsId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeletePhotoDiagnostics(int photoDiagnosticsId)
        {
            using (var httpClient = new HttpClient())
            {
                string requestUri = $"{apiUrl}PhotoDiagnostics/{photoDiagnosticsId}";

                return await httpClient.DeleteAsync(requestUri);
            }
        }

        /// <summary>
        /// Запрос к API для удаления конкретной категории из истории диагностик
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeletePhotoDiagnosticsByCategory(int categoryId)
        {
            using (var httpClient = new HttpClient())
            {
                string requestUri = $"{apiUrl}PhotoDiagnostics/DeleteByCategoryID/{categoryId}?userId={userID}";

                return await httpClient.DeleteAsync(requestUri);
            }
        }

        /// <summary>
        /// Запрос к API для удаления всех фотографий из истории диагностик
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAllPhotoDiagnostics()
        {
            using (var httpClient = new HttpClient())
            {
                string requestUri = $"{apiUrl}PhotoDiagnostics/DeleteAllByUserId/{userID}";

                return await httpClient.DeleteAsync(requestUri);
            }
        }

    }
}
