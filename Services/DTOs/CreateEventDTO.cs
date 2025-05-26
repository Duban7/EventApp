namespace Services.DTOs
{
    public class CreateEventDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public string? EventPlace { get; set; }
        public string? Category { get; set; }
        public int? MaxParticipantsCount { get; set; }
    }
}
