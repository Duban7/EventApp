using AutoMapper;
using Data.Interfaces;
using Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.DTOs;
using Services.Interfaces;
using System.Security.Claims;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IValidator<User> _userValidator;
        private readonly IEventRepository _eventRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public UserService(UserManager<User> userManager,
                           SignInManager<User> signInManager,
                           IValidator<User> userValidator,
                           IEventRepository eventRepository,
                           ITokenService tokenService,
                           IMapper mapper) 
        {
            _userManager = userManager;
            _signInManager = signInManager;            _userValidator = userValidator;
            _eventRepository = eventRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<UserTokenDTO> CreateUser(SignUpUserDTO newUserDTO)
        {
            User newUser = _mapper.Map<User>(newUserDTO);
            string password = newUserDTO.Password ?? throw new Exception("Password wasn't sent");

            _userValidator.ValidateAndThrow(newUser);

            User? foundUser = await _userManager.FindByEmailAsync(newUser.Email!);
            if (foundUser != null) throw new Exception("User already exists");

            newUser.RefreshToken = _tokenService.GenerateRefreshToken();
            newUser.RefreshExpires = DateTime.Now.AddDays(7);

            var res = await _userManager.CreateAsync(newUser, password);
            if (!res.Succeeded) throw new Exception("Couldn't create User");

            foundUser = await _userManager.FindByEmailAsync(newUser.Email!) ?? throw new Exception("User not found");

            Claim[] userClaims = [

                new Claim(ClaimTypes.NameIdentifier, foundUser.Id!),
                new Claim(ClaimTypes.Email, foundUser.Email!),
                new Claim(ClaimTypes.Name, foundUser.Name!),
                new Claim(ClaimTypes.Role, "User")
                ];

            res = await _userManager.AddClaimsAsync(foundUser, userClaims);
            if (!res.Succeeded) throw new Exception("Couldn't add Claims");

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);
            UserTokenDTO userTokenDTO = new();
            var claims = await this.GetUserClaims(newUser.Id);

            userTokenDTO.User = userDTO;
            userTokenDTO.Token = _tokenService.GenerateAccesToken(claims);

            return userTokenDTO;
        }

        public async Task DeleteUser(string userId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new Exception("User not found");

            var res = await _userManager.DeleteAsync(foundUser);
            if (!res.Succeeded) throw new Exception("Couldn't delete User");
        }

        public async Task<UserDTO?> GetUserById(string userId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new Exception("User not found");

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task<List<UserDTO>?> GetUsersByEventId(string eventId)
        {
            List<User> foundUsers = await _userManager.Users
                .Where(u => u.EventParticipations.Any(ep => ep.EventId == eventId))
                .Include(u => u.EventParticipations.Where(ep => ep.EventId == eventId))
                .ToListAsync();

            if (foundUsers.Count <= 0 ) throw new Exception("User not found");

            List<UserDTO> userDTO = foundUsers.Select(_mapper.Map<User, UserDTO>).ToList();

            return userDTO;
        }

        public async Task<UserDTO> LogIn(string email, string password)
        {
            var res = await _signInManager.PasswordSignInAsync(email, password, true, false);
            if(!res.Succeeded) throw new Exception("Invalid email or password");

            User? foundUser = await _userManager.FindByEmailAsync(email);
            if (foundUser == null) throw new Exception("User not found");

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task RegisterUserInEvent(string userId, string eventId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new Exception("User not found");

            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new Exception("Event not found");

            foundEvent.Participants?.Add(foundUser);

            await _eventRepository.UpdateEvent(foundEvent);
        }

        public async Task UnregisterUserInEvent(string userId, string eventId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new Exception("User not found");

            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new Exception("Event not found");

            foundEvent.Participants?.Remove(foundUser);

            await _eventRepository.UpdateEvent(foundEvent);
        }

        public async Task<UserDTO?> UpdateUser(UserDTO updatedUserDTO)
        {
            User updatedUser = _mapper.Map<User>(updatedUserDTO);
            _userValidator.ValidateAndThrow(updatedUser);

            User? foundUser = await _userManager.FindByEmailAsync(updatedUser.Email!);
            if (foundUser == null) throw new Exception("User not found");

            if (updatedUser.Name != null)
                foundUser.Name = updatedUser.Name;

            if (updatedUser.Surname != null)
                foundUser.Surname = updatedUser.Surname;

            if (updatedUser.BirthDate != null) 
                foundUser.BirthDate = updatedUser.BirthDate;

            var res = await _userManager.UpdateAsync(foundUser);
            if (!res.Succeeded) throw new Exception("Couldn't update User");

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task<UserDTO> UpdateRefreshToken(string oldToken, string userId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new Exception("User not found");
            if (foundUser.RefreshToken != oldToken) throw new Exception("Token is expired");
            if (foundUser.RefreshExpires < DateTime.Now) throw new Exception("Token is expired");

            foundUser.RefreshToken = _tokenService.GenerateRefreshToken();

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task<IList<Claim>> GetUserClaims(string userId)
        {
            User? user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("Couldn't find user in claimsGet");

            return await _userManager.GetClaimsAsync(user);
        } 
    }
}
