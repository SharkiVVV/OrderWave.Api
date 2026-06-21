using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using OrderWaveAPI.Data;
using OrderWaveAPI.Models;
using OrderWaveAPI.Services;
using OrderWaveAPI.Transfer.Requests;
using OrderWaveAPI.Transfer.Responses;

namespace OrderWaveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        
        private readonly AppDbContext _dbContext;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;

        public AuthorizationController(
            AppDbContext dbContext,
            PasswordService passwordService,
            TokenService tokenService
        )
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }
        
        [HttpPost("init")]
        [AllowAnonymous]
        public async Task<IActionResult> Initialize([FromBody] RegisterRequest request)
        {
            var anyUser = await _dbContext.Users.AnyAsync();
            if (anyUser)
            {
                return Conflict(new { message = "Initialize already exists" });
            }
            
            var role= await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName=="Admin");

            if (role is null)
            {
                return BadRequest(new { message = "Role not found" });
            }

            var user = new User
            {
                UserLogin = request.Login,
                UserPassword = _passwordService.Hash(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _dbContext.UserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId,
                    CreatedAt = DateTime.UtcNow
                }
            );
            await _dbContext.SaveChangesAsync();
            
            return Ok(new {message = "First user successfully initialized", userId = user.UserId});

        }


        [HttpPost("login")]
        [AllowAnonymous] 
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserLogin == request.Login);
            if (user is null)
                return Unauthorized(new { message = "Username or password is incorrect" });

            if (!user.IsActive)
                return Unauthorized(new { message = "Account is not active" });

            var verifyResult = _passwordService.Verify(request.Password, user.UserPassword);
            if (!verifyResult)
                return Unauthorized(new { message = "Incorrect password or login" });
            
            var role = user.UserRoles.FirstOrDefault()?.Role.RoleName ?? "Waiter";

            if (role == "Admin")
            {
                var waiterExists = await _dbContext.Waiters.AnyAsync(w => w.UserId == user.UserId);
                if (!waiterExists)
                {
                    var waiter = new Waiter
                    {
                        UserId = user.UserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _dbContext.Waiters.Add(waiter);
                    await _dbContext.SaveChangesAsync();
                }
            }
            
            
            
            var token = _tokenService.GenerateToken(user.UserId, user.UserLogin, role);
            
            return Ok(new AuthorizationResponse
            {
                UserId= user.UserId,
                Login= user.UserLogin,
                Role=role,
                Token=token
            }
            );

        }

        // POST api/<AuthorizationController>
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var exists = await _dbContext.Users
                .AnyAsync(u=> u.UserLogin == request.Login);

            if (exists)
            {
                return Conflict(new { message = "User with this login already  exists" });
            }
            
            var role = await _dbContext.Roles
                .FirstOrDefaultAsync(r=>r.RoleName==request.Role);
            if (role is null)
            {
                return BadRequest(new { message = "Role not found" });
                
                
            }

            var user = new User
            {
                UserLogin = request.Login,
                UserPassword = _passwordService.Hash(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId,
                CreatedAt = DateTime.UtcNow

            };
            _dbContext.UserRoles.Add(userRole);
            await _dbContext.SaveChangesAsync();
            return Ok(new {message  = "User successfully registered", userId = user.UserId});

        }
    }
}
