using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordExcercise;
using PasswordGenerator.API.Controllers;
using PasswordGenerator.API.Filters;

namespace PasswordGenerator.Tests
{
    [TestClass]
    public class PasswordControllerTests
    {
        [TestMethod]
        public void Generate_ValidInput_ReturnsOkResultWithPassword()
        {
            // Arrange
            var requirements = new PasswordRequirements
            {
                MinLength = 8,
                MaxLength = 12,
                MinUpperAlphaChars = 1,
                MinLowerAlphaChars = 1,
                MinNumericChars = 1,
                MinSpecialChars = 1
            };

            var expectedPassword = "Ab3#xyz";

            var mockGenerator = new Mock<IPasswordGenerator>();
            mockGenerator
                .Setup(g => g.GeneratePassword(It.IsAny<PasswordRequirements>()))
                .Returns(expectedPassword);

            var controller = new PasswordController(mockGenerator.Object);

            // Act
            var result = controller.Generate(requirements).Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(expectedPassword, result.Value);
        }

        [TestMethod]
        public void Generate_InvalidRequirements_ReturnsBadRequest()
        {
            // Arrange: min length > max length, which will be caught by action filter manually
            var controller = new PasswordController(Mock.Of<IPasswordGenerator>());
            var invalidReq = new PasswordRequirements
            {
                MinLength = 10,
                MaxLength = 5
            };

            // Act
            var filter = new ValidatePasswordRequirementsAttribute();
            var context = new ActionExecutingContext(
                new ActionContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                    ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                },
                new List<IFilterMetadata>(),
                new Dictionary<string, object> { { "req", invalidReq } },
                controller
            );

            filter.OnActionExecuting(context);

            // Assert
            Assert.IsInstanceOfType(context.Result, typeof(BadRequestObjectResult));
        }
    }

}
