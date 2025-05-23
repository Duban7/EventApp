namespace Data.Models
{
    public class Event
    {
        public string? Name { get; set; }
        public string Description { get; set; } = "No Description";
        public DateTime? StartDate { get; set; }
        public String? EventPlace { get; set; } = "not specified";
        public string Category { get; set; } = "No category";
        public int MaxParticipantsCount { get; set; } = 30;
        public string? ImageURL { get; set; } 
        public List<User>? Participants { get; set; }
        public List<EventParticipation>? EventParticipations { get; set; }
    }
}
