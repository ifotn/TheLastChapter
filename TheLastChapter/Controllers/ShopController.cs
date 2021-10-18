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
    }
}
