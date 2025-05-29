namespace Services.DTOs
{
    public class UserTokenDTO
    {
        public UserDTO? User {  get; set; }
        public string? Token { get; set; }
        public string[]? Roles { get; set; }
    }
}
