namespace Services.DTOs
{
    public record class UserTokenDTO
    {
        public UserDTO? User {  get; set; }
        public string? Token { get; set; }
    }
}
