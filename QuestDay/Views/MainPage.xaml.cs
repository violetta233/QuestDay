using QuestDay.Services;
using System.ComponentModel;
using System.Diagnostics;

namespace QuestDay.Views;

public partial class MainPage : ContentPage
{
    private readonly IHouseStateService _houseStateService;
    private readonly IHabitService _habitService;

    public MainPage(IHouseStateService houseStateService, IHabitService habitService)
    {
        InitializeComponent();
        _houseStateService = houseStateService;
        _habitService = habitService;

        _houseStateService.BackgroundChanged += OnBackgroundChanged;
        _houseStateService.DirtLevelChanged += OnDirtLevelChanged;
        _houseStateService.RabbitDirtyStateChanged += OnRabbitDirtyStateChanged;
        App.AvatarAppearance.PropertyChanged += OnAvatarAppearanceChanged;

        UpdateRabbitImage();
        LoadHouseState();

        Debug.WriteLine("MainPage инициализирована");
    }

    private void OnBackgroundChanged(object sender, string imageName)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (HouseBackgroundImage != null)
            {
                HouseBackgroundImage.Source = imageName;
            }
        });
    }

    private void OnDirtLevelChanged(object sender, int dirtyLevel)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (DirtyLevelLabel != null)
            {
                int cleanliness = 100 - dirtyLevel;
                string statusText = "";

                if (cleanliness >= 70)
                    statusText = "Чистый";
                else if (cleanliness >= 30)
                    statusText = "Грязный";
                else
                    statusText = "Очень грязный!";

                DirtyLevelLabel.Text = $"{statusText}\nЧистота: {cleanliness}%";

                if (cleanliness < 30)
                    DirtyLevelLabel.TextColor = Color.FromArgb("#FF5252");
                else if (cleanliness < 70)
                    DirtyLevelLabel.TextColor = Color.FromArgb("#FF9800");
                else
                    DirtyLevelLabel.TextColor = Color.FromArgb("#4CAF50");
            }
        });
    }

    private void OnRabbitDirtyStateChanged(object sender, bool isDirty)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Debug.WriteLine($"MainPage: RabbitDirtyStateChanged = {isDirty}");
            App.AvatarAppearance.IsDirty = isDirty;
        });
    }

    private async void LoadHouseState()
    {
        var state = await _houseStateService.GetCurrentStateAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (HouseBackgroundImage != null)
            {
                HouseBackgroundImage.Source = state.GetMainPageBackground();
            }

            if (DirtyLevelLabel != null)
            {
                int cleanliness = 100 - state.DirtyLevel;
                string statusText = "";

                if (cleanliness >= 70)
                    statusText = "Чистый";
                else if (cleanliness >= 30)
                    statusText = "Грязный";
                else
                    statusText = "Очень грязный!";

                DirtyLevelLabel.Text = $"{statusText}\nЧистота: {cleanliness}%";

                if (cleanliness < 30)
                    DirtyLevelLabel.TextColor = Color.FromArgb("#FF5252");
                else if (cleanliness < 70)
                    DirtyLevelLabel.TextColor = Color.FromArgb("#FF9800");
                else
                    DirtyLevelLabel.TextColor = Color.FromArgb("#4CAF50");
            }

            // 🆕 Принудительно обновляем состояние кролика при загрузке
            App.AvatarAppearance.IsDirty = state.IsRabbitDirty;
        });
    }

    private void OnAvatarAppearanceChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AvatarAppearanceService.CurrentRabbitImage))
        {
            MainThread.BeginInvokeOnMainThread(UpdateRabbitImage);
        }
    }

    private void UpdateRabbitImage()
    {
        if (MainRabbitImage != null)
        {
            MainRabbitImage.Source = App.AvatarAppearance.CurrentRabbitImage;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _houseStateService.UpdateStateAsync();
        var state = await _houseStateService.GetCurrentStateAsync();
        App.AvatarAppearance.IsDirty = state.IsRabbitDirty;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _houseStateService.BackgroundChanged -= OnBackgroundChanged;
        _houseStateService.DirtLevelChanged -= OnDirtLevelChanged;
        _houseStateService.RabbitDirtyStateChanged -= OnRabbitDirtyStateChanged;
        App.AvatarAppearance.PropertyChanged -= OnAvatarAppearanceChanged;
    }
}