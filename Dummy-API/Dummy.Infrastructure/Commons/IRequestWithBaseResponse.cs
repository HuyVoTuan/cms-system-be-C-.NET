using Dummy.Infrastructure.Commons.Base;
using MediatR;

namespace Dummy.Infrastructure.Commons
{
    public interface IRequestWithBaseResponse<T> : IRequest<BaseResponseDTO<T>>
    {
    }

    public interface IRequestWithBaseResponse : IRequest<BaseResponseDTO>
    {
    }
}
