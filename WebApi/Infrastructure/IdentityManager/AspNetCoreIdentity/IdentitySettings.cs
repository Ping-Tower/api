namespace Infrastructure.IdentityManager.AspNetCoreIdentity;

public class IdentitySettings
{
    public PasswordSettings Password { get; init; } = null!;
    public LockoutOptions Lockout { get; init; } = null!;
    public UserOptions User { get; init; } = null!;
    public SignInOptions SignIn { get; init; } = null!;
    public class PasswordSettings
    {
        public bool RequireDigit { get; init; }
        public bool RequireLowercase { get; init; }
        public bool RequireUppercase { get; init; }
        public bool RequireNonAlphanumeric { get; init; }
        public int RequiredLenght { get; init; }
    }
    public class LockoutOptions
    {
        public int DefaultLockoutTimespanInMinutes { get; init; }
        public int MaxFailedAccessAttempts { get; init; }
        public bool AllowedForNewUsers { get; init; }
    }
    public class UserOptions
    {
        public bool RequireUniqueEmail { get; init; }
    }
    public class SignInOptions
    {
        public bool RequireConfirmedEmail { get; init; }
    }
}