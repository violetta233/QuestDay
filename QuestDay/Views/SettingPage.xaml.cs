using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Plugin.Maui.Audio;
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
        TimePicker.PropertyChanged += OnTimePickerPropertyChanged;
    }

    private void LoadAllSettings()
    {
        NameEntry.Text = Preferences.Default.Get(UserNameKey, "");

        string savedTime = Preferences.Default.Get(ReminderTimeKey, "14:00");
        if (TimeSpan.TryParse(savedTime, out TimeSpan time))
        {
            TimePicker.Time = time;
        }
        else
        {
            TimePicker.Time = new TimeSpan(14, 0, 0);
        }

        ReminderTextEntry.Text = "";
        Preferences.Default.Remove(ReminderTextKey);

        ReminderSwitch.IsToggled = Preferences.Default.Get(ReminderEnabledKey, true);

        SoundSwitch.IsToggled = Preferences.Default.Get(SoundEnabledKey, true);
        QuoteSwitch.IsToggled = Preferences.Default.Get(QuotesEnabledKey, true);

        UpdateReminderFieldsState();
    }

    private void UpdateReminderFieldsState()
    {
        bool isEnabled = ReminderSwitch.IsToggled;
        TimePicker.IsEnabled = isEnabled;
        ReminderTextEntry.IsEnabled = isEnabled;

        TimePicker.Opacity = isEnabled ? 1 : 0.5;
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

    private void OnTimePickerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Time")
        {
            string timeString = TimePicker.Time.ToString(@"hh\:mm");
            Preferences.Default.Set(ReminderTimeKey, timeString);
            System.Diagnostics.Debug.WriteLine($"Время сохранено: {timeString}");
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
                App.BackgroundMusic.Play();
            else
                App.BackgroundMusic.Stop();
        }
    }

    private void OnQuoteSwitchToggled(object sender, ToggledEventArgs e)
    {
        Preferences.Default.Set(QuotesEnabledKey, e.Value);
    }
}