namespace Fitmaniac.MAUI.ViewModels;

public partial class WorkoutsViewModel(IWorkoutService workoutService) : BaseViewModel
{
    private ObservableCollection<WorkoutListItemDto> _workouts = [];

    public ObservableCollection<WorkoutListItemDto> Workouts
    {
        get => _workouts;
        set => SetProperty(ref _workouts, value);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var result = await workoutService.GetMyWorkoutsAsync();
            Workouts = result is not null
                ? new ObservableCollection<WorkoutListItemDto>(result.Items)
                : [];
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task ViewDetailAsync(WorkoutListItemDto workout) =>
        Shell.Current.GoToAsync($"workoutdetail?id={workout.Id}");
}
