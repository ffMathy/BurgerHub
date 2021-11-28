using Destructurama.Attributed;

namespace BurgerHub.Api.Infrastructure;

public class MongoOptions
{
    [NotLogged] public string ConnectionString { get; set; } = null!;
    [NotLogged] public string DatabaseName { get; set; } = null!;
}