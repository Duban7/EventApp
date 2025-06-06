﻿using AutoMapper;
using Data.Interfaces;
using Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.DTOs;
using Services.Exeptions;
using Services.Interfaces;
using Services.Validators;
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
            if (!UserValidator.IsPasswordValid(password)) throw new BadRequestException("Password should contain at least 1 letter and 1 number. Minimal password length - 6, max - 100."); 

            User? foundUser = await _userManager.FindByEmailAsync(newUser.Email!);
            if (foundUser != null) throw new ConflictException("User already exists");

            newUser.RefreshToken = _tokenService.GenerateRefreshToken();
            newUser.RefreshExpires = DateTime.Now.AddDays(7);
            newUser.UserName = newUser.Email;

            var res = await _userManager.CreateAsync(newUser, password);
            if (!res.Succeeded) throw new InternalErrorException("Couldn't create User" + res.Errors.ToString());

            foundUser = await _userManager.FindByEmailAsync(newUser.Email!) ?? throw new NotFoundException("User not found");

            Claim[] userClaims = [

                new Claim(ClaimTypes.NameIdentifier, foundUser.Id!),
                new Claim(ClaimTypes.Email, foundUser.Email!),
                new Claim(ClaimTypes.Name, foundUser.Name!),
                new Claim(ClaimTypes.Role, "User")
                ];

            res = await _userManager.AddClaimsAsync(foundUser, userClaims);
            if (!res.Succeeded) throw new InternalErrorException("Couldn't add Claims");

            res = await _userManager.AddToRoleAsync(foundUser, "User");
            if (!res.Succeeded) throw new InternalErrorException("Couldn't add Role");

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

            var res = await _userManager.DeleteAsync(foundUser);
            if (!res.Succeeded) throw new InternalErrorException("Couldn't delete User");
        }

        public async Task<UserDTO?> GetUserById(string userId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new NotFoundException("User not found");

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task<List<PartizipantDTO>?> GetUsersByEventId(int eventId)
        {
            if (await _eventRepository.GetEventById(eventId) == null) throw new NotFoundException("Event not found");

            List<PartizipantDTO> foundUsers = await _userManager.Users
                .Where(u => u.EventParticipations.Any(ep => ep.EventId == eventId))
                .Select(u=> new PartizipantDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    Surname = u.Surname,
                    RegistrationDateTime = u.EventParticipations.First(ep=>ep.EventId == eventId).RegistrationDate
                })
                .ToListAsync();

            if (foundUsers.Count <= 0 ) return [];
            
            return foundUsers;
        }

        public async Task<UserDTO> LogIn(string email, string password)
        {
            var res = await _signInManager.PasswordSignInAsync(email, password, true, false);
            if(!res.Succeeded) throw new BadRequestException("Invalid email or password");

            User? foundUser = await _userManager.FindByEmailAsync(email);
            if (foundUser == null) throw new NotFoundException("User not found");

            foundUser.RefreshToken = _tokenService.GenerateRefreshToken();
            foundUser.RefreshExpires = DateTime.Now.AddDays(7);

            var updateRes = await _userManager.UpdateAsync(foundUser);
            if (!updateRes.Succeeded) throw new InternalErrorException("couldn't update refresh token");

            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);

            return userDTO;
        }

        public async Task RegisterUserInEvent(string userId, int eventId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new NotFoundException("User not found");

            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new NotFoundException("Event not found");
            if (foundEvent.IsFull) throw new BadRequestException("Event is full");
            if (foundEvent.Participants.Any(p => p.Id == foundUser.Id)) throw new BadRequestException("User already registered in this event");

            foundEvent.Participants?.Add(foundUser);

            if (foundEvent.Participants.Count == foundEvent.MaxParticipantsCount) foundEvent.IsFull = true;
            if (foundEvent.Participants.Count > foundEvent.MaxParticipantsCount) throw new InternalErrorException("Somehow you added more participants than you could");

            await _eventRepository.UpdateEvent(foundEvent);
        }

        public async Task UnregisterUserInEvent(string userId, int eventId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser == null) throw new NotFoundException("User not found");

            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new NotFoundException("Event not found");

            foundEvent.Participants?.Remove(foundUser);

            if(foundEvent.IsFull) foundEvent.IsFull= false;

            await _eventRepository.UpdateEvent(foundEvent);
        }

        public async Task<UserDTO> UpdateUser(UpdateUserDTO updatedUserDTO)
        {
            User updatedUser = _mapper.Map<User>(updatedUserDTO);

            _userValidator.ValidateAndThrow(updatedUser);

            User? foundUser = await _userManager.FindByEmailAsync(updatedUser.Email!);
            if (foundUser == null) throw new NotFoundException("User not found");

            if (updatedUser.Name != null)
                foundUser.Name = updatedUser.Name;

            if (updatedUser.Surname != null)
                foundUser.Surname = updatedUser.Surname;

            if (updatedUser.BirthDate != null) 
                foundUser.BirthDate = updatedUser.BirthDate;

            var res = await _userManager.UpdateAsync(foundUser);
            if (!res.Succeeded) throw new InternalErrorException("Couldn't update User");

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
