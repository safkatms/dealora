using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;

[assembly: OwinStartup(typeof(Dealora.Startup))]

namespace Dealora
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure JWT Bearer authentication
            ConfigureJwt(app);
        }

        private void ConfigureJwt(IAppBuilder app)
        {
            var key = Encoding.UTF8.GetBytes("your-very-long-secret-key-of-at-least-32-characters"); // Make sure this key is at least 32 bytes long

            // Enable JWT Bearer authentication
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // Set this to true if you want to validate the issuer
                    ValidateAudience = false, // Set this to true if you want to validate the audience
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", // Specify the role claim type if needed
                }
            });
        }
    }
}
