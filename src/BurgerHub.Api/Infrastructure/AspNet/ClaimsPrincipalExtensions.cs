using System.Security.Claims;
using MongoDB.Bson;

namespace BurgerHub.Api.Infrastructure.AspNet;

public static class ClaimsPrincipalExtensions
{
    public static ObjectId? GetId(this ClaimsPrincipal? claimsPrincipal)
    {
        var userId = Get(
            claimsPrincipal,
            ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        return ObjectId.Parse(userId);
    }
        
    public static ObjectId GetRequiredId(this ClaimsPrincipal? claimsPrincipal)
    {
        return GetId(claimsPrincipal) ?? 
               throw new InvalidOperationException("No user ID could be found. Perhaps the user is anonymous?");
    }

    private static string? Get(
        this ClaimsPrincipal? claimsPrincipal,
        string name)
    {
        return claimsPrincipal?.Claims
            ?.SingleOrDefault(x => x.Type == name)
            ?.Value;
    }
}