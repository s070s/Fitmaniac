namespace Fitmaniac.MAUI.Pages;

public partial class WorkoutDetailPage : ContentPage
{
    private readonly WorkoutDetailViewModel _vm;

    public WorkoutDetailPage(WorkoutDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute(null);
    }
}
