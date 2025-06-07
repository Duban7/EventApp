namespace Services.DTOs
{
    public class ParticipantDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? RegistrationDateTime { get; set; }
    }
}
