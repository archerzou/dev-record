using System.Linq.Expressions;
using DevRecord.Api.Entities;

namespace DevRecord.Api.DTOs.Users;

internal static class UserQueries
{
    public static Expression<Func<User, UserDto>> ProjectToDto()
    {
        return u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Name = u.Name,
            CreatedAtUtc = u.CreatedAtUtc,
            UpdatedAtUtc = u.UpdatedAtUtc
        };
    }
}
