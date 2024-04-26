using System.Collections;
using System.Net;

namespace Dummy.Infrastructure.Exceptions
{
    public class RestfulAPIException : Exception
    {
        public static string STATUS_CODE = "status_code";
        public HttpStatusCode _httpStatusCode { get; }
        public RestfulAPIException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            _httpStatusCode = httpStatusCode;
        }

        public override IDictionary Data => new Dictionary<string, int>()
        {
            {STATUS_CODE, (int)_httpStatusCode }
        };
    }
}
