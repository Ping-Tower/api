using Application.DTOs;

namespace Application.Common.Services.IdentityManager;

public interface IIdentityService
{
    Task<LoginResultDto> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<RegistrationResultDto> RegistrationAsync(string email, string password, string name, CancellationToken cancellationToken);
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken);
    Task<RefreshResultDto> Refresh(string refreshToken, CancellationToken cancellationToken);
    Task<string> VerifyEmail(string email, string code, CancellationToken cancellationToken);
    Task<string> ForgotPassword(string email, CancellationToken cancellationToken);
    Task<string> ResetPasswordAsync(string email, string code, string newPassword, CancellationToken cancellationToken);
    Task<string> ResetVerifyEmailCode(string email, CancellationToken cancellationToken);
    Task<CurrentUserDto> GetCurrentUserAsync(string userId, CancellationToken cancellationToken);
}
