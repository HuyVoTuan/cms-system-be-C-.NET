using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Dummy.Infrastructure.Commons.Base
{
    public class CustomActionResult<T> : IActionResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public T Data { get; set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(Data)
            {
                StatusCode = (int)StatusCode
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
