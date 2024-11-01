using Moq;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using SimulatedAnnealing.Server.Models.Authentication.Dto;
using SimulatedAnnealing.Server.Models.Authentication;
using SimulatedAnnealing.Server.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ReturnsExtensions;
using MockQueryable;
using NSubstitute;

namespace SimulatedAnnealing.Server.UnitTests;

public class UserServiceTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<SignInManager<AppUser>> _signInManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object, new Mock<IHttpContextAccessor>().Object, 
            new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object, 
            null, null, null, null);
        _tokenServiceMock = new Mock<ITokenService>();
        _sut = new UserService(_userManagerMock.Object, _signInManagerMock.Object, _tokenServiceMock.Object);
    }

    [Fact] 
    public async Task TryRegisterAsync_ShouldReturnNewUserDto_WhenProvidedCredentialsAreValid()
    {
        // Arrange
        var registerDTO = new RegisterDto
        {
            Email = "sample@email.com",
            Username = "testLogin",
            Password = "kongStrong1!"
        };
        var appUser = new AppUser { UserName = registerDTO.Username, Email = registerDTO.Email };
        var identityResult = IdentityResult.Success;
        var newUserDto = new NewUserDto { Email = registerDTO.Email, UserName = registerDTO.Username, Token = "test_token" };

        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(identityResult);
        _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User")).ReturnsAsync(identityResult);
        _tokenServiceMock.Setup(ts => ts.CreateToken(It.IsAny<AppUser>())).Returns("test_token");

        // Act
        var (newUser, exception) = await _sut.TryRegisterAsync(registerDTO);

        // Assert
        Assert.NotNull(newUser);
        Assert.Null(exception);
        Assert.Equal(registerDTO.Username, newUser.UserName);
        Assert.Equal(registerDTO.Email, newUser.Email);
        Assert.NotNull(newUser.Token);
    }

    [Theory]
    [InlineData("","", "password")]
    [InlineData("sample@email.com","Login", "")]
    [InlineData("sample@email.com","Login12", "weakpassword")]
    public async Task TryRegisterAsync_ShouldReturnException_WhenProvidedCredentialsAreNotValid(string email, string login, string password)
    {
        // Arrange
        var registerDTO = new RegisterDto
        {
            Email = email,
            Username = login,
            Password = password
        };
        var appUser = new AppUser { UserName = registerDTO.Username, Email = registerDTO.Email };
        var identityResult = IdentityResult.Failed();
        
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(identityResult);
        _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), "User")).ReturnsAsync(identityResult);

        // Act
        var (newUser, exception) = await _sut.TryRegisterAsync(registerDTO);

        // Assert
        Assert.Null(newUser);
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task TryLoginUserAsync_ShouldReturnException_WhenInvalidCredentialsProvided()
    {
        // Arrange
        var loginDto = new LoginDto()
        {
            Username = "DoesNotExist",
            Password = "password"
        };

        var emptyUsersQueryable = new List<AppUser>().AsQueryable().BuildMock<AppUser>();
        _userManagerMock.Setup(um => um.Users).Returns(emptyUsersQueryable);
        
        // Act
        var (user, exception) = await _sut.TryLoginUserAsync(loginDto);
       
        // Assert
        Assert.Null(user);
        Assert.NotNull(exception);   
    }

    [Fact]
    public async Task TryLoginUserAsync_ShouldReturnUser_WhenProperCredentialsProvided()
    {
        // Arrange
        var loginDto = new LoginDto()
        {
            Username = "testing",
            Password = "Testing1!"
        };

        var users = new List<AppUser>() { new AppUser
        {
            UserName = "testing",
        } }
            .AsQueryable()
            .BuildMock(); //Because AsQueryable does not implement async interace
        
        _userManagerMock.Setup(sm => sm.Users).Returns(users);
        _signInManagerMock.Setup(sim => sim.CheckPasswordSignInAsync(It.IsAny<AppUser>(), loginDto.Password, false)).ReturnsAsync(SignInResult.Success); //Returns Task<SigninResult> so ReturnsASYNC!
        _tokenServiceMock.Setup(ts => ts.CreateToken(It.IsAny<AppUser>())).Returns("test_token"); //It.IsAny<AppUser> - placeholder, doesnt require particular instance of a AppUser

        // Act
        var (user, exception) = await _sut.TryLoginUserAsync(loginDto);

        // Assert
        Assert.NotNull(user);
        Assert.Null(exception);
        Assert.Equal("testing", user?.UserName);
        Assert.Equal("test_token", user?.Token);
    }
}
