using BioMech.Models;
using BioMech_API.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using User = BioMech.Models.User;

namespace BioMech.Repositories
{
    public class AuthenticationRepository
    {

        private readonly string apiUrl; // ip API

        public AuthenticationRepository(ApiSettings apiSettings)
        {
            apiUrl = apiSettings.BaseUrl;
        }

        /// <summary>
        /// Запрос к API для авторизации пользователя 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SignIn(User user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiUrl + "Users/SignIn", content);
            }
        }

        /// <summary>
        /// Запрос к API для регистрации пользователя 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SignUp(User user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiUrl + "Users", content);
            }
        }

        /// <summary>
        /// Запрос к API для создания токена для восстановления пароля пользователя 
        /// </summary>
        /// <param name="generateTokenForRecovery"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateTokenForRecoveryPassword(UpdateTokenForRecoveryPasswordDTO generateTokenForRecovery, int userID)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(generateTokenForRecovery), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}Users/UpdateTokenForRecoveryPassword/{userID}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для восстановления пароля пользователя 
        /// </summary>
        /// <param name="changePasswordRequest"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> RecoveryPassword(ChangePasswordRequest changePasswordRequest, int userID)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}Users/RecoveryPassword/{userID}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для получения информации о пользователе по токену
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetUserByRecoveryToken(string token)
        {
            using (var httpClient = new HttpClient())
            {
                var encodedToken = HttpUtility.UrlEncode(token);
                var response = await httpClient.GetAsync($"{apiUrl}Users/GetUserByRecoveryToken?token={encodedToken}");

                return response;
            }
        }
    }
}
