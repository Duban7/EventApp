using Data.Models;
using Services.DTOs;
using System.Security.Claims;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserTokenDTO> CreateUser(SignUpUserDTO newUserDTO);
        public Task<UserDTO?> UpdateUser(UserDTO updatedUserDTO);
        public Task DeleteUser(string userId);
        public Task<UserDTO> LogIn(string email, string password);
        public Task<UserDTO?> GetUserById(string userId);
        public Task<List<UserDTO>?> GetUsersByEventId(string eventId);
        public Task RegisterUserInEvent(string userId, string eventId);
        public Task UnregisterUserInEvent(string userId, string eventId);
        public Task<UserDTO> UpdateRefreshToken(string oldToken, string userId);
        public Task<IList<Claim>> GetUserClaims(string userId);
    }
}
