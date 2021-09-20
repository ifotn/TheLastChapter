using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheLastChapter.Models;

namespace TheLastChapter.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            // use the Category model to generate 10 categories in memory for display in the view
            var categories = new List<Category>();

            for (var i = 1; i < 11; i++)
            {
                categories.Add(new Category { CategoryId = i, Name = "Category " + i.ToString() });
            }

            // must now pass the strongly-typed data to the view
            return View(categories);
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
