using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace QuestDay.Views;

public partial class SettingPage : ContentPage
{
    private const string UserNameKey = "UserName";
    private const string ReminderTimeKey = "ReminderTime";
    private const string ReminderTextKey = "ReminderText";
    private const string ReminderEnabledKey = "ReminderEnabled";
    private const string SoundEnabledKey = "SoundEnabled";
    private const string QuotesEnabledKey = "QuotesEnabled";

    public SettingPage()
    {
        InitializeComponent();
        LoadAllSettings();
    }

    private void LoadAllSettings()
    {
        NameEntry.Text = Preferences.Default.Get(UserNameKey, "");
        TimeEntry.Text = Preferences.Default.Get(ReminderTimeKey, "14:00");
        ReminderTextEntry.Text = Preferences.Default.Get(ReminderTextKey, "");
        ReminderSwitch.IsToggled = Preferences.Default.Get(ReminderEnabledKey, true);

        bool soundEnabled = Preferences.Default.Get(SoundEnabledKey, true);
        SoundSwitch.IsToggled = soundEnabled;

        bool quotesEnabled = Preferences.Default.Get(QuotesEnabledKey, true);
        QuoteSwitch.IsToggled = quotesEnabled;

        UpdateReminderFieldsState();
    }

    private void UpdateReminderFieldsState()
    {
        bool isEnabled = ReminderSwitch.IsToggled;
        TimeEntry.IsEnabled = isEnabled;
        ReminderTextEntry.IsEnabled = isEnabled;

        TimeEntry.Opacity = isEnabled ? 1 : 0.5;
        ReminderTextEntry.Opacity = isEnabled ? 1 : 0.5;
    }

    private void OnNameTextChanged(object sender, TextChangedEventArgs e)
    {
        var text = NameEntry.Text?.Trim() ?? "";
        if (!string.IsNullOrEmpty(text))
        {
            Preferences.Default.Set(UserNameKey, text);
        }
        else
        {
            Preferences.Default.Remove(UserNameKey);
        }
    }

    private void OnReminderSwitchToggled(object sender, ToggledEventArgs e)
    {
        Preferences.Default.Set(ReminderEnabledKey, e.Value);
        UpdateReminderFieldsState();
    }

    private void OnTimeEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var text = TimeEntry.Text?.Trim() ?? "";
        if (!string.IsNullOrEmpty(text))
        {
            Preferences.Default.Set(ReminderTimeKey, text);
        }
        else
        {
            Preferences.Default.Remove(ReminderTimeKey);
        }
    }

    private void OnReminderTextEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var text = ReminderTextEntry.Text?.Trim() ?? "";
        if (!string.IsNullOrEmpty(text))
        {
            Preferences.Default.Set(ReminderTextKey, text);
        }
        else
        {
            Preferences.Default.Remove(ReminderTextKey);
        }
    }

    private void OnSoundSwitchToggled(object sender, ToggledEventArgs e)
    {
        Preferences.Default.Set(SoundEnabledKey, e.Value);

        if (App.BackgroundMusic != null)
        {
            if (e.Value)
            {
                App.BackgroundMusic.Play();
                System.Diagnostics.Debug.WriteLine("Музыка включена");
            }
            else
            {
                App.BackgroundMusic.Pause();
                System.Diagnostics.Debug.WriteLine("Музыка выключена");
            }
        }
    }
   
    private void OnQuoteSwitchToggled(object sender, ToggledEventArgs e)
    {
        Preferences.Default.Set(QuotesEnabledKey, e.Value);
        System.Diagnostics.Debug.WriteLine($"Цитаты {(e.Value ? "включены" : "выключены")}");
    }

    private async void OnNavClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as ImageButton;
            string route = button?.CommandParameter?.ToString();
            if (!string.IsNullOrEmpty(route))
            {
                await Shell.Current.GoToAsync($"//{route}");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось открыть страницу: {ex.Message}", "ОК");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        bool soundEnabled = Preferences.Default.Get(SoundEnabledKey, true);
        bool isMusicPlaying = App.BackgroundMusic?.IsPlaying ?? false;

        if (soundEnabled && App.BackgroundMusic != null && !isMusicPlaying)
        {
            App.BackgroundMusic.Play();
        }

        bool quotesEnabled = Preferences.Default.Get(QuotesEnabledKey, true);
        if (QuoteSwitch.IsToggled != quotesEnabled)
        {
            QuoteSwitch.IsToggled = quotesEnabled;
        }
    }
}