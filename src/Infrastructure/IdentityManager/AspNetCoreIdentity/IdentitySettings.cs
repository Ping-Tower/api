namespace Infrastructure.IdentityManager.AspNetCoreIdentity;

public class IdentitySettings
{
    public PasswordSettings Password { get; init; } = new();
    public LockoutOptions Lockout { get; init; } = new();
    public UserOptions User { get; init; } = new();
    public SignInOptions SignIn { get; init; } = new();
    public class PasswordSettings
    {
        public bool RequireDigit { get; init; } = true;
        public bool RequireLowercase { get; init; } = true;
        public bool RequireUppercase { get; init; } = true;
        public bool RequireNonAlphanumeric { get; init; }
        public int RequiredLength { get; init; } = 8;
    }
    public class LockoutOptions
    {
        public int DefaultLockoutTimespanInMinutes { get; init; } = 5;
        public int MaxFailedAccessAttempts { get; init; } = 5;
        public bool AllowedForNewUsers { get; init; } = true;
    }
    public class UserOptions
    {
        public bool RequireUniqueEmail { get; init; } = true;
    }
    public class SignInOptions
    {
        public bool RequireConfirmedEmail { get; init; } = true;
    }
}
