using BurgerHub.Api.Infrastructure;
using BurgerHub.Api.Seeder;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();
serviceCollection.AddTransient<ISeedingService, SeedingService>();

var registry = new IocRegistry(serviceCollection);
registry.Register();

var serviceProvider = serviceCollection.BuildServiceProvider();

var seedingService = serviceProvider.GetRequiredService<ISeedingService>();
await seedingService.SeedAsync();