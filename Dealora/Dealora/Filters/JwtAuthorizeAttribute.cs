using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Tokens;

public class JwtAuthorizeAttribute : AuthorizeAttribute
{
    private const string SecretKey = "Your_Secret_Key_Must_Be_32_Chars_Long"; // Your secret key

    // Override the AuthorizeCore method to check for the JWT in the Authorization header
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        var token = httpContext.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(token))
            return false;

        token = token.Replace("Bearer ", ""); // Remove the "Bearer" part from the token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);

        try
        {
            // Validate the JWT token
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,  // Checks for token expiration
                ClockSkew = TimeSpan.Zero // No tolerance for clock skew
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            // Extract role from the claims in the JWT
            var role = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

            // Check if the role matches the required role(s) defined in the attribute
            if (string.IsNullOrEmpty(role) || !Roles.Split(',').Contains(role))
            {
                return false;
            }

            // Add the claims to the HttpContext to use it later in the application
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
        }
        catch (Exception ex)
        {
            // Token validation failed
            System.Diagnostics.Debug.WriteLine($"Token validation failed: {ex.Message}");
            return false;
        }

        return true;
    }

    // Override the HandleUnauthorizedRequest to return 401 if authorization fails
    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        // Return 401 Unauthorized if token is missing or invalid
        filterContext.Result = new HttpUnauthorizedResult();
    }

}
