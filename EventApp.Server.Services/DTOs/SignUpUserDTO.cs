﻿namespace Services.DTOs
{
    public class SignUpUserDTO
    {
        public string? Email { get; set; }
        public string? Password { get; set; }  
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
