using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PieLine
{
    public partial class MainWindow : Window
    {
        // Dynamic data
        public ObservableCollection<MenuGroup> MenuGroups { get; } = new();
        private List<MenuItem> _allMenuItems = new();

        // Simple cart
        public ObservableCollection<MenuItem> CartItems { get; } = new();

        // Build-your-own pizza state
        private string _buildSize;
        private string _buildSauce;
        private string _buildCrust;
        private readonly HashSet<string> _buildToppings =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, decimal> _buildSizePrices =
            new()
            {
                { "Small", 8.99m },
                { "Medium", 10.99m },
                { "Large", 12.99m },
                { "ExtraLarge", 14.99m }
            };

        private const decimal BuildToppingPrice = 1.00m;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            Loaded += MainWindow_Loaded;

            CartItems.CollectionChanged += (s, e) => UpdateCartTotal();
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            _allMenuItems = MenuFile.LoadMenuItems();
            RefreshGroups(null);
            UpdateBuildPizzaSummary();
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
            // Reset state when opening
            _buildSize = null;
            _buildSauce = null;
            _buildCrust = null;
            _buildToppings.Clear();
            UpdateBuildPizzaSummary();

            var overlay = GetNamedControl<Grid>("BuildPizzaOverlay");
            if (overlay != null)
                overlay.Visibility = Visibility.Visible;
        }

        private void BuildPizzaClose_Click(object sender, RoutedEventArgs e)
        {
            var overlay = GetNamedControl<Grid>("BuildPizzaOverlay");
            if (overlay != null)
                overlay.Visibility = Visibility.Collapsed;
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

            OpenCartSidebar();
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

                    btn.Foreground = isSelected
                        ? Brushes.White
                        : (Brush)FindResource("AccentRed");
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
                    btn.Foreground = (Brush)FindResource("AccentRed");
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
                        Foreground = Brushes.White
                    }
                };
                toppingsPanel.Children.Add(pill);
            }

            decimal total = 0m;
            if (_buildSize != null && _buildSizePrices.TryGetValue(_buildSize, out decimal basePrice))
                total += basePrice;
            total += _buildToppings.Count * BuildToppingPrice;

            totalText.Text = $"$ {total:0.00}";
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

        private void CartOverlayBackground_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CloseCartSidebar();
        }

        private void OpenCartSidebar()
        {
            var overlay = GetNamedControl<Grid>("CartSidebarOverlay");
            var transform = GetNamedControl<TranslateTransform>("CartSidebarTransform");
            if (overlay == null || transform == null)
                return;

            overlay.Visibility = Visibility.Visible;
            overlay.IsHitTestVisible = true;

            var anim = new DoubleAnimation
            {
                From = 360,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            transform.BeginAnimation(TranslateTransform.XProperty, anim);

            UpdateCartTotal();
        }

        private void CloseCartSidebar()
        {
            var overlay = GetNamedControl<Grid>("CartSidebarOverlay");
            var transform = GetNamedControl<TranslateTransform>("CartSidebarTransform");
            if (overlay == null || transform == null)
                return;

            var anim = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            anim.Completed += (s, e) =>
            {
                overlay.Visibility = Visibility.Collapsed;
                overlay.IsHitTestVisible = false;
            };

            transform.BeginAnimation(TranslateTransform.XProperty, anim);
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
    }
}
