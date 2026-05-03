using System.ComponentModel;

namespace QuestDay.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = App.AvatarAppearance;
            App.AvatarAppearance.PropertyChanged += OnAvatarAppearanceChanged;
            UpdateRabbitImage();
        }

        private void OnAvatarAppearanceChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(Services.AvatarAppearanceService.CurrentRabbitImage))
            {
                MainThread.BeginInvokeOnMainThread(UpdateRabbitImage);
            }
        }

        private void UpdateRabbitImage()
        {
            MainRabbitImage.Source = App.AvatarAppearance.CurrentRabbitImage;
        }
    }
}
