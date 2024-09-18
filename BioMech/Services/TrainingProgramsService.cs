using BioMech.Models;
using BioMech.Repositories;
using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;

namespace BioMech.Services
{
    public class TrainingProgramsService
    {
        private readonly TrainingProgramsRepository trainingProgrammsRepository;

        private readonly IConfiguration _configuration; // Обращение к конфигурационному файлу

        private string userID; // ID пользователя, находящегося в сессии

        private readonly IHttpContextAccessor _httpContextAccessor; // интерфейс для доступа к HttpContext

        public TrainingProgramsService(TrainingProgramsRepository trainingProgrammsRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.trainingProgrammsRepository = trainingProgrammsRepository;

            _configuration = configuration; // Обращение к конфигурационному файлу

            _httpContextAccessor = httpContextAccessor;

            userID = _httpContextAccessor.HttpContext.Session.GetString("UserID")?.Trim('"'); // ID пользователя, находящегося в сессии
        }

        /// <summary>
        /// Получение всех тренировочных программ по категории
        /// </summary>
        /// <param name="idCategory"></param>
        /// <returns></returns>
        public async Task<List<Models.Training>> GetTrainingProgramByCategory(int idCategory)
        {
            var response = await trainingProgrammsRepository.GetTrainingProgramsByCategory(idCategory);
            if (response.IsSuccessStatusCode)
            {
                var trainingProgramJson = await response.Content.ReadAsStringAsync();
                var trainingsResult = JsonConvert.DeserializeObject<List<Models.Training>>(trainingProgramJson);
                var filteredTraining = trainingsResult
                            .Where(d => d.DeletedTraining == null || d.DeletedTraining == false)
                            .ToList();
                return filteredTraining;
            }
            return null;
        }

        /// <summary>
        /// Получение всех тренировочных программ
        /// </summary>
        /// <returns></returns>
        public async Task<List<Models.Training>> GetTrainingProgram()
        {
            var response = await trainingProgrammsRepository.GetTrainingPrograms();
            if (response.IsSuccessStatusCode)
            {
                var trainingProgramJson = await response.Content.ReadAsStringAsync();
                var trainingsResult = JsonConvert.DeserializeObject<List<Models.Training>>(trainingProgramJson);
                var filteredTraining = trainingsResult
                            .Where(d => d.DeletedTraining == null || d.DeletedTraining == false)
                            .ToList();
                return filteredTraining;
            }
            return null;
        }

        /// <summary>
        /// Получение всех тренировочных программ в архиве
        /// </summary>
        /// <returns></returns>
        public async Task<List<Models.Training>> GetTrainingProgramArchive()
        {
            var response = await trainingProgrammsRepository.GetTrainingPrograms();
            if (response.IsSuccessStatusCode)
            {
                var trainingProgramJson = await response.Content.ReadAsStringAsync();
                var trainingsResult = JsonConvert.DeserializeObject<List<Models.Training>>(trainingProgramJson);
                var filteredTraining = trainingsResult
                            .Where(d => d.DeletedTraining == true)
                            .ToList();
                return filteredTraining;
            }
            return null;
        }

        /// <summary>
        /// Получение всех тренировочных программ в архиве
        /// </summary>
        /// <returns></returns>
        public async Task<List<Models.TrainingProgram>> GetAllViewTrainingProgram()
        {
            var response = await trainingProgrammsRepository.GetAllViewTrainingProgram();
            if (response.IsSuccessStatusCode)
            {
                var trainingProgramJson = await response.Content.ReadAsStringAsync();
                var trainingsResult = JsonConvert.DeserializeObject<List<Models.TrainingProgram>>(trainingProgramJson);
                return trainingsResult;
            }
            return null;
        }

        /// <summary>
        /// Получение информации по конкретной тренировочной программе
        /// </summary>
        /// <param name="idTrainingProgram"></param>
        /// <returns></returns>
        public async Task<Models.Training> GetInfoTrainingProgramsById(int idTrainingProgram)
        {
            var response = await trainingProgrammsRepository.GetInfoTrainingProgramsById(idTrainingProgram);
            if (response.IsSuccessStatusCode)
            {
                var trainingProgramJson = await response.Content.ReadAsStringAsync();
                var trainingResult = JsonConvert.DeserializeObject<Models.Training>(trainingProgramJson);
                return trainingResult;
            }
            return null;
        }

