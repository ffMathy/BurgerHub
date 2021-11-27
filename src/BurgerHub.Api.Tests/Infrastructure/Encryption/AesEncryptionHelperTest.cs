using System;
using System.Threading.Tasks;
using BurgerHub.Api.Infrastructure.Encryption;
using BurgerHub.Api.Tests.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace BurgerHub.Api.Tests.Infrastructure.Encryption;

[TestClass]
public class AesEncryptionHelperTest
{
    [TestMethod]
    [TestCategory(TestCategories.UnitCategory)]
    public async Task Encrypt_SameDataEncryptedTwice_ContainsDifferentBytesDueToSalt()
    {
        //Arrange
        var fakeEncryptionOptionsMonitor = Substitute.For<IOptionsMonitor<EncryptionOptions>>();
        fakeEncryptionOptionsMonitor
            .CurrentValue
            .Returns(new EncryptionOptions()
            {
                Pepper = "VQpYmtjVlH$Ys#llTcP9fwPCkzTxs1%f"
            });

        var aesEncryptionHelper = new AesEncryptionHelper(fakeEncryptionOptionsMonitor);

        //Act
        var data1 = await aesEncryptionHelper.EncryptAsync("some-data");
        var data2 = await aesEncryptionHelper.EncryptAsync("some-data");

        //Assert
        Assert.AreNotEqual(
            Convert.ToBase64String(data1),
            Convert.ToBase64String(data2));
    }

    [TestMethod]
    [TestCategory(TestCategories.UnitCategory)]
    public async Task Encrypt_SameDataEncryptedTwiceWithoutSalt_ContainsSameBytes()
    {
        //Arrange
        var fakeEncryptionOptionsMonitor = Substitute.For<IOptionsMonitor<EncryptionOptions>>();
        fakeEncryptionOptionsMonitor
            .CurrentValue
            .Returns(new EncryptionOptions()
            {
                Pepper = "VQpYmtjVlH$Ys#llTcP9fwPCkzTxs1%f"
            });

        var aesEncryptionHelper = new AesEncryptionHelper(fakeEncryptionOptionsMonitor);

        //Act
        var data1 = await aesEncryptionHelper.EncryptAsync("some-data", withoutSalt: true);
        var data2 = await aesEncryptionHelper.EncryptAsync("some-data", withoutSalt: true);

        //Assert
        Assert.AreEqual(
            Convert.ToBase64String(data1),
            Convert.ToBase64String(data2));
    }

    [TestMethod]
    [TestCategory(TestCategories.UnitCategory)]
    public async Task Decrypt_EncryptedString_GetsOriginalValue()
    {
        //Arrange
        var fakeEncryptionOptionsMonitor = Substitute.For<IOptionsMonitor<EncryptionOptions>>();
        fakeEncryptionOptionsMonitor
            .CurrentValue
            .Returns(new EncryptionOptions()
            {
                Pepper = "VQpYmtjVlH$Ys#llTcP9fwPCkzTxs1%f"
            });

        var aesEncryptionHelper = new AesEncryptionHelper(fakeEncryptionOptionsMonitor);

        //Act
        var data = await aesEncryptionHelper.EncryptAsync("some-data");

        //Assert
        Assert.AreEqual("some-data", await aesEncryptionHelper.DecryptAsync(data));
    }
}