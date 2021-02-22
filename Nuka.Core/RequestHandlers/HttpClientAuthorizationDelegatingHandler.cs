using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nuka.Core.RequestHandlers
{
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HttpClientAuthorizationDelegatingHandler> _logger;

        public HttpClientAuthorizationDelegatingHandler(
            IHttpContextAccessor httpContextAccessor,
            ILogger<HttpClientAuthorizationDelegatingHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", new List<string>() {authorizationHeader});
            }

            var token = await GetToken();

            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        async Task<string> GetToken()
        {
            return await (_httpContextAccessor.HttpContext ?? throw new InvalidOperationException()).GetTokenAsync("access_token");
        }
    }   
}