using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace PieLine
{
    public partial class MainWindow : Window
    {
        public string LogoPath { get; set; } = "Images/logo.png";

        // Dynamic data
        public ObservableCollection<MenuGroup> MenuGroups { get; } = new ObservableCollection<MenuGroup>();
        private List<MenuItem> _allMenuItems = new List<MenuItem>();

        // Simple cart
        public ObservableCollection<MenuItem> CartItems { get; } = new ObservableCollection<MenuItem>();

        // Build-your-own pizza state (unchanged)
        private string _buildSize;
        private string _buildSauce;
        private string _buildCrust;
        private readonly HashSet<string> _buildToppings =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, decimal> _buildSizePrices =
            new Dictionary<string, decimal>
            {
                { "Small", 8.99m },
                { "Medium", 10.99m },
                { "Large", 12.99m },
                { "Extra Large", 14.99m }
            };

        private const decimal BuildToppingPrice = 1.00m;

        private const double SidebarWidth = 360.0; // keep consistent with XAML

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // Defer loading and initial UI update until the window is loaded so FindName can locate named controls
            this.Loaded += MainWindow_Loaded;

            // Update cart total when items change
            CartItems.CollectionChanged += (s, e) =>
            {
                UpdateCartTotal();
                UpdateCheckoutButtonState();
            };
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            // Load menu from JSON
            LoadMenuItemsFromJson();

            // Initialize builder summary after XAML names exist
            UpdateBuildPizzaSummary();

            UpdateCheckoutButtonState();

            // ensure overlay starts hidden
            var overlay = GetNamedControl<Border>("CartBackgroundOverlay");
            if (overlay != null)
            {
                overlay.Opacity = 0;
                overlay.Visibility = Visibility.Collapsed;
                //overlay.Width =0; // removed - overlay now covers full screen
                // ensure Width is not fixed to0 from earlier experiments
                overlay.ClearValue(FrameworkElement.WidthProperty);
            }
        }

        // Load menu items from MenuFile helper
        private void LoadMenuItemsFromJson()
        {
            try
            {
                var items = MenuFile.LoadMenuItems() ?? new List<MenuItem>();
                _allMenuItems = items;
                RefreshGroups(null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load menu items: {ex.Message}");
                _allMenuItems = new List<MenuItem>();
                RefreshGroups(null);
            }
        }

        // Helper to safely find named controls in XAML at runtime.
        private T? GetNamedControl<T>(string name) where T : class =>
            FindName(name) as T;

        // Rebuild MenuGroups from _allMenuItems applying optional filter
        private void RefreshGroups(string? query)
        {
            string? q = string.IsNullOrWhiteSpace(query) || query == "Search items"
                ? null
                : query.Trim().ToLowerInvariant();

            var filtered = string.IsNullOrEmpty(q)
                ? _allMenuItems
                : _allMenuItems.Where(m =>
                    (m.Name ?? "").ToLowerInvariant().Contains(q) ||
                    (m.Description ?? "").ToLowerInvariant().Contains(q) ||
                    (m.Category ?? "").ToLowerInvariant().Contains(q) ||
                    string.Join(" ", m.Tags ?? Enumerable.Empty<string>())
                          .ToLowerInvariant().Contains(q)
                  ).ToList();

            var groups = filtered
                .GroupBy(i => string.IsNullOrWhiteSpace(i.Category) ? "Uncategorized" : i.Category)
                // custom ordering: Pizza, Beverage, Dessert, then others alphabetically
                .OrderBy(g =>
                {
                    var priority = new[] { "Pizza", "Beverage", "Dessert" };
                    int idx = Array.IndexOf(priority, g.Key);
                    return idx == -1 ? int.MaxValue : idx;
                })
                .ThenBy(g => g.Key)
                .Select(g => new MenuGroup(g.Key)
                {
                    Items = new ObservableCollection<MenuItem>(g.OrderBy(i => i.Name))
                })
                .ToList();

            MenuGroups.Clear();
            foreach (var g in groups)
                MenuGroups.Add(g);
        }

        // ========= Search box handling =========

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Search items")
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = Brushes.Black;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search items";
                SearchTextBox.Foreground =
                    new SolidColorBrush(Color.FromRgb(136, 136, 136)); // #888
                RefreshGroups(null);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchTextBox.Text;
            if (text == "Search items" || string.IsNullOrWhiteSpace(text))
            {
                RefreshGroups(null);
                return;
            }

            RefreshGroups(text);
        }

        // Called by the "Add to Cart" button inside each dynamic card
        private void CardAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is MenuItem item)
            {
                CartItems.Add(item);
                OpenCartSidebar();
            }
        }

        // ========= Build Your Own Pizza overlay =========

        private void BuildYourOwnPizzaButton_Click(object sender, RoutedEventArgs e)
        {
            ResetBuildPizzaButtons();

            var overlay = GetNamedControl<Grid>("BuildPizzaOverlay");
            if (overlay != null)
                overlay.Visibility = Visibility.Visible;
        }

        private void BuildPizzaClose_Click(object sender, RoutedEventArgs e)
        {
            var overlay = GetNamedControl<Grid>("BuildPizzaOverlay");
            if (overlay != null)
                overlay.Visibility = Visibility.Collapsed;
            ResetBuildPizzaButtons();
        }

        private void BuildPizzaAddToCart_Click(object sender, RoutedEventArgs e)
        {
            decimal total = 0m;
            if (_buildSize != null && _buildSizePrices.TryGetValue(_buildSize, out decimal basePrice))
                total += basePrice;
            total += _buildToppings.Count * BuildToppingPrice;

            var builtItem = new MenuItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"Custom Pizza ({_buildSize ?? "?"})",
                Category = "Pizza",
                Description = string.Join(", ", _buildToppings.OrderBy(t => t)),
                Price = total
            };

            CartItems.Add(builtItem);

            var overlay = GetNamedControl<Grid>("BuildPizzaOverlay");
            if (overlay != null)
                overlay.Visibility = Visibility.Collapsed;

            ResetBuildPizzaButtons();
            OpenCartSidebar();
        }

        private void ResetBuildPizzaButtons()
        {
            _buildSize = null;
            _buildSauce = null;
            _buildCrust = null;
            _buildToppings.Clear();
            UpdateBuildPizzaSummary();

            void ResetGroup(string panelName)
            {
                var panel = GetNamedControl<Panel>(panelName);
                if (panel == null) return;

                foreach (var btn in panel.Children.OfType<Button>())
                {
                    btn.Background = Brushes.White;
                    btn.Foreground = Brushes.Black;
                }
            }

            ResetGroup("BuildPizzaSizeButtonsPanel");
            ResetGroup("BuildPizzaSauceButtonsPanel");
            ResetGroup("BuildPizzaCrustButtonsPanel");
            ResetGroup("VegToppingsButtonsPanel");
            ResetGroup("MeatToppingsButtonsPanel");
        }

        private void BuildPizzaSetExclusiveSelection(Panel parent, Button clicked)
        {
            foreach (var child in parent.Children)
            {
                if (child is Button btn)
                {
                    bool isSelected = (btn == clicked);
                    btn.Background = isSelected
                        ? (Brush)FindResource("AccentRed")
                        : Brushes.White;

                    btn.Foreground = isSelected ? Brushes.White : Brushes.Black;
                }
            }
        }

        private void BuildPizzaSizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                _buildSize = btn.Tag as string;
                BuildPizzaSetExclusiveSelection((Panel)btn.Parent, btn);
                UpdateBuildPizzaSummary();
            }
        }

        private void BuildPizzaSauceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                _buildSauce = btn.Tag as string;
                BuildPizzaSetExclusiveSelection((Panel)btn.Parent, btn);
                UpdateBuildPizzaSummary();
            }
        }

        private void BuildPizzaCrustButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                _buildCrust = btn.Tag as string;
                BuildPizzaSetExclusiveSelection((Panel)btn.Parent, btn);
                UpdateBuildPizzaSummary();
            }
        }

        private void BuildPizzaToppingButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string topping = (btn.Tag as string) ?? btn.Content.ToString();

                if (_buildToppings.Contains(topping))
                {
                    _buildToppings.Remove(topping);
                    btn.Background = Brushes.White;
                    btn.Foreground = Brushes.Black;
                }
                else
                {
                    _buildToppings.Add(topping);
                    btn.Background = (Brush)FindResource("AccentRed");
                    btn.Foreground = Brushes.White;
                }

                UpdateBuildPizzaSummary();
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CartItems.Any())
            {
                MessageBox.Show("Your cart is empty.", "Checkout", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var items = CartItems.ToList();

            var paymentWindow = new PaymentWindow(items);
            paymentWindow.Show();
            this.Close();
        }

        private void UpdateCheckoutButtonState()
        {
            var checkoutButton = GetNamedControl<Button>("CheckoutButton");
            if (checkoutButton == null)
                return;

            checkoutButton.IsEnabled = CartItems.Any();
        }

        private void UpdateBuildPizzaSummary()
        {
            var sizeText = GetNamedControl<TextBlock>("BuildPizzaSummarySizeText");
            var sauceText = GetNamedControl<TextBlock>("BuildPizzaSummarySauceText");
            var crustText = GetNamedControl<TextBlock>("BuildPizzaSummaryCrustText");
            var toppingsPanel = GetNamedControl<WrapPanel>("BuildPizzaSummaryToppingsPanel");
            var totalText = GetNamedControl<TextBlock>("BuildPizzaSummaryTotalText");

            if (sizeText == null || sauceText == null || crustText == null ||
                toppingsPanel == null || totalText == null)
                return; // UI not yet loaded

            sizeText.Text = _buildSize ?? "-";
            sauceText.Text = _buildSauce ?? "-";
            crustText.Text = _buildCrust ?? "-";

            toppingsPanel.Children.Clear();
            foreach (var topping in _buildToppings.OrderBy(t => t))
            {
                var pill = new Border
                {
                    Style = (Style)FindResource("ToppingPillStyle"),
                    Child = new TextBlock
                    {
                        Text = topping,
                        FontSize = 12,
                        Foreground = Brushes.Black
                    }
                };
                toppingsPanel.Children.Add(pill);
            }

            decimal total = 0m;
            if (_buildSize != null && _buildSizePrices.TryGetValue(_buildSize, out decimal basePrice))
                total += basePrice;
            total += _buildToppings.Count * BuildToppingPrice;

            totalText.Text = $"$ {total:0.00}";

            UpdateBuildPizzaAddButtonState();
        }

        private void UpdateBuildPizzaAddButtonState()
        {
            var addButton = GetNamedControl<Button>("BuildPizzaAddToCartButton");
            if (addButton == null)
                return;

            bool ready =
                !string.IsNullOrEmpty(_buildSize) &&
                !string.IsNullOrEmpty(_buildSauce) &&
                !string.IsNullOrEmpty(_buildCrust);

            addButton.IsEnabled = ready;
        }

        // ====== Cart Sidebar Controls ======
        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            OpenCartSidebar();
        }

        private void CartCloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseCartSidebar();
        }

        private void CartOverlayBackground_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CloseCartSidebar();
        }

        private void OpenCartSidebar()
        {
            var overlayGrid = GetNamedControl<Grid>("CartSidebarOverlay");
            var transform = GetNamedControl<TranslateTransform>("CartSidebarTransform");
            var overlay = GetNamedControl<Border>("CartBackgroundOverlay");
            if (overlayGrid == null || transform == null || overlay == null)
                return;

            overlayGrid.Visibility = Visibility.Visible;
            overlayGrid.IsHitTestVisible = true;

            // Animate sidebar in
            var sidebarAnim = new DoubleAnimation
            {
                From = SidebarWidth,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            transform.BeginAnimation(TranslateTransform.XProperty, sidebarAnim);

            // Ensure overlay will stretch (clear any fixed Width) and fade in (full-screen overlay)
            overlay.ClearValue(FrameworkElement.WidthProperty);
            overlay.Visibility = Visibility.Visible;
            var overlayFadeIn = new DoubleAnimation(0, 0.5, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            overlay.BeginAnimation(UIElement.OpacityProperty, overlayFadeIn);

            // Refresh total and ensure binding updates
            UpdateCartTotal();
        }

        private void CloseCartSidebar()
        {
            var overlayGrid = GetNamedControl<Grid>("CartSidebarOverlay");
            var transform = GetNamedControl<TranslateTransform>("CartSidebarTransform");
            var overlay = GetNamedControl<Border>("CartBackgroundOverlay");
            if (overlayGrid == null || transform == null || overlay == null)
                return;

            // Animate sidebar out
            var sidebarAnim = new DoubleAnimation
            {
                From = 0,
                To = SidebarWidth,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            // Fade overlay out (full-screen overlay)
            var overlayFadeOut = new DoubleAnimation(overlay.Opacity, 0, TimeSpan.FromMilliseconds(250))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            overlayFadeOut.Completed += (s, ev) =>
            {
                overlayGrid.Visibility = Visibility.Collapsed;
                overlayGrid.IsHitTestVisible = false;
                overlay.Opacity = 0;
                // reset Width so next open isn't constrained
                overlay.ClearValue(FrameworkElement.WidthProperty);
            };

            transform.BeginAnimation(TranslateTransform.XProperty, sidebarAnim);
            overlay.BeginAnimation(UIElement.OpacityProperty, overlayFadeOut);
        }

        private void CartRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is MenuItem item)
            {
                CartItems.Remove(item);
            }
        }

        private void UpdateCartTotal()
        {
            var totalText = GetNamedControl<TextBlock>("CartTotalText");
            if (totalText == null)
                return;

            decimal total = CartItems.Sum(i => i.Price);
            totalText.Text = $"$ {total:0.00}";
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var companyInfoWindow = new CompanyInformationWindow();
            companyInfoWindow.Show();
            this.Close();
        }
    }
}