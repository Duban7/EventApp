using Data.Models;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task<User> CreateUser(User newUser);
        public Task UpdateUser(User user);
        public Task DeleteUser(string id);
        public Task<bool> LogIn(string login, string password);
        public Task LogOut();
        public Task<User> GetUserById(string userId);
        public Task<List<User>> GetUsersByEventId(string eventId);
        public Task RegisterUserInEvent(string userId, string eventId);
        public Task UnregisterUserInEvent(string userId, string eventId);
    }
}
