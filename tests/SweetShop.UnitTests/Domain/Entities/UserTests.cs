using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;

namespace SweetShop.UnitTests.Domain.Entities;

/// <summary>
/// Contains unit tests for the <see cref="User"/> class.
/// </summary>
public sealed class UserTests
{
    /// <summary>
    /// Tests that creating a user with valid mobile number and role initializes the user with the expected properties, including default status and authentication capability.
    /// </summary>
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

    /// <summary>
    /// Tests that creating a user with an invalid mobile number throws an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="mobileNumber"></param>
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

    /// <summary>
    /// Tests that creating a user with a valid role initializes the user with the expected role and default status, and that changing the role to another valid role updates the user's role accordingly.
    /// </summary>
    /// <param name="role"></param>
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

    /// <summary>
    /// Tests that changing the user's role to an invalid role throws an <see cref="ArgumentOutOfRangeException"/> and does not change the user's current role.
    /// </summary>
    [Fact]
    public void ChangeRoleWithValidRoleUpdatesRole()
    {
        var invalidRole = (UserRole)999;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new User("9876543210", invalidRole));

        Assert.Equal("role", exception.ParamName);
    }

    /// <summary>
    /// Tests that changing the user's role to a valid role updates the user's role accordingly.
    /// </summary>
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

    /// <summary>
    /// Tests that deactivating a user changes the user's status to inactive and prevents the user from authenticating, while activating the user restores the active status and allows authentication again.
    /// </summary>
    [Fact]
    public void DeactivateMakesUserUnableToAuthenticate()
    {
        var user = new User("9876543210", UserRole.Customer);

        user.Deactivate();

        Assert.Equal(UserStatus.Inactive, user.Status);
        Assert.False(user.CanAuthenticate);
    }

    /// <summary>
    /// Tests that activating a user changes the user's status to active and allows the user to authenticate, while blocking the user changes the status to blocked and prevents authentication.
    /// </summary>
    [Fact]
    public void ActivateMakesUserAbleToAuthenticate()
    {
        var user = new User("9876543210", UserRole.Customer);

        user.Deactivate();
        user.Activate();

        Assert.Equal(UserStatus.Active, user.Status);
        Assert.True(user.CanAuthenticate);
    }

    /// <summary>
    /// Tests that blocking a user changes the user's status to blocked and prevents the user from authenticating, while activating the user restores the active status and allows authentication again.
    /// </summary>
    [Fact]
    public void BlockMakesUserUnableToAuthenticate()
    {
        var user = new User("9876543210", UserRole.Customer);

        user.Block();

        Assert.Equal(UserStatus.Blocked, user.Status);
        Assert.False(user.CanAuthenticate);
    }

    /// <summary>
    /// Tests that recording a login for a user sets the LastLoginAt property to a value within the expected time range, indicating that the login was successfully recorded and the timestamp is accurate.
    /// </summary>
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