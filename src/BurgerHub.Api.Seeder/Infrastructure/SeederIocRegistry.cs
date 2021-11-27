using BurgerHub.Api.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace BurgerHub.Api.Seeder.Infrastructure;

public class SeederIocRegistry
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IConfiguration _configuration;

    public SeederIocRegistry(
        IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        _serviceCollection = serviceCollection;
        _configuration = configuration;
    }

    public void Register()
    {
        RegisterApi();
        RegisterSeeding();
        RegisterConfiguration();
        RegisterOptions();
    }

    private void RegisterOptions()
    {
        _serviceCollection.Configure<SeedingOptions>(
            _configuration.GetSection("Seeding"));
    }

    private void RegisterConfiguration()
    {
        _serviceCollection.AddSingleton(_configuration);
    }

    private void RegisterApi()
    {
        var apiIocRegistry = new ApiIocRegistry(
            _serviceCollection,
            _configuration);
        apiIocRegistry.Register();
    }

    private void RegisterSeeding()
    {
        _serviceCollection.AddTransient<IFakerFactory, FakerFactory>();
        _serviceCollection.AddTransient<ISeedingService, SeedingService>();
    }
}