
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevRecord.Api.DTOs.Auth;
using DevRecord.Api.Entities;
using DevRecord.Api.Services;
using DevRecord.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevRecord.UnitTests.Services;
public sealed class TokenProviderTests
{
    private readonly TokenProvider _tokenProvider;
    private readonly JwtAuthOptions _jwtAuthOptions;

    public TokenProviderTests()
    {
        _jwtAuthOptions = new JwtAuthOptions
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            Key = "your-secret-key-here-that-should-also-be-fairly-long",
            ExpirationInMinutes = 30,
            RefreshTokenExpirationDays = 7
        };

        IOptions<JwtAuthOptions> options = Options.Create(_jwtAuthOptions);
        _tokenProvider = new TokenProvider(options);
    }

    [Fact]
    public void Create_ShouldReturnBothTokens()
    {
        // Arrange
        var tokenRequest = new TokenRequest("user123", "test@example.com", [Roles.Member]);

        // Act
        AccessTokensDto result = _tokenProvider.Create(tokenRequest);

        // Assert
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
    }

    [Fact]
    public void Create_ShouldGenerateValidAccessToken()
    {
        // Arrange
        var tokenRequest = new TokenRequest("user123", "test@example.com", [Roles.Member]);

        // Act
        AccessTokensDto result = _tokenProvider.Create(tokenRequest);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtAuthOptions.Key)),
            ValidateIssuer = true,
            ValidIssuer = _jwtAuthOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtAuthOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        ClaimsPrincipal? principal = handler.ValidateToken(
            result.AccessToken,
            validationParameters,
            out SecurityToken? validatedToken);

        Assert.NotNull(validatedToken);
        Assert.Equal(tokenRequest.UserId, principal.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Equal(tokenRequest.Email, principal.FindFirstValue(ClaimTypes.Email));
        Assert.Contains(principal.FindAll(ClaimTypes.Role), claim => claim.Value == "Member");
    }

    [Fact]
    public void Create_ShouldGenerateUniqueRefreshTokens()
    {
        // Arrange
        var tokenRequest = new TokenRequest("user123", "test@example.com", [Roles.Member]);

        // Act
        AccessTokensDto result1 = _tokenProvider.Create(tokenRequest);
        AccessTokensDto result2 = _tokenProvider.Create(tokenRequest);

        // Assert
        Assert.NotEqual(result1.RefreshToken, result2.RefreshToken);
    }

    [Fact]
    public void Create_ShouldGenerateAccessTokenWithCorrectExpiration()
    {
        // Arrange
        var tokenRequest = new TokenRequest("user123", "test@example.com", [Roles.Member]);

        // Act
        AccessTokensDto result = _tokenProvider.Create(tokenRequest);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken? jwt = handler.ReadJwtToken(result.AccessToken);

        DateTime expectedExpiration = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes);
        DateTime actualExpiration = jwt.ValidTo;

        // Allow for a small time difference due to test execution
        Assert.True(Math.Abs((expectedExpiration - actualExpiration).TotalSeconds) < 5);
    }

    [Fact]
    public void Create_ShouldGenerateBase64RefreshToken()
    {
        // Arrange
        var tokenRequest = new TokenRequest("user123", "test@example.com", [Roles.Member]);

        // Act
        AccessTokensDto result = _tokenProvider.Create(tokenRequest);

        // Assert
        Assert.True(IsBase64String(result.RefreshToken));
    }

    [Fact]
    public void Create_ShouldIncludeAllRolesInToken()
    {
        var tokenRequest = new TokenRequest("user123", "test@example.com",
            [Roles.Member, Roles.Admin]);

        AccessTokensDto result = _tokenProvider.Create(tokenRequest);

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt = handler.ReadJwtToken(result.AccessToken);

        IEnumerable<Claim> roleClaims = jwt.Claims.Where(c => c.Type == ClaimTypes.Role);
        Assert.Contains(roleClaims, c => c.Value == Roles.Member);
        Assert.Contains(roleClaims, c => c.Value == Roles.Admin);
    }

    [Fact]
    public void Create_ShouldGenerateValidToken_WhenRolesIsEmpty()
    {
        var tokenRequest = new TokenRequest("user123", "test@example.com", []);
        AccessTokensDto result = _tokenProvider.Create(tokenRequest);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
    }

    private static bool IsBase64String(string base64)
    {
        Span<byte> buffer = new byte[base64.Length];
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
