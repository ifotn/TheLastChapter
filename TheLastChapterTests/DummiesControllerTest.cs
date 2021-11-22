using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheLastChapter.Controllers;

namespace TheLastChapterTests
{
    [TestClass]
    public class DummiesControllerTest
    {
        [TestMethod]
        public void IndexReturnsSomething()
        {
            // arrange - set up any objects / vars / params needed
            var controller = new DummiesController();

            // act - execute the method we want to test
            var result = controller.Index();

            // assert - evaluate the behaviour to see if it matches expected behaviour
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexLoadsCorrectView()
        {
            // arrange - set up any objects / vars / params needed
            var controller = new DummiesController();

            // act - execute the method we want to test. We have to cast the IActionResult return type to a ViewResult
            var result = (ViewResult)controller.Index();

            // assert - did the correct view get returned?
            Assert.AreEqual("Index", result.ViewName);
        }
    }
}
