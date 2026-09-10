using Microsoft.EntityFrameworkCore;
using SweetShop.Application.Authentication;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;
using SweetShop.Infrastructure.Persistence.Context;

namespace SweetShop.Infrastructure.Persistence.Stores;

/// <summary>
/// Provides Entity Framework Core persistence operations for OTP verifications.
/// </summary>
public sealed class OtpVerificationStore : IOtpVerificationStore
{
    private readonly SweetShopDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="OtpVerificationStore"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public OtpVerificationStore(SweetShopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<OtpVerification?> GetPendingByMobileNumberAsync(
        string mobileNumber,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobileNumber);

        return await dbContext.Set<OtpVerification>()
            .Where(otp =>
                otp.MobileNumber == mobileNumber &&
                otp.Status == OtpVerificationStatus.Pending)
            .OrderByDescending(otp => otp.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(OtpVerification verification)
    {
        ArgumentNullException.ThrowIfNull(verification);

        dbContext.Set<OtpVerification>().Add(verification);
    }
}