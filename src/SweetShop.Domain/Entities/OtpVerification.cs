using SweetShop.Domain.Common;
using SweetShop.Domain.Enums;
using SweetShop.Domain.Exceptions;

namespace SweetShop.Domain.Entities;

/// <summary>
/// Represents an OTP verification attempt for a user.
/// </summary>
public sealed class OtpVerification : AuditableEntity
{
    /// <summary>
    /// Gets the identifier of the user associated with this verification.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the mobile number for which the OTP was issued.
    /// </summary>
    public string MobileNumber { get; private set; }

    /// <summary>
    /// Gets the hashed OTP value.
    /// </summary>
    public string CodeHash { get; private set; }

    /// <summary>
    /// Gets the time at which the OTP expires.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Gets the number of verification attempts made.
    /// </summary>
    public int AttemptCount { get; private set; }

    /// <summary>
    /// Gets the maximum number of verification attempts allowed.
    /// </summary>
    public int MaxAttempts { get; private set; }

    /// <summary>
    /// Gets the current OTP verification status.
    /// </summary>
    public OtpVerificationStatus Status { get; private set; }

    /// <summary>
    /// Gets the time at which the OTP was successfully verified.
    /// </summary>
    public DateTime? VerifiedAt { get; private set; }

    /// <summary>
    /// Initializes a new OTP verification.
    /// </summary>
    /// <param name="userId">The user associated with the OTP.</param>
    /// <param name="mobileNumber">The mobile number receiving the OTP.</param>
    /// <param name="codeHash">The hashed OTP.</param>
    /// <param name="expiresAt">The OTP expiration time.</param>
    /// <param name="maxAttempts">The maximum allowed verification attempts.</param>
    public OtpVerification(
        Guid userId,
        string mobileNumber,
        string codeHash,
        DateTime expiresAt,
        int maxAttempts)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new ArgumentException(
                "Mobile number is required.",
                nameof(mobileNumber));
        }

        if (string.IsNullOrWhiteSpace(codeHash))
        {
            throw new ArgumentException(
                "OTP code hash is required.",
                nameof(codeHash));
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new ArgumentException(
                "OTP expiration must be in the future.",
                nameof(expiresAt));
        }

        if (maxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxAttempts),
                maxAttempts,
                "Maximum OTP attempts must be greater than zero.");
        }

        UserId = userId;
        MobileNumber = mobileNumber.Trim();
        CodeHash = codeHash.Trim();
        ExpiresAt = expiresAt;
        MaxAttempts = maxAttempts;
        Status = OtpVerificationStatus.Pending;
    }

    /// <summary>
    /// Records an unsuccessful OTP verification attempt.
    /// </summary>
    /// <param name="currentTime">The current UTC time.</param>
    public void RecordFailedAttempt(DateTime currentTime)
    {
        if (Status != OtpVerificationStatus.Pending)
        {
            throw new BusinessRuleViolationException(
                "Only pending OTP verifications can record attempts.");
        }

        if (currentTime >= ExpiresAt)
        {
            Status = OtpVerificationStatus.Expired;
            UpdatedAt = currentTime;
            return;
        }

        AttemptCount++;

        if (AttemptCount >= MaxAttempts)
        {
            Status = OtpVerificationStatus.Failed;
        }

        UpdatedAt = currentTime;
    }

    /// <summary>
    /// Marks the OTP as successfully verified.
    /// </summary>
    /// <param name="currentTime">The current UTC time.</param>
    public void MarkVerified(DateTime currentTime)
    {
        if (Status != OtpVerificationStatus.Pending)
        {
            throw new BusinessRuleViolationException(
                "Only pending OTP verifications can be verified.");
        }

        if (currentTime >= ExpiresAt)
        {
            Status = OtpVerificationStatus.Expired;
            UpdatedAt = currentTime;

            throw new BusinessRuleViolationException(
                "OTP verification has expired.");
        }

        if (AttemptCount >= MaxAttempts)
        {
            Status = OtpVerificationStatus.Failed;
            UpdatedAt = currentTime;

            throw new BusinessRuleViolationException(
                "Maximum OTP verification attempts have been exceeded.");
        }

        Status = OtpVerificationStatus.Verified;
        VerifiedAt = currentTime;
        UpdatedAt = currentTime;
    }

    /// <summary>
    /// Marks the OTP as expired when its expiration time has passed.
    /// </summary>
    /// <param name="currentTime">The current UTC time.</param>
    public void Expire(DateTime currentTime)
    {
        if (Status != OtpVerificationStatus.Pending)
        {
            return;
        }

        if (currentTime < ExpiresAt)
        {
            throw new BusinessRuleViolationException(
                "OTP cannot be expired before its expiration time.");
        }

        Status = OtpVerificationStatus.Expired;
        UpdatedAt = currentTime;
    }
}