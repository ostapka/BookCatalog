using BookCatalog.Server.ModelBinders;
using BookCatalog.Shared.Request.Books;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace BookCatalog.Server.Tests.ModelBinders
{
    [TestClass]
    public class SortModelBinderTests
    {
        private readonly Mock<IModelBinder> defaultBinderMock = new Mock<IModelBinder>();
        private readonly Mock<ModelBindingContext> modelBindingContext = new Mock<ModelBindingContext>();

        private readonly SortModelBinder sortModelBinder;

        public SortModelBinderTests()
        {
            sortModelBinder = new SortModelBinder(defaultBinderMock.Object);
        }

        [TestMethod]
        [DataRow("sort.Title")]
        [DataRow("sort.Author")]
        [DataRow("SORT.Title")]
        public async Task BindModelAsync_WhenSortOrderSpecified_ShouldPutSortOrderIntoModel(string sortParam1)
        {
            // Arrange
            List<string> queryParams = new List<string>
            {
                sortParam1
            };
            var bindingSource = new BindingSource("", "", false, false);
            var routeValueDictionary = new RouteValueDictionary
            {
                [queryParams[0]] = "asc"
            };
            var request = new GetBooksRequest();

            modelBindingContext
                .Setup(c => c.ValueProvider)
                .Returns(new RouteValueProvider(bindingSource, routeValueDictionary));
            modelBindingContext
                .Setup(c => c.Model)
                .Returns(request);
            modelBindingContext
                .Setup(c => c.HttpContext.Request.Query.Keys)
                .Returns(queryParams);

            // Act
            await sortModelBinder.BindModelAsync(modelBindingContext.Object);

            // Assert
            Assert.AreEqual(routeValueDictionary.Count, request.Sort.Length);
            Assert.AreEqual(queryParams[0].Split(".")[1].ToUpper(), request.Sort[0].SortName);
            Assert.AreEqual(routeValueDictionary[queryParams[0]], request.Sort[0].SortOrder);
        }

        [TestMethod]
        public async Task BindModelAsync_WhenArgumentIsNull_ShouldThrowException()
        {
            // Arrange
            ModelBindingContext context = null;

            // Act
            // Assert
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                async () => await sortModelBinder.BindModelAsync(context)
            );
        }
    }
}
