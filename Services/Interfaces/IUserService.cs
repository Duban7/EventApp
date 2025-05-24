using Data.Models;
using Services.DTOs;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserDTO?> CreateUser(LogInUserDTO newUserDTO);
        public Task<UserDTO?> UpdateUser(UserDTO updatedUserDTO);
        public Task DeleteUser(string userId);
        public Task<UserDTO?> LogIn(string email, string password);
        public Task<UserDTO?> GetUserById(string userId);
        public Task<List<UserDTO>?> GetUsersByEventId(string eventId);
        public Task RegisterUserInEvent(string userId, string eventId);
        public Task UnregisterUserInEvent(string userId, string eventId);
    }
}
