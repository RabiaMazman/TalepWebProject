using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalepWebProject.MVC.Controllers
{
    public class GenelMudurController : Controller
    {
        private string url = "https://localhost:44356/api/";

        public IActionResult Index()
        {
            return View();
        }
    }
}
