using AutoMapper;
using Data.Interfaces;
using Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.DTOs;
using Services.Exeptions;
using Services.Interfaces;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
            _signInManager = signInManager;
            _userValidator = userValidator;
            _eventRepository = eventRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<UserTokenDTO> CreateUser(SignUpUserDTO newUserDTO)
        {
            User newUser = _mapper.Map<User>(newUserDTO);
            string password = newUserDTO.Password ?? throw new BadRequestException("Password wasn't sent");

            _userValidator.ValidateAndThrow(newUser);
            if (newUser.BirthDate >= DateTime.Now) throw new BadRequestException("User cannot be born in future");

            if (!Regex.IsMatch(password, @"(?=.*[0-9])(?=.*[A-Za-z])[0-9a-zA-Z_\-]{6,100}"))
                throw new BadRequestException("Password should contain at least 1 letter and 1 number. Minimal password length - 6, max - 100."); 

            User? foundUser = await _userManager.FindByEmailAsync(newUser.Email!);
            if (foundUser != null) throw new ConflictException("User already exists");

            newUser.RefreshToken = _tokenService.GenerateRefreshToken();
            newUser.RefreshExpires = DateTime.Now.AddDays(7);
            newUser.UserName = newUser.Email;

            await _userManager.CreateAsync(newUser, password);

            foundUser = await _userManager.FindByEmailAsync(newUser.Email!);

            Claim[] userClaims = [

                new Claim(ClaimTypes.NameIdentifier, foundUser.Id!),
                new Claim(ClaimTypes.Email, foundUser.Email!),
                new Claim(ClaimTypes.Name, foundUser.Name!),
                new Claim(ClaimTypes.Role, "User")
                ];

            await _userManager.AddClaimsAsync(foundUser, userClaims);

            await _userManager.AddToRoleAsync(foundUser, "User");

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);
            UserTokenDTO userTokenDTO = new();
            var claims = await this.GetUserClaims(newUser.Id);

            userTokenDTO.User = userDTO;
            userTokenDTO.Token = _tokenService.GenerateAccesToken(claims);
            userTokenDTO.Roles = ["User"];

            return userTokenDTO;
        }

        public async Task DeleteUser(string userId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new NotFoundException("User not found");

            await _userManager.DeleteAsync(foundUser);
        }

        public async Task<UserDTO?> GetUserById(string userId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new NotFoundException("User not found");

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task<List<ParticipantDTO>?> GetUsersByEventId(int eventId, CancellationToken cancellationToken)
        {
            if (await _eventRepository.GetEventById(eventId, cancellationToken) == null) throw new NotFoundException("Event not found");

            List<ParticipantDTO> foundUsers =  _userManager.Users
                .Where(u => u.EventParticipations
                .Any(ep => ep.EventId == eventId))
                .Include(u=>u.EventParticipations)
                .AsNoTracking()
                .AsEnumerable()
                .Select(u =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var dto = _mapper.Map<User, ParticipantDTO>(u);
                    dto.RegistrationDateTime = u.EventParticipations.FirstOrDefault(ep => ep.EventId == eventId).RegistrationDate;
                    return dto;
                })
                .ToList();
            
            return foundUsers;
        }

        public async Task<UserDTO> LogIn(string email, string password)
        {
            var res = await _signInManager.PasswordSignInAsync(email, password, true, false);
            if(!res.Succeeded) throw new BadRequestException("Invalid email or password");

            User? foundUser = await _userManager.FindByEmailAsync(email);

            foundUser.RefreshToken = _tokenService.GenerateRefreshToken();
            foundUser.RefreshExpires = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(foundUser);

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task RegisterUserInEvent(string userId, int eventId, CancellationToken cancellationToken)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new NotFoundException("User not found");

            Event? foundEvent = await _eventRepository.GetEventById(eventId, cancellationToken);
            if (foundEvent == null) throw new NotFoundException("Event not found");
            if (foundEvent.IsFull) throw new BadRequestException("Event is full");
            if (foundEvent.Participants.Any(p => p.Id == foundUser.Id)) throw new BadRequestException("User already registered in this event");

            foundEvent.Participants?.Add(foundUser);

            if (foundEvent.Participants.Count == foundEvent.MaxParticipantsCount) foundEvent.IsFull = true;

            await _eventRepository.UpdateEvent(foundEvent);
        }

        public async Task UnregisterUserInEvent(string userId, int eventId, CancellationToken cancellationToken)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new NotFoundException("User not found");

            Event? foundEvent = await _eventRepository.GetEventById(eventId, cancellationToken);
            if (foundEvent == null) throw new NotFoundException("Event not found");

            foundEvent.Participants?.Remove(foundUser);

            if(foundEvent.IsFull) foundEvent.IsFull= false;

            await _eventRepository.UpdateEvent(foundEvent);
        }

        public async Task<UserDTO> UpdateUser(UpdateUserDTO updatedUserDTO)
        {
            User updatedUser = _mapper.Map<User>(updatedUserDTO);

            _userValidator.ValidateAndThrow(updatedUser);
            if (updatedUser.BirthDate >= DateTime.Now) throw new BadRequestException("User cannot be born in future");

            User? foundUser = await _userManager.FindByEmailAsync(updatedUser.Email!);
            if (foundUser == null) throw new NotFoundException("User not found");

            if (updatedUser.Name != null)
                foundUser.Name = updatedUser.Name;

            if (updatedUser.Surname != null)
                foundUser.Surname = updatedUser.Surname;

            if (updatedUser.BirthDate != null) 
                foundUser.BirthDate = updatedUser.BirthDate;

            await _userManager.UpdateAsync(foundUser);

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task<UserDTO> UpdateRefreshToken(string oldToken, string userId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new NotFoundException("User not found");
            if (foundUser.RefreshToken != oldToken) throw new BadRequestException("Token is expired");
            if (foundUser.RefreshExpires < DateTime.Now) throw new BadRequestException("Token is expired");

            foundUser.RefreshToken = _tokenService.GenerateRefreshToken();
            foundUser.RefreshExpires = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(foundUser);

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task<IList<Claim>> GetUserClaims(string userId)
        {
            User? user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not found");

            return await _userManager.GetClaimsAsync(user);
        } 
    }
}
