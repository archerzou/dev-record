using System.Net.Mime;
using DevRecord.Api.DTOs.Common;
using DevRecord.Api.DTOs.GitHub;
using DevRecord.Api.Entities;
using DevRecord.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevRecord.Api.Controllers;

[Authorize()]
[ApiController]
[Route("github")]

public sealed class GitHubController(
    GitHubAccessTokenService gitHubAccessTokenService,
    GitHubService gitHubService,
    UserContext userContext,
    LinkService linkService) : ControllerBase
{
    [HttpPut("personal-access-token")]
    public async Task<IActionResult> StoreAccessToken(StoreGitHubAccessTokenDto storeGitHubAccessTokenDto)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await gitHubAccessTokenService.StoreAsync(userId, storeGitHubAccessTokenDto);

        return NoContent();
    }

    [HttpDelete("personal-access-token")]
    public async Task<IActionResult> RevokeAccessToken()
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await gitHubAccessTokenService.RevokeAsync(userId);

        return NoContent();
    }

    [HttpGet("profile")]
    public async Task<ActionResult<GitHubUserProfileDto>> GetUserProfile([FromHeader] AcceptHeaderDto acceptHeader)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        string? accessToken = await gitHubAccessTokenService.GetAsync(userId);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return NotFound();
        }

        GitHubUserProfileDto? userProfile = await gitHubService.GetUserProfileAsync(accessToken);
        if (userProfile is null)
        {
            return NotFound();
        }

        if (acceptHeader.IncludeLinks)
        {
            userProfile.Links =
            [
                linkService.Create(nameof(GetUserProfile), "self", HttpMethods.Get),
                linkService.Create(nameof(StoreAccessToken), "store-token", HttpMethods.Put),
                linkService.Create(nameof(RevokeAccessToken), "revoke-token", HttpMethods.Delete)
            ];
        }

        return Ok(userProfile);
    }
}
