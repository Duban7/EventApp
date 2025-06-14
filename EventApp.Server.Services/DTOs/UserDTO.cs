﻿namespace Services.DTOs
{
    public class UserDTO
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshExpires { get; set; }
    }
}
