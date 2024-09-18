using BioMech.Models;
using BioMech.Repositories;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace BioMech.Services
{
    public class DiagnosticsService
    {
        private readonly DiagnosticsRepository diagnosticsRepository;

        private readonly IConfiguration _configuration; // Обращение к конфигурационному файлу

        private string linkPhotoProfile; // Ссылка загруженной фотографии, сгенерируемая Firebase

        private string userID; // ID пользователя, находящегося в сессии

        private readonly IHttpContextAccessor _httpContextAccessor; // интерфейс для доступа к HttpContext

        public DiagnosticsService(DiagnosticsRepository diagnosticsRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.diagnosticsRepository = diagnosticsRepository;
            _configuration = configuration; // Обращение к конфигурационному файлу  
            _httpContextAccessor = httpContextAccessor;
            userID = _httpContextAccessor.HttpContext.Session.GetString("UserID")?.Trim('"'); // ID пользователя, находящегося в сессии
        }

        /// <summary>
        /// Загрузка фотографий модели в облачное хранилище Firebase
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> UploadPhotoForModel(Stream stream, string fileName)
        {
            var firebaseSettings = _configuration.GetSection("FirebaseSettings");
            var auth = new FirebaseAuthProvider(new FirebaseConfig(firebaseSettings["ApiKey"]));
            await auth.SignInWithEmailAndPasswordAsync(firebaseSettings["AuthEmail"], firebaseSettings["AuthPassword"]);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                firebaseSettings["Bucket"],
                new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                })
                .Child("Models")
                .Child("Results")
                .Child("User_ID_" + userID)
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
        /// Работа с моделью "Крыловидные лопатки" 
        /// </summary>
        /// <param name="shoulderBlades"></param>
        /// <returns></returns>
        public async Task<DetectModelYoloResponse?> DetectShoulderBlades(DetectModelYolo shoulderBlades)
        {
            var firebaseSettings = _configuration.GetSection("FirebaseSettings");
            var auth = new FirebaseAuthProvider(new FirebaseConfig(firebaseSettings["ApiKey"]));
            var a = await auth.SignInWithEmailAndPasswordAsync(firebaseSettings["AuthEmail"], firebaseSettings["AuthPassword"]);

            var cancellation = new CancellationTokenSource();

            var storage = new FirebaseStorage(firebaseSettings["Bucket"], new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                });

            var fileStoragePath = $"Models/ModelShoulderBlades.pt";

            try
            {
                // Получение прямой ссылки на файл модели
                var modelUrl = await storage.Child(fileStoragePath).GetDownloadUrlAsync();

                shoulderBlades.PathToModel = modelUrl;
                shoulderBlades.UserID = userID;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception was thrown while getting model URL: {ex}");
                return null;
            }

            HttpResponseMessage response;
            try
            {
                response = await diagnosticsRepository.DetectShoulderBlades(shoulderBlades);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling API: {ex.Message}");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                return new DetectModelYoloResponse { ImageUrl = result.imageUrl, LabelUrl = result.labelUrl };
            }
            else
            {
                Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                return null;
            }
        }

        /// <summary>
        /// Добавление фотографии результата в историю диагностики
        /// </summary>
        /// <param name="photoDiagnostic"></param>
        /// <returns></returns>
        public async Task<bool> CreateNotesPhotoDiagnostics(PhotoDiagnostic photoDiagnostic)
        {

            try
            {
                photoDiagnostic.UserId = int.Parse(userID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception was thrown while getting model URL: {ex}");
                return false;
            }
            var response = await diagnosticsRepository.CreateNotesPhotoDiagnostics(photoDiagnostic);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Работа с моделью "Косточка на стопе" 
        /// </summary>
        /// <param name="footBone"></param>
        /// <returns></returns>
        public async Task<DetectModelYoloResponse?> DetectFootBone(DetectModelYolo footBone)
        {
            var firebaseSettings = _configuration.GetSection("FirebaseSettings");
            var auth = new FirebaseAuthProvider(new FirebaseConfig(firebaseSettings["ApiKey"]));
            var a = await auth.SignInWithEmailAndPasswordAsync(firebaseSettings["AuthEmail"], firebaseSettings["AuthPassword"]);

            var cancellation = new CancellationTokenSource();

            var storage = new FirebaseStorage(firebaseSettings["Bucket"], new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true
            });

            var fileStoragePath = $"Models/FootBoneModel.pt";

            try
            {
                // Получение прямой ссылки на файл модели
                var modelUrl = await storage.Child(fileStoragePath).GetDownloadUrlAsync();

                footBone.PathToModel = modelUrl;
                footBone.UserID = userID;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception was thrown while getting model URL: {ex}");
                return null;
            }

            HttpResponseMessage response;
            try
            {
                response = await diagnosticsRepository.DetectFootBone(footBone);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling API: {ex.Message}");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                return new DetectModelYoloResponse { ImageUrl = result.imageUrl, LabelUrl = result.labelUrl };
            }
            else
            {
                Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                return null;
            }
        }

        /// <summary>
        /// Работа с моделью "Протракция шеи" 
        /// </summary>
        /// <param name="neckProtraction"></param>
        /// <returns></returns>
        public async Task<DetectModelOpenPoseResponse?> DetectNeckProtraction(DetectModelOpenPose neckProtraction)
        {

            try
            {
                neckProtraction.UserID = userID;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception was thrown while getting model URL: {ex}");
                return null;
            }

            HttpResponseMessage response;
            try
            {
                response = await diagnosticsRepository.DetectNeckProtraction(neckProtraction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling API: {ex.Message}");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                return new DetectModelOpenPoseResponse { DegreeAngle = result.degreeAngle, ImageUrl = result.imageUrl };
            }
            else
            {
                Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                return null;
            }
        }

        /// <summary>
        /// Работа с моделью "Вальгусные или варусные колени" 
        /// </summary>
        /// <param name="kneesProblems"></param>
        /// <returns></returns>
        public async Task<DetectModelOpenPoseResponse?> DetectKneesProblems(DetectModelOpenPose kneesProblems)
        {

            try
            {
                kneesProblems.UserID = userID;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception was thrown while getting model URL: {ex}");
                return null;
            }

            HttpResponseMessage response;
            try
            {
                response = await diagnosticsRepository.DetectKneesProblems(kneesProblems);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling API: {ex.Message}");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                return new DetectModelOpenPoseResponse { DegreeAngle = result.degreeAngle, ImageUrl = result.imageUrl };
            }
            else
            {
                Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                return null;
            }
        }


    }
}
