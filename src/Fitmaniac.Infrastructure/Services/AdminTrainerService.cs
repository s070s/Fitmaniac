using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.Constants;
using Fitmaniac.Shared.DTOs.Trainers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class AdminTrainerService : IAdminTrainerService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public AdminTrainerService(UserManager<ApplicationUser> userManager, IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _userManager = userManager;
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<TrainerDto>> CreateAsync(CreateTrainerDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user is null) return ServiceResult<TrainerDto>.NotFound("User not found.");

        if (user.Role == UserRole.Trainer)
            return ServiceResult<TrainerDto>.Conflict("User already has a trainer profile.");

        user.Role = UserRole.Trainer;
        await _userManager.AddToRoleAsync(user, RoleNames.Trainer);
        await _userManager.RemoveFromRoleAsync(user, RoleNames.Client);
        await _userManager.UpdateAsync(user);

        var trainer = new Trainer
        {
            UserId = user.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Bio = dto.Bio,
            Specializations = dto.Specializations?.ToList() ?? [],
        };
        _db.Trainers.Add(trainer);
        await _db.SaveChangesAsync(ct);
        trainer.User = user;

        return ServiceResult<TrainerDto>.Created(_mapper.ToDto(trainer)!);
    }

    public async Task<ServiceResult<TrainerDto>> UpdateAsync(int id, UpdateTrainerProfileDto dto, CancellationToken ct = default)
    {
        var trainer = await _db.Trainers.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id, ct);
        if (trainer is null) return ServiceResult<TrainerDto>.NotFound("Trainer not found.");

        trainer.FirstName = dto.FirstName ?? trainer.FirstName;
        trainer.LastName = dto.LastName ?? trainer.LastName;
        trainer.Bio = dto.Bio ?? trainer.Bio;
        if (dto.Specializations is not null) trainer.Specializations = dto.Specializations.ToList();

        await _db.SaveChangesAsync(ct);
        return ServiceResult<TrainerDto>.Ok(_mapper.ToDto(trainer)!);
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var trainer = await _db.Trainers.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id, ct);
        if (trainer is null) return ServiceResult<object>.NotFound("Trainer not found.");
        _db.Trainers.Remove(trainer);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }
}
