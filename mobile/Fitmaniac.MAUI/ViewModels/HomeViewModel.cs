namespace Fitmaniac.MAUI.ViewModels;

public partial class HomeViewModel(IProgressService progressService, IWorkoutService workoutService) : BaseViewModel
{
    private ProgressSummaryDto? _summary;
    private ObservableCollection<WorkoutListItemDto> _recentWorkouts = [];

    public ProgressSummaryDto? Summary
    {
        get => _summary;
        set => SetProperty(ref _summary, value);
    }

    public ObservableCollection<WorkoutListItemDto> RecentWorkouts
    {
        get => _recentWorkouts;
        set => SetProperty(ref _recentWorkouts, value);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            Summary = await progressService.GetSummaryAsync();
            var workouts = await workoutService.GetMyWorkoutsAsync();
            RecentWorkouts = workouts is not null
                ? new ObservableCollection<WorkoutListItemDto>(workouts.Items.Take(5))
                : [];
        }
        finally { IsBusy = false; }
    }
}
