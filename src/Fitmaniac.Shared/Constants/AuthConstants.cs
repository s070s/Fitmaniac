namespace Fitmaniac.Shared.Constants;

public static class AuthConstants
{
    public const string WebCookieName = ".FMC.Auth";
    public const string RefreshCookieName = "refreshToken";
    public const string MobileClientHeader = "X-Client";
    public const string MobileClientValue = "mobile";
    public const string AccessTokenClaim = "fitmaniac.access_token";
    public const string SubscriptionTierClaim = "subscriptionTier";
    public const string DataProtectionPurpose = "Fitmaniac.Web.AuthService.Ticket";
}
