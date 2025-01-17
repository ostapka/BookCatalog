using AutoMapper;
using BookCatalog.Server.Infrastructure;

namespace BookCatalog.Server.Tests.Infrastructure
{
    [TestClass]
    public class BookProfileTests
    {
        [TestMethod]
        public void BookProfile_ShouldValidateMappingProfiles()
        {
            // Arrange
            var config = new MapperConfiguration(configure =>
            {
                configure.AddProfiles(new Profile[] {
                    new BookProfile()
                });
            });
            var mapper = config.CreateMapper();

            // Act
            // Assert
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
