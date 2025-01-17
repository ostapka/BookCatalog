using AutoMapper;
using BookCatalog.Server.Infrastructure;

namespace BookCatalog.Server.Tests.Infrastructure
{
    [TestClass]
    public class CommonEntitiesProfileTest
    {
        [TestMethod]
        public void CommonEntitiesProfileTest_ShouldValidateMappingProfiles()
        {
            // Arrange
            var config = new MapperConfiguration(configure =>
            {
                configure.AddProfile(typeof(CommonEntitiesProfile));
            });
            var mapper = config.CreateMapper();

            // Act
            // Assert
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
