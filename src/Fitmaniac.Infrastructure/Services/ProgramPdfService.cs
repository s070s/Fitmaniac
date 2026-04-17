using Fitmaniac.Application.Common;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Fitmaniac.Infrastructure.Services;

public sealed class ProgramPdfService : IProgramPdfService
{
    static ProgramPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<ServiceResult<byte[]>> GenerateProgramPdfAsync(WeeklyProgram program, CancellationToken ct = default)
    {
        var bytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(t => t.FontSize(11));

                page.Header().Text(program.Name).Bold().FontSize(18);

                page.Content().Column(col =>
                {
                    col.Item().Text($"Duration: {program.DurationInWeeks} weeks");
                    col.Item().Text($"Current Week: {program.CurrentWeek}");
                    if (!string.IsNullOrEmpty(program.Description))
                        col.Item().Text(program.Description);

                    col.Item().PaddingTop(10).Text("Workouts").Bold().FontSize(14);

                    foreach (var workout in program.Workouts.OrderBy(w => w.ScheduledDateTime))
                    {
                        col.Item().PaddingTop(5).Text($"• {workout.ScheduledDateTime:ddd dd MMM} — {workout.Type} ({workout.DurationInMinutes} min)");
                        if (!string.IsNullOrEmpty(workout.Notes))
                            col.Item().PaddingLeft(15).Text(workout.Notes).FontColor(Colors.Grey.Medium);
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();

        return Task.FromResult(ServiceResult<byte[]>.Ok(bytes));
    }
}
