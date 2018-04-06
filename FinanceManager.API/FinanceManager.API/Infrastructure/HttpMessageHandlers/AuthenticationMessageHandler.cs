using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace FinanceManager.API.Infrastructure.HttpMessageHandlers
{
    public class AuthenticationFilter : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var req = context.Request;
            // Get credential from the Authorization header 
            //(if present) and authenticate
            if (req.Headers.Any(h => h.Key == "X-Authentication"))
            {
                var id = new ClaimsIdentity("true");
                var principal = new ClaimsPrincipal(new[] { id });
                // The request message contains valid credential
                context.Principal = principal;
            }
            else
            {
                // The request message contains invalid credential
                context.ErrorResult = new UnauthorizedResult(
                  new AuthenticationHeaderValue[0], context.Request);

            }
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }

        public class ResultWithChallenge : IHttpActionResult
        {
            private readonly IHttpActionResult next;
            public ResultWithChallenge(IHttpActionResult next)
            {
                this.next = next;
            }
            public async Task<HttpResponseMessage> ExecuteAsync(
              CancellationToken cancellationToken)
            {
                var response = await next.ExecuteAsync(cancellationToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    response.Headers.WwwAuthenticate.Add(
                      new AuthenticationHeaderValue("somescheme", "somechallenge"));
                }
                return response;
            }
        }
    }
    public class AuthenticationMessageHandler : DelegatingHandler
    {
        private int _count;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _count);
            request.Headers.Add("Authenticated", "true");
            return base.SendAsync(request, cancellationToken);
        }
    }
}