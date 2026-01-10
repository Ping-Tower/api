namespace Domain.IdentityManager.Security;

public class AppRoles
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);
    public static string[] Roles => [Admin, User];
}