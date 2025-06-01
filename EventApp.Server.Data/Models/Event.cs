namespace Data.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; } = "No Description";
        public DateTime? StartDate { get; set; }
        public string? EventPlace { get; set; } = "not specified";
        public string Category { get; set; } = "No category";
        public int MaxParticipantsCount { get; set; } = 30;
        public bool IsFull { get; set; } = false;
        public string? ImagePath { get; set; } 
        public List<User>? Participants { get; set; }
        public List<EventParticipation>? EventParticipations { get; set; }
    }
}
