using System;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Threading.Tasks;
using Authentication.Lib.Config;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Lib
{
    internal class Authorization : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public Response Result { get; internal set; }
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Response result = AuthorizationFromToken(context);

            if (result.StatusCode != (int)HttpStatusCode.OK)
            {
                context.Result = new StatusCodeResult(result.StatusCode);
                Result = result;
                return null;
            }

            return Task.CompletedTask;
        }

        private Response AuthorizationFromToken(AuthorizationFilterContext context)
        {
            try
            {
                var token = context.HttpContext.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    return new Response
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Error = "Missing authorization token"
                    };
                }

                return ValidateJwtToken(token.Replace("Bearer ", ""));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Error = ex.Message
                };
            }
        }

        public Response ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("Claims.SecretKey"));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = Environment.GetEnvironmentVariable("Claims.Issuer"),
                    ValidAudience = Environment.GetEnvironmentVariable("Claims.Audience")
                }, out SecurityToken validatedToken);

                return new Response
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Response
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "You must authenticate first"
                };
            }
        }
    }
}