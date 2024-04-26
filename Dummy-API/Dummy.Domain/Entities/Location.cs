using Dummy.Domain.Commons;

namespace Dummy.Domain.Entities
{
    public class Location : BaseEntity<Guid>
    {
        public String Address { get; set; }
        public String District { get; set; }
        public String City { get; set; }
    }
}
