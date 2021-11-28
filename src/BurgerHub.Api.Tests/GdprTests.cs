using System.Linq;
using System.Reflection;
using BurgerHub.Api.Tests.Helpers;
using Destructurama.Attributed;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BurgerHub.Api.Tests;

[TestClass]
public class GdprTests
{
    [TestMethod]
    [TestCategory(TestCategories.IntegrationCategory)]
    public void Reflection_ScanningAllAssemblyClasses_NoPotentiallyLoggedGdprReferencesFound()
    {
        //Arrange
        var assemblies = new [] {
            typeof(Program).Assembly
        };
        var types = assemblies.SelectMany(x => x.GetTypes());

        var suspiciousNames = new[]
        {
            "Email",
            "FirstName",
            "PublicName",
            "FullName",
            "LastName",
            "Address",
            "Street",
            "Phone",
            "City",
            "Zip",
            "ApiKey",
            "Password",
            "Postal",
            "Mobile",
            "Token"
        };
            
        //Act & Assert
        foreach(var type in types)
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(string))
                    continue;

                var propertyName = property.Name;
                var attributes = property.GetCustomAttributes();
                var hasLogMaskedAttribute = attributes.Any(x => x is NotLoggedAttribute);
                if (suspiciousNames.Any(propertyName.Contains) && !hasLogMaskedAttribute)
                {
                    Assert.Fail($"The property name {propertyName} on type {type.FullName} looks like personal or sensitive data, but is not marked with a [NotLogged] attribute. This could lead to GDPR issues, because personal data could leak into logs by accident.");
                }
            }
        }
    }
}