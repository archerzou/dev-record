using DevRecord.Api.Database;
using DevRecord.Api.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevRecord.Api.Controllers;

[ApiController]
[Route("users")]
internal sealed class UsersController(ApplicationDbContext dbContext): ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == id)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
