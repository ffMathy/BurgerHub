using BurgerHub.Api.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BurgerHub.Api.Seeder.Infrastructure;

public class SeederIocRegistry
{
    private readonly IServiceCollection _serviceCollection;

    public SeederIocRegistry(
        IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void Register()
    {
        RegisterApi();
        RegisterFakers();
    }

    private void RegisterApi()
    {
        var apiIocRegistry = new ApiIocRegistry(_serviceCollection);
        apiIocRegistry.Register();
    }

    private void RegisterFakers()
    {
        _serviceCollection.AddTransient<IFakerFactory, FakerFactory>();
    }
}