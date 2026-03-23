using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Storage;

namespace QuestDay.Services;

public sealed class AvatarAppearanceService : INotifyPropertyChanged
{
    private const string RabbitVariantPreferenceKey = "avatar.rabbitVariantImage";
    private const string TopPreferenceKey = "avatar.topImage";
    private const string HatPreferenceKey = "avatar.hatImage";

    private string? _rabbitVariantImage;
    private string? _topImage;
    private string? _hatImage;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AvatarAppearanceService()
    {
        _rabbitVariantImage = ReadPreference(RabbitVariantPreferenceKey);
        _topImage = ReadPreference(TopPreferenceKey);
        _hatImage = ReadPreference(HatPreferenceKey);
    }

    public string? RabbitVariantImage
    {
        get => _rabbitVariantImage;
        set
        {
            if (SetProperty(ref _rabbitVariantImage, value))
            {
                WritePreference(RabbitVariantPreferenceKey, value);
                OnPropertyChanged(nameof(IsAlternateRabbitVisible));
            }
        }
    }

    public string? TopImage
    {
        get => _topImage;
        set
        {
            if (SetProperty(ref _topImage, value))
            {
                WritePreference(TopPreferenceKey, value);
                OnPropertyChanged(nameof(HasTopImage));
            }
        }
    }

    public string? HatImage
    {
        get => _hatImage;
        set
        {
            if (SetProperty(ref _hatImage, value))
            {
                WritePreference(HatPreferenceKey, value);
                OnPropertyChanged(nameof(HasHatImage));
            }
        }
    }

    public bool HasTopImage => !string.IsNullOrWhiteSpace(TopImage);

    public bool HasHatImage => !string.IsNullOrWhiteSpace(HatImage);

    public bool IsAlternateRabbitVisible => !string.IsNullOrWhiteSpace(RabbitVariantImage);

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static string? ReadPreference(string key)
    {
        var value = Preferences.Default.Get(key, string.Empty);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static void WritePreference(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Preferences.Default.Remove(key);
            return;
        }

        Preferences.Default.Set(key, value);
    }
}
