using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Enums;
using REST.Interfaces;
using REST.Models;
using User = REST.Models.User;
namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        public UserController(IUserService userService,ITokenService tokenService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }
        [Authorize]
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult?> Login([FromBody] User? credentials){
            var user = await _userService.Login(credentials);
            if(user != null) 
            {
                var token = _tokenService.GenerateJWT(user);
                return Ok(token);
            }
            return NotFound("User could not be found!");
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] User? user)
        {
           var userStatus = await _userService.Register(user);
           switch (userStatus)
           {
                case UserStatus.Found:
                    return Conflict("User already have been registed.");
                case UserStatus.OK:
                    return Ok("User was registered");
                default:
                    return BadRequest(userStatus);
           }
        }
        [Authorize]
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userStatus = await _userService.Delete(id);
            switch(userStatus){
                case UserStatus.NotFound:
                    return NotFound("User could not be found.");
                case UserStatus.OK:
                    return Ok("User have been deleted.");
                default:
                    return BadRequest(userStatus);
            }
        }
        [Authorize]
        [HttpPut]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit([FromBody] User? user,int id)
        {
            var userStatus = await _userService.Edit(user,id);
            switch(userStatus){
                case UserStatus.OK:
                    return Ok("User have been edited");
                case UserStatus.Found:
                    return Conflict("Username or Email has taken.");
                case UserStatus.NotFound:
                    return NotFound("User could not be found.");
                default:
                    return BadRequest(userStatus);
            }
        }
    }
}