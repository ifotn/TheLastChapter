using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastChapter.Controllers
{
    public class DummiesController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
