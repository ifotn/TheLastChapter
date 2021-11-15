using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
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

        // config var to read appsettings for Stripe keys
        private IConfiguration _iconfiguration;

        // constructor that accepts a DbContext instance & now a configuration dependency too
        public ShopController (ApplicationDbContext context, IConfiguration iconfiguration)
        {
            _context = context;
            _iconfiguration = iconfiguration;
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

        // GET: /Shop/Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        // POST: /Shop/Checkout
        [HttpPost]
        [Authorize]
        public IActionResult Checkout([Bind("FirstName,LastName,Address,City,Province,PostalCode,Phone")] Order order)
        {
            // fill the CustomerId and OrderDate properties
            order.CustomerId = User.Identity.Name;
            order.PurchaseDate = DateTime.Now;

            // save order to session var so we can retrieve it and save to db after successful payment
            // use SessionExtensions 3rd party library since native .NET Core session can't store a complex object
            HttpContext.Session.SetObject("Order", order);

            return RedirectToAction("Payment");
        }

        // GET: /Shop/Payment
        [Authorize]
        public IActionResult Payment()
        {
            // read Stripe public key from iconfiguration class var
            ViewBag.PublishableKey = _iconfiguration["Stripe:PublishableKey"];
            return View();
        }

        // POST: /Shop/ProcessPayment
        [Authorize]
        [HttpPost]
        public IActionResult ProcessPayment()
        {
            // get the order from the session var
            var order = HttpContext.Session.GetObject<Models.Order>("Order");
            var customerId = order.CustomerId;
            var total = (from c in _context.CartItems
                         where c.CustomerId == customerId
                         select (c.Price * c.Quantity)).Sum();

            // get Stripe Secret Key from appsettings
            StripeConfiguration.ApiKey = _iconfiguration.GetSection("Stripe")["SecretKey"];

            // create Stripe payment object
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions> { 
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = ((long?)(total * 100)),
                            Currency = "cad",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "The Last Chapter Purchase"
                            }
                        }, 
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = "https://" + Request.Host + "/Shop/SaveOrder",
                CancelUrl = "https://" + Request.Host + "/Shop/Cart"
            };

            // invoke Stripe now with the above payment object
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
