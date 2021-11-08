using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheLastChapter.Data;
using TheLastChapter.Models;

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

        // POST: /Shop/AddToCart
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult AddToCart(int BookId, int Quantity)
        {
            // get book price
            var book = _context.Books.Find(BookId);
            var price = book.Price;

            // identify user
            var customerId = "Random-Customer";

            // create new CartItem
            var cartItem = new CartItem
            {
                BookId = BookId,
                Quantity = Quantity,
                Price = price,
                CustomerId = customerId,
                DateAdded = DateTime.Now
            };

            // save to db
            _context.CartItems.Add(cartItem);
            _context.SaveChanges();

            return RedirectToAction("Cart");
        }

        // GET: /Shop/Cart
        public IActionResult Cart()
        {
            // get current cart items and parent Book objects
            var cartItems = _context.CartItems.Include(c => c.Book).OrderBy(c => c.Book.Title).ToList();
            return View(cartItems);
        }

        // identify customer for each shopping cart
        //private string GetCustomerId()
        //{
        //    // is CustomerId session var already set?
        //    if (HttpContext.Session.S)
        //}
    }
}
