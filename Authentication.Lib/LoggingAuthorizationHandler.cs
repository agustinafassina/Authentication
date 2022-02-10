using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Lib
{
    public class LoggingAuthorizationHandler : AuthorizationHandler<LoggingAuthorizationHandler>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LoggingAuthorizationHandler requirement)
        {
            var authorizationFilterContext = (AuthorizationFilterContext)context.Resource;
            var authorization = new Authorization();
            var result = authorization.OnAuthorizationAsync(authorizationFilterContext);

            if (result != null && result.IsCompletedSuccessfully)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();

                HttpResponse response = authorizationFilterContext.HttpContext.Response;
                var statusCodeResult = authorizationFilterContext.Result as StatusCodeResult;

                string jsonResult = JsonSerializer.Serialize(authorization.Result);
                byte[] message = Encoding.UTF8.GetBytes(jsonResult);
                response.Body.WriteAsync(message, 0, message.Length).Wait();

                response.OnStarting(async () =>
                {
                    response.StatusCode = statusCodeResult.StatusCode;
                    response.Headers.Add("Content-Type", "application/json");
                });
            }

            return Task.CompletedTask;
        }
    }
}