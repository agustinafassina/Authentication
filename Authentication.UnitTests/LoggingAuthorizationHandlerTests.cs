using System;
using System.Security.Claims;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Authentication.Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.UnitTests
{
    public class LoggingAuthorizationHandlerTests
    {
        private const int Expiration = 1960052866;
        private const string SecretKey = "af123";
        private const string Audience = "https://af.com.ar";
        private const string Issuer = "https://af.com.ar";
        public LoggingAuthorizationHandlerTests()
        {
            Environment.SetEnvironmentVariable("Claims.SecretKey", SecretKey);
            Environment.SetEnvironmentVariable("Claims.Issuer", Issuer);
            Environment.SetEnvironmentVariable("Claims.Audience", Audience);
        }

        [Test, Order(1)]
        public void LoggingAuthorizationHandler_ShouldReturnFail()
        {
            string tokenMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6ImFmMTIzIn0.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkFndXMgRmFzc2luYSIsImlhdCI6MTk2MDA1Mjg2NiwiYXVkIjoiaHR0cHM6Ly9hZi5jb20uYXIiLCJpc3MiOiJodHRwczovL2FmLmNvbS5hciIsImV4cCI6MTk2MDA1Mjg2Nn0.187wdTr7eahxVtoMEaia5u1E8JmWMcdj4pEu9HBqBZk";
            var requirement = new List<IAuthorizationRequirement> { new LoggingAuthorizationHandler() };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                new Claim("Issuer", Issuer),
                new Claim("Audience", Audience)
                }
            ));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = tokenMock;

            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor());
            var otherr = new AuthorizationFilterContext(actionContext, new IFilterMetadata[]{});
            var authorizationContext = new AuthorizationHandlerContext(requirement, user, otherr);
            var authorizationHandler = new LoggingAuthorizationHandler();

            authorizationHandler.HandleAsync(authorizationContext);
            authorizationContext.HasSucceeded.Should().BeFalse();
        }

        [Test, Order(2)]
        public void LoggingAuthorizationHandler_ShouldReturnSucceed()
        {
            string tokenMock = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjIxMTY2OTY4ODYsImlzcyI6Imh0dHBzOi8vdGVzdC55b3ViaW0uY29tIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnlvdWJpbS5jb20iLCJyb2xlIjoiWisrIiwiaWF0IjoxNjQzNjU2ODg3LCJ1c2VyIjp7ImlkIjoxMywidXNlcm5hbWUiOiJ6aW1wbGVtZW50IiwiZW1haWwiOiJhZG1pbkB5b3ViaW0uY29tIiwiYWNjb3VudF9pZCI6MywiYnVpbGRpbmdzIjpbeyJpZCI6MTIxNCwibmFtZSI6IkFBQUFBQUFBQUFBQUEiLCJwZXJtaXNzaW9uIjpmYWxzZX0seyJpZCI6MTU5MywibmFtZSI6InJldml0IiwicGVybWlzc2lvbiI6ZmFsc2V9LHsiaWQiOjE1OTQsIm5hbWUiOiJOYXZpcyIsInBlcm1pc3Npb24iOmZhbHNlfV19fQ.sQDq0l2lQXXW35j0NG-Z0cSn-4sZc5Q0EZ_6BJfcopc";
            var requirement = new List<IAuthorizationRequirement> { new LoggingAuthorizationHandler() };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                new Claim("Issuer", Issuer),
                new Claim("Audience", Audience),
                new Claim("Exp", Expiration.ToString())
                }
            ));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = tokenMock;

            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor());
            var otherr = new AuthorizationFilterContext(actionContext, new IFilterMetadata[]{});
            var authorizationContext = new AuthorizationHandlerContext(requirement, user, otherr);
            var authorizationHandler = new LoggingAuthorizationHandler();

            authorizationHandler.HandleAsync(authorizationContext);

            authorizationContext.HasSucceeded.Should().BeTrue();
        }
    }
}