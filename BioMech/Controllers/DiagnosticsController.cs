using BioMech.Models;
using BioMech.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static Google.Apis.Requests.BatchRequest;

namespace BioMech.Controllers
{
    public class DiagnosticsController : Controller
    {
        private readonly DiagnosticsService diagnosticsService;
        public DiagnosticsController(DiagnosticsService diagnosticsService)
        {
            this.diagnosticsService = diagnosticsService;
        }

        /// <summary>
        /// Страница описание диагностик
        /// </summary>
        /// <returns></returns>
        public IActionResult DiagnosticsDescription()
        {
            return View();
        }
        /// <summary>
        /// Страница диагностики крыловидных лопаток
        /// </summary>
        /// <returns></returns>
        public IActionResult DiagnosticsBlade()
        {
            return View();
        }
        /// <summary>
        /// Страница диагностики колен
        /// </summary>
        /// <returns></returns>
        public IActionResult DiagnosticsKnees()
        {
            return View();
        }
        /// <summary>
        /// Страница диагностики стопы
        /// </summary>
        /// <returns></returns>
        public IActionResult DiagnosticsFeet()
        {
            return View();
        }
        /// <summary>
        /// Страница диагностики тела
        /// </summary>
        /// <returns></returns>
        public IActionResult DiagnosticsBody()
        {
            return View();
        }
        /// <summary>
        /// Страница диагностики положения шеи
        /// </summary>
        /// <returns></returns>
        public IActionResult DiagnosticsNeck()
        {
            return View();
        }

