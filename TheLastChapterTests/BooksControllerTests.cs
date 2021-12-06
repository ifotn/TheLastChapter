using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                CategoryId = 1000,
                Name = "My Test Category"
            };

            books.Add(new Book
            {
                BookId = 642,
                Title = "A Book",
                Author = "Some Person",
                Price = 9.99,
                CategoryId = 1000,
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

        #region Index

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

        #endregion

        #region Details

        [TestMethod]
        public void DetailsNoIdLoads404()
        {
            // act
            var result = (ViewResult)controller.Details(null).Result;

            // assert
            Assert.AreEqual("404", result.ViewName);
        }

        [TestMethod]
        public void DetailsInvalidIdLoads404()
        {
            // act
            var result = (ViewResult)controller.Details(-1).Result;

            // assert
            Assert.AreEqual("404", result.ViewName);
        }

        [TestMethod]
        public void DetailsValidIdLoadsBook()
        {
            // act
            var result = (ViewResult)controller.Details(642).Result;
            Book book = (Book)result.Model;

            // assert
            Assert.AreEqual(books[0], book);
        }

        [TestMethod]
        public void DetailsValidIdLoadsView()
        {
            // act
            var result = (ViewResult)controller.Details(642).Result;

            // assert
            Assert.AreEqual("Details", result.ViewName);
        }
        #endregion

        #region Delete

        [TestMethod]
        public void DeleteNullId()
        {
            var result = controller.Delete(null);
            var viewResult = (ViewResult)result.Result;
            Assert.AreEqual("Error", viewResult.ViewName);
        }

        [TestMethod]
        public void DeleteIdNotExists()
        {
            var result = controller.Delete(99);
            var viewResult = (ViewResult)result.Result;
            Assert.AreEqual("Error", viewResult.ViewName);
        }

        [TestMethod]
        public void DeleteCorrectView()
        {
            var id = 642;
            var result = controller.Delete(id);
            var viewResult = (ViewResult)result.Result;
            Assert.AreEqual("Delete", viewResult.ViewName);
        }

        [TestMethod]
        public void DeleteCorrectProduct()
        {
            var id = 642;
            var result = controller.Delete(id);
            var viewResult = (ViewResult)result.Result;
            Book book = (Book)viewResult.Model;
            Assert.AreEqual(books[0], book);
        }

        [TestMethod]
        public void DeleteConfirmedSuccess()
        {
            var id = 10;
            var result = controller.DeleteConfirmed(id);
            var book = _context.Books.Find(id);
            Assert.AreEqual(book, null);
        }

        [TestMethod]
        public void DeleteConfirmedRedirectIndex()
        {
            var id = 642;
            var result = controller.DeleteConfirmed(id);
            var actionResult = (RedirectToActionResult)result.Result;
            Assert.AreEqual("Index", actionResult.ActionName);
        }
        #endregion

        #region Edit

        // Test 404 page loads if bookId does not match book
        [TestMethod]
        public void EditBookIDNotFound()
        {
            // act
            var result = (ViewResult)controller.Edit(7345, books[0], null).Result;


            // assert
            Assert.AreEqual("404", result.ViewName);

        }

        // Test 404 loads if bookId is invalid
        [TestMethod]
        public void EditInvalidBookIDFound()
        {
            // act
            var result = (ViewResult)controller.Edit(-1, books[3], null).Result;


            // assert
            Assert.AreEqual("404", result.ViewName);

        }


        // Test when an invalid model is saved/passed in it will return the the user to the edit page
        [TestMethod]
        public void EditInValidModelEditViewLoads()
        {
            // act 
            controller.ModelState.AddModelError("Mock Category", "1002");

            var result = (ViewResult)controller.Edit(642, books[0], null).Result;

            // assert
            Assert.AreEqual("Edit", result.ViewName);
        }


        // Test when a valid model is passed in that it will return user to the index page
        [TestMethod]
        public void EditValidModelLoadsIndex()
        {
            // act
            var result = (RedirectToActionResult)controller.Edit(642, books[0], null).Result;

            // assert
            Assert.AreEqual("Index", result.ActionName);
        }


        // Test for the view data
        [TestMethod]
        public void ValidCategoryID()
        {
            controller.ModelState.AddModelError("Mock Category", "1000");

            var result = (ViewResult)controller.Edit(642, books[0], null).Result;
            var selectList = (SelectList)result.ViewData["CategoryId"];
            var items = selectList.Items;
            Assert.AreEqual(items,_context.Categories.ToList());
        }

        #endregion
    }
}
