using SweetShop.Application.Authentication;
using SweetShop.Application.Authentication.Requests;
using SweetShop.Application.Interfaces;
using SweetShop.Domain.Entities;
using SweetShop.Domain.Enums;

namespace SweetShop.UnitTests.Authentication;

/// <summary>
/// Contains unit tests for the <see cref="AuthenticationService"/> class.
/// </summary>
public sealed class AuthenticationServiceTests
{
    /// <summary>
    /// Tests that when a user does not exist, the <see cref="AuthenticationService.RequestOtpAsync(RequestOtpRequest, CancellationToken)"/> method creates a new user and sends an OTP.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RequestOtpAsyncWhenUserDoesNotExistCreatesUserAndSendsOtp()
    {
        var userStore = new FakeUserStore();
        var otpStore = new FakeOtpVerificationStore();
        var otpGenerator = new FakeOtpGenerator("123456");
        var otpHasher = new FakeOtpHasher();
        var otpSender = new FakeOtpSender();
        var jwtTokenService = new FakeJwtTokenService();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            userStore,
            otpStore,
            otpGenerator,
            otpHasher,
            otpSender,
            jwtTokenService,
            unitOfWork);

        var result = await service.RequestOtpAsync(
            new RequestOtpRequest("9876543210"),
            CancellationToken.None);

        Assert.NotEqual(default, result.ExpiresAt);
        Assert.Single(userStore.Users);
        Assert.Equal("9876543210", userStore.Users[0].MobileNumber);
        Assert.Equal(UserRole.Customer, userStore.Users[0].Role);

        Assert.Single(otpStore.AddedVerifications);
        Assert.Equal("123456", otpHasher.LastHashedValue);
        Assert.Equal("9876543210", otpSender.MobileNumber);
        Assert.Equal("123456", otpSender.Otp);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    /// <summary>
    /// Tests that when a user already exists and is active, the <see cref="AuthenticationService.RequestOtpAsync(RequestOtpRequest, CancellationToken)"/> method does not create another user but still sends an OTP.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RequestOtpAsyncWithExistingActiveUserDoesNotCreateAnotherUser()
    {
        var existingUser = new User(
            "9876543210",
            UserRole.Customer);

        var userStore = new FakeUserStore(existingUser);
        var otpStore = new FakeOtpVerificationStore();
        var otpGenerator = new FakeOtpGenerator("123456");
        var otpHasher = new FakeOtpHasher();
        var otpSender = new FakeOtpSender();
        var jwtTokenService = new FakeJwtTokenService();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            userStore,
            otpStore,
            otpGenerator,
            otpHasher,
            otpSender,
            jwtTokenService,
            unitOfWork);

        await service.RequestOtpAsync(
            new RequestOtpRequest("9876543210"),
            CancellationToken.None);

        Assert.Single(userStore.Users);
        Assert.Single(otpStore.AddedVerifications);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    /// <summary>
    /// Tests that when a user is blocked, the <see cref="AuthenticationService.RequestOtpAsync(RequestOtpRequest, CancellationToken)"/> method throws an exception and does not send an OTP.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RequestOtpAsyncWithBlockedUserThrows()
    {
        var user = new User(
            "9876543210",
            UserRole.Customer);

        user.Block();

        var userStore = new FakeUserStore(user);
        var otpStore = new FakeOtpVerificationStore();
        var otpGenerator = new FakeOtpGenerator("123456");
        var otpHasher = new FakeOtpHasher();
        var otpSender = new FakeOtpSender();
        var jwtTokenService = new FakeJwtTokenService();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            userStore,
            otpStore,
            otpGenerator,
            otpHasher,
            otpSender,
            jwtTokenService,
            unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RequestOtpAsync(
                new RequestOtpRequest("9876543210"),
                CancellationToken.None));

