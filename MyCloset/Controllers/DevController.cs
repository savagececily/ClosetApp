using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCloset.Models;
using MyCloset.Models.DBModels;
using MyCloset.Services.Interfaces;

namespace MyCloset.Controllers
{
    /// <summary>
    /// Development-only endpoints for testing without OAuth authentication
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DevController : ControllerBase
    {
        private readonly ILogger<DevController> _logger;
        private readonly IUserService _userService;
        private readonly MyClosetAppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public DevController(
            ILogger<DevController> logger, 
            IUserService userService,
            MyClosetAppDbContext dbContext,
            IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Check if dev endpoints are enabled
        /// </summary>
        private bool IsDevMode()
        {
            var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
            return environment == "Development" || environment == "Staging";
        }

        /// <summary>
        /// Seed test users for development/testing
        /// </summary>
        [HttpPost("seed-users")]
        public async Task<IActionResult> SeedUsers()
        {
            if (!IsDevMode())
            {
                return NotFound("Dev endpoints only available in Development/Staging");
            }

            try
            {
                var testUsers = new List<User>
                {
                    new User
                    {
                        id = Guid.NewGuid().ToString().ToLower(),
                        UserId = Guid.Parse(Guid.NewGuid().ToString()),
                        DisplayName = "Alice Test",
                        Email = "alice@test.com",
                        AccountProvider = "dev-test",
                        IsPublic = true,
                        DateAdded = DateTime.UtcNow,
                        LastModified = DateTime.UtcNow,
                        LastLogin = DateTime.UtcNow
                    },
                    new User
                    {
                        id = Guid.NewGuid().ToString().ToLower(),
                        UserId = Guid.Parse(Guid.NewGuid().ToString()),
                        DisplayName = "Bob Demo",
                        Email = "bob@test.com",
                        AccountProvider = "dev-test",
                        IsPublic = true,
                        DateAdded = DateTime.UtcNow,
                        LastModified = DateTime.UtcNow,
                        LastLogin = DateTime.UtcNow
                    }
                };

                foreach (var user in testUsers)
                {
                    _dbContext.Users.Add(user);
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Seeded {Count} test users", testUsers.Count);

                return Ok(new
                {
                    message = "Test users created successfully",
                    users = testUsers.Select(u => new
                    {
                        userId = u.UserId,
                        displayName = u.DisplayName,
                        email = u.Email
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding test users");
                return StatusCode(500, new { error = "Failed to seed users", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all test users for development
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!IsDevMode())
            {
                return NotFound("Dev endpoints only available in Development/Staging");
            }

            try
            {
                var users = await _dbContext.Users.ToListAsync();
                
                return Ok(new
                {
                    count = users.Count,
                    users = users.Select(u => new
                    {
                        userId = u.UserId,
                        displayName = u.DisplayName,
                        email = u.Email,
                        accountProvider = u.AccountProvider,
                        lastLogin = u.LastLogin
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return StatusCode(500, new { error = "Failed to fetch users", details = ex.Message });
            }
        }

        /// <summary>
        /// Create a single test user
        /// </summary>
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateTestUserRequest request)
        {
            if (!IsDevMode())
            {
                return NotFound("Dev endpoints only available in Development/Staging");
            }

            try
            {
                var result = await _userService.CreateUser(
                    request.Email,
                    "dev-test",
                    request.DisplayName
                );

                if (result.StatusCode == System.Net.HttpStatusCode.OK || 
                    result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return Ok(new
                    {
                        message = "User created successfully",
                        user = result.Data
                    });
                }
                else
                {
                    return BadRequest(new { error = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test user");
                return StatusCode(500, new { error = "Failed to create user", details = ex.Message });
            }
        }

        /// <summary>
        /// Get user by email for testing
        /// </summary>
        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (!IsDevMode())
            {
                return NotFound("Dev endpoints only available in Development/Staging");
            }

            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                return Ok(new
                {
                    userId = user.UserId,
                    displayName = user.DisplayName,
                    email = user.Email,
                    accountProvider = user.AccountProvider,
                    isPublic = user.IsPublic,
                    lastLogin = user.LastLogin,
                    dateAdded = user.DateAdded
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email");
                return StatusCode(500, new { error = "Failed to fetch user", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete all test users (cleanup)
        /// </summary>
        [HttpDelete("cleanup")]
        public async Task<IActionResult> CleanupTestUsers()
        {
            if (!IsDevMode())
            {
                return NotFound("Dev endpoints only available in Development/Staging");
            }

            try
            {
                var testUsers = await _dbContext.Users.Where(u => u.AccountProvider == "dev-test").ToListAsync();
                
                foreach (var user in testUsers)
                {
                    _dbContext.Users.Remove(user);
                }

                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Test users deleted successfully",
                    count = testUsers.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up test users");
                return StatusCode(500, new { error = "Failed to cleanup users", details = ex.Message });
            }
        }

        /// <summary>
        /// Get test authentication help and available test users
        /// </summary>
        [HttpGet("auth-help")]
        public async Task<IActionResult> GetAuthHelp()
        {
            if (!IsDevMode())
            {
                return NotFound("Dev endpoints only available in Development/Staging");
            }

            try
            {
                var testUsers = await _dbContext.Users.Where(u => u.AccountProvider == "dev-test").ToListAsync();

                return Ok(new
                {
                    message = "Test Authentication Help",
                    instructions = "To authenticate as a test user, add the following header to your API requests:",
                    headerName = "X-Test-User-Id",
                    headerDescription = "Set this header to the userId of the test user you want to authenticate as",
                    example = "X-Test-User-Id: c791f12f-e393-4f30-a3f6-f73b8cc2ae3e",
                    curlExample = $"curl -H \"X-Test-User-Id: {(testUsers.FirstOrDefault()?.UserId.ToString() ?? "USER_ID_HERE")}\" https://app-euz4k3e3dqvkq.azurewebsites.net/api/User/AccountDetails",
                    availableTestUsers = testUsers.Select(u => new
                    {
                        userId = u.UserId,
                        displayName = u.DisplayName,
                        email = u.Email
                    }).ToList(),
                    note = "This authentication bypass only works in Development and Staging environments"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching auth help");
                return StatusCode(500, new { error = "Failed to fetch auth help", details = ex.Message });
            }
        }
    }

    public class CreateTestUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
