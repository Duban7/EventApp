using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs;
using Services.Interfaces;
using System.Security.Claims;

namespace EventApp.Controllers
{
    [ApiController]
    [Route("EventApi")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService,
                              ITokenService tokenService,
                              ILogger<UserController> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost]
        [Route("/sign-up")]
        public async Task<ActionResult<UserTokenDTO>> CreateUser([FromForm] LogInUserDTO userDTO)
        {
            UserDTO? newUser = await _userService.CreateUser(userDTO);

            UserTokenDTO res = new();
            res.User = newUser;
            res.User.RefreshToken = _tokenService.GenerateRefreshToken();
            res.Token = _tokenService.GenerateAccesToken(GetUserClaims(res.User));

            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [Route("/update-user")]
        public async Task<ActionResult<UserDTO>> UpdateUser([FromBody] UserDTO userDTO)
        {
            return Ok(await _userService.UpdateUser(userDTO));
        }

        [HttpDelete]
        [Authorize]
        [Route("/delete-user")]
        public async Task<ActionResult> DeleteUser()
        {
            await  _userService.DeleteUser(GetUserId());

            return Ok();
        }

        [HttpPost]
        [Route("/log-in")]
        public async Task<ActionResult<UserTokenDTO>> LogIn([FromForm] LogInUserDTO userDTO)
        {
            UserDTO? foundUser = await _userService.LogIn(userDTO.Email, userDTO.Password);
            UserTokenDTO res = new();

            res.User = foundUser;
            res.User.RefreshToken = _tokenService.GenerateRefreshToken();
            res.Token = _tokenService.GenerateAccesToken(GetUserClaims(res.User));

            return Ok(res);
        }

        [HttpGet]
        [Route("/get-event-participants/{eventId}")]
        public async Task<ActionResult<List<UserDTO>>> GetEventParticipants(string eventId)
        {
            List<UserDTO>? foundUsers = await _userService.GetUsersByEventId(eventId);

            return Ok(foundUsers);
        }

        [HttpGet]
        [Authorize]
        [Route("/get-user/{userId}")]
        public async Task<ActionResult<UserDTO>> GetUser(string userId)
        {
            UserDTO? foudnUser = await _userService.GetUserById(userId);

            return Ok(foudnUser);
        }

        [HttpPost]
        [Route("/reg-user-event/{eventId}")]
        public async Task<ActionResult> RegUserEvent(string eventId)
        {
            await _userService.RegisterUserInEvent(GetUserId(), eventId);

            return Ok();
        }

        [HttpPost]
        [Route("/unreg-user-event/{eventId}")]
        public async Task<ActionResult> UnregUserEvent(string eventId)
        {
            await _userService.UnregisterUserInEvent(GetUserId(), eventId);

            return Ok();
        }

        [HttpPost]
        [Route("/refresh-token")]

        private List<Claim> GetUserClaims(UserDTO user) => new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Name!)
            };

        private string GetUserId() =>
             this.User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value ?? throw new Exception("User not found");
    }
}
