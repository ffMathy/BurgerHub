using BurgerHub.Api.Seeder.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BurgerHub.Api.Seeder.Tests;

[TestClass]
public class FakerRuleTest
{
    private readonly IFakerFactory _fakerFactory;
    
    public FakerRuleTest()
    {
        var serviceCollection = new ServiceCollection();

        var registry = new SeederIocRegistry(
            serviceCollection,
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build());
        registry.Register();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        _fakerFactory = serviceProvider.GetRequiredService<IFakerFactory>();
    }
    
    [TestMethod]
    public void UserFaker_CreatedViaFactory_HasValidConfiguration()
    {
        //Arrange
        var faker = _fakerFactory.CreateUserFaker();
        
        //Act
        var user = faker.Generate();
        
        //Assert
        faker.AssertConfigurationIsValid();
        Assert.IsNotNull(user);
    }
    
    [TestMethod]
    public void PhotoFaker_CreatedViaFactory_HasValidConfiguration()
    {
        //Arrange
        var faker = _fakerFactory.CreatePhotoFaker();
        
        //Act
        var photo = faker.Generate();
        
        //Assert
        faker.AssertConfigurationIsValid();
        Assert.IsNotNull(photo);
    }
    
    [TestMethod]
    public void RestaurantFaker_CreatedViaFactory_HasValidConfiguration()
    {
        //Arrange
        var faker = _fakerFactory.CreateRestaurantFaker();
        
        //Act
        var restaurant = faker.Generate();
        
        //Assert
        faker.AssertConfigurationIsValid();
        Assert.IsNotNull(restaurant);
    }
    
    [TestMethod]
    public void ReviewFaker_CreatedViaFactory_HasValidConfiguration()
    {
        //Arrange
        var faker = _fakerFactory.CreateReviewFaker();
        
        //Act
        var review = faker.Generate();
        
        //Assert
        faker.AssertConfigurationIsValid();
        Assert.IsNotNull(review);
    }
}