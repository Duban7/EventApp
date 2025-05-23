using Data.Interfaces;
using Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IValidator<User> _userValidator;
        private readonly IEventRepository _eventRepository;
        public UserService(UserManager<User> userManager,
                           SignInManager<User> signInManager,
                           IValidator<User> userValidator,
                           IEventRepository eventRepository) 
        {
            _userManager = userManager;
            _signInManager = signInManager;            _userValidator = userValidator;
            _eventRepository = eventRepository;
        }

        public async Task<User?> CreateUser(User newUser)
        {
            _userValidator.ValidateAndThrow(newUser);

            User? foundUser = await _userManager.FindByEmailAsync(newUser.Email!);

            if (foundUser != null) throw new Exception("User already exists");

            var res = await _userManager.CreateAsync(newUser);

            if (!res.Succeeded) throw new Exception("Couldn't create User");

            return await _userManager.FindByEmailAsync(newUser.Email!);
        }

        public async Task DeleteUser(string userId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);

            if (foundUser == null) throw new Exception("User not found");

            var res = await _userManager.DeleteAsync(foundUser);

            if (!res.Succeeded) throw new Exception("Couldn't delete User");
        }

        public async Task<User?> GetUserById(string userId)
        {
            User? foundUser = await _userManager.FindByIdAsync(userId);

            if (foundUser == null) throw new Exception("User not found");

            return foundUser;
        }

        public async Task<List<User>?> GetUsersByEventId(string eventId)
        {
            List<User> foundUsers = await _userManager.Users.Where(u=>u.Events.Any(e=>e.Id == eventId)).ToListAsync<User>();

            if (foundUsers.Count <= 0 ) throw new Exception("User not found");

            return foundUsers;
        }

        public async Task<User?> LogIn(string email, string password)
        {
            var res = await _signInManager.PasswordSignInAsync(email, password, true, false);

            if(!res.Succeeded) throw new Exception("Invalid email or password");

            User? foundUser = await _userManager.FindByEmailAsync(email);

            if (foundUser == null) throw new Exception("User not found");

            return foundUser;
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

        public async Task<User?> UpdateUser(User updatedUser)
        {
            _userValidator.ValidateAndThrow(updatedUser);

            User? foundUser = await _userManager.FindByIdAsync(updatedUser.Id);

            if (foundUser == null) throw new Exception("User not found");

            if (updatedUser.Name != null)
                foundUser.Name = updatedUser.Name;

            if (updatedUser.Surname != null)
                foundUser.Surname = updatedUser.Surname;

            if (updatedUser.BirthDate != null) 
                foundUser.BirthDate = updatedUser.BirthDate;

            var res = await _userManager.UpdateAsync(foundUser);

            if (!res.Succeeded) throw new Exception("Couldn't update User");

            return foundUser;
        }
    }
}
