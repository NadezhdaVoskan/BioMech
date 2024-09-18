using BioMech.Models;
using BioMech.Services;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Runtime.InteropServices;

namespace BioMech.Controllers
{
    public class TrainingProgramsController : Controller
    {

        private readonly TrainingProgramsService trainingProgramsService;
        public TrainingProgramsController(TrainingProgramsService trainingProgramsService)
        {
            this.trainingProgramsService = trainingProgramsService;
        }

        /// <summary>
        /// Страница тренировочных программ всех
        /// </summary>
        /// <returns></returns>
        public IActionResult Programs()
        {
            return View();
        }

        /// <summary>
        /// Страница опубликованных видео тренировок
        /// </summary>
        /// <returns></returns>
        public IActionResult PublishedProgramsVideo()
        {
            return View();
        }

        /// <summary>
        /// Создание видео программ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreatePrograms(int? id = null)
        {
            var model = new TrainingsModel();
            model.TrainingCategories = await trainingProgramsService.GetTrainingCategory(); // Загрузка категорий

            if (id.HasValue)
            {
                var training = await trainingProgramsService.GetInfoTrainingProgramsById(id.Value);
                model.Training = training;
            }
            else
            {
                model.Training = new Training();
            }

            return View(model);
        }

        /// <summary>
        /// Создание статьи
        /// </summary>
        /// <param name="nameTraining"></param>
        /// <param name="shortDescription"></param>
        /// <param name="linkVideoYouTube"></param>
        /// <param name="categoryTrainingProgramId"></param>
        /// <param name="imageVideoTraining"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateNewTrainingVideo(string nameTraining, string shortDescription, string linkVideoYouTube, int categoryTrainingProgramId, string imageVideoTraining)
        {
            var training = new Training
            {
               NameTraining = nameTraining,
               DescriptionTraining = shortDescription,
               LinkVideo = linkVideoYouTube,
               TrainingCategoryId = categoryTrainingProgramId,
               Photo_Training_Video = imageVideoTraining
            };

            var isSuccess = await trainingProgramsService.CreateNewTrainingVideo(training);

            if (isSuccess)
            {
                return RedirectToAction("Programs");
            }
            else
            {
                return BadRequest("Failed to create training video");
            }
        }

        /// <summary>
        /// Создание и добавление в архив новой статьи
        /// </summary>
        /// <param name="nameTraining"></param>
        /// <param name="shortDescription"></param>
        /// <param name="linkVideoYouTube"></param>
        /// <param name="categoryTrainingProgramId"></param>
        /// <param name="imageVideoTraining"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateTrainingVideoAndArchive(string nameTraining, string shortDescription, string linkVideoYouTube, int categoryTrainingProgramId, string imageVideoTraining)
        {

            var training = new Training
            {
                NameTraining = nameTraining,
                DescriptionTraining = shortDescription,
                LinkVideo = linkVideoYouTube,
                TrainingCategoryId = categoryTrainingProgramId,
                Photo_Training_Video = imageVideoTraining
            };

            // Создание статьи
            var createdTraining = await trainingProgramsService.CreateNewVideoTrainingForArchive(training);

            if (createdTraining == null)
            {
                return BadRequest("Failed to create training");
            }

            // Архивирование статьи (пометка удаления)
            createdTraining.DeletedTraining = true;
            var archiveSuccess = await trainingProgramsService.UpdateTraining(createdTraining, (int)createdTraining.IdTraining);

            if (archiveSuccess)
            {
                return RedirectToAction("Programs");
            }
            else
            {
                return BadRequest("Failed to archive training");
            }
        }

