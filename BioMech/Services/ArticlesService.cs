using BioMech.Models;
using BioMech.Repositories;
using Firebase.Auth;
using Firebase.Storage;
using Newtonsoft.Json;
using System.Globalization;

namespace BioMech.Services
{
    public class ArticlesService
    {

        private readonly ArticlesRepository articlesRepository;

        private readonly IConfiguration _configuration; // Обращение к конфигурационному файлу

        private string linkPhotoArticle; // Ссылка загруженной фотографии, сгенерируемая Firebase

        private string userID; // ID пользователя, находящегося в сессии

        private readonly IHttpContextAccessor _httpContextAccessor; // интерфейс для доступа к HttpContext

        public ArticlesService(ArticlesRepository articlesRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.articlesRepository = articlesRepository;

            _configuration = configuration; // Обращение к конфигурационному файлу

            _httpContextAccessor = httpContextAccessor;
            userID = _httpContextAccessor.HttpContext.Session.GetString("UserID")?.Trim('"'); // ID пользователя, находящегося в сессии
        }

        /// <summary>
        /// Получение информации конкретной статьи
        /// </summary>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public async Task<Models.Article> GetArticleInfo(int articleID)
        {
            var response = await articlesRepository.GetArticleInfo(articleID);
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();

                var article = JsonConvert.DeserializeObject<Models.Article>(userJson);

                return article;
            }
            return null;
        }

        /// <summary>
        /// Получение информации всех статей
        /// </summary>
        /// <returns></returns>
        public async Task<List<Models.Article>> GetAllArticlesInfo()
        {
            var response = await articlesRepository.GetAllArticlesInfo();
            if (response.IsSuccessStatusCode)
            {
                var articlesJson = await response.Content.ReadAsStringAsync();
                var articles = JsonConvert.DeserializeObject<List<Models.Article>>(articlesJson);
                var filteredArticleCategory = articles
                            .Where(d => d.DeletedArticle == null || d.DeletedArticle == false)
                            .ToList();
                return filteredArticleCategory;
            }
            return null;
        }

        /// <summary>
        /// Получение информации всех статей
        /// </summary>
        /// <returns></returns>
        public async Task<List<Models.Article>> GetAllArchiveArticlesInfo()
        {
            var response = await articlesRepository.GetAllArticlesInfo();
            if (response.IsSuccessStatusCode)
            {
                var articlesJson = await response.Content.ReadAsStringAsync();
                var articles = JsonConvert.DeserializeObject<List<Models.Article>>(articlesJson);
                var filteredArticleCategory = articles
                            .Where(d => d.DeletedArticle == true)
                            .ToList();
                return filteredArticleCategory;
            }
            return null;
        }


        /// <summary>
        /// Поиск статьи
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<Article>> SearchArticles(string query)
        {
            var allArticles = await GetAllArticlesInfo();

            // Преобразуйте текст запроса к нижнему регистру
            var lowerQuery = query.ToLower();

            // Выполните фильтрацию статей по названию или краткому описанию без учета регистра
            var searchResults = allArticles.Where(a => a.Name_Article.ToLower().Contains(lowerQuery) ||
                                                       a.ShortDescriptionArticle.ToLower().Contains(lowerQuery)).ToList();

            return searchResults;
        }

        /// <summary>
        /// Поиск статьи в архиве
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<Article>> SearchArticlesArchive(string query)
        {
            var allArticles = await GetAllArchiveArticlesInfo();

            // Преобразуйте текст запроса к нижнему регистру
            var lowerQuery = query.ToLower();

            // Выполните фильтрацию статей по названию или краткому описанию без учета регистра
            var searchResults = allArticles.Where(a => a.Name_Article.ToLower().Contains(lowerQuery) ||
                                                       a.ShortDescriptionArticle.ToLower().Contains(lowerQuery)).ToList();

            return searchResults;
        }

        /// <summary>
        /// Получение всех категорий статей
        /// </summary>
        /// <returns></returns>
        public async Task<List<Models.ArticleCategory>> ArticleCategories()
        {
            var response = await articlesRepository.GetArticleCategories();
            if (response.IsSuccessStatusCode)
            {
                var articlesJson = await response.Content.ReadAsStringAsync();
                var articlesCategories = JsonConvert.DeserializeObject<List<Models.ArticleCategory>>(articlesJson);
                var filteredArticleCategory = articlesCategories
                            .Where(d => d.DeletedCategoryArticle == null || d.DeletedCategoryArticle == false)
                            .ToList();
                return filteredArticleCategory;
            }
            return null;
        }