        Assert.Empty(otpStore.AddedVerifications);
        Assert.Equal(0, otpSender.SendCount);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }
    
    /// <summary>
    /// Tests that when a valid OTP is provided, the <see cref="AuthenticationService.VerifyOtpAsync(VerifyOtpRequest, CancellationToken)"/> method returns an access token and records the login.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyOtpAsyncWithValidOtpReturnsAccessTokenAndRecordsLogin()
    {
        var user = new User(
            "9876543210",
            UserRole.Customer);

        var otp = new OtpVerification(
            user.Id,
            user.MobileNumber,
            "hashed-otp",
            DateTime.UtcNow.AddMinutes(5),
            5);

        var userStore = new FakeUserStore(user);
        var otpStore = new FakeOtpVerificationStore(otp);
        var otpGenerator = new FakeOtpGenerator("123456");
        var otpHasher = new FakeOtpHasher
        {
            VerificationResult = true
        };
        var otpSender = new FakeOtpSender();
        var jwtTokenService = new FakeJwtTokenService("access-token");
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            userStore,
            otpStore,
            otpGenerator,
            otpHasher,
            otpSender,
            jwtTokenService,
            unitOfWork);

        var result = await service.VerifyOtpAsync(
            new VerifyOtpRequest("9876543210", "123456"),
            CancellationToken.None);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal(OtpVerificationStatus.Verified, otp.Status);
        Assert.NotNull(user.LastLoginAt);

        Assert.Equal(user.Id, jwtTokenService.UserId);
        Assert.Equal("9876543210", jwtTokenService.MobileNumber);
        Assert.Equal(UserRole.Customer.ToString(), jwtTokenService.Role);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    /// <summary>
    /// Tests that when an invalid OTP is provided, the <see cref="AuthenticationService.VerifyOtpAsync(VerifyOtpRequest, CancellationToken)"/> method records the failed attempt and does not generate an access token.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyOtpAsyncWithInvalidOtpRecordsFailedAttempt()
    {
        var user = new User(
            "9876543210",
            UserRole.Customer);

        var otp = new OtpVerification(
            user.Id,
            user.MobileNumber,
            "hashed-otp",
            DateTime.UtcNow.AddMinutes(5),
            5);

        var userStore = new FakeUserStore(user);
        var otpStore = new FakeOtpVerificationStore(otp);
        var otpGenerator = new FakeOtpGenerator("123456");
        var otpHasher = new FakeOtpHasher
        {
            VerificationResult = false
        };
        var otpSender = new FakeOtpSender();
        var jwtTokenService = new FakeJwtTokenService();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            userStore,
            otpStore,
            otpGenerator,
            otpHasher,
            otpSender,
            jwtTokenService,
            unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.VerifyOtpAsync(
                new VerifyOtpRequest("9876543210", "000000"),
                CancellationToken.None));

        Assert.Equal(1, otp.AttemptCount);
        Assert.Equal(OtpVerificationStatus.Pending, otp.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
        Assert.Equal(0, jwtTokenService.GenerateCount);
    }

    /// <summary>
    /// Tests that when a user does not exist, the <see cref="AuthenticationService.VerifyOtpAsync(VerifyOtpRequest, CancellationToken)"/> method throws an exception.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyOtpAsyncWhenUserDoesNotExistThrows()
    {
        var service = CreateService(
            new FakeUserStore(),
            new FakeOtpVerificationStore(),
            new FakeOtpGenerator("123456"),
            new FakeOtpHasher(),
            new FakeOtpSender(),
            new FakeJwtTokenService(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.VerifyOtpAsync(
                new VerifyOtpRequest("9876543210", "123456"),
                CancellationToken.None));
    }

    /// <summary>
    /// Tests that when a pending OTP does not exist for a user, the <see cref="AuthenticationService.VerifyOtpAsync(VerifyOtpRequest, CancellationToken)"/> method throws an exception.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyOtpAsyncWhenPendingOtpDoesNotExistThrows()
    {
        var user = new User(
            "9876543210",
            UserRole.Customer);

        var service = CreateService(
            new FakeUserStore(user),
            new FakeOtpVerificationStore(),
            new FakeOtpGenerator("123456"),
            new FakeOtpHasher(),
            new FakeOtpSender(),
            new FakeJwtTokenService(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.VerifyOtpAsync(
                new VerifyOtpRequest("9876543210", "123456"),
                CancellationToken.None));
    }

/// <summary>
/// Tests that when a mobile number with the country code +91 is provided, the <see cref="AuthenticationService.RequestOtpAsync(RequestOtpRequest, CancellationToken)"/> method normalizes it to a 10-digit number before storing and sending the OTP.
/// </summary>
/// <returns></returns>
    [Fact]
    public async Task RequestOtpAsyncNormalizesPlus91MobileNumber()
    {
        var userStore = new FakeUserStore();
        var otpStore = new FakeOtpVerificationStore();
        var otpSender = new FakeOtpSender();

        var service = CreateService(
            userStore,
            otpStore,
            new FakeOtpGenerator("123456"),
            new FakeOtpHasher(),
            otpSender,
            new FakeJwtTokenService(),
            new FakeUnitOfWork());

        await service.RequestOtpAsync(
            new RequestOtpRequest("+919876543210"),
            CancellationToken.None);

        Assert.Equal("9876543210", userStore.Users.Single().MobileNumber);
        Assert.Equal("9876543210", otpSender.MobileNumber);
    }

    private static AuthenticationService CreateService(
        IUserStore userStore,
        IOtpVerificationStore otpStore,
        IOtpGenerator otpGenerator,
        IOtpHasher otpHasher,
        IOtpSender otpSender,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        return new AuthenticationService(
            userStore,
            otpStore,
            otpGenerator,
            otpHasher,
            otpSender,
            jwtTokenService,
            unitOfWork);
    }

    private sealed class FakeUserStore : IUserStore
    {
        public List<User> Users { get; } = [];

        public FakeUserStore(params User[] users)
        {
            Users.AddRange(users);
        }

        public Task<User?> GetByMobileNumberAsync(
            string mobileNumber,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                Users.SingleOrDefault(
                    user => user.MobileNumber == mobileNumber));
        }

        public void Add(User user)
        {
            Users.Add(user);
        }
    }

    private sealed class FakeOtpVerificationStore
        : IOtpVerificationStore
    {
        public List<OtpVerification> AddedVerifications { get; } = [];

        private readonly OtpVerification? existingVerification;

        public FakeOtpVerificationStore(
            OtpVerification? existingVerification = null)
        {
            this.existingVerification = existingVerification;
        }

        public Task<OtpVerification?> GetPendingByMobileNumberAsync(
            string mobileNumber,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(existingVerification);
        }

        public void Add(OtpVerification verification)
        {
            AddedVerifications.Add(verification);
        }
    }

    private sealed class FakeOtpGenerator : IOtpGenerator
    {
        private readonly string otp;

        public FakeOtpGenerator(string otp)
        {
            this.otp = otp;
        }

        public string Generate()
        {
            return otp;
        }
    }

    private sealed class FakeOtpHasher : IOtpHasher
    {
        public string LastHashedValue { get; private set; } = string.Empty;

        public bool VerificationResult { get; set; }

        public string Hash(string otp)
        {
            LastHashedValue = otp;
            return $"hash:{otp}";
        }

        public bool Verify(
            string otp,
            string hash)
        {
            return VerificationResult;
        }
    }

    private sealed class FakeOtpSender : IOtpSender
    {
        public string? MobileNumber { get; private set; }

        public string? Otp { get; private set; }

        public int SendCount { get; private set; }

        public Task SendAsync(
            string mobileNumber,
            string otp)
        {
            MobileNumber = mobileNumber;
            Otp = otp;
            SendCount++;

            return Task.CompletedTask;
        }
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        private readonly string token;

        public Guid UserId { get; private set; }

        public string? MobileNumber { get; private set; }

        public string? Role { get; private set; }

        public int GenerateCount { get; private set; }

        public FakeJwtTokenService(
            string token = "token")
        {
            this.token = token;
        }

        public string GenerateAccessToken(
            Guid userId,
            string mobileNumber,
            string role)
        {
            UserId = userId;
            MobileNumber = mobileNumber;
            Role = role;
            GenerateCount++;

            return token;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCount++;

            return Task.FromResult(1);
        }
    }
}