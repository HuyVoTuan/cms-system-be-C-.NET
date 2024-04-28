using System.Net;

namespace Dummy.Infrastructure.Commons.Base
{
    public class BaseResponseDTO<T>
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        // Base constructor
        public BaseResponseDTO()
        {
        }

        // Data return only constructor
        public BaseResponseDTO(T data)
        {
            Data = data;
        }
    }

    public class BaseResponseDTO
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
    }
}
