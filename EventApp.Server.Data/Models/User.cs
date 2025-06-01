using Microsoft.AspNetCore.Identity;

namespace Data.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshExpires { get; set; }
        public List<Event>? Events { get; set; }
        public List<EventParticipation>? EventParticipations { get; set; }
    }
}