        /// <summary>
        /// Изменение видео тренировки
        /// </summary>
        /// <param name="trainingId"></param>
        /// <param name="nameTraining"></param>
        /// <param name="shortDescription"></param>
        /// <param name="linkVideoYouTube"></param>
        /// <param name="categoryTrainingProgramId"></param>
        /// <param name="imageVideoTraining"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateTraining(int trainingId, string nameTraining, string shortDescription, string linkVideoYouTube, int categoryTrainingProgramId, string imageVideoTraining)
        {
            Training training = await trainingProgramsService.GetInfoTrainingProgramsById(trainingId);

            training.NameTraining = nameTraining;
            training.DescriptionTraining = shortDescription;
            training.LinkVideo = linkVideoYouTube;
            training.TrainingCategoryId = categoryTrainingProgramId;
            training.Photo_Training_Video = imageVideoTraining;

            var isSuccess = await trainingProgramsService.UpdateTraining(training, trainingId);

            if (isSuccess)
            {
                return RedirectToAction("Programs");
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Изменение одного дня тренировочной программы
        /// </summary>
        /// <param name="videoTrainingId"></param>
        /// <param name="trainingDayProgramId"></param>
        /// <param name="serialNumberId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ManageDailyTraining(int videoTrainingId, int trainingDayProgramId, int serialNumberId)
        {
            // Получение информации о тренировочной программе для обновления
            var dailyTrainingList = await trainingProgramsService.GetDailyTrainingProgramInfoForUpdate(trainingDayProgramId, serialNumberId);

            if (dailyTrainingList == null || !dailyTrainingList.Any())
            {
                // Создание новой записи, если данных нет
                var newDailyTraining = new DailyTraining
                {
                    OneDayTrainingProgramId = trainingDayProgramId,
                    VideoSerialNumber = serialNumberId,
                    TrainingId = videoTrainingId
                    
                };
                bool postResult = await trainingProgramsService.PostDailyTraining(newDailyTraining);
                if (postResult)
                    return RedirectToAction("TrainingPrograms");
                else
                    return StatusCode(500, "Failed to create new daily training.");
            }
            else
            {
                // Обновление существующих данных
                var dailyTrainingToUpdate = dailyTrainingList.First();
                dailyTrainingToUpdate.VideoSerialNumber = serialNumberId;
                dailyTrainingToUpdate.OneDayTrainingProgramId = trainingDayProgramId; 
                dailyTrainingToUpdate.TrainingId = videoTrainingId;
                bool updateResult = await trainingProgramsService.UpdateDailyTraining(dailyTrainingToUpdate, (int)dailyTrainingToUpdate.IdDailyTraining);
                if (updateResult)
                    return RedirectToAction("TrainingPrograms");
                else
                    return StatusCode(500, "Failed to update daily training.");
            }
        }

        /// <summary>
        /// Изменение тренировочной программы
        /// </summary>
        /// <param name="trainingProgramId"></param>
        /// <param name="nameTrainingProgram"></param>
        /// <param name="shortDescriptionTrainingProgram"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateTrainingProgram(int trainingProgramId, string nameTrainingProgram, string shortDescriptionTrainingProgram)
        {
            TrainingProgram trainingProgram = await trainingProgramsService.GetTrainingProgramInfo(trainingProgramId);

            trainingProgram.IdTrainingProgram = trainingProgramId;
            trainingProgram.NameTrainingProgram = nameTrainingProgram;
            trainingProgram.DescriptionTrainingProgram = shortDescriptionTrainingProgram;

            var isSuccess = await trainingProgramsService.UpdateTrainingProgram(trainingProgram, trainingProgramId);

            if (isSuccess)
            {
                return RedirectToAction("TrainingPrograms");
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Архивирование видео тренировки
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ArchivePrograms()
        {
            var allTraining = await trainingProgramsService.GetTrainingProgramArchive();
            var model = new TrainingsModel { TrainingsList = allTraining};

            return View(model);
        }

        /// <summary>
        /// Вывод всех видео тренировочных программ
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> TrainingPrograms()
        {
            var allTrainingProgram = await trainingProgramsService.GetAllViewTrainingProgram();
            var model = new TrainingsModel { TrainingsProgramsList = allTrainingProgram };

            return View(model);
        }

        /// <summary>
        ///  Метод для просмотра тренировочных программ.
        /// </summary>
        /// <param name="dayTrainingProgram"></param>
        /// <param name="trainingProgramID"></param>
        /// <returns></returns>
        public async Task<IActionResult> ViewingTrainingPrograms(int dayTrainingProgram, int trainingProgramID)
        {
            var trainingProgram = await trainingProgramsService.GetTrainingProgramInfo(trainingProgramID);

            if (trainingProgram != null)
            {
                var oneDayTrainingProgram = await trainingProgramsService.GetOneDayTrainingProgramInfo(trainingProgramID);
                if (oneDayTrainingProgram != null)
                {
                    var dailyTrainingProgram = await trainingProgramsService.GetDailyTrainingProgramInfo(dayTrainingProgram);
                    if (dailyTrainingProgram != null)
                    {
                        var trainingIds = dailyTrainingProgram
                                          .Select(dt => dt.TrainingId.Value)
                                          .Distinct()
                                          .ToList();

                        var trainings = await trainingProgramsService.GetTrainingsByIds(trainingIds);

                        // Словарь для связи ID тренировки с данными тренировки
                        var trainingsDictionary = trainings.ToDictionary(t => t.IdTraining);

                        var modelTrainingProgram = new TrainingProgramModel
                        {
                            TrainingProgram = trainingProgram,
                            OneDayTrainingProgram = oneDayTrainingProgram,
                            DailyTraining = dailyTrainingProgram,
                            Training = dailyTrainingProgram
                                .Where(dt => dt.TrainingId.HasValue && trainingsDictionary.ContainsKey(dt.TrainingId.Value))
                                .Select(dt => trainingsDictionary[dt.TrainingId.Value])
                                .ToList()
                        };

                        return View(modelTrainingProgram);
                    }
                }
            }

            return View("Error");
        }

        /// <summary>
        /// Редактирование программ тренировок.
        /// </summary>
        /// <param name="trainingProgramID"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditingPrograms(int trainingProgramID)
        {
            var trainingProgram = await trainingProgramsService.GetTrainingProgramInfo(trainingProgramID);
            var oneDayTrainingProgram = await trainingProgramsService.GetDaysTrainingProgramInfo(trainingProgramID);
            var allTraining = await trainingProgramsService.GetTrainingProgram();
            var model = new TrainingProgramModel { TrainingProgram = trainingProgram, OneDayTrainingProgram = oneDayTrainingProgram, Training = allTraining };

            return View(model);
        }

        /// <summary>
        /// Метод для удаления тренировки.
        /// </summary>
        /// <param name="trainingId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteTraining(int trainingId)
        {
            Training training = await trainingProgramsService.GetInfoTrainingProgramsById(trainingId);

            training.DeletedTraining = true;

            var isSuccess = await trainingProgramsService.UpdateTraining(training, trainingId);

            if (isSuccess)
            {
                return RedirectToAction("Programs");
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Метод для публикации тренировки из архива
        /// </summary>
        /// <param name="trainingId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PublicTraining(int trainingId)
        {
            Training training = await trainingProgramsService.GetInfoTrainingProgramsById(trainingId);

            training.DeletedTraining = false;

            var isSuccess = await trainingProgramsService.UpdateTraining(training, trainingId);

            if (isSuccess)
            {
                return RedirectToAction("Programs");
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Вывод информации конкретной тренировочной программы
        /// </summary>
        /// <param name="dayTrainingProgram"></param>
        /// <param name="trainingProgramID"></param>
        /// <returns></returns>
        public async Task<IActionResult> ProgramsCorrection(int dayTrainingProgram, int trainingProgramID)
        {
            
            var trainingProgram = await trainingProgramsService.GetTrainingProgramInfo(trainingProgramID);

            if (trainingProgram != null)
            {
                var oneDayTrainingProgram = await trainingProgramsService.GetOneDayTrainingProgramInfo(trainingProgramID);
                if (oneDayTrainingProgram != null)
                {
                    var dailyTrainingProgram = await trainingProgramsService.GetDailyTrainingProgramInfo(dayTrainingProgram);
                    if (dailyTrainingProgram != null)
                    {
                        var trainingIds = dailyTrainingProgram
                                          .Select(dt => dt.TrainingId.Value)
                                          .Distinct()
                                          .ToList();

                        var trainings = await trainingProgramsService.GetTrainingsByIds(trainingIds);

                        // Словарь для связи ID тренировки с данными тренировки
                        var trainingsDictionary = trainings.ToDictionary(t => t.IdTraining);

                        var modelTrainingProgram = new TrainingProgramModel
                        {
                            TrainingProgram = trainingProgram,
                            OneDayTrainingProgram = oneDayTrainingProgram,
                            DailyTraining = dailyTrainingProgram,
                            Training = dailyTrainingProgram
                                .Where(dt => dt.TrainingId.HasValue && trainingsDictionary.ContainsKey(dt.TrainingId.Value))
                                .Select(dt => trainingsDictionary[dt.TrainingId.Value])
                                .ToList()
                        };

                        return View(modelTrainingProgram);
                    }
                }
            }

            return View("Error");
        }

        /// <summary>
        /// Добавление записи в журнале тренировки
        /// </summary>
        /// <param name="trainingLogId"></param>
        /// <param name="dateTrainingLog"></param>
        /// <param name="descriptionTraining"></param>
        /// <param name="ratingTraining"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostTrainingLogCalendar(int? trainingLogId, DateTime dateTrainingLog, string descriptionTraining, int ratingTraining)
        {
            var trainingLog = new TrainingLog
            {
                StatusDone = false,
                DateTrainingLog = dateTrainingLog,
                DescriptionTrainingLog = descriptionTraining,
                Rating = ratingTraining
            };

            bool isSuccess = false;
            if (trainingLogId.HasValue)
            {
                trainingLog.IdTrainingLog = trainingLogId.Value;
                isSuccess = await trainingProgramsService.UpdateTrainingLogs(trainingLog, trainingLogId.Value);
            }
            else
            {
                isSuccess = await trainingProgramsService.PostPassingOneDayTrainingLogs(trainingLog);
            }

            if (isSuccess)
            {
                return RedirectToAction("Profile", "PersonalData");
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Отметка прохождения дня тренировочной программы
        /// </summary>
        /// <param name="dateTrainingLog"></param>
        /// <param name="oneDayTrainingProgramID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostPassingOneDayProgramTrainingLog(DateTime dateTrainingLog, int oneDayTrainingProgramID)
        {

            var trainingLog = new TrainingLog
            {
                StatusDone = true,
                OneDayTrainingProgramId = oneDayTrainingProgramID,
                DateTrainingLog = dateTrainingLog

            };

            var isSuccess = await trainingProgramsService.PostPassingOneDayTrainingLogs(trainingLog);

            if (isSuccess)
            {
                return RedirectToAction("ProgramsCorrection");
            }
            else
            {
                return BadRequest("");
            }

        }

        /// <summary>
        /// Изменение статуса выполнения тренировочной программы
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateStatusTrainingLogs()
        {
            var isSuccess = await trainingProgramsService.UpdateStatusTrainingLogs();

            if (isSuccess)
            {
                return RedirectToAction("ProgramsCorrection");
            }
            else
            {
                return BadRequest("");
            }

        }

        /// <summary>
        /// Получение записи из журнала тренировок по дате
        /// </summary>
        /// <param name="dateTrainingLog"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTrainingLogs(DateTime dateTrainingLog)
        {
            var trainingLogs = await trainingProgramsService.GetTrainingLogs(dateTrainingLog);
            var trainingLog = trainingLogs.First();

            if (trainingLogs != null)
            {
                return Json(new { rating = trainingLog.Rating, description = trainingLog.DescriptionTrainingLog, trainingId = trainingLog.IdTrainingLog });
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Получение отмеченных дней в проходимой пользователем тренировочной программе
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTrainingInfoPassingDays()
        {
            var trainingLogs = await trainingProgramsService.GetTrainingInfoPassingDaysService();

            return Json(trainingLogs);

        }

        /// <summary>
        /// Получение списка оценок по выбранному периоду
        /// </summary>
        /// <param name="dateTrainingLogOne"></param>
        /// <param name="dateTrainingLogTwo"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetTrainingRating(DateTime dateTrainingLogOne, DateTime dateTrainingLogTwo)
        {
            var trainingLogs = await trainingProgramsService.GetTrainingLogsRating(dateTrainingLogOne, dateTrainingLogTwo);

            if (trainingLogs != null && trainingLogs.Any())
            {
                var datesList = new List<string>();
                var ratingsList = new List<int>();

                foreach (var log in trainingLogs)
                {
                    if (log.DateTrainingLog.HasValue)
                    {
                        datesList.Add(log.DateTrainingLog.Value.ToShortDateString());
                        ratingsList.Add((int)log.Rating);
                    }
                }

                var ratings = new
                {
                    datesMark = datesList,
                    ratingsMark = ratingsList
                };

                return Json(ratings);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Получение средней оценки по месяцу
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetAverageRatingByMonth()
        {
            var data = await trainingProgramsService.GetAverageRatingByMonth();
            return Json(data);
        }


        /// <summary>
        /// Получение информации о видео тренировки
        /// </summary>
        /// <param name="dayTrainingProgram"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTrainingDetails(int dayTrainingProgram)
        {
            var dailyTrainingProgram = await trainingProgramsService.GetDailyTrainingProgramInfo(dayTrainingProgram);
            if (dailyTrainingProgram != null)
            {
                var trainingIds = dailyTrainingProgram.Select(dt => dt.TrainingId.Value).Distinct().ToList();
                var trainings = await trainingProgramsService.GetTrainingsByIds(trainingIds);
                var trainingsData = trainings.Select(t => new {
                    IdTraining = t.IdTraining,
                    NameTraining = t.NameTraining,
                    DescriptionTraining = t.DescriptionTraining,
                    Photo_Training_Video = t.Photo_Training_Video
                });
                return Json(trainingsData);
            }
            return Json(new { error = "No trainings found" });
        }

        /// <summary>
        /// Выполнение или завершение тренировочной программы
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="trainingProgramId"></param>
        /// <param name="checkOnly"></param>
        /// <returns></returns>
        public async Task<IActionResult> ToggleTrainingProgram(int userId, int trainingProgramId, bool checkOnly = false)
        {
            var existingPrograms = await trainingProgramsService.GetPassingTrainingPrograms(trainingProgramId);
            var existingProgram = existingPrograms.FirstOrDefault();

            if (checkOnly)
            {
                bool isStarted = existingProgram != null;
                return Json(new
                {
                    IsStarted = isStarted,
                    ButtonText = isStarted ? "Закончить выполнение программы" : "Начать выполнение программы"
                });
            }

            // Логика для изменения состояния программы
            bool wasStarted = existingProgram != null;
            if (wasStarted)
            {
                await trainingProgramsService.StopPassingTrainingProgram(existingProgram.IdPassingTrainingProgram.Value);
            }
            else
            {
                var newProgram = new PassingTrainingProgram
                {
                    UserId = userId,
                    TrainingProgramId = trainingProgramId,
                    StatusStart = true
                };
                await trainingProgramsService.StartPassingTrainingProgram(newProgram);
            }

            return Json(new
            {
                IsStarted = !wasStarted,
                ButtonText = !wasStarted ? "Закончить выполнение программы" : "Начать выполнение программы"
            });
        }


        /// <summary>
        /// Вывод видео тренировочных программ в зависимости от категории
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Program(int categoryId)
        {
            var trainings = await trainingProgramsService.GetTrainingProgramByCategory(categoryId);

            var categoryName = "Название категории";

            if (categoryId == 1) { categoryName = "Миофасциальный релиз"; }
            else if (categoryId == 2) { categoryName = "Дыхание"; }
            else if (categoryId == 3) { categoryName = "Стопы"; }
            else if (categoryId == 4) { categoryName = "Мобилизация грудного отдела"; }
            else if (categoryId == 5) { categoryName = "Ягодицы"; }
            else if (categoryId == 6) { categoryName = "Осанка"; }
            else if (categoryId == 7) { categoryName = "ТБС и МТД"; }
            else if (categoryId == 8) { categoryName = "Кор и пресс"; }
            else if (categoryId == 9) { categoryName = "Активация мышц"; }

            var viewModel = new TrainingCategoryViewModel
            {
                Trainings = trainings,
                CategoryName = categoryName
            };

            return View(viewModel);
        }

        /// <summary>
        /// Вывод видео программы
        /// </summary>
        /// <param name="idTrainingProgram"></param>
        /// <returns></returns>
        public async Task<IActionResult> ProgramVideo(int idTrainingProgram)
        {
            var trainingProgram = await trainingProgramsService.GetInfoTrainingProgramsById(idTrainingProgram);
            if (trainingProgram != null)
            {  
                return View(trainingProgram);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Метод для получения данных по категории
        /// </summary>
        /// <param name="idCategory"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetTrainingByCategory(int idCategory)
        {
            var trainings = await trainingProgramsService.GetTrainingProgramByCategory(idCategory);
            if (trainings != null)
            {
                return Json(trainings); 
            }
            else
            {
                return NotFound(); 
            }
        }

        /// <summary>
        /// Метод для получения информации по конкретной тренировочной программе
        /// </summary>
        /// <param name="idTrainingProgram"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetInfoTrainingProgram(int idTrainingProgram)
        {
            var training = await trainingProgramsService.GetInfoTrainingProgramsById(idTrainingProgram);
            if (training != null)
            {
                return Json(training);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Поиск тренировочных программ
        /// </summary>
        /// <param name="queryTrainingPrograms"></param>
        /// <returns></returns>
        public async Task<IActionResult> SearchTrainingProgramsVideo(string queryTrainingPrograms)
        {
            if (string.IsNullOrEmpty(queryTrainingPrograms))
            {
                // Если запрос пустой, просто выводятся все тренировочные программы
                return RedirectToAction(nameof(Programs));
            }

            // Преобразование текст запроса к нижнему регистру
            var lowerQuery = queryTrainingPrograms.ToLower();

            // Выполнение поиска тренировочных программ без учета регистра
            var searchResults = await trainingProgramsService.SearchTrainingPrograms(lowerQuery);

            var articleCategories = await trainingProgramsService.GetTrainingProgram();

            var model = new TrainingCategoryViewModel { Trainings = searchResults, CategoryName = $"Результат поиска по запросу '{queryTrainingPrograms}'"};

            return View("Program", model);
        }

        /// <summary>
        /// Поиск тренировочных программ в архиве
        /// </summary>
        /// <param name="queryTrainingPrograms"></param>
        /// <returns></returns>
        public async Task<IActionResult> SearchTrainingProgramsVideoArchive(string queryTrainingPrograms)
        {
            if (string.IsNullOrEmpty(queryTrainingPrograms))
            {
                // Если запрос пустой, просто выводятся все тренировочные программы
                return RedirectToAction(nameof(Programs));
            }

            // Преобразование текст запроса к нижнему регистру
            var lowerQuery = queryTrainingPrograms.ToLower();

            // Выполнение поиска тренировочных программ без учета регистра
            var searchResults = await trainingProgramsService.SearchTrainingProgramsArchive(lowerQuery);

            var model = new TrainingsModel { TrainingsList = searchResults };

            return View("ArchivePrograms", model);
        }

    }
}
