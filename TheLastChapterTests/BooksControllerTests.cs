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

namespace TheLastChapterTests
{
    [TestClass] // must add public scope for tests to be visible
    public class BooksControllerTests
    {
        // class level vars for db
        private ApplicationDbContext _context;  // in-memory db connection object
        private BooksController controller;

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
    } 
}