        /// <summary>
        /// Поиск тренировочной программы
        /// </summary>
        /// <param name="queryTrainingPrograms"></param>
        /// <returns></returns>
        public async Task<List<Training>> SearchTrainingPrograms(string queryTrainingPrograms)
        {
            var allTrainingPrograms = await GetTrainingProgram();

            // Преобразование текста запроса к нижнему регистру
            var lowerQuery = queryTrainingPrograms.ToLower();

            // Выполнение фильтрации статей по названию или краткому описанию без учета регистра
            var searchResults = allTrainingPrograms.Where(a => a.NameTraining.ToLower().Contains(lowerQuery) ||
                                                       a.DescriptionTraining.ToLower().Contains(lowerQuery)).ToList();

            return searchResults;
        }

        /// <summary>
        /// Поиск тренировочной программы в архиве
        /// </summary>
        /// <param name="queryTrainingPrograms"></param>
        /// <returns></returns>
        public async Task<List<Training>> SearchTrainingProgramsArchive(string queryTrainingPrograms)
        {
            var allTrainingPrograms = await GetTrainingProgramArchive();

            // Преобразование текста запроса к нижнему регистру
            var lowerQuery = queryTrainingPrograms.ToLower();

            // Выполнение фильтрации статей по названию или краткому описанию без учета регистра
            var searchResults = allTrainingPrograms.Where(a => a.NameTraining.ToLower().Contains(lowerQuery) ||
                                                       a.DescriptionTraining.ToLower().Contains(lowerQuery)).ToList();

            return searchResults;
        }

        /// <summary>
        /// Получение всех категорий тренировок
        /// </summary>
        /// <returns></returns>
        public async Task<List<TrainingCategory>> GetTrainingCategory()
        {
            var response = await trainingProgrammsRepository.GetTrainingCategory();
            if (response.IsSuccessStatusCode)
            {
                var trainingCategoryJson = await response.Content.ReadAsStringAsync();

                var trainingCategory = JsonConvert.DeserializeObject<List<TrainingCategory>>(trainingCategoryJson);

                return trainingCategory;
            }
            return null;
        }

        /// <summary>
        /// Получение информации конкретной тренировочной программы
        /// </summary>
        /// <param name="trainingProgramID"></param>
        /// <returns></returns>
        public async Task<Models.TrainingProgram> GetTrainingProgramInfo(int trainingProgramID)
        {
            var response = await trainingProgrammsRepository.GetTrainingProgramInfoRep(trainingProgramID);
            if (response.IsSuccessStatusCode)
            {
                var trainingProgramJson = await response.Content.ReadAsStringAsync();

                var trainingProgram = JsonConvert.DeserializeObject<Models.TrainingProgram>(trainingProgramJson);

                return trainingProgram;
            }
            return null;
        }


        /// <summary>
        /// Получение всех дней тренировочной программы
        /// </summary>
        /// <param name="trainingProgramId"></param>
        /// <returns></returns>
        public async Task<List<OneDayTrainingProgram>> GetOneDayTrainingProgramInfo(int trainingProgramId)
        {
            var response = await trainingProgrammsRepository.GetOneDayTrainingProgramInfoRep();

            if (response.IsSuccessStatusCode)
            {
                var oneDayTrainingProgramJson = await response.Content.ReadAsStringAsync();
                var oneDayTrainingProgramAll = JsonConvert.DeserializeObject<List<OneDayTrainingProgram>>(oneDayTrainingProgramJson);
                // Фильтр по программе тренировок и отсортировать по дате
                var filteredOneDayTrainingProgram = oneDayTrainingProgramAll
                    .Where(d => d.TrainingProgramId == trainingProgramId)
                    .Select(day => {
                        // Проверка, содержится ли слово "Выходной" в названии дня
                        if (day.DayTrainingProgram.Contains("Выходной"))
                        {
                            // Замена на "Выходной день"
                            day.DayTrainingProgram = "Выходной день";
                        }
                        return day;
                    })
                    .ToList();
                return filteredOneDayTrainingProgram;
            }
            return null;
        }

