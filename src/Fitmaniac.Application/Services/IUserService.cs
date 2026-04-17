using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Users;
using Microsoft.AspNetCore.Http;

namespace Fitmaniac.Application.Services;

public interface IUserService
{
    Task<ServiceResult<UserDto>> GetCurrentUserAsync(int userId, CancellationToken ct = default);
    Task<ServiceResult<object>> UploadPhotoAsync(int userId, IFormFile photo, CancellationToken ct = default);
    Task<ServiceResult<UserDto>> UpdateProfileAsync(int userId, object dto, CancellationToken ct = default);
}
