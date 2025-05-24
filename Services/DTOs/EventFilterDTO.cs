namespace Services.DTOs
{
    public record class EventFilterDTO
    {
        public string? Category { get; set; }
        public DateTime? StartDate { get; set; }
        public string? EventPlace { get; set; }
    }
}
