using Microsoft.AspNetCore.Mvc;

namespace BioMech.Controllers
{
    public class ProxyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        ///   GET-метод действия для получения контента по указанному URL-адресу.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetContent(string url)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "text/html");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"");
            }
        }
    }
}
