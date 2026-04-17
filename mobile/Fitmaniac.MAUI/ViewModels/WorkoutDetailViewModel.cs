namespace Fitmaniac.MAUI.ViewModels;

[QueryProperty(nameof(WorkoutId), "id")]
public partial class WorkoutDetailViewModel(IWorkoutService workoutService) : BaseViewModel
{
    private int _workoutId;
    private WorkoutDto? _workout;
    private int? _perceivedIntensity;

    public int WorkoutId
    {
        get => _workoutId;
        set => SetProperty(ref _workoutId, value);
    }

    public WorkoutDto? Workout
    {
        get => _workout;
        set => SetProperty(ref _workout, value);
    }

    public int? PerceivedIntensity
    {
        get => _perceivedIntensity;
        set => SetProperty(ref _perceivedIntensity, value);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try { Workout = await workoutService.GetByIdAsync(WorkoutId); }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task CompleteAsync()
    {
        IsBusy = true;
        try
        {
            await workoutService.CompleteWorkoutAsync(WorkoutId, PerceivedIntensity);
            await Shell.Current.GoToAsync("..");
        }
        finally { IsBusy = false; }
    }
}