        /// <summary>
        /// Получение всех дней тренировочной программы для редактирования
        /// </summary>
        /// <param name="trainingProgramId"></param>
        /// <returns></returns>
        public async Task<List<OneDayTrainingProgram>> GetDaysTrainingProgramInfo(int trainingProgramId)
        {
            var response = await trainingProgrammsRepository.GetOneDayTrainingProgramInfoRep();

            if (response.IsSuccessStatusCode)
            {
                var oneDayTrainingProgramJson = await response.Content.ReadAsStringAsync();
                var oneDayTrainingProgramAll = JsonConvert.DeserializeObject<List<OneDayTrainingProgram>>(oneDayTrainingProgramJson);
                // Фильтр по программе тренировок и отсортировать по дате
                var filteredOneDayTrainingProgram = oneDayTrainingProgramAll
                    .Where(d => d.TrainingProgramId == trainingProgramId)
                    .ToList();
                return filteredOneDayTrainingProgram;
            }
            return null;
        }

        /// <summary>
        /// Получение одного для тренировочной программы
        /// </summary>
        /// <param name="trainingDayProgramId"></param>
        /// <returns></returns>
        public async Task<List<DailyTraining>> GetDailyTrainingProgramInfo(int trainingDayProgramId)
        {
            var response = await trainingProgrammsRepository.GetDailyTrainingProgramInfoRep();

            if (response.IsSuccessStatusCode)
            {
                var DailyTrainingProgramJson = await response.Content.ReadAsStringAsync();
                var DailyTrainingProgramAll = JsonConvert.DeserializeObject<List<DailyTraining>>(DailyTrainingProgramJson);
                var filteredDailyTrainingProgram = DailyTrainingProgramAll
                            .Where(d => d.OneDayTrainingProgramId == trainingDayProgramId)
                            .OrderBy(d => d.VideoSerialNumber) 
                            .ToList();
                return filteredDailyTrainingProgram;
            }
            return null;
        }

        /// <summary>
        /// Получение одного для тренировочной программы для редактирования
        /// </summary>
        /// <param name="trainingDayProgramId"></param>
        /// <param name="serialNumberId"></param>
        /// <returns></returns>
        public async Task<List<DailyTraining>> GetDailyTrainingProgramInfoForUpdate(int trainingDayProgramId, int serialNumberId)
        {
            var response = await trainingProgrammsRepository.GetDailyTrainingProgramInfoRep();

            if (response.IsSuccessStatusCode)
            {
                var DailyTrainingProgramJson = await response.Content.ReadAsStringAsync();
                var DailyTrainingProgramAll = JsonConvert.DeserializeObject<List<DailyTraining>>(DailyTrainingProgramJson);
                var filteredDailyTrainingProgram = DailyTrainingProgramAll
                            .Where(d => d.OneDayTrainingProgramId == trainingDayProgramId)
                            .Where(d => d.VideoSerialNumber == serialNumberId)
                            .ToList();
                return filteredDailyTrainingProgram;
            }
            return null;
        }

        /// <summary>
        /// Изменение одного для тренировочной программы
        /// </summary>
        /// <param name="dailyTraining"></param>
        /// <param name="dailyTrainingId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDailyTraining(DailyTraining dailyTraining, int dailyTrainingId)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await trainingProgrammsRepository.UpdateDailyTraining(dailyTraining, dailyTrainingId);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Добавление одного для тренировочной программы
        /// </summary>
        /// <param name="dailyTraining"></param>
        /// <returns></returns>
        public async Task<bool> PostDailyTraining(DailyTraining dailyTraining)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await trainingProgrammsRepository.CreateNewDailyTrainings(dailyTraining);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Получение информации по видео
        /// </summary>
        /// <param name="trainingIds"></param>
        /// <returns></returns>
        public async Task<List<Models.Training>> GetTrainingsByIds(List<int> trainingIds)
        {
            var allTrainings = await GetTrainingProgram(); // Получаем все тренировочные программы

            // Фильтруем тренировки, возвращая только те, которые содержатся в списке ID
            var filteredTrainings = allTrainings.Where(t => trainingIds.Contains((int)t.IdTraining)).ToList();

            return filteredTrainings;
        }

