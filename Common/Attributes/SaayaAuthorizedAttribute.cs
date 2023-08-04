using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Saaya.API.Db;
using Saaya.API.Db.Extensions;

namespace Saaya.API.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SaayaAuthorizedAttribute : Attribute, IAuthorizationFilter
    {
        public SaayaAuthorizedAttribute()
        { }

        /// <summary>
        /// Called early in the filter pipeline to authenticate the request.
        /// </summary>
        /// <param name="context">The filter context.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var auth))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //var authToken = context.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var _db = context.HttpContext.RequestServices.GetRequiredService(typeof(ApiContext)) as ApiContext;
            if (!_db.Users.UserExists(auth.ToString().Split(" ")[1]))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.Items["AuthToken"] = auth.ToString().Split(" ")[1];
        }
    }
}