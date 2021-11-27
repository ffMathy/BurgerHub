using Bogus;
using BurgerHub.Api.Seeder;
using BurgerHub.Api.Seeder.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Randomizer.Seed = new Random(1337);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json")
    .AddEnvironmentVariables()
    .Build();

var serviceCollection = new ServiceCollection();

var registry = new SeederIocRegistry(
    serviceCollection,
    configuration);
registry.Register();

var serviceProvider = serviceCollection.BuildServiceProvider();

var seedingService = serviceProvider.GetRequiredService<ISeedingService>();
await seedingService.SeedAsync();