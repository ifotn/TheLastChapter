using Microsoft.AspNetCore.Http;
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
            var customerId = GetCustomerId();

            // does user already have this book in their cart?
            var cartItem = _context.CartItems.SingleOrDefault(c => c.BookId == BookId && c.CustomerId == customerId);
            if (cartItem == null)
            {
                // create new CartItem
                cartItem = new CartItem
                {
                    BookId = BookId,
                    Quantity = Quantity,
                    Price = price,
                    CustomerId = customerId,
                    DateAdded = DateTime.Now
                };

                // save to db
                _context.CartItems.Add(cartItem);
            }
            else
            {
                // user already has this book in cart, so update the quantity
                cartItem.Quantity += Quantity;
                _context.Update(cartItem);
            }
                
            _context.SaveChanges();

            return RedirectToAction("Cart");
        }

        // GET: /Shop/Cart
        public IActionResult Cart()
        {
            // get current customer to filter CartItems query
            var customerId = GetCustomerId();

            // get current cart items and parent Book objects for current customer
            var cartItems = _context.CartItems.Where(c => c.CustomerId == customerId)
                .Include(c => c.Book).OrderBy(c => c.Book.Title).ToList();
            return View(cartItems);
        }

        //identify customer for each shopping cart
        private string GetCustomerId()
        {
            // is CustomerId session var already set?
            if (HttpContext.Session.GetString("CustomerId") == null) {
                var customerId = "";
                // if user is logged in, use their email as session var
                if (User.Identity.IsAuthenticated)
                {
                    customerId = User.Identity.Name;
                }
                else
                {
                    // user is anonymous so generate randomId to attach to their session
                    customerId = Guid.NewGuid().ToString();
                }

                // store customerId in session var
                HttpContext.Session.SetString("CustomerId", customerId);
            }

            return HttpContext.Session.GetString("CustomerId");
        }

        // GET: /Shop/RemoveFromCart/5
        public IActionResult RemoveFromCart(int id)
        {
            var cartItem = _context.CartItems.Find(id);
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }
    }
}