        /// <summary>
        /// Загрузка фотографии модели в FireBase
        /// </summary>
        /// <param name="photoForModel"></param>
        /// <param name="modelType"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadPhotoForModel(IFormFile photoForModel, string modelType)
        {
            if (photoForModel != null && photoForModel.Length > 0)
            {
                var fileName = Path.GetFileName(photoForModel.FileName);
                using (var stream = photoForModel.OpenReadStream())
                {
                    var link = await diagnosticsService.UploadPhotoForModel(stream, fileName);

                    if (!string.IsNullOrEmpty(link))
                    {

                        IActionResult actionResult;

                        switch (modelType)
                        {
                            case "ShoulderBlades":
                                actionResult = await DetectShoulderBlades(link);
                                break;
                            case "FootBone":
                                actionResult = await DetectFootBone(link);
                                break;
                            case "NeckProtraction":
                                actionResult = await DetectNeckProtraction(link);
                                break;
                            case "KneesProblems":
                                actionResult = await DetectKneesProblems(link);
                                break;

                            default:
                                return BadRequest("Invalid model type");
                        }


                        if (actionResult is OkObjectResult okResult)
                        {
                            var createNotesPhotoDiagnostics = new PhotoDiagnostic { };


                            if (modelType == "ShoulderBlades" || modelType == "FootBone") 

                            {
                                
                                DetectModelYoloResponse imageUrlResponse;
                                imageUrlResponse = okResult.Value as DetectModelYoloResponse;

                                createNotesPhotoDiagnostics.Photo = imageUrlResponse.ImageUrl;


                                if (DateTime.TryParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createDate))
                                {
                                    createNotesPhotoDiagnostics.DateDownload = createDate;
                                }

                                switch (modelType)
                                {
                                    case "ShoulderBlades":

                                        
                                        createNotesPhotoDiagnostics.DiagnosticsCategoryId = 1;

                                        if (imageUrlResponse.LabelUrl == null) { createNotesPhotoDiagnostics.ProblemCategoryId = 1; }
                                        else { createNotesPhotoDiagnostics.ProblemCategoryId = 2; }

                                        await diagnosticsService.CreateNotesPhotoDiagnostics(createNotesPhotoDiagnostics);

                                        break;
                                    case "FootBone":

                                        createNotesPhotoDiagnostics.DiagnosticsCategoryId = 3;

                                        if (imageUrlResponse.LabelUrl == null) { createNotesPhotoDiagnostics.ProblemCategoryId = 1; }
                                        else { createNotesPhotoDiagnostics.ProblemCategoryId = 5; }

                                        await diagnosticsService.CreateNotesPhotoDiagnostics(createNotesPhotoDiagnostics);

                                        break;
                                    default:
                                        return BadRequest("Invalid model type");
                                }


                                if (imageUrlResponse != null && imageUrlResponse.ImageUrl != null)
                                {
                                    return Ok(new { imageUrl = imageUrlResponse.ImageUrl, labelUrl = imageUrlResponse.LabelUrl });
                                }
                                else
                                {
                                    return BadRequest("Failed to process image");
                                }

                            }
                            else
                            {
                                DetectModelOpenPoseResponse imageUrlResponse;
                                imageUrlResponse = okResult.Value as DetectModelOpenPoseResponse;

                                createNotesPhotoDiagnostics.Photo = imageUrlResponse.ImageUrl;


                                if (DateTime.TryParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createDate))
                                {
                                    createNotesPhotoDiagnostics.DateDownload = createDate;
                                }

                                double angle;

                                // Преобразование градусов и условие заполнения бд в зависимости от числа
                                if (double.TryParse(imageUrlResponse.DegreeAngle, NumberStyles.Any, CultureInfo.InvariantCulture, out angle))
                                {
                                    switch (modelType)
                                    {
                                        case "NeckProtraction":
                                            createNotesPhotoDiagnostics.DiagnosticsCategoryId = 5; // Шейный отдел

                                            if (angle > 7)
                                            {
                                                createNotesPhotoDiagnostics.ProblemCategoryId = 8; // Протракция шеи
                                            }
                                            else
                                            {
                                                createNotesPhotoDiagnostics.ProblemCategoryId = 1; // Норма
                                            }

                                            await diagnosticsService.CreateNotesPhotoDiagnostics(createNotesPhotoDiagnostics);

                                            break;

                                        case "KneesProblems":
                                            createNotesPhotoDiagnostics.DiagnosticsCategoryId = 2;
                                           
                                            if (angle <= 174)
                                            {
                                                createNotesPhotoDiagnostics.ProblemCategoryId = 3;
                                            }
                                            else if (angle >= 182)
                                            {
                                                createNotesPhotoDiagnostics.ProblemCategoryId = 4;
                                            }
                                            else if (angle < 182 && angle > 174)
                                            {
                                                createNotesPhotoDiagnostics.ProblemCategoryId = 1;
                                            }

                                            await diagnosticsService.CreateNotesPhotoDiagnostics(createNotesPhotoDiagnostics);
                                            break;

                                        default:
                                            return BadRequest("Invalid model type");
                                    }
                                }
                                else
                                {

                                }

                               return Ok(new { imageUrl = imageUrlResponse.ImageUrl, degreeAngle = imageUrlResponse.DegreeAngle });
                            }

                            
                        }
                        else
                        {
                            return BadRequest("Failed to process image");
                        }
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
        /// Работа с модулью компьютерного зрения "Крыловидные лопатки"
        /// </summary>
        /// <param name="linkPhotoImage"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DetectShoulderBlades(string linkPhotoImage)
        {

            var detectShoulderBlades = new DetectModelYolo
            {
                ImagePath = linkPhotoImage
            };


            var detectionResult = await diagnosticsService.DetectShoulderBlades(detectShoulderBlades);

            if (detectionResult != null)
            {

                return Ok(detectionResult);
            }
            else
            {
                return BadRequest("Load model failed");
            }
        }

        /// <summary>
        /// Работа с модулью компьютерного зрения "Косточка на стопе"
        /// </summary>
        /// <param name="linkPhotoImage"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DetectFootBone(string linkPhotoImage)
        {

            var detectFootBone = new DetectModelYolo
            {
                ImagePath = linkPhotoImage
            };

            var detectionResult = await diagnosticsService.DetectFootBone(detectFootBone);

            if (detectionResult != null)
            {
                return Ok(detectionResult);;
            }
            else
            {
                return BadRequest("Load model failed");
            }
        }

        /// <summary>
        /// Работа с модулью компьютерного зрения "Протракция шеи"
        /// </summary>
        /// <param name="linkPhotoImage"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DetectNeckProtraction(string linkPhotoImage)
        {

            var detectNeckProtraction = new DetectModelOpenPose
            {
                ImagePath = linkPhotoImage
            };

            var detectionResult = await diagnosticsService.DetectNeckProtraction(detectNeckProtraction);

            if (detectionResult != null)
            {

                return Ok(detectionResult);
            }
            else
            {
                return BadRequest("Load model failed");
            }
        }

        /// <summary>
        /// Работа с модулью компьютерного зрения "Варус или вальгус колен"
        /// </summary>
        /// <param name="linkPhotoImage"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DetectKneesProblems(string linkPhotoImage)
        {

            var detectKneesProblems = new DetectModelOpenPose
            {
                ImagePath = linkPhotoImage
            };

            var detectionResult = await diagnosticsService.DetectKneesProblems(detectKneesProblems);

            if (detectionResult != null)
            {

                return Ok(detectionResult);
            }
            else
            {
                return BadRequest("Load model failed");
            }
        }


    }
}
