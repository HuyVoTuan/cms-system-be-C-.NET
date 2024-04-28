using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public record DeleteMemberCommand(String Slug) : IRequestWithBaseResponse;
    internal class DeleteMemberCommandHandler : IRequestWithBaseResponseHandler<DeleteMemberCommand>
    {
        private readonly MainDBContext _mainDBContext;

        public DeleteMemberCommandHandler(MainDBContext mainDBContext)
        {
            _mainDBContext = mainDBContext;
        }
        public async Task<BaseResponseDTO> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
        {
            var query = await _mainDBContext.Members.FirstOrDefaultAsync(x => x.Slug == request.Slug, cancellationToken);

            if (query is null)
            {
                throw new RestfulAPIException(HttpStatusCode.NotFound, $"{request.Slug} member does not exists!");
            }

            _mainDBContext.Members.Remove(query);
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO
            {
                Code = HttpStatusCode.NoContent,
                Message = $"Successfully delete ${query.Email} user"
            };
        }
    }
}
