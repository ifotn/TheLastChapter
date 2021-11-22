using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLastChapter.Controllers;
using TheLastChapter.Data;
using TheLastChapter.Models;

namespace TheLastChapterTests
{
    [TestClass] // must add public scope for tests to be visible
    public class BooksControllerTests
    {
        // class level vars for db
        private ApplicationDbContext _context;  // in-memory db connection object
        private BooksController controller;
        List<Book> books = new List<Book>();

         // this runs automatically before each test in this class
        [TestInitialize]
        public void TestInitialize()
        {
            // set up in-memory db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // create mock data for testing
            var Category = new Category
            {
                CategoryId = 1000, Name = "My Test Category"
            };

            books.Add(new Book
            {
                BookId = 642, Title = "A Book", Author = "Some Person", Price = 9.99, CategoryId = 1000,
                Category = Category
            });
            books.Add(new Book
            {
                BookId = 529,
                Title = "My Book",
                Author = "Some Writer",
                Price = 9.99,
                CategoryId = 1000,
                Category = Category
            });
            books.Add(new Book
            {
                BookId = 753,
                Title = "Great Book",
                Author = "Famous Writer",
                Price = 9.99,
                CategoryId = 1000,
                Category = Category
            });

            foreach (var book in books)
            {
                _context.Books.Add(book);
            }
            _context.SaveChanges();

            // arrange: create controller for all tests
            controller = new BooksController(_context);
        }

        [TestMethod]
        public void IndexLoadsCorrectView()
        {
            // arrange - now done in TestInitialize()       

            // act
            var result = (ViewResult)controller.Index().Result;

            // assert
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void IndexLoadsBooks()
        {
            // act
            var result = (ViewResult)controller.Index().Result;
            List<Book> model = (List<Book>)result.Model;

            // assert
            CollectionAssert.AreEqual(books.OrderBy(b => b.Author).ThenBy(b => b.Title).ToList(), model);
        }
    } 
}
