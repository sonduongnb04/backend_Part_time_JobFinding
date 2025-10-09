using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PTJ.Api.Models;
using PTJ.Api.Services;

namespace PTJ.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IJwtTokenService _jwt;

        public AuthController(UserManager<ApplicationUser> um, RoleManager<IdentityRole<Guid>> rm, IJwtTokenService jwt)
        {
            _userManager = um;
            _roleManager = rm;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var user = new ApplicationUser { UserName = req.Email, Email = req.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));

            await _userManager.AddToRoleAsync(user, "User");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwt.Generate(user, roles);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null) return Unauthorized("Invalid email or password.");

            if (!await _userManager.CheckPasswordAsync(user, req.Password))
                return Unauthorized("Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwt.Generate(user, roles);
            return Ok(new { token });
        }
    }

    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
}
