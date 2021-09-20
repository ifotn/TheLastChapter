using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastChapter.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Browse(string category)
        {
            // if category empty, redirect to index so user can first pick a category
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            // display the selected category using the ViewBag object
            ViewBag.category = category;
            return View();
        }
    }
}
