using Moq;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.Users.Queries;

namespace RoadSignCapture.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserService> _userServiceMock;

    public UserServiceTests()
    {
        _userServiceMock = new Mock<IUserService>();
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnListOfUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new User { Email = "test1@example.com" },
            new User { Email = "test2@example.com" }
        };

        _userServiceMock.Setup(x => x.GetAllUsersAsync())
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userServiceMock.Object.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("test1@example.com", result.First().Email);
    }

    [Fact]
    public async Task GetUserBy_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUser = new User { Email = email };

        _userServiceMock.Setup(x => x.GetUserBy(email))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userServiceMock.Object.GetUserBy(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public void UserExists_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var email = "test@example.com";
        _userServiceMock.Setup(x => x.UserExists(email))
            .Returns(true);

        // Act
        var result = _userServiceMock.Object.UserExists(email);

        // Assert
        Assert.True(result);
    }
}