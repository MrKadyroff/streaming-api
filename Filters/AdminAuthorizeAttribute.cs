using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using StreamApi.Options;

namespace StreamApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var opts = context.HttpContext.RequestServices.GetRequiredService<IOptions<AuthOptions>>().Value;
        var auth = context.HttpContext.Request.Headers.Authorization.ToString();

        var token = auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? auth[7..]
            : auth;

        if (string.IsNullOrWhiteSpace(token) || token != opts.AdminToken)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
