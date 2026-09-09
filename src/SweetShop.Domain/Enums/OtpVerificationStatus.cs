namespace SweetShop.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of an OTP verification.
/// </summary>
public enum OtpVerificationStatus
{
    /// <summary>
    /// The OTP verification is pending and has not yet been completed.
    /// </summary>
    Pending = 1,
    /// <summary>
    /// The OTP verification has been successfully completed and verified.
    /// </summary>
    Verified = 2,
/// <summary>
/// The OTP verification has expired and is no longer valid.
/// </summary>
    Expired = 3,
    /// <summary>
    /// The OTP verification has failed due to an incorrect OTP or other reasons.
    /// </summary>
    Failed = 4
}