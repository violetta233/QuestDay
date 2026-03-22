using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuestDay.Views;

public partial class userPage : ContentPage
{
    private readonly Dictionary<WardrobeCategory, List<WardrobeOption>> _optionsByCategory;
    private WardrobeCategory _activeCategory;
    private string _activeRabbitImage = "rabbit_warm.png";
    private string? _activeTopImage;
    private string? _activeHatImage;
    private string _activeCategoryTitle = "Шляпы";

    public ObservableCollection<WardrobeOption> ActiveOptions { get; } = [];

    public string ActiveRabbitImage
    {
        get => _activeRabbitImage;
        set
        {
            if (SetProperty(ref _activeRabbitImage, value))
            {
                OnPropertyChanged(nameof(HasRabbitImage));
            }
        }
    }

    public string? ActiveTopImage
    {
        get => _activeTopImage;
        set
        {
            if (SetProperty(ref _activeTopImage, value))
            {
                OnPropertyChanged(nameof(HasTopImage));
            }
        }
    }

    public string? ActiveHatImage
    {
        get => _activeHatImage;
        set
        {
            if (SetProperty(ref _activeHatImage, value))
            {
                OnPropertyChanged(nameof(HasHatImage));
            }
        }
    }

    public string ActiveCategoryTitle
    {
        get => _activeCategoryTitle;
        set => SetProperty(ref _activeCategoryTitle, value);
    }

    public bool HasRabbitImage => !string.IsNullOrWhiteSpace(ActiveRabbitImage);

    public bool HasTopImage => !string.IsNullOrWhiteSpace(ActiveTopImage);

    public bool HasHatImage => !string.IsNullOrWhiteSpace(ActiveHatImage);

    public userPage()
    {
        InitializeComponent();
        BindingContext = this;

        _optionsByCategory = CreateWardrobeOptions();

        ApplySelectedOption(_optionsByCategory[WardrobeCategory.Hat].First(option => option.IsSelected));
        ApplySelectedOption(_optionsByCategory[WardrobeCategory.Top].First(option => option.IsSelected));
        ApplySelectedOption(_optionsByCategory[WardrobeCategory.Palette].First(option => option.IsSelected));

        SwitchCategory(WardrobeCategory.Hat);
    }

    private Dictionary<WardrobeCategory, List<WardrobeOption>> CreateWardrobeOptions()
    {
        return new Dictionary<WardrobeCategory, List<WardrobeOption>>
        {
            [WardrobeCategory.Hat] =
            [
                new WardrobeOption(
                    WardrobeCategory.Hat,
                    "Без шляпы",
                    "Оставить образ легким и открытым.",
                    "rabbit_warm.png",
                    null,
                    isSelected: true),
                new WardrobeOption(
                    WardrobeCategory.Hat,
                    "Панама",
                    "Добавляет кролику акцент сверху.",
                    "headdress.png",
                    "headdress.png")
            ],
            [WardrobeCategory.Top] =
            [
                new WardrobeOption(
                    WardrobeCategory.Top,
                    "Без одежды",
                    "Базовый вид без дополнительного слоя.",
                    "rabbit_warm.png",
                    null,
                    isSelected: true),
                new WardrobeOption(
                    WardrobeCategory.Top,
                    "Комбинезон",
                    "Мягкий повседневный образ в теплой палитре.",
                    "overalls_skin1_blue.png",
                    "overalls_skin1_blue.png"),
                new WardrobeOption(
                    WardrobeCategory.Top,
                    "Футболка",
                    "Более активный и яркий вариант для персонажа.",
                    "t_shirt_mechmat.png",
                    "t_shirt_mechmat.png")
            ],
            [WardrobeCategory.Palette] =
            [
                new WardrobeOption(
                    WardrobeCategory.Palette,
                    "Теплый",
                    "Мягкий бежевый оттенок под текущий стиль приложения.",
                    "rabbit_warm.png",
                    "rabbit_warm.png",
                    isSelected: true),
                new WardrobeOption(
                    WardrobeCategory.Palette,
                    "Светлый",
                    "Более светлый вариант окраски кролика.",
                    "rabbit_1_warm_bg_skintone.png",
                    "rabbit_1_warm_bg_skintone.png")
            ]
        };
    }

    private void OnHatButtonClicked(object? sender, EventArgs e) => SwitchCategory(WardrobeCategory.Hat);

    private void OnTopButtonClicked(object? sender, EventArgs e) => SwitchCategory(WardrobeCategory.Top);

    private void OnPaletteButtonClicked(object? sender, EventArgs e) => SwitchCategory(WardrobeCategory.Palette);

    private void OnOptionActionClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not WardrobeOption option)
        {
            return;
        }

        foreach (var wardrobeOption in _optionsByCategory[option.Category])
        {
            wardrobeOption.IsSelected = ReferenceEquals(wardrobeOption, option);
        }

        ApplySelectedOption(option);
        RefreshVisibleOptions();
    }

    private void SwitchCategory(WardrobeCategory category)
    {
        _activeCategory = category;
        ActiveCategoryTitle = category switch
        {
            WardrobeCategory.Hat => "Шляпы",
            WardrobeCategory.Top => "Одежда",
            _ => "Окраска"
        };

        UpdateCategoryButtons();
        RefreshVisibleOptions();
    }

    private void RefreshVisibleOptions()
    {
        ActiveOptions.Clear();

        foreach (var option in _optionsByCategory[_activeCategory])
        {
            ActiveOptions.Add(option);
        }
    }

    private void ApplySelectedOption(WardrobeOption option)
    {
        switch (option.Category)
        {
            case WardrobeCategory.Hat:
                ActiveHatImage = option.AppliedImage;
                break;
            case WardrobeCategory.Top:
                ActiveTopImage = option.AppliedImage;
                break;
            case WardrobeCategory.Palette:
                ActiveRabbitImage = option.AppliedImage ?? "rabbit_warm.png";
                break;
        }
    }

    private void UpdateCategoryButtons()
    {
        ApplyCategoryState(HatCategoryBorder, _activeCategory == WardrobeCategory.Hat);
        ApplyCategoryState(TopCategoryBorder, _activeCategory == WardrobeCategory.Top);
        ApplyCategoryState(PaletteCategoryBorder, _activeCategory == WardrobeCategory.Palette);
    }

    private static void ApplyCategoryState(Border border, bool isActive)
    {
        border.BackgroundColor = isActive ? Color.FromArgb("#FFF4DD") : Color.FromArgb("#FFF6EE");
        border.Stroke = isActive ? Color.FromArgb("#FFCD72") : Color.FromArgb("#F0D8C7");
        border.StrokeThickness = isActive ? 2 : 1.5;
    }

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

}

public sealed class WardrobeOption : INotifyPropertyChanged
{
    private bool _isSelected;

    public WardrobeOption(
        WardrobeCategory category,
        string title,
        string description,
        string previewImage,
        string? appliedImage,
        bool isSelected = false)
    {
        Category = category;
        Title = title;
        Description = description;
        PreviewImage = previewImage;
        AppliedImage = appliedImage;
        _isSelected = isSelected;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public WardrobeCategory Category { get; }

    public string Title { get; }

    public string Description { get; }

    public string PreviewImage { get; }

    public string? AppliedImage { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ActionText));
        }
    }

    public string ActionText => IsSelected
        ? "Выбрано"
        : AppliedImage is null
            ? "Снять"
            : Category == WardrobeCategory.Palette
                ? "Применить"
                : "Надеть";

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public enum WardrobeCategory
{
    Hat,
    Top,
    Palette
}
