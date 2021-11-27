using System;
using BurgerHub.Api.Infrastructure;
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

        var registry = new SeederIocRegistry(serviceCollection);
        registry.Register();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        _fakerFactory = serviceProvider.GetRequiredService<IFakerFactory>();
    }
    
    [TestMethod]
    public void UserFaker_CreatedViaFactory_HasValidConfiguration()
    {
        //Arrange
        var faker = _fakerFactory.CreateUserFaker();
        
        //Act & Assert
        faker.AssertConfigurationIsValid();
    }
    
    [TestMethod]
    public void PhotoFaker_CreatedViaFactory_HasValidConfiguration()
    {
        //Arrange
        var faker = _fakerFactory.CreatePhotoFaker();
        
        //Act & Assert
        faker.AssertConfigurationIsValid();
    }
    
    [TestMethod]
    public void RestaurantFaker_CreatedViaFactory_HasValidConfiguration()
    {
        //Arrange
        var faker = _fakerFactory.CreateRestaurantFaker();
        
        //Act & Assert
        faker.AssertConfigurationIsValid();
    }
    
    [TestMethod]
    public void ReviewFaker_CreatedViaFactory_HasValidConfiguration()
    {
        //Arrange
        var faker = _fakerFactory.CreateReviewFaker();
        
        //Act & Assert
        faker.AssertConfigurationIsValid();
    }
}