// using Microsoft.AspNetCore.Mvc;
// using dotnetapp.Models;
// using dotnetapp.Services;
// using Microsoft.Extensions.Configuration;

// namespace dotnetapp.Controllers
// {
//     [ApiController]
//     [Route("api/")]
//     public class UserController : ControllerBase
//     {
//         private readonly UserService _userService;

//         public UserController(UserService userService)
//         {
//             _userService = userService;
//         }

//         [HttpPost("register")]
//         public IActionResult register([FromBody] User user)
//         {
//             var registeredUser = _userService.register(user);

//             if (registeredUser != null)
//             {
//                 var token = _userService.GenerateJwtToken(registeredUser);
//                 return CreatedAtAction(nameof(login), new { email = registeredUser.Email, token });
//             }

//             return Conflict(new { Message = "Email already registered." });
//         }

//         [HttpPost("login")]
//         public IActionResult login([FromBody] User user)
//         {
//             var authenticatedUser = _userService.login(user);

//             if (authenticatedUser != null)
//             {
//                 var token = _userService.GenerateJwtToken(authenticatedUser);
//                 return Ok(new { Message = "User login successful.", User = authenticatedUser, Token = token });
//             }
//             else
//             {
//                 return NotFound(new { Message = "Invalid email or password." });
//             }
//         }
//     }
// }


using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using dotnetapp.Data;

namespace dotnetapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IUserService _userService;

        public AuthController(IUserService userService, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _userService = userService;
            _context = context;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
                return BadRequest("Invalid user data");

            if (user.Role == "admin" || user.Role == "applicant")
            {
                Console.WriteLine("asd  "+user.Role);

                var isRegistered = await _userService.RegisterAsync(user);
                Console.WriteLine("status"+isRegistered);

                if (isRegistered)
                {
                    var customUser = new User
                    {
                        Username = user.Username,
                        Password = user.Password,
                        Email = user.Email,
                        MobileNumber = user.MobileNumber,
                        Role = user.Role,
                    };

                    // Add the customUser to the DbSet and save it
                    _context.Users.Add(customUser);
                    await _context.SaveChangesAsync();

                    return Ok(user);
                }
            }

            return BadRequest("Registration failed. Username may already exist.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Invalid login request");

            var token = await _userService.LoginAsync(request.Username, request.Password);

            if (token == null)
                return Unauthorized("Invalid username or password");

            // Retrieve the user from UserManager to get their roles
            var user = await _userManager.FindByNameAsync(request.Username);
            Console.WriteLine("role"+user);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new { Token = token, Roles = roles });
        }


        [Authorize(Roles = "admin")]
        [HttpGet("admin")]
        public IActionResult AdminProtected()
        {
            return Ok("This is an admin-protected endpoint.");
        }

        [Authorize(Roles = "applicant")]
        [HttpGet("applicant")]
        public IActionResult applicantProtected()
        {
            return Ok("This is an applicant-protected endpoint.");
        }
    }
}

