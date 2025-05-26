namespace Services.DTOs
{
    public class UpdateEventDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public string? EventPlace { get; set; }
        public string? Category { get; set; }
        public int? MaxParticipantsCount { get; set; }
    }
}
