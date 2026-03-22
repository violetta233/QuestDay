using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuestDay.Views;

public partial class userPage : ContentPage
{
    private const string DefaultRabbitBaseImage = "rabbit_warm.png";
    private const int PlaceholderSlotsPerCategory = 6;
    private const string HatsTitle = "\u0413\u043E\u043B\u043E\u0432\u043D\u044B\u0435 \u0443\u0431\u043E\u0440\u044B";
    private const string TopsTitle = "\u041E\u0434\u0435\u0436\u0434\u0430";
    private const string PaletteTitle = "\u041E\u043A\u0440\u0430\u0441\u043A\u0430";
    private const string WearText = "\u041D\u0430\u0434\u0435\u0442\u044C";
    private const string RemoveText = "\u0421\u043D\u044F\u0442\u044C";
    private const string EmptyText = "\u041F\u0443\u0441\u0442\u043E";

    private readonly Dictionary<WardrobeCategory, List<WardrobeItem>> _itemsByCategory;
    private readonly Dictionary<WardrobeCategory, WardrobeItem?> _equippedItems = [];

    private WardrobeCategory _activeCategory = WardrobeCategory.Hat;
    private WardrobeSlot? _selectedSlot;
    private string? _activeRabbitVariantImage;
    private string? _activeTopImage;
    private string? _activeHatImage;
    private string _activeCategoryTitle = HatsTitle;
    private string _actionButtonText = WearText;
    private bool _isSelectionPanelOpen;

    public ObservableCollection<WardrobeSlot> ActiveSlots { get; } = [];

    public string? ActiveRabbitVariantImage
    {
        get => _activeRabbitVariantImage;
        set
        {
            if (SetProperty(ref _activeRabbitVariantImage, value))
            {
                OnPropertyChanged(nameof(IsAlternateRabbitVisible));
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

    public string ActionButtonText
    {
        get => _actionButtonText;
        set => SetProperty(ref _actionButtonText, value);
    }

    public bool IsActionButtonEnabled => _selectedSlot is not null && _selectedSlot.Kind != WardrobeSlotKind.Placeholder;

    public bool HasTopImage => !string.IsNullOrWhiteSpace(ActiveTopImage);

    public bool HasHatImage => !string.IsNullOrWhiteSpace(ActiveHatImage);

    public bool IsAlternateRabbitVisible => !string.IsNullOrWhiteSpace(ActiveRabbitVariantImage);

    public userPage()
    {
        InitializeComponent();
        BindingContext = this;

        _itemsByCategory = CreateWardrobeItems();
        ResetWardrobeState();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width <= 0 || height <= 0)
        {
            return;
        }

        var leftRailWidth = width < 430 ? 150 : 168;
        var panelWidth = Math.Min(310, Math.Max(215, width - leftRailWidth - 44));
        SelectionPanel.WidthRequest = panelWidth;

        var isCompact = width < 430;
        RabbitPreview.Scale = isCompact ? 0.93 : 1.0;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ResetWardrobeState();
    }

    private Dictionary<WardrobeCategory, List<WardrobeItem>> CreateWardrobeItems()
    {
        return new Dictionary<WardrobeCategory, List<WardrobeItem>>
        {
            [WardrobeCategory.Hat] =
            [
                new WardrobeItem(WardrobeCategory.Hat, "\u0428\u043B\u044F\u043F\u0430", "headdress.png", "headdress.png")
            ],
            [WardrobeCategory.Top] =
            [
                new WardrobeItem(WardrobeCategory.Top, "\u041A\u043E\u043C\u0431\u0438\u043D\u0435\u0437\u043E\u043D", "overalls_skin1_blue.png", "overalls_skin1_blue.png"),
                new WardrobeItem(WardrobeCategory.Top, "\u0424\u0443\u0442\u0431\u043E\u043B\u043A\u0430", "t_shirt_mechmat.png", "t_shirt_mechmat.png"),
                new WardrobeItem(WardrobeCategory.Top, "\u0420\u0435\u043C\u0435\u043D\u044C", "belt_1_brown.png", "belt_1_brown.png")
            ],
            [WardrobeCategory.Palette] =
            [
                new WardrobeItem(WardrobeCategory.Palette, "\u0422\u0451\u043F\u043B\u044B\u0439", "rabbit_warm.png", "rabbit_warm.png"),
                new WardrobeItem(WardrobeCategory.Palette, "\u0421\u0432\u0435\u0442\u043B\u044B\u0439", "rabbit_1_warm_bg_skintone.png", "rabbit_1_warm_bg_skintone.png")
            ]
        };
    }

    private async void OnHatButtonClicked(object? sender, EventArgs e)
    {
        await OpenCategoryAsync(WardrobeCategory.Hat);
    }

    private async void OnHatButtonTapped(object? sender, TappedEventArgs e)
    {
        await OpenCategoryAsync(WardrobeCategory.Hat);
    }

    private async void OnTopButtonClicked(object? sender, EventArgs e)
    {
        await OpenCategoryAsync(WardrobeCategory.Top);
    }

    private async void OnTopButtonTapped(object? sender, TappedEventArgs e)
    {
        await OpenCategoryAsync(WardrobeCategory.Top);
    }

    private async void OnPaletteButtonClicked(object? sender, EventArgs e)
    {
        await OpenCategoryAsync(WardrobeCategory.Palette);
    }

    private async void OnPaletteButtonTapped(object? sender, TappedEventArgs e)
    {
        await OpenCategoryAsync(WardrobeCategory.Palette);
    }

    private void OnCategoryPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is not PointerGestureRecognizer { Parent: Border border })
        {
            return;
        }

        border.Background = new SolidColorBrush(Color.FromArgb("#FFF3E4"));
        border.Stroke = Color.FromArgb("#C9965A");
        border.StrokeThickness = 2.6;
        border.Scale = 1.03;
    }

