using Data.Context;
using Data.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EventAppDbContext _context;
        private readonly UserManager<User> _manager;
        public UserRepository(EventAppDbContext context, UserManager<User> manager) 
        {
            _context = context;
            _manager = manager;
        }

        public Task CreateUser(User newUser)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUser(string userId)
        {
            throw new NotImplementedException();
        }
            
        public Task<User> GetUserById(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersByEventId(int eventId)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUser(User updatedUser)
        {
            throw new NotImplementedException();
        }

        Task IUserRepository.UpdateUser(User updatedUser)
        {
            return UpdateUser(updatedUser);
        }
    }
}
