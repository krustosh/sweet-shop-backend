using SweetShop.Infrastructure.Authentication;

namespace SweetShop.UnitTests.Infrastructure.Authentication;

/// <summary>
/// Unit tests for the <see cref="OtpHasher"/> class, verifying the correctness of OTP hashing and verification functionality.
/// </summary>
public sealed class OtpHasherTests
{
    /// <summary>
    /// Tests that hashing an OTP and then verifying it with the same OTP returns true, confirming that the hashing and verification processes are consistent.
    /// </summary>
    [Fact]
    public void HashAndVerifyWithSameOtpReturnsTrue()
    {
        var hasher = new OtpHasher();

        var otp = "118519";

        var hash = hasher.Hash(otp);

        var result = hasher.Verify(otp, hash);

        Assert.True(result);
    }

    /// <summary>
    /// Tests that verifying an OTP with a different OTP than the one used to generate the hash returns false, confirming that the verification process correctly identifies mismatched OTPs.
    /// </summary>
    [Fact]
    public void VerifyWithDifferentOtpReturnsFalse()
    {
        var hasher = new OtpHasher();

        var hash = hasher.Hash("118519");

        var result = hasher.Verify("123456", hash);

        Assert.False(result);
    }
}
