namespace Dummy.Infrastructure.Services.Auth
{
    public interface ICurrentUserService
    {
        public Guid? Id { get; }
        public bool IsAdmin { get; }
    }
}
