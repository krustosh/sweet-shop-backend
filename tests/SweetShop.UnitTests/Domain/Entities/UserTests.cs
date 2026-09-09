using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;

namespace SweetShop.UnitTests.Domain.Entities;

public sealed class UserTests
{
    [Fact]
    public void ConstructorWithInvalidMobileNumberThrowsArgumentException()
    {
        var user = new User("9876543210", UserRole.Customer);

        Assert.Equal("9876543210", user.MobileNumber);
        Assert.Equal(UserRole.Customer, user.Role);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.True(user.CanAuthenticate);
        Assert.Null(user.LastLoginAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567890")]
    [InlineData("5123456789")]
    [InlineData("987654321")]
    [InlineData("98765432101")]
    [InlineData("98765A3210")]
    public void ConstructorWithSupportedRoleCreatesUser(
        string mobileNumber)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new User(mobileNumber, UserRole.Customer));

        Assert.Equal("mobileNumber", exception.ParamName);
    }

    [Theory]
    [InlineData(UserRole.Customer)]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Delivery)]
    public void ConstructorWithInvalidRoleThrowsArgumentOutOfRangeException(
        UserRole role)
    {
        var user = new User("9876543210", role);

        Assert.Equal(role, user.Role);
        Assert.Equal(UserStatus.Active, user.Status);
    }

    [Fact]
    public void ChangeRoleWithValidRoleUpdatesRole()
    {
        var invalidRole = (UserRole)999;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new User("9876543210", invalidRole));

        Assert.Equal("role", exception.ParamName);
    }

    [Fact]
    public void ChangeRoleWithInvalidRoleThrowsArgumentOutOfRangeException()
    {
        var user = new User("9876543210", UserRole.Customer);
        var invalidRole = (UserRole)999;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => user.ChangeRole(invalidRole));

        Assert.Equal("role", exception.ParamName);
        Assert.Equal(UserRole.Customer, user.Role);
    }

    [Fact]
    public void DeactivateMakesUserUnableToAuthenticate()
    {
        var user = new User("9876543210", UserRole.Customer);

        user.Deactivate();

        Assert.Equal(UserStatus.Inactive, user.Status);
        Assert.False(user.CanAuthenticate);
    }

    [Fact]
    public void ActivateMakesUserAbleToAuthenticate()
    {
        var user = new User("9876543210", UserRole.Customer);

        user.Deactivate();
        user.Activate();

        Assert.Equal(UserStatus.Active, user.Status);
        Assert.True(user.CanAuthenticate);
    }

    [Fact]
    public void BlockMakesUserUnableToAuthenticate()
    {
        var user = new User("9876543210", UserRole.Customer);

        user.Block();

        Assert.Equal(UserStatus.Blocked, user.Status);
        Assert.False(user.CanAuthenticate);
    }

    [Fact]
    public void RecordLoginSetsLastLoginAt()
    {
        var user = new User("9876543210", UserRole.Customer);

        var before = DateTime.UtcNow;

        user.RecordLogin();

        var after = DateTime.UtcNow;

        Assert.NotNull(user.LastLoginAt);
        Assert.InRange(user.LastLoginAt.Value, before, after);
    }
}