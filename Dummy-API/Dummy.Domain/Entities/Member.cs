using Dummy.Domain.Commons;

namespace Dummy.Domain.Entities
{
    public class Member : BaseEntity<Guid>
    {
        public String Slug { get; set; }
        public String Avatar { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String Position { get; set; }
        public String Password { get; set; }
        public String Role { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Location> Locations { get; set; }
        public ICollection<RefreshToken> RefreshToken { get; set; }
    }
}
