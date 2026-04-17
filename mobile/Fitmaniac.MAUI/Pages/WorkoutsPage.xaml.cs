namespace Fitmaniac.MAUI.Pages;

public partial class WorkoutsPage : ContentPage
{
    private readonly WorkoutsViewModel _vm;

    public WorkoutsPage(WorkoutsViewModel vm)
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
