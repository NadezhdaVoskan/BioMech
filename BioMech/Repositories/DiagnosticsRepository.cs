using BioMech.Models;
using Firebase.Auth;
using Newtonsoft.Json;
using System.Text;

namespace BioMech.Repositories
{
    public class DiagnosticsRepository
    {
        private readonly string apiModelUrl; // ip API
        private readonly string apiUrl; // ip API

        private readonly IHttpContextAccessor _httpContextAccessor; // интерфейс для доступа к HttpContext

        private string userID; // ID пользователя, находящегося в сессии

        public DiagnosticsRepository(ApiSettings apiSettings, IHttpContextAccessor httpContextAccessor)
        {
            apiModelUrl = apiSettings.ModelsAPIUrl;
            apiUrl = apiSettings.BaseUrl;
            _httpContextAccessor = httpContextAccessor;
            userID = _httpContextAccessor.HttpContext.Session.GetString("UserID")?.Trim('"'); // ID пользователя, находящегося в сессии
        }

        /// <summary>
        /// Запрос к API для работы модели "Крыловидные лопатки" 
        /// </summary>
        /// <param name="detectShoulderBlades"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DetectShoulderBlades(DetectModelYolo detectShoulderBlades)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(detectShoulderBlades), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiModelUrl + "detectShoulderBlades/", content);
            }
        }

        /// <summary>
        /// Запрос к API для работы модели "Косточка на стопе"
        /// </summary>
        /// <param name="detectFootBone"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DetectFootBone(DetectModelYolo detectFootBone)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(detectFootBone), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiModelUrl + "detectFootBone/", content);
            }
        }

        /// <summary>
        /// Запрос к API для работы модели "Протракция шеи"
        /// </summary>
        /// <param name="detectNeckProtraction"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DetectNeckProtraction(DetectModelOpenPose detectNeckProtraction)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(detectNeckProtraction), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiModelUrl + "detectNeckProtraction/", content);
            }
        }

        /// <summary>
        /// Запрос к API для работы модели "Вальгусные или варусные колени"
        /// </summary>
        /// <param name="detectKneesProblems"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DetectKneesProblems(DetectModelOpenPose detectKneesProblems)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(detectKneesProblems), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiModelUrl + "detectKneesProblems/", content);
            }
        }

        /// <summary>
        /// Запрос к API для сохранение результатов диагностики в базу данных
        /// </summary>
        /// <param name="photoDiagnostic"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CreateNotesPhotoDiagnostics(PhotoDiagnostic photoDiagnostic)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(photoDiagnostic), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiUrl + "PhotoDiagnostics", content);
            }
        }

    }
}
