using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Common;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileStorage _fileStorage;
    private readonly IValidationService _validation;
    private readonly IModelToDtoMapper _mapper;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IFileStorage fileStorage,
        IValidationService validation,
        IModelToDtoMapper mapper)
    {
        _userManager = userManager;
        _fileStorage = fileStorage;
        _validation = validation;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UserDto>> GetCurrentUserAsync(int userId, CancellationToken ct = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return ServiceResult<UserDto>.NotFound("User not found.");
        return ServiceResult<UserDto>.Ok(_mapper.ToDto(user)!);
    }

    public async Task<ServiceResult<object>> UploadPhotoAsync(int userId, IFormFile photo, CancellationToken ct = default)
    {
        await using var photoStream = photo.OpenReadStream();
        if (!_validation.IsValidImage(photoStream, photo.FileName))
            return ServiceResult<object>.BadRequest("Invalid image file.");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return ServiceResult<object>.NotFound("User not found.");

        await using var stream = photo.OpenReadStream();
        var url = await _fileStorage.SaveAsync(stream, photo.FileName, photo.ContentType, ct);

        user.PhoneNumber = user.PhoneNumber; // keep other fields
        await _userManager.UpdateAsync(user);

        return ServiceResult<object>.Ok(new { photoUrl = url });
    }

    public async Task<ServiceResult<UserDto>> UpdateProfileAsync(int userId, object dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return ServiceResult<UserDto>.NotFound("User not found.");
        // Profile-specific updates handled by sub-services (Trainer/Client)
        await _userManager.UpdateAsync(user);
        return ServiceResult<UserDto>.Ok(_mapper.ToDto(user)!);
    }
}
