using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Storage;

namespace QuestDay.Services;

public sealed class AvatarAppearanceService : INotifyPropertyChanged
{
    private const string WarmPaletteImage = "rabbit_1_warm_bg_skintone.png";
    private const string WhitePaletteImage = "rabbit_3_white_bg_skintone.png";

    // Грязные варианты кролика (rabbit_2)
    private const string WarmDirtyPaletteImage = "rabbit_2_dark_skintone.png";
    private const string WhiteDirtyPaletteImage = "rabbit_2_dark_skintone.png"; // Временно используем тот же, пока нет белого грязного

    private const string OverallsImage = "overalls_skin1_blue.png";
    private const string TShirtImage = "t_shirt_mechmat.png";
    private const string OverallsAndBeltImage = "overalls_and_balt.png";
    private const string HatOneImage = "hat_one.png";
    private const string HatTwoImage = "hat_two.png";

    // Чистые комбинации (тёплый кролик - rabbit_1)
    private const string WarmOverallsRabbitImage = "rabbit_1_overalls.png";
    private const string WarmTShirtRabbitImage = "rabbit_1_t_shirt_mechm.png";
    private const string WarmOverallsAndBeltRabbitImage = "rabbit_1_overalls_belt.png";
    private const string WarmHatOneRabbitImage = "rabbit_1_hat_one.png";
    private const string WarmHatTwoRabbitImage = "rabbit_1_hat_two.png";
    private const string WarmOverallsHatOneRabbitImage = "rabbit_1_overalls_hat_one.png";
    private const string WarmOverallsHatTwoRabbitImage = "rabbit_1_overalls_hat_two.png";
    private const string WarmTShirtHatOneRabbitImage = "rabbit_1_t_shirt_hat_one.png";
    private const string WarmTShirtHatTwoRabbitImage = "rabbit_1_t_shirt_hat_two.png";
    private const string WarmOverallsAndBeltHatOneRabbitImage = "rabbit_1_overalls_belt_hat_one.png";
    private const string WarmOverallsAndBeltHatTwoRabbitImage = "rabbit_1_overalls_belt_hat_two.png";

    // Грязные комбинации (тёплый кролик - rabbit_2)
    private const string WarmDirtyOverallsRabbitImage = "rabbit_2_overalls.png";
    private const string WarmDirtyTShirtRabbitImage = "rabbit_2_t_shirt_mechm.png";
    private const string WarmDirtyOverallsAndBeltRabbitImage = "rabbit_2_overalls_belt.png";
    private const string WarmDirtyHatOneRabbitImage = "rabbit_2_hat_one.png";
    private const string WarmDirtyHatTwoRabbitImage = "rabbit_2_hat_two.png";
    private const string WarmDirtyOverallsHatOneRabbitImage = "rabbit_2_overalls_hat_one.png";
    private const string WarmDirtyOverallsHatTwoRabbitImage = "rabbit_2_overalls_hat_two.png";
    private const string WarmDirtyTShirtHatOneRabbitImage = "rabbit_2_t_shirt_hat_one.png";
    private const string WarmDirtyTShirtHatTwoRabbitImage = "rabbit_2_t_shirt_hat_two.png";
    private const string WarmDirtyOverallsAndBeltHatOneRabbitImage = "rabbit_2_overalls_belt_hat_one.png";
    private const string WarmDirtyOverallsAndBeltHatTwoRabbitImage = "rabbit_2_overalls_belt_hat_two.png";

    // Чистые комбинации (белый кролик - rabbit_3)
    private const string WhiteOverallsRabbitImage = "rabbit_3_overalls.png";
    private const string WhiteTShirtRabbitImage = "rabbit_3_t_shirt_mechm.png";
    private const string WhiteOverallsAndBeltRabbitImage = "rabbit_3_overalls_belt.png";
    private const string WhiteHatOneRabbitImage = "rabbit_3_hat_1.png";
    private const string WhiteHatTwoRabbitImage = "rabbit_3_hat_2.png";
    private const string WhiteOverallsHatOneRabbitImage = "rabbit_3_overalls_hat_one.png";
    private const string WhiteOverallsHatTwoRabbitImage = "rabbit_3_overalls_hat_two.png";
    private const string WhiteTShirtHatOneRabbitImage = "rabbit_3_t_shirt_hat_one.png";
    private const string WhiteTShirtHatTwoRabbitImage = "rabbit_3_t_shirt_hat_two.png";
    private const string WhiteOverallsAndBeltHatOneRabbitImage = "rabbit_3_overalls_belt_hat_one.png";
    private const string WhiteOverallsAndBeltHatTwoRabbitImage = "rabbit_3_overalls_belt_hat_two.png";

