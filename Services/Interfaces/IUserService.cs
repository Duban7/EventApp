using Data.Models;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> CreateUser(User newUser);
        public Task<User?> UpdateUser(User updatedUser);
        public Task DeleteUser(string userId);
        public Task<User?> LogIn(string email, string password);
        public Task<User?> GetUserById(string userId);
        public Task<List<User>?> GetUsersByEventId(string eventId);
        public Task RegisterUserInEvent(string userId, string eventId);
        public Task UnregisterUserInEvent(string userId, string eventId);
    }
}
