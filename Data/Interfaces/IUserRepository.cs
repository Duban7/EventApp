using Data.Models;

namespace Data.Interfaces
{
    public interface IUserRepository
    {
        public Task CreateUser(User newUser);
        public Task UpdateUser(User updatedUser);
        public Task DeleteUser(string userId);
        public Task<User> GetUserById(string userId);
        public Task<List<User>> GetUsersByEventId(int eventId);
    }
}
