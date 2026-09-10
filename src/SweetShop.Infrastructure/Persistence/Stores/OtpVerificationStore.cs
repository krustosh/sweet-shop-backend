using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Authentication;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Represents a store for managing OTP verification records in the database, providing methods to retrieve and add OTP verification entities.
/// </summary>
public sealed class OtpVerificationStore : IOtpVerificationStore
{
    private readonly SweetShopDbContext dbContext;

/// <summary>
/// Initializes a new instance of the <see cref="OtpVerificationStore"/> class.
/// </summary>
/// <param name="dbContext"></param>
/// <exception cref="ArgumentNullException"></exception>
    public OtpVerificationStore(
        SweetShopDbContext dbContext)
    {
        this.dbContext = dbContext
            ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Retrieves the most recent pending OTP verification record for the specified mobile number, if it exists and has not expired.
    /// </summary>
    /// <param name="mobileNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OtpVerification?> GetPendingByMobileNumberAsync(
        string mobileNumber,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await dbContext
            .Set<OtpVerification>()
            .Where(otp =>
                otp.MobileNumber == mobileNumber &&
                otp.Status == OtpVerificationStatus.Pending &&
                otp.ExpiresAt > now)
            .OrderByDescending(otp => otp.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new OTP verification record to the database context.
    /// </summary>
    /// <param name="verification"></param>
    public void Add(OtpVerification verification)
    {
        ArgumentNullException.ThrowIfNull(verification);

        dbContext.Set<OtpVerification>().Add(verification);
    }
}