        /// <summary>
        /// Получение информации о категории статьи по id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<List<Models.ArticleCategory>> ArticleCategoriesById(int categoryId)
        {
            var response = await articlesRepository.GetArticleCategories();

            if (response.IsSuccessStatusCode)
            {
                var articlesJson = await response.Content.ReadAsStringAsync();
                var articlesCategories = JsonConvert.DeserializeObject<List<Models.ArticleCategory>>(articlesJson);
                var filteredArticleCategory = articlesCategories
                            .Where(d => d.IdArticleCategory == categoryId)
                            .ToList();

                return filteredArticleCategory;
            }
            return null;
        }

        /// <summary>
        /// Получение статьи по категории
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<List<Article>> GetArticlesByCategory(int categoryId)
        {
            var allArticles = await GetAllArticlesInfo();
            var articlesInCategory = allArticles.Where(a => a.ArticleCategoryId == categoryId).ToList();
            return articlesInCategory;
        }

        /// <summary>
        /// Создание статей
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public async Task<bool> CreateNewArticle(Article article)
        {
            article.UserId = int.Parse(userID);

            if (DateTime.TryParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createDate))
            {
                article.PublicationDateArticle = createDate;
            }

            var response = await articlesRepository.CreateNewArticle(article);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Создание статей и возвращение созданной статьи
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public async Task<Article> CreateNewArticleForArchive(Article article)
        {
            article.UserId = int.Parse(userID);

            if (DateTime.TryParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createDate))
            {
                article.PublicationDateArticle = createDate;
            }

            var response = await articlesRepository.CreateNewArticle(article);
            if (response.IsSuccessStatusCode)
            {
                var articleJson = await response.Content.ReadAsStringAsync();
                var createdArticle = JsonConvert.DeserializeObject<Article>(articleJson);
                return createdArticle; 
            }
            return null;
        }

        /// <summary>
        /// Загрузка фотографий статьи в облачное хранилище Firebase
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> UploadArticlePhoto(Stream stream, string fileName)
        {
            var firebaseSettings = _configuration.GetSection("FirebaseSettings");
            var auth = new FirebaseAuthProvider(new FirebaseConfig(firebaseSettings["ApiKey"]));
            var a = await auth.SignInWithEmailAndPasswordAsync(firebaseSettings["AuthEmail"], firebaseSettings["AuthPassword"]);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                firebaseSettings["Bucket"],
                new FirebaseStorageOptions
                {
                    //AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child("imagesArticle")
                .Child("User_ID_" + userID)
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);

            try
            {
                // ссылка на загруженную фотографию в Firebase
                linkPhotoArticle = await task;
                return linkPhotoArticle;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Загрузка html файлов в облачное хранилище Firebase
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> UploadArticleHtml(Stream stream, string fileName)
        {
            var firebaseSettings = _configuration.GetSection("FirebaseSettings");
            var auth = new FirebaseAuthProvider(new FirebaseConfig(firebaseSettings["ApiKey"]));
            var a = await auth.SignInWithEmailAndPasswordAsync(firebaseSettings["AuthEmail"], firebaseSettings["AuthPassword"]);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                firebaseSettings["Bucket"],
                new FirebaseStorageOptions
                {
                    //AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child("htmlArticle")
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);

            try
            {
                string linkHtmlArticle = await task;
                return linkHtmlArticle;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Получение информации конкретной статьи
        /// </summary>
        /// <param name="idArticle"></param>
        /// <returns></returns>
        public async Task<List<Models.Article>> GetCorrectArticleInfo (int idArticle)
        {
            var response = await articlesRepository.GetAllArticlesInfo();

            if (response.IsSuccessStatusCode)
            {
                var articlesJson = await response.Content.ReadAsStringAsync();
                var articles = JsonConvert.DeserializeObject<List<Models.Article>>(articlesJson);
                var filteredArticle = articles
                            .Where(d => d.IdArticle == idArticle)
                            .ToList();
                return filteredArticle;
            }
            return null;
        }

        /// <summary>
        /// Создание категорий
        /// </summary>
        /// <param name="articleCategory"></param>
        /// <returns></returns>
        public async Task<bool> CreateNewArticleCategory(ArticleCategory articleCategory)
        {
            var response = await articlesRepository.CreateNewArticleCategory(articleCategory);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Изменение/удаление/восстановление категории статьи
        /// </summary>
        /// <param name="articleCategory"></param>
        /// <param name="categoryArticleId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateArticleCategory(ArticleCategory articleCategory, int categoryArticleId)
        {
            // Вызов метода репозитория для отправки запроса к API
            var response = await articlesRepository.UpdateArticleCategory(articleCategory, categoryArticleId);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Изменение/удаление/восстановление статьи
        /// </summary>
        /// <param name="article"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateArticle(Article article, int articleId)
        {
            article.UserId = int.Parse(userID);

            if (DateTime.TryParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime createDate))
            {
                article.PublicationDateArticle = createDate;
            }
            // Вызов метода репозитория для отправки запроса к API
            var response = await articlesRepository.UpdateArticle(article, articleId);

            return response.IsSuccessStatusCode;
        }

    }
}
