using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BioMech.Controllers;
using BioMech.Models;
using BioMech.Repositories;
using BioMech.Services;
using Microsoft.Extensions.Configuration;

namespace Diagnostics.nUnitTests
{
    public class DiagnosticsControllerIntegrationTests
    {
        private DiagnosticsController _controller;

        [SetUp]
        public void Setup()
        {
            // Конфигурация
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Создание ApiSettings
            var apiSettings = new ApiSettings();
            configuration.GetSection("ApiSettings").Bind(apiSettings);

            // Создание HttpContextAccessor с настроенной сессией пользователя
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            httpContext.Session = session;

            var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

            // Создание сервисов и контроллера 
            var diagnosticsRepository = new DiagnosticsRepository(apiSettings, httpContextAccessor);
            var diagnosticsService = new DiagnosticsService(diagnosticsRepository, configuration, httpContextAccessor);
            _controller = new DiagnosticsController(diagnosticsService);
        }

        [Test]
        public async Task UploadPhotoForModel_NullPhoto_ShoulderBlades_ReturnsBadRequest()
        {
            IFormFile formFile = null;
            var modelType = "ShoulderBlades";

            var result = await _controller.UploadPhotoForModel(formFile, modelType) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.AreEqual("Invalid file", result.Value); 
        }

        [Test]
        public async Task UploadPhotoForModel_NullPhoto_NeckProtraction_ReturnsBadRequest()
        {
            IFormFile formFile = null;
            var modelType = "NeckProtraction";

            var result = await _controller.UploadPhotoForModel(formFile, modelType) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.AreEqual("Invalid file", result.Value);
        }

        [Test]
        public async Task UploadPhotoForModel_NullPhoto_FootBone_ReturnsBadRequest()
        {
            IFormFile formFile = null;
            var modelType = "FootBone";

            var result = await _controller.UploadPhotoForModel(formFile, modelType) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.AreEqual("Invalid file", result.Value);
        }

        [Test]
        public async Task UploadPhotoForModel_NullPhoto_KneesProblems_ReturnsBadRequest()
        {
            IFormFile formFile = null;
            var modelType = "KneesProblems";

            var result = await _controller.UploadPhotoForModel(formFile, modelType) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.AreEqual("Invalid file", result.Value);
        }

        [Test]
        public async Task UploadPhotoForModel_ShoulderBlades_ValidInput_ReturnsOk()
        {
            var directory = Directory.GetCurrentDirectory();
            var imagePath = Path.Combine(directory, "test", "blades.jpg");

            var fileBytes = File.ReadAllBytes(imagePath);

            using (var memoryStream = new MemoryStream(fileBytes))
            {
                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "photoForModel", Path.GetFileName(imagePath));

                var result = await _controller.UploadPhotoForModel(formFile, "ShoulderBlades") as OkObjectResult;

                Assert.IsNotNull(result);
                Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
                Assert.IsNotNull(result.Value);
            }
        }

        [Test]
        public async Task UploadPhotoForModel_FeetBone_ValidInput_ReturnsOk()
        {
            var directory = Directory.GetCurrentDirectory();
            var imagePath = Path.Combine(directory, "test", "feet.PNG");

            var fileBytes = File.ReadAllBytes(imagePath);

            using (var memoryStream = new MemoryStream(fileBytes))
            {
                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "photoForModel", Path.GetFileName(imagePath));

                var result = await _controller.UploadPhotoForModel(formFile, "FootBone") as OkObjectResult;

                Assert.IsNotNull(result);
                Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
                Assert.IsNotNull(result.Value);
            }
        }

        [Test]
        public async Task UploadPhotoForModel_NeckProtraction_ValidInput_ReturnsOk()
        {
            var directory = Directory.GetCurrentDirectory();
            var imagePath = Path.Combine(directory, "test", "neck.PNG");

            var fileBytes = File.ReadAllBytes(imagePath);

            using (var memoryStream = new MemoryStream(fileBytes))
            {
                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "photoForModel", Path.GetFileName(imagePath));

                var result = await _controller.UploadPhotoForModel(formFile, "NeckProtraction") as OkObjectResult;

                Assert.IsNotNull(result);
                Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
                Assert.IsNotNull(result.Value);
            }
        }

        [Test]
        public async Task UploadPhotoForModel_KneesProblems_ValidInput_ReturnsOk()
        {
            var directory = Directory.GetCurrentDirectory();
            var imagePath = Path.Combine(directory, "test", "knees.PNG");

            var fileBytes = File.ReadAllBytes(imagePath);

            using (var memoryStream = new MemoryStream(fileBytes))
            {
                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "photoForModel", Path.GetFileName(imagePath));

                var result = await _controller.UploadPhotoForModel(formFile, "KneesProblems") as OkObjectResult;

                Assert.IsNotNull(result);
                Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
                Assert.IsNotNull(result.Value);
            }
        }

    }


    // Класс для имитации сессии
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionDictionary = new Dictionary<string, byte[]>();

        public string Id => throw new NotImplementedException();

        public bool IsAvailable => throw new NotImplementedException();

        public IEnumerable<string> Keys => _sessionDictionary.Keys;

        public void Clear()
        {
            _sessionDictionary.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            _sessionDictionary.Remove(key);
        }

        public void Set(string key, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            _sessionDictionary[key] = bytes;
        }

        public void Set(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return _sessionDictionary.TryGetValue(key, out value);
        }
    }
}
