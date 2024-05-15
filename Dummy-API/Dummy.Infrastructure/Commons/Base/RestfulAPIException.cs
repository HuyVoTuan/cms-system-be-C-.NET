using System.Collections;
using System.Net;

namespace Dummy.Infrastructure.Commons.Base
{
    public class RestfulAPIException : Exception
    {
        public static string STATUS_CODE = "status_code";
        public HttpStatusCode _httpStatusCode { get; }
        public IDictionary<string, IEnumerable<string>> _errors { get; }

        public RestfulAPIException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            _httpStatusCode = httpStatusCode;
        }

        // Constructor for [Unauthorized], [NoContent], etc   
        public RestfulAPIException(HttpStatusCode httpStatusCode)
        {
            _httpStatusCode = httpStatusCode;
        }

        // Constructor for Fluent Validation Pipeline 
        public RestfulAPIException(HttpStatusCode httpStatusCode, IDictionary<string, IEnumerable<string>> errors) 
        {
            _httpStatusCode = httpStatusCode;
            _errors = errors;
        }

        public override IDictionary Data => new Dictionary<string, int>()
        {
            {STATUS_CODE, (int)_httpStatusCode }
        };
    }
}
