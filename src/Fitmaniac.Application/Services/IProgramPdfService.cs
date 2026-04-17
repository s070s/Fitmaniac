using Fitmaniac.Application.Common;
using Fitmaniac.Domain.Entities;

namespace Fitmaniac.Application.Services;

public interface IProgramPdfService
{
    Task<ServiceResult<byte[]>> GenerateProgramPdfAsync(WeeklyProgram program, CancellationToken ct = default);
}
