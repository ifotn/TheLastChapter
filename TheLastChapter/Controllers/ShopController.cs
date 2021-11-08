using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheLastChapter.Data;

namespace TheLastChapter.Controllers
{
    public class ShopController : Controller
    {
        // class level DbContext connection object
        private readonly ApplicationDbContext _context;

        // constructor that accepts a DbContext instance
        public ShopController (ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // query the list of categories in a-z order for display
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();
            return View(categories);
        }

        // GET: /Shop/ShopByCategory/5
        public IActionResult ShopByCategory(int id)
        {
            // query the Books filtered by the selected CategoryId param
            var books = _context.Books.Where(b => b.CategoryId == id).OrderBy(b => b.Title).ToList();

            // get selected Category Name to display
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Category = category.Name;

            return View(books);
        }
    }
}
