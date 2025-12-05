using Microsoft.Maui.Controls;
using QuestDay.ViewModels;

namespace QuestDay.Views
{
    public partial class AddPage : ContentPage
    {
        public AddPage(AddHabitViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
         protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is AddHabitViewModel viewModel)
            {
                viewModel.Name = string.Empty;
                viewModel.Description = string.Empty;
                viewModel.DaysOfWeekSelection.Reset();
            }
        }
    }
}