        /// <summary>
        /// Начало выполнения тренировочной программы
        /// </summary>
        /// <param name="passing"></param>
        /// <returns></returns>
        public async Task<bool> StartPassingTrainingProgram(PassingTrainingProgram passing)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await trainingProgrammsRepository.PostStartPassingTrainingProgram(passing);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Прекращение выполнения программы 
        /// </summary>
        /// <param name="passingId"></param>
        /// <returns></returns>
        public async Task<bool> StopPassingTrainingProgram(int passingId)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await trainingProgrammsRepository.DeletePassingTrainingProgram(passingId);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Получение информации по прохождению в зависимости от пользователя
        /// </summary>
        /// <param name="trainingProgramId"></param>
        /// <returns></returns>
        public async Task<List<PassingTrainingProgram>> GetPassingTrainingPrograms(int trainingProgramId)
        {
            var response = await trainingProgrammsRepository.GetPassingTrainingProgram();

            if (response.IsSuccessStatusCode)
            {
                var PassingTrainingProgramJson = await response.Content.ReadAsStringAsync();
                var PassingTrainingProgramsAll = JsonConvert.DeserializeObject<List<PassingTrainingProgram>>(PassingTrainingProgramJson);
                var filteredPassingTrainingProgram = PassingTrainingProgramsAll
                            .Where(d => d.UserId == int.Parse(userID))
                            .Where(d => d.TrainingProgramId == trainingProgramId)
                            .ToList();
                return filteredPassingTrainingProgram;
            }
            return null;
        }

        /// <summary>
        /// Создание тренировок
        /// </summary>
        /// <param name="training"></param>
        /// <returns></returns>
        public async Task<bool> CreateNewTrainingVideo(Training training)
        {
            var response = await trainingProgrammsRepository.CreateNewTrainingVideo(training);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Создание тренировки и возвращение созданной тренировки
        /// </summary>
        /// <param name="training"></param>
        /// <returns></returns>
        public async Task<Training> CreateNewVideoTrainingForArchive(Training training)
        {

            var response = await trainingProgrammsRepository.CreateNewTrainingVideo(training);
            if (response.IsSuccessStatusCode)
            {
                var trainingJson = await response.Content.ReadAsStringAsync();
                var createdTraining = JsonConvert.DeserializeObject<Training>(trainingJson);
                return createdTraining;
            }
            return null;
        }

        /// <summary>
        /// Изменение/удаление/восстановление статьи
        /// </summary>
        /// <param name="training"></param>
        /// <param name="trainingId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTraining(Training training, int trainingId)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await trainingProgrammsRepository.UpdateTraining(training, trainingId);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Изменение тренировочной программы
        /// </summary>
        /// <param name="trainingProgram"></param>
        /// <param name="trainingProgramId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTrainingProgram(TrainingProgram trainingProgram, int trainingProgramId)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await trainingProgrammsRepository.UpdateTrainingProgram(trainingProgram, trainingProgramId);

            return response.IsSuccessStatusCode;
        }


