using BioMech.Models;
using BioMech.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace BioMech.Controllers
{
    public class ArticlesController : Controller
    {

        private readonly ArticlesService articlesService; 
        private readonly PersonalAccountService personalAccountService;
        private readonly HttpClient _httpClient;

        public ArticlesController(ArticlesService articlesService, IHttpClientFactory httpClientFactory)
        {
            this.articlesService = articlesService;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Archive()
        {
            var allArticles = await articlesService.GetAllArchiveArticlesInfo();
            var articleCategories = await articlesService.ArticleCategories();
            var model = new ArticlesModel { Articles = allArticles, ArticlesCategory = articleCategories };

            return View(model);
        }
        public async Task<IActionResult> CreateCategories()
        {
            var articleCategories = await articlesService.ArticleCategories();
            var model = new ArticlesModel { ArticlesCategory = articleCategories };

            return View(model);
        }
        public async Task<IActionResult> CreateArticles(int? id = null)
        {
            var model = new ArticlesModel();
            model.ArticlesCategory = await articlesService.ArticleCategories(); // Загрузка категорий

            if (id.HasValue)
            {
                var articles = await articlesService.GetCorrectArticleInfo(id.Value);
                if (articles == null || !articles.Any())
                {
                    return NotFound();
                }
                model.Articles = articles;
            }
            else
            {
                model.Articles = new List<Article>();
            }

            return View(model);
        }


        // Создание статьи
        [HttpPost]
        public async Task<IActionResult> CreateNewArticle(string nameArticle, string shortDescription, int articleCategoryId, IFormFile articlePhoto, IFormFile htmlFile)
        {

            // Проверка наличия загруженного файла
            if (articlePhoto == null || articlePhoto.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            if (htmlFile == null || htmlFile.Length == 0)
            {
                return BadRequest("HTML file is missing");
            }

            var fileName = Path.GetFileName(articlePhoto.FileName);
            string linkImageArticle;

            using (var stream = articlePhoto.OpenReadStream())
            {
                linkImageArticle = await articlesService.UploadArticlePhoto(stream, fileName);
            }

            if (string.IsNullOrEmpty(linkImageArticle))
            {
                return View("Error");
            }

            string linkHtml;
            using (var stream = htmlFile.OpenReadStream())
            {
                linkHtml = await articlesService.UploadArticleHtml(stream, htmlFile.FileName);
            }

            var article = new Article
            {
                Name_Article = nameArticle,
                ShortDescriptionArticle = shortDescription,
                ArticleCategoryId = articleCategoryId,
                Image_Article = linkImageArticle,
                ContentAtricle =linkHtml
            };

            var isSuccess = await articlesService.CreateNewArticle(article);

            if (isSuccess)
            {
                return RedirectToAction("Articles");
            }
            else
            {
                return BadRequest("Failed to create article");
            }
        }

        // Создание статьи
        [HttpPost]
        public async Task<IActionResult> UpdateArticle(int idArticle, string nameArticle, string shortDescription, int articleCategoryId, IFormFile articlePhoto, IFormFile htmlFile)
        {
            string linkImageArticle = null;
            string linkHtml = null;

            var article = new Article
            {
                IdArticle = idArticle,
                Name_Article = nameArticle,
                ShortDescriptionArticle = shortDescription,
                ArticleCategoryId = articleCategoryId
            };

            // Проверка наличия загруженного файла
            if (articlePhoto != null)
            {
                var fileName = Path.GetFileName(articlePhoto.FileName);


                using (var stream = articlePhoto.OpenReadStream())
                {
                    linkImageArticle = await articlesService.UploadArticlePhoto(stream, fileName);
                }

                if (string.IsNullOrEmpty(linkImageArticle))
                {
                    return View("Error");
                }

                article.Image_Article = linkImageArticle;
            }
            else
            {
                var articleCorrect = await articlesService.GetCorrectArticleInfo(idArticle);
                var firstArticle = articleCorrect.First();
                article.Image_Article = firstArticle.Image_Article;

            }

            if (htmlFile != null)
            {

                using (var stream = htmlFile.OpenReadStream())
                {
                    linkHtml = await articlesService.UploadArticleHtml(stream, htmlFile.FileName);
                    article.ContentAtricle = linkHtml;
                }
            }

            

            var isSuccess = await articlesService.UpdateArticle(article, idArticle);

            if (isSuccess)
            {
                return RedirectToAction("Articles");
            }
            else
            {
                return BadRequest("Failed to create article");
            }
        }

        /// <summary>
        /// Архивирование статьи
        /// </summary>
        /// <param name="nameArticleCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {
            Article article = await articlesService.GetArticleInfo(articleId);

            article.DeletedArticle = true;

            var isSuccess = await articlesService.UpdateArticle(article, articleId);

            if (isSuccess)
            {
                return RedirectToAction("Articles");
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Публикация статьи из архива
        /// </summary>
        /// <param name="nameArticleCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PublicArticle(int articleId)
        {
            Article article = await articlesService.GetArticleInfo(articleId);

            article.DeletedArticle = false;

            var isSuccess = await articlesService.UpdateArticle(article, articleId);

            if (isSuccess)
            {
                return RedirectToAction("Articles");
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Архивирование новой статьи
        /// </summary>
        /// <param name="nameArticleCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ArchiveNewArticle(string nameArticle, string shortDescription, int articleCategoryId, IFormFile articlePhoto, IFormFile htmlFile)
        {
            // Проверка наличия загруженных файлов
            if (articlePhoto == null || articlePhoto.Length == 0 || htmlFile == null || htmlFile.Length == 0)
            {
                return BadRequest("Invalid file or HTML file is missing");
            }

            // Загрузка изображения статьи
            var fileName = Path.GetFileName(articlePhoto.FileName);
            string linkImageArticle;
            using (var stream = articlePhoto.OpenReadStream())
            {
                linkImageArticle = await articlesService.UploadArticlePhoto(stream, fileName);
            }
            if (string.IsNullOrEmpty(linkImageArticle))
            {
                return View("Error", "Error uploading image");
            }

            // Загрузка HTML файла
            string linkHtml;
            using (var stream = htmlFile.OpenReadStream())
            {
                linkHtml = await articlesService.UploadArticleHtml(stream, htmlFile.FileName);
            }

            // Создание объекта статьи
            var article = new Article
            {
                Name_Article = nameArticle,
                ShortDescriptionArticle = shortDescription,
                ArticleCategoryId = articleCategoryId,
                Image_Article = linkImageArticle,
                ContentAtricle = linkHtml
            };

            // Создание статьи
            var createdArticle = await articlesService.CreateNewArticleForArchive(article);

            if (createdArticle == null)
            {
                return BadRequest("Failed to create article");
            }

            // Архивирование статьи (пометка удаления)
            createdArticle.DeletedArticle = true;
            var archiveSuccess = await articlesService.UpdateArticle(createdArticle, (int)createdArticle.IdArticle);

            if (archiveSuccess)
            {
                return RedirectToAction("Articles");
            }
            else
            {
                return BadRequest("Failed to archive article");
            }
        }

        /// <summary>
        /// Архивирование опубликованных статей
        /// </summary>
        /// <param name="idArticle"></param>
        /// <param name="nameArticle"></param>
        /// <param name="shortDescription"></param>
        /// <param name="articleCategoryId"></param>
        /// <param name="articlePhoto"></param>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ArchiveArticle(int idArticle, string nameArticle, string shortDescription, int articleCategoryId, IFormFile articlePhoto, IFormFile htmlFile)
        {
            string linkImageArticle = null;
            string linkHtml = null;

            var article = new Article
            {
                IdArticle = idArticle,
                Name_Article = nameArticle,
                ShortDescriptionArticle = shortDescription,
                ArticleCategoryId = articleCategoryId
            };

            // Проверка наличия загруженного файла
            if (articlePhoto != null)
            {
                var fileName = Path.GetFileName(articlePhoto.FileName);


                using (var stream = articlePhoto.OpenReadStream())
                {
                    linkImageArticle = await articlesService.UploadArticlePhoto(stream, fileName);
                }

                if (string.IsNullOrEmpty(linkImageArticle))
                {
                    return View("Error");
                }

                article.Image_Article = linkImageArticle;
            }
            else
            {
                var articleCorrect = await articlesService.GetCorrectArticleInfo(idArticle);
                var firstArticle = articleCorrect.First();
                article.Image_Article = firstArticle.Image_Article;

            }

            if (htmlFile != null)
            {

                using (var stream = htmlFile.OpenReadStream())
                {
                    linkHtml = await articlesService.UploadArticleHtml(stream, htmlFile.FileName);
                    article.ContentAtricle = linkHtml;
                }
            }

            article.DeletedArticle = true;

            var isSuccess = await articlesService.UpdateArticle(article, idArticle);

            if (isSuccess)
            {
                return RedirectToAction("Articles");
            }
            else
            {
                return BadRequest("Failed to create article");
            }
        }


        /// <summary>
        /// Создание новой категории статьи
        /// </summary>
        /// <param name="nameArticleCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateNewArticleCategory(string nameArticleCategory)
        {
            var articleCategory = new ArticleCategory
            {
                NameArticleCategory = nameArticleCategory,
            };

            var isSuccess = await articlesService.CreateNewArticleCategory(articleCategory);

            if (isSuccess)
            {
                return RedirectToAction("CreateCategories"); 
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Изменение категории статьи
        /// </summary>
        /// <param name="nameArticleCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateArticleCategory(int categoryArticleId, string newNameArticleCategory)
        {
            var articleCategory = new ArticleCategory
            {
                IdArticleCategory = categoryArticleId,
                NameArticleCategory = newNameArticleCategory,
            };

            var isSuccess = await articlesService.UpdateArticleCategory(articleCategory, categoryArticleId);

            if (isSuccess)
            {
                return RedirectToAction("CreateCategories");
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Удаление категории статьи
        /// </summary>
        /// <param name="nameArticleCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteArticleCategory(int categoryArticleId)
        {
            var categoryArticles = await articlesService.ArticleCategoriesById(categoryArticleId);

            if (categoryArticles == null || !categoryArticles.Any())
            {
                return BadRequest("Category not found");
            }

            var firstArticleCategory = categoryArticles.First();

            var articleCategory = new ArticleCategory
            {
                IdArticleCategory = categoryArticleId,
                NameArticleCategory = firstArticleCategory.NameArticleCategory,
                DeletedCategoryArticle = true
            };

            var isSuccess = await articlesService.UpdateArticleCategory(articleCategory, categoryArticleId);

            if (isSuccess)
            {
                return RedirectToAction("CreateCategories");
            }
            else
            {
                return BadRequest("Failed");
            }
        }

        /// <summary>
        /// Вывод данных на странице опубликованных статей
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Articles()
        {
            var allArticles = await articlesService.GetAllArticlesInfo();
            var articleCategories = await articlesService.ArticleCategories();
            var model = new ArticlesModel { Articles = allArticles, ArticlesCategory = articleCategories };

            return View(model);
        }

        /// <summary>
        /// Вывод данных на странице одной из статей
        /// </summary>
        /// <param name="idArticle"></param>
        /// <returns></returns>
        [HttpGet("/articles/Article")]
        public async Task<IActionResult> Article(int idArticle)
        {
            // Получение статьи по идентификатору
            var article = await articlesService.GetArticleInfo(idArticle);

            if (article != null)
            {
                // Получение HTML-код статьи по ссылке
                var htmlContent = await _httpClient.GetStringAsync(article.ContentAtricle);

                // Создание объекта статьи с полученным HTML-кодом
                var fullArticle = new Article { ContentAtricle = htmlContent };

                // Передача объекта статьи в представление Article.cshtml для отображения
                return View("Article", fullArticle);
            }

            return NotFound();
        }

        /// <summary>
        /// Поиск статьи
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IActionResult> SearchArticle(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Если запрос пустой, просто показываем все статьи
                return RedirectToAction(nameof(Articles));
            }

            // Преобразование текста запроса к нижнему регистру
            var lowerQuery = query.ToLower();

            // Выполнение поиска статей без учета регистра
            var searchResults = await articlesService.SearchArticles(lowerQuery);

            var articleCategories = await articlesService.ArticleCategories();
            // Создание объекта ArticlesModel и установка в него результатов поиска
            var model = new ArticlesModel { Articles = searchResults, ArticlesCategory = articleCategories };

            return View("Articles", model);
        }

        /// <summary>
        /// Поиск статьи в архиве
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IActionResult> SearchArticleArchive(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Если запрос пустой, просто показываем все статьи
                return RedirectToAction(nameof(Articles));
            }

            // Преобразование текста запроса к нижнему регистру
            var lowerQuery = query.ToLower();

            // Выполнение поиска статей без учета регистра
            var searchResults = await articlesService.SearchArticlesArchive(lowerQuery);

            var articleCategories = await articlesService.ArticleCategories();
            // Создание объекта ArticlesModel и установка в него результатов поиска
            var model = new ArticlesModel { Articles = searchResults, ArticlesCategory = articleCategories };

            return View("Archive", model);
        }

        /// <summary>
        /// Получение всех статей по определенной категории
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("/Articles/GetArticlesByCategory")]
        public async Task<IActionResult> GetArticlesByCategory(int categoryId)
        {
            var articles = await articlesService.GetArticlesByCategory(categoryId);
            return Json(articles);
        }

    }

}
