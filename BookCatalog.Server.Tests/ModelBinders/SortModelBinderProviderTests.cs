using BookCatalog.Server.ModelBinders;
using BookCatalog.Shared.Request.Books;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace BookCatalog.Server.Tests.ModelBinders
{
    [TestClass]
    public class SortModelBinderProviderTests
    {
        private readonly Mock<IModelBinder> defaultBinderMock = new Mock<IModelBinder>();
        private readonly Mock<IModelBinderProvider> binderProviderMock = new Mock<IModelBinderProvider>();

        private readonly SortModelBinderProvider sortModelBinderProvider;

        public SortModelBinderProviderTests()
        {
            sortModelBinderProvider = new SortModelBinderProvider(new List<IModelBinderProvider>() { binderProviderMock.Object });
        }

        [TestMethod]
        public void GetBinder_WhenContextPassed_ShouldReturnProperBinder()
        {
            // Arrange
            var stub = new ModelBinderContextStub(typeof(GetBooksRequest));
            binderProviderMock
                .Setup(p => p.GetBinder(It.IsAny<ModelBinderProviderContext>()))
                .Returns(defaultBinderMock.Object);

            // Act
            var binder = sortModelBinderProvider.GetBinder(stub);

            // Assert
            Assert.IsTrue(binder is not null);
            Assert.IsTrue(typeof(IModelBinder).IsAssignableFrom(typeof(SortModelBinder)));
        }

        [TestMethod]
        public void GetBinder_WhenDefaultBinderNotFound_ShouldReturnNull()
        {
            // Arrange
            var stub = new ModelBinderContextStub(typeof(GetBooksRequest));

            // Act
            var binder = sortModelBinderProvider.GetBinder(stub);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_WhenContextIsNull_ShouldThrowException()
        {
            // Arrange
            ModelBinderContextStub stub = null;

            // Act
            // Assert
            var binder = Assert.ThrowsException<ArgumentNullException>(() => sortModelBinderProvider.GetBinder(stub));
        }
    }
}
