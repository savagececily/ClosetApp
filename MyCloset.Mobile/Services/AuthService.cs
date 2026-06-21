using System;
using System.Threading.Tasks;
using MyCloset.Mobile.Models;

namespace MyCloset.Mobile.Services;

public interface IAuthService
{
    bool IsAuthenticated { get; }
    User? CurrentUser { get; }
    string? AuthToken { get; }
    
    Task<(bool Success, string Message)> LoginAsync(string email, string password, string provider = "Google");
    Task LogoutAsync();
    Task<User?> GetCurrentUserAsync();
}

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    
    public bool IsAuthenticated => !string.IsNullOrEmpty(AuthToken);
    public User? CurrentUser { get; private set; }
    public string? AuthToken { get; private set; }

    public AuthService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<(bool Success, string Message)> LoginAsync(string email, string password, string provider = "Google")
    {
        try
        {
            // In a real implementation, you would call your authentication endpoint
            // For now, this is a placeholder that sets mock authentication
            
            // TODO: Implement actual OAuth flow with your backend
            // Example: POST /api/Auth/Login with Google/Facebook/Microsoft token
            
            AuthToken = "mock-token-" + Guid.NewGuid().ToString();
            _apiService.AuthToken = AuthToken;
            
            CurrentUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = email,
                FirstName = "Test",
                LastName = "User"
            };

            // Store token in secure storage
            await SecureStorage.SetAsync("auth_token", AuthToken);
            await SecureStorage.SetAsync("user_id", CurrentUser.UserId.ToString());
            await SecureStorage.SetAsync("user_email", CurrentUser.Email);

            return (true, "Login successful");
        }
        catch (Exception ex)
        {
            return (false, $"Login failed: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        AuthToken = null;
        CurrentUser = null;
        _apiService.AuthToken = string.Empty;

        // Clear secure storage
        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("user_id");
        SecureStorage.Remove("user_email");

        await Task.CompletedTask;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (CurrentUser != null)
            return CurrentUser;

        // Try to restore from secure storage
        var token = await SecureStorage.GetAsync("auth_token");
        var userIdStr = await SecureStorage.GetAsync("user_id");
        var email = await SecureStorage.GetAsync("user_email");

        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userIdStr))
        {
            AuthToken = token;
            _apiService.AuthToken = token;
            
            CurrentUser = new User
            {
                UserId = Guid.Parse(userIdStr),
                Email = email ?? "",
                FirstName = "User", // TODO: fetch from API
                LastName = ""
            };

            return CurrentUser;
        }

        return null;
    }
}