    private void OnCategoryPointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is PointerGestureRecognizer { Parent: Border border })
        {
            border.Scale = 1.0;
        }

        UpdateCategoryButtons();
    }

    private async void OnBackToPreviewClicked(object? sender, EventArgs e)
    {
        await CloseSelectionPanelAsync();
    }

    private void OnSlotClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: WardrobeSlot slot })
        {
            return;
        }

        SelectSlot(slot, applyImmediately: true);
    }

    private void OnSlotTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Border { BindingContext: WardrobeSlot slot })
        {
            return;
        }

        SelectSlot(slot, applyImmediately: true);
    }

    private void OnSlotPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is not PointerGestureRecognizer { Parent: Border border } ||
            border.BindingContext is not WardrobeSlot slot ||
            slot.IsSelected)
        {
            return;
        }

        border.Background = new SolidColorBrush(Color.FromArgb("#FFF5EB"));
        border.Stroke = Color.FromArgb("#CFA98D");
        border.Scale = 1.03;
    }

    private void OnSlotPointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is not PointerGestureRecognizer { Parent: Border border } ||
            border.BindingContext is not WardrobeSlot slot)
        {
            return;
        }

        border.Background = new SolidColorBrush(slot.SlotBackgroundColor);
        border.Stroke = slot.SlotBorderColor;
        border.Scale = 1.0;
    }

    private void OnPanelButtonPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is not PointerGestureRecognizer { Parent: Button button } || !button.IsEnabled)
        {
            return;
        }

        button.BackgroundColor = Color.FromArgb("#E5C5B0");
        button.BorderColor = Color.FromArgb("#C7997D");
        button.TextColor = Color.FromArgb("#422B21");
        button.Scale = 1.03;
    }

    private void OnPanelButtonPointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is not PointerGestureRecognizer { Parent: Button button })
        {
            return;
        }

        if (button.IsEnabled)
        {
            button.BackgroundColor = Color.FromArgb("#D8B39B");
            button.BorderColor = Color.FromArgb("#BF9277");
            button.TextColor = Color.FromArgb("#4B3429");
        }

        button.Scale = 1.0;
    }

    private void OnActionButtonClicked(object? sender, EventArgs e)
    {
        if (_selectedSlot is null || _selectedSlot.Kind == WardrobeSlotKind.Placeholder)
        {
            return;
        }

        ApplySlotSelection(_selectedSlot);
        RefreshSlots();
    }

    private async Task OpenCategoryAsync(WardrobeCategory category)
    {
        _activeCategory = category;
        ActiveCategoryTitle = GetCategoryTitle(category);
        RefreshSlots(selectEquippedItem: true);
        UpdateCategoryButtons();

        if (_isSelectionPanelOpen)
        {
            return;
        }

        _isSelectionPanelOpen = true;
        SelectionPanel.TranslationX = 360;
        SelectionPanel.IsVisible = true;
        await SelectionPanel.TranslateTo(0, 0, 240, Easing.CubicOut);
    }

    private async Task CloseSelectionPanelAsync()
    {
        if (!_isSelectionPanelOpen)
        {
            return;
        }

        await SelectionPanel.TranslateTo(360, 0, 200, Easing.CubicIn);
        SelectionPanel.IsVisible = false;
        _isSelectionPanelOpen = false;
        UpdateCategoryButtons();
    }

    private void ResetWardrobeState()
    {
        ActiveRabbitVariantImage = null;
        ActiveTopImage = null;
        ActiveHatImage = null;

        _equippedItems[WardrobeCategory.Hat] = null;
        _equippedItems[WardrobeCategory.Top] = null;
        _equippedItems[WardrobeCategory.Palette] = _itemsByCategory[WardrobeCategory.Palette]
            .FirstOrDefault(item => item.AppliedImage == DefaultRabbitBaseImage);

        _activeCategory = WardrobeCategory.Hat;
        ActiveCategoryTitle = GetCategoryTitle(_activeCategory);
        _selectedSlot = null;
        ActionButtonText = WearText;
        OnPropertyChanged(nameof(IsActionButtonEnabled));

        SelectionPanel.IsVisible = false;
        SelectionPanel.TranslationX = 360;
        _isSelectionPanelOpen = false;

        RefreshSlots(selectEquippedItem: false);
        UpdateCategoryButtons();
    }

    private void RefreshSlots(bool selectEquippedItem = false)
    {
        ActiveSlots.Clear();

        var removeSlot = WardrobeSlot.CreateRemoveSlot(_activeCategory, RemoveText);
        ActiveSlots.Add(removeSlot);

        foreach (var item in _itemsByCategory[_activeCategory])
        {
            ActiveSlots.Add(WardrobeSlot.CreateItemSlot(item));
        }

        while (ActiveSlots.Count < PlaceholderSlotsPerCategory)
        {
            ActiveSlots.Add(WardrobeSlot.CreatePlaceholder(_activeCategory, EmptyText));
        }

        var slotToSelect = selectEquippedItem
            ? GetDefaultSlotForCategory()
            : _selectedSlot is not null
                ? FindMatchingSlot(_selectedSlot)
                : null;

        SelectSlot(slotToSelect ?? removeSlot, applyImmediately: false);
    }

    private WardrobeSlot GetDefaultSlotForCategory()
    {
        var equippedItem = _equippedItems[_activeCategory];
        if (equippedItem is null)
        {
            return ActiveSlots[0];
        }

        return ActiveSlots.First(slot => slot.Item == equippedItem);
    }

    private WardrobeSlot? FindMatchingSlot(WardrobeSlot sourceSlot)
    {
        if (sourceSlot.Kind == WardrobeSlotKind.Remove)
        {
            return ActiveSlots.FirstOrDefault(slot => slot.Kind == WardrobeSlotKind.Remove);
        }

        if (sourceSlot.Kind == WardrobeSlotKind.Item)
        {
            return ActiveSlots.FirstOrDefault(slot => slot.Item == sourceSlot.Item);
        }

        return ActiveSlots.FirstOrDefault(slot => slot.Kind == WardrobeSlotKind.Placeholder);
    }

    private void SelectSlot(WardrobeSlot slot, bool applyImmediately)
    {
        foreach (var existingSlot in ActiveSlots)
        {
            existingSlot.IsSelected = ReferenceEquals(existingSlot, slot);
        }

        _selectedSlot = slot;
        UpdateActionButtonState();
        OnPropertyChanged(nameof(IsActionButtonEnabled));

        if (applyImmediately)
        {
            ApplySlotSelection(slot);
            RefreshSlots();
        }
    }

    private void ApplySlotSelection(WardrobeSlot slot)
    {
        switch (slot.Kind)
        {
            case WardrobeSlotKind.Remove:
                RemoveEquippedItem(_activeCategory);
                break;
            case WardrobeSlotKind.Item when slot.Item is not null:
                if (IsSelectedItemEquipped(slot.Item))
                {
                    RemoveEquippedItem(slot.Item.Category);
                }
                else
                {
                    EquipItem(slot.Item);
                }
                break;
        }
    }

    private void EquipItem(WardrobeItem item)
    {
        _equippedItems[item.Category] = item;

        switch (item.Category)
        {
            case WardrobeCategory.Hat:
                ActiveHatImage = item.AppliedImage;
                break;
            case WardrobeCategory.Top:
                ActiveTopImage = item.AppliedImage;
                break;
            case WardrobeCategory.Palette:
                ActiveRabbitVariantImage = item.AppliedImage == DefaultRabbitBaseImage ? null : item.AppliedImage;
                break;
        }
    }

    private void RemoveEquippedItem(WardrobeCategory category)
    {
        _equippedItems[category] = category == WardrobeCategory.Palette
            ? _itemsByCategory[WardrobeCategory.Palette].FirstOrDefault(item => item.AppliedImage == DefaultRabbitBaseImage)
            : null;

        switch (category)
        {
            case WardrobeCategory.Hat:
                ActiveHatImage = null;
                break;
            case WardrobeCategory.Top:
                ActiveTopImage = null;
                break;
            case WardrobeCategory.Palette:
                ActiveRabbitVariantImage = null;
                break;
        }
    }

    private void UpdateActionButtonState()
    {
        ActionButtonText = _selectedSlot?.Kind switch
        {
            WardrobeSlotKind.Remove => RemoveText,
            WardrobeSlotKind.Item => IsSelectedItemEquipped(_selectedSlot.Item) ? RemoveText : WearText,
            _ => WearText
        };
    }

    private bool IsSelectedItemEquipped(WardrobeItem? item)
    {
        if (item is null)
        {
            return false;
        }

        return _equippedItems.TryGetValue(item.Category, out var equippedItem)
            && ReferenceEquals(equippedItem, item);
    }

    private void UpdateCategoryButtons()
    {
        ApplyCategoryState(HatCategoryButton, _activeCategory == WardrobeCategory.Hat, _isSelectionPanelOpen);
        ApplyCategoryState(TopCategoryButton, _activeCategory == WardrobeCategory.Top, _isSelectionPanelOpen);
        ApplyCategoryState(PaletteCategoryButton, _activeCategory == WardrobeCategory.Palette, _isSelectionPanelOpen);
    }

    private static string GetCategoryTitle(WardrobeCategory category) => category switch
    {
        WardrobeCategory.Hat => HatsTitle,
        WardrobeCategory.Top => TopsTitle,
        _ => PaletteTitle
    };

    private static void ApplyCategoryState(Border border, bool isActive, bool isPanelOpen)
    {
        border.Stroke = isActive && isPanelOpen ? Color.FromArgb("#C9965A") : Color.FromArgb("#D9BFAE");
        border.StrokeThickness = isActive && isPanelOpen ? 2.5 : 2;
        border.Background = new LinearGradientBrush(
            new GradientStopCollection
            {
                new GradientStop(Color.FromArgb("#FFFDF9"), 0.0f),
                new GradientStop(Color.FromArgb("#F7EBDD"), 1.0f)
            },
            new Point(0, 0),
            new Point(1, 1));
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

public sealed class WardrobeItem
{
    public WardrobeItem(WardrobeCategory category, string title, string previewImage, string? appliedImage)
    {
        Category = category;
        Title = title;
        PreviewImage = previewImage;
        AppliedImage = appliedImage;
    }

    public WardrobeCategory Category { get; }

    public string Title { get; }

    public string PreviewImage { get; }

    public string? AppliedImage { get; }
}

public sealed class WardrobeSlot : INotifyPropertyChanged
{
    private bool _isSelected;

    private WardrobeSlot(WardrobeSlotKind kind, WardrobeCategory category, WardrobeItem? item, string displayText)
    {
        Kind = kind;
        Category = category;
        Item = item;
        DisplayText = displayText;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public WardrobeSlotKind Kind { get; }

    public WardrobeCategory Category { get; }

    public WardrobeItem? Item { get; }

    public string DisplayText { get; }

    public string? PreviewImage => Kind == WardrobeSlotKind.Item ? Item?.PreviewImage : null;

    public bool HasPreviewImage => !string.IsNullOrWhiteSpace(PreviewImage);

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
            OnPropertyChanged(nameof(SlotBackgroundColor));
            OnPropertyChanged(nameof(SlotBorderColor));
            OnPropertyChanged(nameof(SlotBorderWidth));
            OnPropertyChanged(nameof(SlotTextColor));
        }
    }

    public Color SlotBackgroundColor => Kind switch
    {
        WardrobeSlotKind.Remove when IsSelected => Color.FromArgb("#FBE0D9"),
        WardrobeSlotKind.Remove => Color.FromArgb("#FFF8F1"),
        WardrobeSlotKind.Placeholder when IsSelected => Color.FromArgb("#F4E2D6"),
        WardrobeSlotKind.Placeholder => Color.FromArgb("#FAF4EE"),
        _ when IsSelected => Color.FromArgb("#FCE7C8"),
        _ => Color.FromArgb("#FFFDF9")
    };

    public Color SlotBorderColor => IsSelected ? Color.FromArgb("#C9965A") : Color.FromArgb("#D9BFAE");

    public double SlotBorderWidth => IsSelected ? 2 : 1.5;

    public Color SlotTextColor => Kind == WardrobeSlotKind.Placeholder
        ? Color.FromArgb("#A48C80")
        : Color.FromArgb("#4C382F");

    public static WardrobeSlot CreateItemSlot(WardrobeItem item) => new(WardrobeSlotKind.Item, item.Category, item, item.Title);

    public static WardrobeSlot CreateRemoveSlot(WardrobeCategory category, string displayText) => new(WardrobeSlotKind.Remove, category, null, displayText);

    public static WardrobeSlot CreatePlaceholder(WardrobeCategory category, string displayText) => new(WardrobeSlotKind.Placeholder, category, null, displayText);

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public enum WardrobeSlotKind
{
    Remove,
    Item,
    Placeholder
}

public enum WardrobeCategory
{
    Hat,
    Top,
    Palette
}
