using System.Diagnostics.CodeAnalysis;

namespace Nuka.Core.Models
{
    [ExcludeFromCodeCoverage]
    public static class StandardLogProperties
    {
        public const string CorrelationId = "correlation_id";
        public const string Hostname = "hostname";
            
        // Http request properties
        public const string HttpResponseStatus = "http_response_status";
        public const string HttpRequestScheme = "http_request_scheme";
        public const string HttpRequestMethod = "http_request_method";
        public const string HttpRequestRoute = "http_request_route";
        public const string HttpRequestUri = "http_request_uri";
        public const string HttpRequestHost = "http_request_host";
        public const string HttpResponseTime = "http_response_time";
        public const string HttpContentType = "http_content_type";
        public const string HttpContentLength = "http_content_length";
    }
}