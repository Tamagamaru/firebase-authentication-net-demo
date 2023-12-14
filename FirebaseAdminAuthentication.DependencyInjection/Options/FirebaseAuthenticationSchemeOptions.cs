using Microsoft.AspNetCore.Authentication;

namespace FirebaseAdminAuthentication.DependencyInjection.Options
{
    public class FirebaseAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public bool CheckRevoked { get; set; }
        public bool GetCustomClaims { get; set; } = true;
        public bool SplitRoleClaimArrays { get; set; }
    }
}
