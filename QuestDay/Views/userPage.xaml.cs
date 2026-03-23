using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QuestDay.Services;

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
    private const string OverallsImageName = "overalls_skin1_blue.png";
    private const string TShirtImageName = "t_shirt_mechmat.png";
    private const string OverallsAndBeltImageName = "overalls_and_balt.png";
    private const string DefaultPaletteImageName = "rabbit_1_warm_bg_skintone.png";
    private const string WhitePaletteImageName = "rabbit_3_white_bg_skintone.png";

    private readonly Dictionary<WardrobeCategory, List<WardrobeItem>> _itemsByCategory;
    private readonly Dictionary<WardrobeCategory, WardrobeItem?> _equippedItems = [];
    private readonly AvatarAppearanceService _avatarAppearance;

    private WardrobeCategory _activeCategory = WardrobeCategory.Hat;
    private string? _activeRabbitVariantImage;
    private string _activeRabbitCompositeImage = DefaultPaletteImageName;
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

    public string ActiveRabbitCompositeImage
    {
        get => _activeRabbitCompositeImage;
        set => SetProperty(ref _activeRabbitCompositeImage, value);
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

    public string OverallsActionText => GetActionText(OverallsImageName, WardrobeCategory.Top);

    public string TShirtActionText => GetActionText(TShirtImageName, WardrobeCategory.Top);

    public string OverallsAndBeltActionText => GetActionText(OverallsAndBeltImageName, WardrobeCategory.Top);

    public string PaletteActionText => GetActionText(DefaultPaletteImageName, WardrobeCategory.Palette);

    public string WhitePaletteActionText => GetActionText(WhitePaletteImageName, WardrobeCategory.Palette);

    public Color OverallsCardBackgroundColor => GetCardBackgroundColor(OverallsImageName, WardrobeCategory.Top);
    public Color OverallsCardBorderColor => GetCardBorderColor(OverallsImageName, WardrobeCategory.Top);
    public double OverallsCardBorderWidth => GetCardBorderWidth(OverallsImageName, WardrobeCategory.Top);
    public Color OverallsImageBackgroundColor => GetImageBackgroundColor(OverallsImageName, WardrobeCategory.Top);
    public Color OverallsImageBorderColor => GetImageBorderColor(OverallsImageName, WardrobeCategory.Top);
    public Color OverallsButtonBackgroundColor => GetButtonBackgroundColor(OverallsImageName, WardrobeCategory.Top);
    public Color OverallsButtonBorderColor => GetButtonBorderColor(OverallsImageName, WardrobeCategory.Top);
    public Color OverallsButtonTextColor => GetButtonTextColor(OverallsImageName, WardrobeCategory.Top);

    public Color TShirtCardBackgroundColor => GetCardBackgroundColor(TShirtImageName, WardrobeCategory.Top);
    public Color TShirtCardBorderColor => GetCardBorderColor(TShirtImageName, WardrobeCategory.Top);
    public double TShirtCardBorderWidth => GetCardBorderWidth(TShirtImageName, WardrobeCategory.Top);
    public Color TShirtImageBackgroundColor => GetImageBackgroundColor(TShirtImageName, WardrobeCategory.Top);
    public Color TShirtImageBorderColor => GetImageBorderColor(TShirtImageName, WardrobeCategory.Top);
    public Color TShirtButtonBackgroundColor => GetButtonBackgroundColor(TShirtImageName, WardrobeCategory.Top);
    public Color TShirtButtonBorderColor => GetButtonBorderColor(TShirtImageName, WardrobeCategory.Top);
    public Color TShirtButtonTextColor => GetButtonTextColor(TShirtImageName, WardrobeCategory.Top);

    public Color OverallsAndBeltCardBackgroundColor => GetCardBackgroundColor(OverallsAndBeltImageName, WardrobeCategory.Top);
    public Color OverallsAndBeltCardBorderColor => GetCardBorderColor(OverallsAndBeltImageName, WardrobeCategory.Top);
    public double OverallsAndBeltCardBorderWidth => GetCardBorderWidth(OverallsAndBeltImageName, WardrobeCategory.Top);
    public Color OverallsAndBeltImageBackgroundColor => GetImageBackgroundColor(OverallsAndBeltImageName, WardrobeCategory.Top);
    public Color OverallsAndBeltImageBorderColor => GetImageBorderColor(OverallsAndBeltImageName, WardrobeCategory.Top);
    public Color OverallsAndBeltButtonBackgroundColor => GetButtonBackgroundColor(OverallsAndBeltImageName, WardrobeCategory.Top);
    public Color OverallsAndBeltButtonBorderColor => GetButtonBorderColor(OverallsAndBeltImageName, WardrobeCategory.Top);
    public Color OverallsAndBeltButtonTextColor => GetButtonTextColor(OverallsAndBeltImageName, WardrobeCategory.Top);

    public Color PaletteCardBackgroundColor => GetCardBackgroundColor(DefaultPaletteImageName, WardrobeCategory.Palette);
    public Color PaletteCardBorderColor => GetCardBorderColor(DefaultPaletteImageName, WardrobeCategory.Palette);
    public double PaletteCardBorderWidth => GetCardBorderWidth(DefaultPaletteImageName, WardrobeCategory.Palette);
    public Color PaletteImageBackgroundColor => GetImageBackgroundColor(DefaultPaletteImageName, WardrobeCategory.Palette);
    public Color PaletteImageBorderColor => GetImageBorderColor(DefaultPaletteImageName, WardrobeCategory.Palette);
    public Color PaletteButtonBackgroundColor => GetButtonBackgroundColor(DefaultPaletteImageName, WardrobeCategory.Palette);
    public Color PaletteButtonBorderColor => GetButtonBorderColor(DefaultPaletteImageName, WardrobeCategory.Palette);
    public Color PaletteButtonTextColor => GetButtonTextColor(DefaultPaletteImageName, WardrobeCategory.Palette);

    public Color WhitePaletteCardBackgroundColor => GetCardBackgroundColor(WhitePaletteImageName, WardrobeCategory.Palette);
    public Color WhitePaletteCardBorderColor => GetCardBorderColor(WhitePaletteImageName, WardrobeCategory.Palette);
    public double WhitePaletteCardBorderWidth => GetCardBorderWidth(WhitePaletteImageName, WardrobeCategory.Palette);
    public Color WhitePaletteImageBackgroundColor => GetImageBackgroundColor(WhitePaletteImageName, WardrobeCategory.Palette);
    public Color WhitePaletteImageBorderColor => GetImageBorderColor(WhitePaletteImageName, WardrobeCategory.Palette);
    public Color WhitePaletteButtonBackgroundColor => GetButtonBackgroundColor(WhitePaletteImageName, WardrobeCategory.Palette);
    public Color WhitePaletteButtonBorderColor => GetButtonBorderColor(WhitePaletteImageName, WardrobeCategory.Palette);
    public Color WhitePaletteButtonTextColor => GetButtonTextColor(WhitePaletteImageName, WardrobeCategory.Palette);

    public userPage()
    {
        InitializeComponent();
        BindingContext = this;

        _avatarAppearance = App.AvatarAppearance;
        _avatarAppearance.PropertyChanged += OnAvatarAppearanceChanged;
        _itemsByCategory = CreateWardrobeItems();
        InitializeWardrobeState();
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
        SyncAvatarStateFromService();
        EnsurePaletteIsEquipped();
        RefreshSlots();
        NotifyItemStateChanged();
    }

    private Dictionary<WardrobeCategory, List<WardrobeItem>> CreateWardrobeItems()
    {
        return new Dictionary<WardrobeCategory, List<WardrobeItem>>
        {
            [WardrobeCategory.Hat] = [],
            [WardrobeCategory.Top] =
            [
                new WardrobeItem(WardrobeCategory.Top, "\u041A\u043E\u043C\u0431\u0438\u043D\u0435\u0437\u043E\u043D", OverallsImageName, OverallsImageName),
                new WardrobeItem(WardrobeCategory.Top, "\u0424\u0443\u0442\u0431\u043E\u043B\u043A\u0430", TShirtImageName, TShirtImageName),
                new WardrobeItem(WardrobeCategory.Top, "\u041A\u043E\u043C\u0431\u0438\u043D\u0435\u0437\u043E\u043D \u0441 \u0440\u0435\u043C\u043D\u0451\u043C", OverallsAndBeltImageName, OverallsAndBeltImageName)
            ],
            [WardrobeCategory.Palette] =
            [
                new WardrobeItem(WardrobeCategory.Palette, "\u0421\u0432\u0435\u0442\u043B\u044B\u0439", DefaultPaletteImageName, DefaultPaletteImageName),
                new WardrobeItem(WardrobeCategory.Palette, "\u0411\u0435\u043B\u044B\u0439", WhitePaletteImageName, WhitePaletteImageName)
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

    private void OnOverallsClicked(object? sender, EventArgs e) => ToggleItem(OverallsImageName, WardrobeCategory.Top);

    private void OnTShirtClicked(object? sender, EventArgs e) => ToggleItem(TShirtImageName, WardrobeCategory.Top);

    private void OnOverallsAndBeltClicked(object? sender, EventArgs e) => ToggleItem(OverallsAndBeltImageName, WardrobeCategory.Top);

    private void OnPaletteVariantClicked(object? sender, EventArgs e) => ToggleItem(DefaultPaletteImageName, WardrobeCategory.Palette);

    private void OnWhitePaletteVariantClicked(object? sender, EventArgs e) => ToggleItem(WhitePaletteImageName, WardrobeCategory.Palette);

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

    private void OnAvatarAppearanceChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(AvatarAppearanceService.TopImage)
            and not nameof(AvatarAppearanceService.HatImage)
            and not nameof(AvatarAppearanceService.RabbitVariantImage)
            and not nameof(AvatarAppearanceService.CurrentRabbitImage)
            and not nameof(AvatarAppearanceService.HasTopImage)
            and not nameof(AvatarAppearanceService.HasHatImage)
            and not nameof(AvatarAppearanceService.IsAlternateRabbitVisible))
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            SyncAvatarStateFromService();
            RefreshSlots();
            NotifyItemStateChanged();
        });
    }

    private void InitializeWardrobeState()
    {
        SyncAvatarStateFromService();
        EnsurePaletteIsEquipped();

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

    private void SyncAvatarStateFromService()
    {
        ActiveHatImage = _avatarAppearance.HatImage;
        ActiveTopImage = _avatarAppearance.TopImage;
        ActiveRabbitVariantImage = _avatarAppearance.RabbitVariantImage;
        ActiveRabbitCompositeImage = _avatarAppearance.CurrentRabbitImage;
        UpdateRabbitImages();

        _equippedItems[WardrobeCategory.Hat] = FindEquippedItem(WardrobeCategory.Hat, _avatarAppearance.HatImage);
        _equippedItems[WardrobeCategory.Top] = FindEquippedItem(WardrobeCategory.Top, _avatarAppearance.TopImage);
        _equippedItems[WardrobeCategory.Palette] = FindEquippedItem(WardrobeCategory.Palette, _avatarAppearance.RabbitVariantImage);
    }

    private WardrobeItem? FindEquippedItem(WardrobeCategory category, string? appliedImage)
    {
        if (string.IsNullOrWhiteSpace(appliedImage))
        {
            return null;
        }

        return _itemsByCategory[category].FirstOrDefault(item => item.AppliedImage == appliedImage);
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
                _avatarAppearance.HatImage = item.AppliedImage;
                break;
            case WardrobeCategory.Top:
                ActiveTopImage = item.AppliedImage;
                _avatarAppearance.TopImage = item.AppliedImage;
                break;
            case WardrobeCategory.Palette:
                ActiveRabbitVariantImage = item.AppliedImage;
                _avatarAppearance.RabbitVariantImage = item.AppliedImage;
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
                _avatarAppearance.HatImage = null;
                break;
            case WardrobeCategory.Top:
                ActiveTopImage = null;
                _avatarAppearance.TopImage = null;
                break;
            case WardrobeCategory.Palette:
                ActiveRabbitVariantImage = null;
                _avatarAppearance.RabbitVariantImage = null;
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
            if (category == WardrobeCategory.Palette)
            {
                EquipAlternatePalette(item);
            }
            else
            {
                RemoveEquippedItem(category);
            }
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
            ? Color.FromArgb("#E3C1AB")
            : Color.FromArgb("#E8CCB8");
    }

    private Color GetCardBorderColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#B6866C")
            : Color.FromArgb("#BF9277");
    }

    private double GetCardBorderWidth(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category) ? 2.0 : 1.6;
    }

    private Color GetImageBackgroundColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#D9D9D9")
            : Color.FromArgb("#E6E6E6");
    }

    private Color GetImageBorderColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#BFA797")
            : Color.FromArgb("#C9B4A5");
    }

    private Color GetButtonBackgroundColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#D5D3D0")
            : Color.FromArgb("#F6EEE8");
    }

    private Color GetButtonBorderColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#AAA59F")
            : Color.FromArgb("#BF9277");
    }

    private Color GetButtonTextColor(string previewImage, WardrobeCategory category)
    {
        return IsItemEquipped(previewImage, category)
            ? Color.FromArgb("#4E4A46")
            : Color.FromArgb("#4B3429");
    }

    private void NotifyItemStateChanged()
    {
        UpdateActionButtons();

        OnPropertyChanged(nameof(OverallsActionText));
        OnPropertyChanged(nameof(TShirtActionText));
        OnPropertyChanged(nameof(OverallsAndBeltActionText));
        OnPropertyChanged(nameof(PaletteActionText));
        OnPropertyChanged(nameof(WhitePaletteActionText));

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

        OnPropertyChanged(nameof(OverallsAndBeltCardBackgroundColor));
        OnPropertyChanged(nameof(OverallsAndBeltCardBorderColor));
        OnPropertyChanged(nameof(OverallsAndBeltCardBorderWidth));
        OnPropertyChanged(nameof(OverallsAndBeltImageBackgroundColor));
        OnPropertyChanged(nameof(OverallsAndBeltImageBorderColor));
        OnPropertyChanged(nameof(OverallsAndBeltButtonBackgroundColor));
        OnPropertyChanged(nameof(OverallsAndBeltButtonBorderColor));
        OnPropertyChanged(nameof(OverallsAndBeltButtonTextColor));

        OnPropertyChanged(nameof(PaletteCardBackgroundColor));
        OnPropertyChanged(nameof(PaletteCardBorderColor));
        OnPropertyChanged(nameof(PaletteCardBorderWidth));
        OnPropertyChanged(nameof(PaletteImageBackgroundColor));
        OnPropertyChanged(nameof(PaletteImageBorderColor));
        OnPropertyChanged(nameof(PaletteButtonBackgroundColor));
        OnPropertyChanged(nameof(PaletteButtonBorderColor));
        OnPropertyChanged(nameof(PaletteButtonTextColor));

        OnPropertyChanged(nameof(WhitePaletteCardBackgroundColor));
        OnPropertyChanged(nameof(WhitePaletteCardBorderColor));
        OnPropertyChanged(nameof(WhitePaletteCardBorderWidth));
        OnPropertyChanged(nameof(WhitePaletteImageBackgroundColor));
        OnPropertyChanged(nameof(WhitePaletteImageBorderColor));
        OnPropertyChanged(nameof(WhitePaletteButtonBackgroundColor));
        OnPropertyChanged(nameof(WhitePaletteButtonBorderColor));
        OnPropertyChanged(nameof(WhitePaletteButtonTextColor));
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
            OverallsAndBeltActionButton,
            OverallsAndBeltActionText,
            OverallsAndBeltButtonBackgroundColor,
            OverallsAndBeltButtonBorderColor,
            OverallsAndBeltButtonTextColor);

        ApplyActionButtonState(
            PaletteActionButton,
            PaletteActionText,
            PaletteButtonBackgroundColor,
            PaletteButtonBorderColor,
            PaletteButtonTextColor);

        ApplyActionButtonState(
            WhitePaletteActionButton,
            WhitePaletteActionText,
            WhitePaletteButtonBackgroundColor,
            WhitePaletteButtonBorderColor,
            WhitePaletteButtonTextColor);
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

    private void EnsurePaletteIsEquipped()
    {
        if (_equippedItems.TryGetValue(WardrobeCategory.Palette, out var equippedPalette) && equippedPalette is not null)
        {
            return;
        }

        var defaultPalette = _itemsByCategory[WardrobeCategory.Palette]
            .FirstOrDefault(item => item.PreviewImage == DefaultPaletteImageName);

        if (defaultPalette is not null)
        {
            EquipItem(defaultPalette);
        }
    }

    private void EquipAlternatePalette(WardrobeItem currentItem)
    {
        var alternatePalette = _itemsByCategory[WardrobeCategory.Palette]
            .FirstOrDefault(item => !string.Equals(item.PreviewImage, currentItem.PreviewImage, StringComparison.OrdinalIgnoreCase));

        if (alternatePalette is null)
        {
            return;
        }

        EquipItem(alternatePalette);
    }

    private void UpdateRabbitImages()
    {
        if (PreviewRabbitImage is not null)
        {
            PreviewRabbitImage.Source = ActiveRabbitCompositeImage;
        }

        if (PanelRabbitImage is not null)
        {
            PanelRabbitImage.Source = ActiveRabbitCompositeImage;
        }
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
