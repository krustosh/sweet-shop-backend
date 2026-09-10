using SweetShop.Application.Authentication.Requests;
using SweetShop.Application.Authentication.Responses;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;

namespace SweetShop.Application.Authentication;

/// <summary>
/// Provides application-level authentication operations.
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private const int OtpMaxAttempts = 5;
    private const int OtpExpirationMinutes = 5;

    private readonly IUserStore userStore;
    private readonly IOtpVerificationStore otpVerificationStore;
    private readonly IOtpGenerator otpGenerator;
    private readonly IOtpHasher otpHasher;
    private readonly IOtpSender otpSender;
    private readonly IJwtTokenService jwtTokenService;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="userStore">The user persistence store.</param>
    /// <param name="otpVerificationStore">The OTP persistence store.</param>
    /// <param name="otpGenerator">The OTP generator.</param>
    /// <param name="otpHasher">The OTP hasher.</param>
    /// <param name="otpSender">The OTP sender.</param>
    /// <param name="jwtTokenService">The JWT token service.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public AuthenticationService(
        IUserStore userStore,
        IOtpVerificationStore otpVerificationStore,
        IOtpGenerator otpGenerator,
        IOtpHasher otpHasher,
        IOtpSender otpSender,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(userStore);
        ArgumentNullException.ThrowIfNull(otpVerificationStore);
        ArgumentNullException.ThrowIfNull(otpGenerator);
        ArgumentNullException.ThrowIfNull(otpHasher);
        ArgumentNullException.ThrowIfNull(otpSender);
        ArgumentNullException.ThrowIfNull(jwtTokenService);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        this.userStore = userStore;
        this.otpVerificationStore = otpVerificationStore;
        this.otpGenerator = otpGenerator;
        this.otpHasher = otpHasher;
        this.otpSender = otpSender;
        this.jwtTokenService = jwtTokenService;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<RequestOtpResponse> RequestOtpAsync(
        RequestOtpRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var mobileNumber = NormalizeMobileNumber(request.MobileNumber);

        var user = await userStore.GetByMobileNumberAsync(
            mobileNumber,
            cancellationToken);

        if (user is null)
        {
            user = new User(mobileNumber, UserRole.Customer);

            userStore.Add(user);
        }

        if (!user.CanAuthenticate)
        {
            throw new InvalidOperationException(
                "User is not allowed to authenticate.");
        }

        var existingOtp =
            await otpVerificationStore.GetPendingByMobileNumberAsync(
                mobileNumber,
                cancellationToken);

        if (existingOtp is not null)
        {
            existingOtp.Expire(DateTime.UtcNow);
        }

        var otp = otpGenerator.Generate();
        var codeHash = otpHasher.Hash(otp);
        var expiresAt = DateTime.UtcNow.AddMinutes(
            OtpExpirationMinutes);

        var verification = new OtpVerification(
            user.Id,
            mobileNumber,
            codeHash,
            expiresAt,
            OtpMaxAttempts);

        otpVerificationStore.Add(verification);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await otpSender.SendAsync(
            mobileNumber,
            otp);

        return new RequestOtpResponse(expiresAt);
    }

    /// <inheritdoc />
    public async Task<VerifyOtpResponse> VerifyOtpAsync(
        VerifyOtpRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var mobileNumber = NormalizeMobileNumber(request.MobileNumber);

        if (string.IsNullOrWhiteSpace(request.Otp))
        {
            throw new ArgumentException(
                "OTP is required.",
                nameof(request));
        }

        var user = await userStore.GetByMobileNumberAsync(
            mobileNumber,
            cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException(
                "User was not found.");
        }

        if (!user.CanAuthenticate)
        {
            throw new InvalidOperationException(
                "User is not allowed to authenticate.");
        }

        var verification =
            await otpVerificationStore.GetPendingByMobileNumberAsync(
                mobileNumber,
                cancellationToken);

        if (verification is null)
        {
            throw new InvalidOperationException(
                "No pending OTP verification was found.");
        }

        var now = DateTime.UtcNow;

        if (!otpHasher.Verify(
                request.Otp,
                verification.CodeHash))
        {
            verification.RecordFailedAttempt(now);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            throw new InvalidOperationException(
                "Invalid OTP.");
        }

        verification.MarkVerified(now);

        user.RecordLogin();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = jwtTokenService.GenerateAccessToken(
            user.Id,
            user.MobileNumber,
            user.Role.ToString());

        return new VerifyOtpResponse(
            accessToken,
            now.AddMinutes(60));
    }

    private static string NormalizeMobileNumber(string mobileNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobileNumber);

        var normalized = mobileNumber.Trim();

        if (normalized.StartsWith("+91", StringComparison.Ordinal))
        {
            normalized = normalized[3..];
        }

        if (normalized.StartsWith("91", StringComparison.Ordinal) &&
            normalized.Length == 12)
        {
            normalized = normalized[2..];
        }

        if (normalized.Length != 10 ||
            !normalized.All(char.IsDigit))
        {
            throw new ArgumentException(
                "Mobile number must contain exactly 10 digits.",
                nameof(mobileNumber));
        }

        return normalized;
    }
}