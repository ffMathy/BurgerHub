using Bogus;
using BurgerHub.Api.Infrastructure;
using BurgerHub.Api.Seeder;
using Microsoft.Extensions.DependencyInjection;

Randomizer.Seed = new Random(1337);

var serviceCollection = new ServiceCollection();
serviceCollection.AddTransient<ISeedingService, SeedingService>();

var registry = new ApiIocRegistry(serviceCollection);
registry.Register();

var serviceProvider = serviceCollection.BuildServiceProvider();

var seedingService = serviceProvider.GetRequiredService<ISeedingService>();
await seedingService.SeedAsync();