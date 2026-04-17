using Fitmaniac.Shared.DTOs.Progress;

namespace Fitmaniac.MAUI.ViewModels;

public partial class ProgressViewModel(IProgressService progressService) : BaseViewModel
{
    private ProgressSummaryDto? _summary;
    private ObservableCollection<WeeklyProgressDto> _weekly = [];

    public ProgressSummaryDto? Summary
    {
        get => _summary;
        set => SetProperty(ref _summary, value);
    }

    public ObservableCollection<WeeklyProgressDto> Weekly
    {
        get => _weekly;
        set => SetProperty(ref _weekly, value);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            Summary = await progressService.GetSummaryAsync();
            var w = await progressService.GetWeeklyAsync();
            Weekly = w is not null ? new ObservableCollection<WeeklyProgressDto>(w) : [];
        }
        finally { IsBusy = false; }
    }
}
