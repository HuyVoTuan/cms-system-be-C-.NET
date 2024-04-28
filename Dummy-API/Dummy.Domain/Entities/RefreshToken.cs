using Dummy.Domain.Commons;

namespace Dummy.Domain.Entities
{
    public class RefreshToken : BaseEntity<Guid>
    {
        public Member Member { get; set; }
        public Guid MemberId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
