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
        [HttpGet]
        [Authorize]
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
                return Json(
                    new {
                        Username = user.Username,
                        Email = user.Email,
                        Token = token
                        });
            }
            return NotFound("User could not be found!");
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] User? user)
        {
            var userStatus = await _userService.Register(user);
            IActionResult result = userStatus switch 
            {
                UserStatus.Found => Conflict("User already have been registed."),
                UserStatus.OK =>  Ok("User was registered"),
                _ => NotFound("")
            };
            return result;
        }
        [Authorize]
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userStatus = await _userService.Delete(id);
            IActionResult result = userStatus switch{
                UserStatus.NotFound => NotFound("User could not be found."),
                UserStatus.OK => Ok("User have been deleted."),
                _ => BadRequest(userStatus)
            };
            return result;
        }
        [Authorize]
        [HttpPut]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit([FromBody] User? user,int id)
        {
            var userStatus = await _userService.Edit(user,id);
            IActionResult result = userStatus switch{
                UserStatus.NotFound => NotFound("User could not be found."),
                UserStatus.Found => Conflict("Username or Email has taken."),
                UserStatus.OK => Ok("User have been edited"),
                _ => BadRequest(userStatus)
            };
            return result;
        }
    }
}