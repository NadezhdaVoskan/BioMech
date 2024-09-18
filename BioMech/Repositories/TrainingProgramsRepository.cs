using BioMech.Models;
using Firebase.Auth;
using Newtonsoft.Json;
using System.Text;

namespace BioMech.Repositories
{
    public class TrainingProgramsRepository
    {
        private readonly string apiUrl; // ip API

        private readonly IHttpContextAccessor _httpContextAccessor; // интерфейс для доступа к HttpContext

        private string userID; // ID пользователя, находящегося в сессии

        public TrainingProgramsRepository(ApiSettings apiSettings, IHttpContextAccessor httpContextAccessor)
        {
            apiUrl = apiSettings.BaseUrl;
            _httpContextAccessor = httpContextAccessor;
            userID = _httpContextAccessor.HttpContext.Session.GetString("UserID")?.Trim('"'); // ID пользователя, находящегося в сессии
        }

        /// <summary>
        /// Запрос к API для получения тренировочных программ по категории
        /// </summary>
        /// <param name="idCategory"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetTrainingProgramsByCategory(int idCategory)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}Trainings/GetTrainingByCategory/{idCategory}");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения всех тренировочных программ
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetTrainingPrograms()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}Trainings");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для создания видео тренировочной программы 
        /// </summary>
        /// <param name="training"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CreateNewTrainingVideo(Training training)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(training), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiUrl + "Trainings", content);
            }
        }

        /// <summary>
        /// Запрос к API для редактирования видео тренировочной программы
        /// </summary>
        /// <param name="training"></param>
        /// <param name="trainingID"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateTraining(Training training, int trainingID)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(training), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}Trainings/{trainingID}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для редактирования тренировочной программы
        /// </summary>
        /// <param name="trainingProgram"></param>
        /// <param name="trainingProgramId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateTrainingProgram(TrainingProgram trainingProgram, int trainingProgramId)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(trainingProgram), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}TrainingPrograms/{trainingProgramId}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для получения категорий тренировок
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetTrainingCategory()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}TrainingCategories");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения информации о конекртной тренировочной программе
        /// </summary>
        /// <param name="idTrainingProgram"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetInfoTrainingProgramsById(int idTrainingProgram)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}Trainings/{idTrainingProgram}");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения информации о всех видах тренировочных программ
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAllViewTrainingProgram()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}TrainingPrograms");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения информации конкретной тренировочной программы
        /// </summary>
        /// <param name="trainingProgramID"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetTrainingProgramInfoRep(int trainingProgramID)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}TrainingPrograms/{trainingProgramID}");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения информации одного дня тренировочной программы
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetOneDayTrainingProgramInfoRep()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}OneDayTrainingPrograms");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения информации об одном дне тренировочной программы
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetDailyTrainingProgramInfoRep()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}DailyTrainings");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для создания видео тренировочной программы 
        /// </summary>
        /// <param name="dailyTraining"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CreateNewDailyTrainings(DailyTraining dailyTraining)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(dailyTraining), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiUrl + "DailyTrainings", content);
            }
        }

        /// <summary>
        /// Запрос к API для редактирования тренировочной программы
        /// </summary>
        /// <param name="dailyTraining"></param>
        /// <param name="dailyTrainingId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateDailyTraining(DailyTraining dailyTraining, int dailyTrainingId)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(dailyTraining), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}DailyTrainings/{dailyTrainingId}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для начала выполнения программы 
        /// </summary>
        /// <param name="passingTrainingProgram"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostStartPassingTrainingProgram(PassingTrainingProgram passingTrainingProgram)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(passingTrainingProgram), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiUrl + "PassingTrainingProgram", content);
            }
        }

        /// <summary>
        /// Запрос к API для прекращения выполнения программы 
        /// </summary>
        /// <param name="passingId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeletePassingTrainingProgram(int passingId)
        {
            using (var httpClient = new HttpClient())
            {
                string requestUri = $"{apiUrl}PassingTrainingProgram/{passingId}";

                return await httpClient.DeleteAsync(requestUri);
            }
        }

        /// <summary>
        /// Запрос к API для получения информации об одном дне тренировочной программы
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetPassingTrainingProgram()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}PassingTrainingProgram");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для записи в журнал тренировок
        /// </summary>
        /// <param name="trainingLog"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostTrainingLogs(TrainingLog trainingLog)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(trainingLog), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiUrl + "TrainingLogs", content);
            }
        }

        /// <summary>
        /// Запрос к API для получения записей из журнала
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetTrainingLogs()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}TrainingLogs");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для изменения статуса в журнале при завершении трен.программы
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateStatusTrainingLogs()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsync($"{apiUrl}TrainingLogs/UpdateStatusByUser/{userID}", null);

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для изменения информации в журнале
        /// </summary>
        /// <param name="trainingLog"></param>
        /// <param name="trainingLogId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateTrainingLogs(TrainingLog trainingLog, int trainingLogId)
        {

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(trainingLog), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}TrainingLogs/{trainingLogId}";

                return await httpClient.PutAsync(requestUri, content);
            }
           
        }
    }
}
