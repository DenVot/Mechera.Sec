using Mechera.Sec.Authorization.Tools;
using Mechera.Sec.Data.Models;
using Mechera.Sec.Data.Repositories;
using Moq;
using System.Security.Cryptography;
using System.Text;

namespace Mechera.Sec.Authorization.Tests;

public class UserAuthenticatorTests
{
    [Fact]
    public async Task Login_Test_Must_Be_Success()
    {
        const string password = "password";
        var testUser = new User()
        {
            Username = "test",
            PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password)),
            IsRoot = true
        };


        var usersRepoMock = new Mock<IUsersRepository>();

        usersRepoMock.Setup(repo => repo.GetAsync(It.IsAny<string>())).ReturnsAsync(testUser);
           
        var userAuth = new UserAuthenticator(usersRepoMock.Object);
        var result = await userAuth.AuthenticateAsync("test", password);

        Assert.NotNull(result);
        Assert.Equal(testUser, result);
    }

    [Fact]
    public async Task Login_Test_Must_Be_Failed_Because_Wrong_Password()
    {
        const string password = "password";
        var testUser = new User()
        {
            Username = "test",
            PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password)),
            IsRoot = true
        };


        var usersRepoMock = new Mock<IUsersRepository>();

        usersRepoMock.Setup(repo => repo.GetAsync(It.IsAny<string>())).ReturnsAsync(testUser);

        var userAuth = new UserAuthenticator(usersRepoMock.Object);
        var result = await userAuth.AuthenticateAsync("test", "wrong password");

        Assert.Null(result);
    }


    [Fact]
    public async Task Login_Test_Must_Be_Failed_Because_User_Not_Found()
    {  
        var usersRepoMock = new Mock<IUsersRepository>();

        usersRepoMock.Setup(repo => repo.GetAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var userAuth = new UserAuthenticator(usersRepoMock.Object);
        var result = await userAuth.AuthenticateAsync("test", "password");

        Assert.Null(result);
    }
}
