using Microsoft.Extensions.Logging;
using SweetShop.Application.Authentication;

namespace SweetShop.Infrastructure.Authentication;

/// <summary>
/// Represents a development implementation of the <see cref="IOtpSender"/> interface that logs OTPs instead of sending them, intended for use in development environments.
/// </summary>
public sealed partial class DevelopmentOtpSender : IOtpSender
{
    private readonly ILogger<DevelopmentOtpSender> logger;

/// <summary>
/// Initializes a new instance of the <see cref="DevelopmentOtpSender"/> class with the specified logger.
/// </summary>
/// <param name="logger"></param>
/// <exception cref="ArgumentNullException"></exception>
    public DevelopmentOtpSender(
        ILogger<DevelopmentOtpSender> logger)
    {
        this.logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }
/// <summary>
/// Sends an OTP to the specified mobile number by logging the OTP instead of sending it, for development purposes.
/// </summary>
/// <param name="mobileNumber">The mobile number to which the OTP will be sent.</param>
/// <param name="otp">The OTP to be sent.</param>
/// <returns>A task representing the asynchronous operation.</returns>
    public Task SendAsync(
        string mobileNumber,
        string otp)
    {
        LogDevelopmentOtp(
            logger,
            mobileNumber,
            otp);

        return Task.CompletedTask;
    }

/// <summary>
/// Logs the development OTP for the specified mobile number, providing information for developers to verify OTP functionality without sending actual messages.
/// </summary>
/// <param name="logger"></param>
/// <param name="mobileNumber"></param>
/// <param name="otp"></param>
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Development OTP for mobile number {MobileNumber}: {Otp}")]
    private static partial void LogDevelopmentOtp(
        ILogger logger,
        string mobileNumber,
        string otp);
}