        /// <summary>
        /// Изменение статуса в журнале при завершении трен.программы
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateStatusTrainingLogs ()
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await trainingProgrammsRepository.UpdateStatusTrainingLogs();

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Запись в журнале
        /// </summary>
        /// <param name="trainingLog"></param>
        /// <returns></returns>
        public async Task<bool> PostPassingOneDayTrainingLogs(TrainingLog trainingLog)
        {
            trainingLog.UserId = int.Parse(userID);
            // Вызов метода репозитория для отправки запроса к API
            var response = await trainingProgrammsRepository.PostTrainingLogs(trainingLog);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Получение записей из журнала
        /// </summary>
        /// <param name="dateTrainingLog"></param>
        /// <returns></returns>
        public async Task<List<TrainingLog>> GetTrainingLogs(DateTime dateTrainingLog)
        {
            var response = await trainingProgrammsRepository.GetTrainingLogs();

            if (response.IsSuccessStatusCode)
            {
                var trainingLogJson = await response.Content.ReadAsStringAsync();
                var trainingLogsAll = JsonConvert.DeserializeObject<List<TrainingLog>>(trainingLogJson);
                var filteredPassingTrainingProgram = trainingLogsAll
                            .Where(d => d.UserId == int.Parse(userID))
                            .Where(d => d.DateTrainingLog == dateTrainingLog)
                            .Where(d => d.DescriptionTrainingLog != null)
                            .ToList();
                return filteredPassingTrainingProgram;
            }
            return null;
        }

        public async Task<List<TrainingLog>> GetTrainingInfoPassingDaysService()
        {
            var response = await trainingProgrammsRepository.GetTrainingLogs();

            if (response.IsSuccessStatusCode)
            {
                var trainingLogJson = await response.Content.ReadAsStringAsync();
                var trainingLogsAll = JsonConvert.DeserializeObject<List<TrainingLog>>(trainingLogJson);
                var filteredPassingTrainingProgram = trainingLogsAll
                            .Where(d => d.UserId == int.Parse(userID))
                            .Where(d => d.DescriptionTrainingLog == null)
                            .Where(d => d.StatusDone == true)
                            .ToList();
                return filteredPassingTrainingProgram;
            }
            return null;
        }

        /// <summary>
        /// Получение оценки за каждый день в выбранном периоде
        /// </summary>
        /// <param name="dateTrainingLogOne"></param>
        /// <param name="dateTrainingLogTwo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<TrainingLog>> GetTrainingLogsRating(DateTime dateTrainingLogOne, DateTime dateTrainingLogTwo)
        {
            var response = await trainingProgrammsRepository.GetTrainingLogs();

            if (response.IsSuccessStatusCode)
            {
                var trainingLogJson = await response.Content.ReadAsStringAsync();
                var trainingLogsAll = JsonConvert.DeserializeObject<List<TrainingLog>>(trainingLogJson);
                var filteredTrainingLogs = trainingLogsAll
                    .Where(log => log.DateTrainingLog >= dateTrainingLogOne && log.DateTrainingLog <= dateTrainingLogTwo)
                    .Where(d => d.UserId == int.Parse(userID))
                    .Where(d => d.DescriptionTrainingLog != null)
                    .OrderBy(log => log.DateTrainingLog) 
                    .ToList();

                return filteredTrainingLogs;
            }
            else
            {
                throw new Exception("Error");
            }
        }

        /// <summary>
        /// Получение средней оценки за каждый месяц
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Dictionary<string, double?>> GetAverageRatingByMonth()
        {
            var response = await trainingProgrammsRepository.GetTrainingLogs();

            if (response.IsSuccessStatusCode)
            {
                var trainingLogJson = await response.Content.ReadAsStringAsync();
                var trainingLogsAll = JsonConvert.DeserializeObject<List<TrainingLog>>(trainingLogJson);
                var currentDate = DateTime.Now;

                var filteredTrainingLogs = trainingLogsAll
                    .Where(log => log.DateTrainingLog.HasValue && log.DateTrainingLog.Value.Year == currentDate.Year) // Отфильтровать по текущему году
                    .Where(d => d.UserId == int.Parse(userID))
                    .Where(d => d.DescriptionTrainingLog != null)
                    .OrderBy(log => log.DateTrainingLog)
                    .ToList();

                var averageRatingByMonth = filteredTrainingLogs
                    .GroupBy(log => log.DateTrainingLog.Value.Month) // Группировка по месяцам
                    .ToDictionary(
                         group =>
                         {
                             var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key); // Получение названия месяца на текущем языке
                             return char.ToUpper(monthName[0]) + monthName.Substring(1); // Преобразование первой буквы в верхний регистр
                         },
                        group => group.Average(log => log.Rating) // Вычисление среднего значения Rating для каждого месяца
                    );

                return averageRatingByMonth;
            }
            else
            {
                throw new Exception("Error");
            }
        }


        /// <summary>
        /// Изменение информации в журнале
        /// </summary>
        /// <param name="trainingLog"></param>
        /// <param name="trainingLogId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTrainingLogs(TrainingLog trainingLog, int trainingLogId)
        {
            trainingLog.UserId = int.Parse(userID);
            // Вызов метода репозитория для отправки запроса к API
            var response = await trainingProgrammsRepository.UpdateTrainingLogs(trainingLog, trainingLogId);

            return response.IsSuccessStatusCode;
        }
    }
}