    // Грязные комбинации (белый кролик - пока нет, используем rabbit_2 как заглушку)
    // TODO: Заменить на rabbit_3_dirty_* когда появятся
    private const string WhiteDirtyOverallsRabbitImage = "rabbit_2_overalls.png";
    private const string WhiteDirtyTShirtRabbitImage = "rabbit_2_t_shirt_mechm.png";
    private const string WhiteDirtyOverallsAndBeltRabbitImage = "rabbit_2_overalls_belt.png";
    private const string WhiteDirtyHatOneRabbitImage = "rabbit_2_hat_one.png";
    private const string WhiteDirtyHatTwoRabbitImage = "rabbit_2_hat_two.png";
    private const string WhiteDirtyOverallsHatOneRabbitImage = "rabbit_2_overalls_hat_one.png";
    private const string WhiteDirtyOverallsHatTwoRabbitImage = "rabbit_2_overalls_hat_two.png";
    private const string WhiteDirtyTShirtHatOneRabbitImage = "rabbit_2_t_shirt_hat_one.png";
    private const string WhiteDirtyTShirtHatTwoRabbitImage = "rabbit_2_t_shirt_hat_two.png";
    private const string WhiteDirtyOverallsAndBeltHatOneRabbitImage = "rabbit_2_overalls_belt_hat_one.png";
    private const string WhiteDirtyOverallsAndBeltHatTwoRabbitImage = "rabbit_2_overalls_belt_hat_two.png";

    private const string RabbitVariantPreferenceKey = "avatar.rabbitVariantImage";
    private const string TopPreferenceKey = "avatar.topImage";
    private const string HatPreferenceKey = "avatar.hatImage";

