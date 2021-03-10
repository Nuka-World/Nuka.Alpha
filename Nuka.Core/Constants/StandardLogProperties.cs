using System.Diagnostics.CodeAnalysis;

namespace Nuka.Core.Constants
{
    [ExcludeFromCodeCoverage]
    public static class StandardLogProperties
    {
        public const string CorrelationId = "correlation_id";
        public const string Hostname = "hostname";

        // Error related properties
        public const string ErrorName = "error_name";
        public const string ErrorDescription = "error_description";
        public const string ErrorStack = "error_stack";
        public const string ErrorColumn = "error_column";
        public const string ErrorLine = "error_line";
        public const string ErrorResponse = "error_response";

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

        // Outbound dependency call properties - Critical
        public const string OutboundHttpRequestUri = "outbound_http_request_uri";
        public const string OutboundHttpResponseStatus = "outbound_http_response_status";
        public const string OutboundHttpResponseTime = "outbound_http_response_time";
        public const string OutboundHttpRequestMethod = "outbound_http_request_method";

        // Outbound dependency call properties - Extra
        public const string OutboundHttpRequestHost = "outbound_http_request_host";
        public const string OutboundHttpRequestScheme = "outbound_http_request_scheme";
        public const string OutboundHttpRequestSizeBytes = "outbound_http_request_size_in_bytes";
        public const string OutboundHttpResponseSizeBytes = "outbound_http_response_size_in_bytes";
        public const string OutboundHttpRequestHeaderPrefix = "outbound_http_request_header";
        public const string OutboundHttpResponseHeaderPrefix = "outbound_http_response_header";
    }
}