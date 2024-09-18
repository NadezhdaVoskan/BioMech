using BioMech.Models;
using BioMech.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BioMech.Controllers
{
    public class HomeController : Controller
    {

        /// <summary>
        /// Страница главной страницы
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Страница политики в отношении обработки персональных данных
        /// </summary>
        /// <returns></returns>
        public IActionResult Politics()
        {
            return View();
        }

        /// <summary>
        /// Страница условия использования
        /// </summary>
        /// <returns></returns>
        public IActionResult Conditions()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}