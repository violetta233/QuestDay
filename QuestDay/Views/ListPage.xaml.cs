using Microsoft.Maui.Controls;
using QuestDay.ViewModels;
using QuestDay.Services;
namespace QuestDay.Views
{
    public partial class ListPage : ContentPage
    {

        public ListPage()
        {
            InitializeComponent();
       
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var habitService = new HabitService();
            await habitService.InitializeAsync();

            var viewModel = new HabitListViewModel(habitService);
            await viewModel.InitializeAsync();

            await viewModel.LoadHabitsCommand.ExecuteAsync(null);

            BindingContext = viewModel;
        }
    }
}