    private string? _rabbitVariantImage;
    private string? _topImage;
    private string? _hatImage;
    private bool _isDirty;

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
                OnPropertyChanged(nameof(CurrentRabbitImage));
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
                OnPropertyChanged(nameof(CurrentRabbitImage));
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
                OnPropertyChanged(nameof(CurrentRabbitImage));
            }
        }
    }

    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (SetProperty(ref _isDirty, value))
            {
                OnPropertyChanged(nameof(CurrentRabbitImage));
            }
        }
    }

    public bool HasTopImage => !string.IsNullOrWhiteSpace(TopImage);

    public bool HasHatImage => !string.IsNullOrWhiteSpace(HatImage);

    public bool IsAlternateRabbitVisible => !string.IsNullOrWhiteSpace(RabbitVariantImage);

    public string CurrentRabbitImage => ResolveCurrentRabbitImage();

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

    private string ResolveCurrentRabbitImage()
    {
        var isWhitePalette = string.Equals(RabbitVariantImage, WhitePaletteImage, StringComparison.OrdinalIgnoreCase);
        var hatVariant = ResolveHatVariant(HatImage);

        if (IsDirty)
        {
            return ResolveDirtyRabbitImage(isWhitePalette, hatVariant);
        }

        return ResolveCleanRabbitImage(isWhitePalette, hatVariant);
    }

    private string ResolveCleanRabbitImage(bool isWhitePalette, HatVariant hatVariant)
    {
        return (TopImage, hatVariant, isWhitePalette) switch
        {
            (OverallsImage, HatVariant.HatOne, true) => WhiteOverallsHatOneRabbitImage,
            (OverallsImage, HatVariant.HatTwo, true) => WhiteOverallsHatTwoRabbitImage,
            (OverallsImage, _, true) => WhiteOverallsRabbitImage,
            (TShirtImage, HatVariant.HatOne, true) => WhiteTShirtHatOneRabbitImage,
            (TShirtImage, HatVariant.HatTwo, true) => WhiteTShirtHatTwoRabbitImage,
            (TShirtImage, _, true) => WhiteTShirtRabbitImage,
            (OverallsAndBeltImage, HatVariant.HatOne, true) => WhiteOverallsAndBeltHatOneRabbitImage,
            (OverallsAndBeltImage, HatVariant.HatTwo, true) => WhiteOverallsAndBeltHatTwoRabbitImage,
            (OverallsAndBeltImage, _, true) => WhiteOverallsAndBeltRabbitImage,
            (_, HatVariant.HatOne, true) => WhiteHatOneRabbitImage,
            (_, HatVariant.HatTwo, true) => WhiteHatTwoRabbitImage,
            (OverallsImage, HatVariant.HatOne, false) => WarmOverallsHatOneRabbitImage,
            (OverallsImage, HatVariant.HatTwo, false) => WarmOverallsHatTwoRabbitImage,
            (OverallsImage, _, false) => WarmOverallsRabbitImage,
            (TShirtImage, HatVariant.HatOne, false) => WarmTShirtHatOneRabbitImage,
            (TShirtImage, HatVariant.HatTwo, false) => WarmTShirtHatTwoRabbitImage,
            (TShirtImage, _, false) => WarmTShirtRabbitImage,
            (OverallsAndBeltImage, HatVariant.HatOne, false) => WarmOverallsAndBeltHatOneRabbitImage,
            (OverallsAndBeltImage, HatVariant.HatTwo, false) => WarmOverallsAndBeltHatTwoRabbitImage,
            (OverallsAndBeltImage, _, false) => WarmOverallsAndBeltRabbitImage,
            (_, HatVariant.HatOne, false) => WarmHatOneRabbitImage,
            (_, HatVariant.HatTwo, false) => WarmHatTwoRabbitImage,
            (_, _, true) => WhitePaletteImage,
            _ => WarmPaletteImage
        };
    }

    private string ResolveDirtyRabbitImage(bool isWhitePalette, HatVariant hatVariant)
    {
        if (isWhitePalette)
        {
            // Белый кролик (грязные варианты - заглушки, пока нет rabbit_3_dirty)
            return (TopImage, hatVariant) switch
            {
                (OverallsImage, HatVariant.HatOne) => WhiteDirtyOverallsHatOneRabbitImage,
                (OverallsImage, HatVariant.HatTwo) => WhiteDirtyOverallsHatTwoRabbitImage,
                (OverallsImage, _) => WhiteDirtyOverallsRabbitImage,
                (TShirtImage, HatVariant.HatOne) => WhiteDirtyTShirtHatOneRabbitImage,
                (TShirtImage, HatVariant.HatTwo) => WhiteDirtyTShirtHatTwoRabbitImage,
                (TShirtImage, _) => WhiteDirtyTShirtRabbitImage,
                (OverallsAndBeltImage, HatVariant.HatOne) => WhiteDirtyOverallsAndBeltHatOneRabbitImage,
                (OverallsAndBeltImage, HatVariant.HatTwo) => WhiteDirtyOverallsAndBeltHatTwoRabbitImage,
                (OverallsAndBeltImage, _) => WhiteDirtyOverallsAndBeltRabbitImage,
                (_, HatVariant.HatOne) => WhiteDirtyHatOneRabbitImage,
                (_, HatVariant.HatTwo) => WhiteDirtyHatTwoRabbitImage,
                _ => WhiteDirtyPaletteImage
            };
        }

        // Тёплый кролик (rabbit_2)
        return (TopImage, hatVariant) switch
        {
            (OverallsImage, HatVariant.HatOne) => WarmDirtyOverallsHatOneRabbitImage,
            (OverallsImage, HatVariant.HatTwo) => WarmDirtyOverallsHatTwoRabbitImage,
            (OverallsImage, _) => WarmDirtyOverallsRabbitImage,
            (TShirtImage, HatVariant.HatOne) => WarmDirtyTShirtHatOneRabbitImage,
            (TShirtImage, HatVariant.HatTwo) => WarmDirtyTShirtHatTwoRabbitImage,
            (TShirtImage, _) => WarmDirtyTShirtRabbitImage,
            (OverallsAndBeltImage, HatVariant.HatOne) => WarmDirtyOverallsAndBeltHatOneRabbitImage,
            (OverallsAndBeltImage, HatVariant.HatTwo) => WarmDirtyOverallsAndBeltHatTwoRabbitImage,
            (OverallsAndBeltImage, _) => WarmDirtyOverallsAndBeltRabbitImage,
            (_, HatVariant.HatOne) => WarmDirtyHatOneRabbitImage,
            (_, HatVariant.HatTwo) => WarmDirtyHatTwoRabbitImage,
            _ => WarmDirtyPaletteImage
        };
    }

    private static HatVariant ResolveHatVariant(string? hatImage)
    {
        if (string.Equals(hatImage, HatOneImage, StringComparison.OrdinalIgnoreCase))
        {
            return HatVariant.HatOne;
        }

        if (string.Equals(hatImage, HatTwoImage, StringComparison.OrdinalIgnoreCase))
        {
            return HatVariant.HatTwo;
        }

        return HatVariant.None;
    }

    private enum HatVariant
    {
        None,
        HatOne,
        HatTwo
    }
}