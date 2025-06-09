using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs;
using Services.Interfaces;
using System.Security.Claims;

namespace EventApp.Controllers
{
    [ApiController]
    [Route("EventApi/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public UserController(IUserService userService,
                              ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<ActionResult<UserTokenDTO>> CreateUser([FromForm] SignUpUserDTO userDTO)
        {
            UserTokenDTO newUser = await _userService.CreateUser(userDTO);

            return Ok(newUser);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserPolicy")]
        [Route("update")]
        public async Task<ActionResult<UserDTO>> UpdateUser([FromForm] UpdateUserDTO userDTO)
        {
            UserDTO updatedUser = await _userService.UpdateUser(userDTO);

            return Ok(updatedUser);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserPolicy")]
        [Route("delete")]
        public async Task<ActionResult> DeleteUser()
        {
            await _userService.DeleteUser(GetUserId());

            return Ok();
        }

        [HttpPost]
        [Route("log-in")]
        public async Task<ActionResult<UserTokenDTO>> LogIn([FromForm] LogInUserDTO userDTO)
        {
            UserDTO foundUser = await _userService.LogIn(userDTO.Email, userDTO.Password);
            UserTokenDTO res = new();
            var claims = await _userService.GetUserClaims(foundUser.Id);

            res.User = foundUser;
            res.Token = _tokenService.GenerateAccesToken(claims);
            res.Roles = claims.Where(i => i.Type == ClaimTypes.Role)
                              .Select(c=>c.Value)
                              .ToArray();

            return Ok(res);
        }

        [HttpGet]
        [Route("get-event-participants/{eventId}")]
        public async Task<ActionResult<List<ParticipantDTO>>> GetEventParticipants(int eventId, CancellationToken cancellationToken)
        {
            List<ParticipantDTO>? foundUsers = await _userService.GetUsersByEventId(eventId, cancellationToken);

            return Ok(foundUsers);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserPolicy")]
        [Route("get/{userId}")]
        public async Task<ActionResult<UserDTO>> GetUser(string userId)
        {
            UserDTO? foundUser = await _userService.GetUserById(userId);

            return Ok(foundUser);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserPolicy")]
        [Route("reg-event/{eventId}")]
        public async Task<ActionResult> RegUserEvent(int eventId, CancellationToken cancellationToken)
        {
            await _userService.RegisterUserInEvent(GetUserId(), eventId, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserPolicy")]
        [Route("unreg-event/{eventId}")]
        public async Task<ActionResult> UnregUserEvent(int eventId, CancellationToken cancellationToken)
        {
            await _userService.UnregisterUserInEvent(GetUserId(), eventId, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("refresh/{userId}")]
        public async Task<ActionResult<UserTokenDTO>> Refresh([FromBody] string oldRFToken, string userId)
        {
            UserDTO foundUser = await _userService.UpdateRefreshToken(oldRFToken, userId);
            UserTokenDTO res = new();
            var claims = await _userService.GetUserClaims(foundUser.Id);

            res.User = foundUser;
            res.Token = _tokenService.GenerateAccesToken(claims);

            return Ok(res);
        }

        private string GetUserId() =>
             this.User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value ?? throw new Exception("User not found");
    }
}
