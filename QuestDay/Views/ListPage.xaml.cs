using Microsoft.Maui.Controls;
using QuestDay.ViewModels;

namespace QuestDay.Views
{
    public partial class ListPage : ContentPage
    {
        private readonly HabitListViewModel _viewModel;

        public ListPage(HabitListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadHabitsCommand.ExecuteAsync(null);
        }
    }
}