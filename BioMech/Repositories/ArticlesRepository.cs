using BioMech.Models;
using BioMech_API.Models;
using Firebase.Auth;
using Newtonsoft.Json;
using System.Text;

namespace BioMech.Repositories
{
    public class ArticlesRepository
    {
        private readonly string apiUrl; // ip API

        public ArticlesRepository(ApiSettings apiSettings)
        {
            apiUrl = apiSettings.BaseUrl;
        }

        /// <summary>
        /// Запрос к API для получения информации конкретной статьи
        /// </summary>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetArticleInfo(int articleID)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}Articles/{articleID}");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения всех статей
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAllArticlesInfo()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}Articles");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для получения всех категорий статей
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetArticleCategories()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{apiUrl}ArticleCategories");

                return response;
            }
        }

        /// <summary>
        /// Запрос к API для создания статьи 
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CreateNewArticle(Article article)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(article), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiUrl + "Articles", content);
            }
        }


        /// <summary>
        /// Запрос к API для редактирования статьи
        /// </summary>
        /// <param name="article"></param>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateArticle(Article article, int articleID)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(article), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}Articles/{articleID}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Запрос к API для создания категории статьи 
        /// </summary>
        /// <param name="articleCategory"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CreateNewArticleCategory(ArticleCategory articleCategory)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(articleCategory), Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(apiUrl + "ArticleCategories", content);
            }
        }

        /// <summary>
        /// Запрос к API для редактирования категории статьи
        /// </summary>
        /// <param name="articleCategory"></param>
        /// <param name="articleCategoryId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateArticleCategory(ArticleCategory articleCategory, int articleCategoryId)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(articleCategory), Encoding.UTF8, "application/json");

                string requestUri = $"{apiUrl}ArticleCategories/{articleCategoryId}";

                return await httpClient.PutAsync(requestUri, content);
            }
        }


    }
}
