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

    private readonly Dictionary<WardrobeCategory, List<WardrobeItem>> _itemsByCategory;
    private readonly Dictionary<WardrobeCategory, WardrobeItem?> _equippedItems = [];

    private WardrobeCategory _activeCategory = WardrobeCategory.Hat;
    private string? _activeRabbitVariantImage;
    private string? _activeTopImage;
    private string? _activeHatImage;
    private string _activeCategoryTitle = HatsTitle;
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

    public bool HasTopImage => !string.IsNullOrWhiteSpace(ActiveTopImage);

    public bool HasHatImage => !string.IsNullOrWhiteSpace(ActiveHatImage);

    public bool IsAlternateRabbitVisible => !string.IsNullOrWhiteSpace(ActiveRabbitVariantImage);

    public string OverallsActionText => GetActionText("overalls_skin1_blue.png", WardrobeCategory.Top);

    public string TShirtActionText => GetActionText("t_shirt_mechmat.png", WardrobeCategory.Top);

    public string BeltActionText => GetActionText("belt_1_brown.png", WardrobeCategory.Top);

    public string PaletteActionText => GetActionText("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);

    public Color OverallsCardBackgroundColor => GetCardBackgroundColor("overalls_skin1_blue.png", WardrobeCategory.Top);
    public Color OverallsCardBorderColor => GetCardBorderColor("overalls_skin1_blue.png", WardrobeCategory.Top);
    public double OverallsCardBorderWidth => GetCardBorderWidth("overalls_skin1_blue.png", WardrobeCategory.Top);
    public Color OverallsImageBackgroundColor => GetImageBackgroundColor("overalls_skin1_blue.png", WardrobeCategory.Top);
    public Color OverallsImageBorderColor => GetImageBorderColor("overalls_skin1_blue.png", WardrobeCategory.Top);
    public Color OverallsButtonBackgroundColor => GetButtonBackgroundColor("overalls_skin1_blue.png", WardrobeCategory.Top);
    public Color OverallsButtonBorderColor => GetButtonBorderColor("overalls_skin1_blue.png", WardrobeCategory.Top);
    public Color OverallsButtonTextColor => GetButtonTextColor("overalls_skin1_blue.png", WardrobeCategory.Top);

    public Color TShirtCardBackgroundColor => GetCardBackgroundColor("t_shirt_mechmat.png", WardrobeCategory.Top);
    public Color TShirtCardBorderColor => GetCardBorderColor("t_shirt_mechmat.png", WardrobeCategory.Top);
    public double TShirtCardBorderWidth => GetCardBorderWidth("t_shirt_mechmat.png", WardrobeCategory.Top);
    public Color TShirtImageBackgroundColor => GetImageBackgroundColor("t_shirt_mechmat.png", WardrobeCategory.Top);
    public Color TShirtImageBorderColor => GetImageBorderColor("t_shirt_mechmat.png", WardrobeCategory.Top);
    public Color TShirtButtonBackgroundColor => GetButtonBackgroundColor("t_shirt_mechmat.png", WardrobeCategory.Top);
    public Color TShirtButtonBorderColor => GetButtonBorderColor("t_shirt_mechmat.png", WardrobeCategory.Top);
    public Color TShirtButtonTextColor => GetButtonTextColor("t_shirt_mechmat.png", WardrobeCategory.Top);

    public Color BeltCardBackgroundColor => GetCardBackgroundColor("belt_1_brown.png", WardrobeCategory.Top);
    public Color BeltCardBorderColor => GetCardBorderColor("belt_1_brown.png", WardrobeCategory.Top);
    public double BeltCardBorderWidth => GetCardBorderWidth("belt_1_brown.png", WardrobeCategory.Top);
    public Color BeltImageBackgroundColor => GetImageBackgroundColor("belt_1_brown.png", WardrobeCategory.Top);
    public Color BeltImageBorderColor => GetImageBorderColor("belt_1_brown.png", WardrobeCategory.Top);
    public Color BeltButtonBackgroundColor => GetButtonBackgroundColor("belt_1_brown.png", WardrobeCategory.Top);
    public Color BeltButtonBorderColor => GetButtonBorderColor("belt_1_brown.png", WardrobeCategory.Top);
    public Color BeltButtonTextColor => GetButtonTextColor("belt_1_brown.png", WardrobeCategory.Top);

    public Color PaletteCardBackgroundColor => GetCardBackgroundColor("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);
    public Color PaletteCardBorderColor => GetCardBorderColor("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);
    public double PaletteCardBorderWidth => GetCardBorderWidth("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);
    public Color PaletteImageBackgroundColor => GetImageBackgroundColor("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);
    public Color PaletteImageBorderColor => GetImageBorderColor("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);
    public Color PaletteButtonBackgroundColor => GetButtonBackgroundColor("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);
    public Color PaletteButtonBorderColor => GetButtonBorderColor("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);
    public Color PaletteButtonTextColor => GetButtonTextColor("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);

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
        SelectionPanel.Padding = width < 520 ? new Thickness(14, 16, 14, 14) : new Thickness(18);

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
            [WardrobeCategory.Hat] = [],
            [WardrobeCategory.Top] =
            [
                new WardrobeItem(WardrobeCategory.Top, "\u041A\u043E\u043C\u0431\u0438\u043D\u0435\u0437\u043E\u043D", "overalls_skin1_blue.png", "overalls_skin1_blue.png"),
                new WardrobeItem(WardrobeCategory.Top, "\u0424\u0443\u0442\u0431\u043E\u043B\u043A\u0430", "t_shirt_mechmat.png", "t_shirt_mechmat.png"),
                new WardrobeItem(WardrobeCategory.Top, "\u0420\u0435\u043C\u0435\u043D\u044C", "belt_1_brown.png", "belt_1_brown.png")
            ],
            [WardrobeCategory.Palette] =
            [
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

    private void OnSlotPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is not PointerGestureRecognizer { Parent: Border border } ||
            border.BindingContext is not WardrobeSlot slot ||
            slot.IsEquipped)
        {
            return;
        }

        border.Background = new SolidColorBrush(Color.FromArgb("#F3F3F3"));
        border.Stroke = Color.FromArgb("#D4C8BD");
        border.Scale = 1.02;
    }

    private void OnSlotPointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is not PointerGestureRecognizer { Parent: Border border } ||
            border.BindingContext is not WardrobeSlot slot)
        {
            return;
        }

        border.Background = new SolidColorBrush(slot.CardBackgroundColor);
        border.Stroke = slot.CardBorderColor;
        border.Scale = 1.0;
    }

    private void OnSlotActionClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: WardrobeSlot slot })
        {
            return;
        }

        ApplySlotSelection(slot);
        RefreshSlots();
    }

    private void OnOverallsClicked(object? sender, EventArgs e) => ToggleItem("overalls_skin1_blue.png", WardrobeCategory.Top);

    private void OnTShirtClicked(object? sender, EventArgs e) => ToggleItem("t_shirt_mechmat.png", WardrobeCategory.Top);

    private void OnBeltClicked(object? sender, EventArgs e) => ToggleItem("belt_1_brown.png", WardrobeCategory.Top);

    private void OnPaletteVariantClicked(object? sender, EventArgs e) => ToggleItem("rabbit_1_warm_bg_skintone.png", WardrobeCategory.Palette);

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

    private async Task OpenCategoryAsync(WardrobeCategory category)
    {
        _activeCategory = category;
        ActiveCategoryTitle = GetCategoryTitle(category);
        UpdateCategorySections();
        RefreshSlots();
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
        _equippedItems[WardrobeCategory.Palette] = null;

        _activeCategory = WardrobeCategory.Hat;
        ActiveCategoryTitle = GetCategoryTitle(_activeCategory);
        UpdateCategorySections();
        SelectionPanel.IsVisible = false;
        SelectionPanel.TranslationX = 360;
        _isSelectionPanelOpen = false;

        RefreshSlots();
        UpdateCategoryButtons();
        NotifyItemStateChanged();
    }

    private void RefreshSlots()
    {
        ActiveSlots.Clear();

        foreach (var item in _itemsByCategory[_activeCategory])
        {
            ActiveSlots.Add(WardrobeSlot.CreateItemSlot(item));
        }

        while (ActiveSlots.Count < PlaceholderSlotsPerCategory)
        {
            ActiveSlots.Add(WardrobeSlot.CreatePlaceholder(_activeCategory));
        }

        foreach (var slot in ActiveSlots)
        {
            slot.IsEquipped = slot.Kind == WardrobeSlotKind.Item && IsSelectedItemEquipped(slot.Item);
        }
    }

    private void ApplySlotSelection(WardrobeSlot slot)
    {
        if (slot.Kind != WardrobeSlotKind.Item || slot.Item is null)
        {
            return;
        }

        if (IsSelectedItemEquipped(slot.Item))
        {
            RemoveEquippedItem(slot.Item.Category);
        }
        else
        {
            EquipItem(slot.Item);
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
                ActiveRabbitVariantImage = item.AppliedImage;
                break;
        }

        NotifyItemStateChanged();
    }

    private void RemoveEquippedItem(WardrobeCategory category)
    {
        _equippedItems[category] = null;

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

        NotifyItemStateChanged();
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

    private void ToggleItem(string previewImage, WardrobeCategory category)
    {
        var item = _itemsByCategory[category].FirstOrDefault(x => x.PreviewImage == previewImage);
        if (item is null)
        {
            return;
        }

        if (IsSelectedItemEquipped(item))
        {
            RemoveEquippedItem(category);
        }
        else
        {
            EquipItem(item);
        }

        NotifyItemStateChanged();
    }

    private string GetActionText(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category) ? RemoveText : WearText;
    }

    private bool IsItemEquipped(string previewImage, WardrobeCategory category)
    {
        return _equippedItems.TryGetValue(category, out var equippedItem)
            && equippedItem?.PreviewImage == previewImage;
    }

    private Color GetCardBackgroundColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#E1B79D")
            : Color.FromArgb("#E8CCB8");
    }

    private Color GetCardBorderColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#AD7F66")
            : Color.FromArgb("#BF9277");
    }

    private double GetCardBorderWidth(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category) ? 2.0 : 1.6;
    }

    private Color GetImageBackgroundColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#E8D2C1")
            : Color.FromArgb("#EADBCF");
    }

    private Color GetImageBorderColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#C69C82")
            : Color.FromArgb("#D2B39F");
    }

    private Color GetButtonBackgroundColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#CFA488")
            : Color.FromArgb("#D8B39B");
    }

    private Color GetButtonBorderColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#A9785D")
            : Color.FromArgb("#BF9277");
    }

    private Color GetButtonTextColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#38231A")
            : Color.FromArgb("#4B3429");
    }

    private void NotifyItemStateChanged()
    {
        UpdateActionButtons();

        OnPropertyChanged(nameof(OverallsActionText));
        OnPropertyChanged(nameof(TShirtActionText));
        OnPropertyChanged(nameof(BeltActionText));
        OnPropertyChanged(nameof(PaletteActionText));

        OnPropertyChanged(nameof(OverallsCardBackgroundColor));
        OnPropertyChanged(nameof(OverallsCardBorderColor));
        OnPropertyChanged(nameof(OverallsCardBorderWidth));
        OnPropertyChanged(nameof(OverallsImageBackgroundColor));
        OnPropertyChanged(nameof(OverallsImageBorderColor));
        OnPropertyChanged(nameof(OverallsButtonBackgroundColor));
        OnPropertyChanged(nameof(OverallsButtonBorderColor));
        OnPropertyChanged(nameof(OverallsButtonTextColor));

        OnPropertyChanged(nameof(TShirtCardBackgroundColor));
        OnPropertyChanged(nameof(TShirtCardBorderColor));
        OnPropertyChanged(nameof(TShirtCardBorderWidth));
        OnPropertyChanged(nameof(TShirtImageBackgroundColor));
        OnPropertyChanged(nameof(TShirtImageBorderColor));
        OnPropertyChanged(nameof(TShirtButtonBackgroundColor));
        OnPropertyChanged(nameof(TShirtButtonBorderColor));
        OnPropertyChanged(nameof(TShirtButtonTextColor));

        OnPropertyChanged(nameof(BeltCardBackgroundColor));
        OnPropertyChanged(nameof(BeltCardBorderColor));
        OnPropertyChanged(nameof(BeltCardBorderWidth));
        OnPropertyChanged(nameof(BeltImageBackgroundColor));
        OnPropertyChanged(nameof(BeltImageBorderColor));
        OnPropertyChanged(nameof(BeltButtonBackgroundColor));
        OnPropertyChanged(nameof(BeltButtonBorderColor));
        OnPropertyChanged(nameof(BeltButtonTextColor));

        OnPropertyChanged(nameof(PaletteCardBackgroundColor));
        OnPropertyChanged(nameof(PaletteCardBorderColor));
        OnPropertyChanged(nameof(PaletteCardBorderWidth));
        OnPropertyChanged(nameof(PaletteImageBackgroundColor));
        OnPropertyChanged(nameof(PaletteImageBorderColor));
        OnPropertyChanged(nameof(PaletteButtonBackgroundColor));
        OnPropertyChanged(nameof(PaletteButtonBorderColor));
        OnPropertyChanged(nameof(PaletteButtonTextColor));
    }

    private void UpdateActionButtons()
    {
        ApplyActionButtonState(
            OverallsActionButton,
            OverallsActionText,
            OverallsButtonBackgroundColor,
            OverallsButtonBorderColor,
            OverallsButtonTextColor);

        ApplyActionButtonState(
            TShirtActionButton,
            TShirtActionText,
            TShirtButtonBackgroundColor,
            TShirtButtonBorderColor,
            TShirtButtonTextColor);

        ApplyActionButtonState(
            BeltActionButton,
            BeltActionText,
            BeltButtonBackgroundColor,
            BeltButtonBorderColor,
            BeltButtonTextColor);

        ApplyActionButtonState(
            PaletteActionButton,
            PaletteActionText,
            PaletteButtonBackgroundColor,
            PaletteButtonBorderColor,
            PaletteButtonTextColor);
    }

    private static void ApplyActionButtonState(Button? button, string text, Color backgroundColor, Color borderColor, Color textColor)
    {
        if (button is null)
        {
            return;
        }

        button.Text = text;
        button.BackgroundColor = backgroundColor;
        button.BorderColor = borderColor;
        button.TextColor = textColor;
    }

    private void UpdateCategorySections()
    {
        HatCardsSection.IsVisible = _activeCategory == WardrobeCategory.Hat;
        TopCardsSection.IsVisible = _activeCategory == WardrobeCategory.Top;
        PaletteCardsSection.IsVisible = _activeCategory == WardrobeCategory.Palette;
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
    private bool _isEquipped;

    private WardrobeSlot(WardrobeSlotKind kind, WardrobeCategory category, WardrobeItem? item)
    {
        Kind = kind;
        Category = category;
        Item = item;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public WardrobeSlotKind Kind { get; }

    public WardrobeCategory Category { get; }

    public WardrobeItem? Item { get; }

    public string? ImageSource => Item?.PreviewImage;

    public bool HasImage => !string.IsNullOrWhiteSpace(ImageSource);

    public bool IsActionVisible => Kind == WardrobeSlotKind.Item;

    public bool IsPlaceholderVisible => Kind == WardrobeSlotKind.Placeholder;

    public bool IsEquipped
    {
        get => _isEquipped;
        set
        {
            if (_isEquipped == value)
            {
                return;
            }

            _isEquipped = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ActionText));
            OnPropertyChanged(nameof(CardBackgroundColor));
            OnPropertyChanged(nameof(CardBorderColor));
            OnPropertyChanged(nameof(CardBorderWidth));
            OnPropertyChanged(nameof(ImageBackgroundColor));
            OnPropertyChanged(nameof(ImageBorderColor));
            OnPropertyChanged(nameof(ImageBorderWidth));
            OnPropertyChanged(nameof(ActionBackgroundColor));
            OnPropertyChanged(nameof(ActionBorderColor));
            OnPropertyChanged(nameof(ActionTextColor));
        }
    }

    public string ActionText => IsEquipped ? "\u0421\u043D\u044F\u0442\u044C" : "\u041D\u0430\u0434\u0435\u0442\u044C";

    public Color CardBackgroundColor => Kind switch
    {
        WardrobeSlotKind.Item when IsEquipped => Color.FromArgb("#F0F0F0"),
        WardrobeSlotKind.Item => Color.FromArgb("#FAFAFA"),
        WardrobeSlotKind.Placeholder => Color.FromArgb("#F1F1F1"),
        _ => Color.FromArgb("#FAFAFA")
    };

    public Color CardBorderColor => Kind switch
    {
        WardrobeSlotKind.Item when IsEquipped => Color.FromArgb("#D5C8BD"),
        WardrobeSlotKind.Placeholder => Color.FromArgb("#D9D9D9"),
        _ => Color.FromArgb("#E4DFDA")
    };

    public double CardBorderWidth => Kind == WardrobeSlotKind.Placeholder ? 1.4 : IsEquipped ? 1.8 : 1.2;

    public Color ImageBackgroundColor => Kind switch
    {
        WardrobeSlotKind.Item when IsEquipped => Color.FromArgb("#ECECEC"),
        WardrobeSlotKind.Placeholder => Color.FromArgb("#E9E9E9"),
        _ => Color.FromArgb("#F4F4F4")
    };

    public Color ImageBorderColor => Kind switch
    {
        WardrobeSlotKind.Item when IsEquipped => Color.FromArgb("#D9CDC2"),
        WardrobeSlotKind.Placeholder => Color.FromArgb("#DCDCDC"),
        _ => Color.FromArgb("#ECE7E2")
    };

    public double ImageBorderWidth => IsEquipped ? 1.2 : 1.0;

    public Color ActionBackgroundColor => Kind switch
    {
        WardrobeSlotKind.Item when IsEquipped => Color.FromArgb("#F7F7F7"),
        _ => Color.FromArgb("#FFFFFF")
    };

    public Color ActionBorderColor => Kind switch
    {
        WardrobeSlotKind.Item when IsEquipped => Color.FromArgb("#D7CBC0"),
        _ => Color.FromArgb("#E4E0DB")
    };

    public Color ActionTextColor => Kind switch
    {
        WardrobeSlotKind.Item when IsEquipped => Color.FromArgb("#4B3429"),
        _ => Color.FromArgb("#2C1F1A")
    };

    public static WardrobeSlot CreateItemSlot(WardrobeItem item) => new(WardrobeSlotKind.Item, item.Category, item);

    public static WardrobeSlot CreatePlaceholder(WardrobeCategory category) => new(WardrobeSlotKind.Placeholder, category, null);

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public enum WardrobeSlotKind
{
    Item,
    Placeholder
}

public enum WardrobeCategory
{
    Hat,
    Top,
    Palette
}
