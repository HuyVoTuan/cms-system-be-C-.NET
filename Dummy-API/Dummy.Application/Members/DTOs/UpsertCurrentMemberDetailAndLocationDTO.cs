using Dummy.Application.Locations.DTOs;

namespace Dummy.Application.Members.DTOs
{
    public class UpsertCurrentMemberDetailAndLocationDTO
    {
        public String Slug { get; set; }
        public String Avatar { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Position { get; set; }
        public IEnumerable<LocationDTO> Locations { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
