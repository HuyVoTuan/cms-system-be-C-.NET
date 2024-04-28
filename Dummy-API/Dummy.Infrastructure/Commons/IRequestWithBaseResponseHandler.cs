using Dummy.Infrastructure.Commons.Base;
using MediatR;

namespace Dummy.Infrastructure.Commons
{
    public interface IRequestWithBaseResponseHandler<TRequest, TResponse> : IRequestHandler<TRequest, BaseResponseDTO<TResponse>> where TRequest : IRequestWithBaseResponse<TResponse>
    {
    }

    public interface IRequestWithBaseResponseHandler<TRequest> : IRequestHandler<TRequest, BaseResponseDTO> where TRequest : IRequestWithBaseResponse
    {
    }
}
