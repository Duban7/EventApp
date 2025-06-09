using Services.DTOs;
using System.Security.Claims;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserTokenDTO> CreateUser(SignUpUserDTO newUserDTO);
        public Task<UserDTO> UpdateUser(UpdateUserDTO updatedUserDTO);
        public Task DeleteUser(string userId);
        public Task<UserDTO> LogIn(string email, string password);
        public Task<UserDTO?> GetUserById(string userId);
        public Task<List<ParticipantDTO>?> GetUsersByEventId(int eventId, CancellationToken cancellationToken = default);
        public Task RegisterUserInEvent(string userId, int eventId, CancellationToken cancellationToken = default);
        public Task UnregisterUserInEvent(string userId, int eventId, CancellationToken cancellationToken = default);
        public Task<UserDTO> UpdateRefreshToken(string oldToken, string userId);
        public Task<IList<Claim>> GetUserClaims(string userId);
    }
}
