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

namespace PieLine
{
    public partial class MainWindow : Window
    {
        // Dynamic data
        public ObservableCollection<MenuGroup> MenuGroups { get; } = new ObservableCollection<MenuGroup>();
        private List<FoodItem> _allMenuItems = new List<FoodItem>();

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
                { "ExtraLarge", 14.99m }
            };

        private const decimal BuildToppingPrice = 1.00m;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // Load menu from JSON
            LoadMenuItemsFromJson();

            // Initialize builder summary
            UpdateBuildPizzaSummary();
        }

        // Helper to safely find named controls in XAML at runtime.
        private T? GetNamedControl<T>(string name) where T : class
        {
            return this.FindName(name) as T;
        }

        private void LoadMenuItemsFromJson()
        {
            try
            {
                string outDir = AppContext.BaseDirectory;
                var candidates = new[]
                {
                    //Path.Combine(outDir, "menuitems.json"),
                    //Path.Combine(outDir, "Data", "menuitems.json"),
                    //Path.Combine(outDir, "data", "menuitems.json"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PieLine", "menuitems.json")
                };

                // development-time candidate (when running from IDE)
                var devCandidate = Path.GetFullPath(Path.Combine(outDir, "..", "..", "..", "Data", "menuitems.json"));
                var path = candidates.FirstOrDefault(File.Exists) ?? (File.Exists(devCandidate) ? devCandidate : null);

                if (path == null)
                {
                    Debug.WriteLine("menuitems.json not found in any candidate paths.");
                    return; // no file found — leave menu empty
                }

                Debug.WriteLine($"Loading menuitems.json from: {path}");

#if DEBUG
                // Temporary: show the exact runtime path in a debug-only popup so it's obvious where the file came from.
                MessageBox.Show($"Loading menuitems.json from:\n{path}", "Debug: menuitems.json path", MessageBoxButton.OK, MessageBoxImage.Information);
#endif

                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Debug.WriteLine($"menuitems.json found at {path} but file is empty. No items loaded.");
                    return; // empty file — don't attempt to deserialize
                }

                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var items = JsonSerializer.Deserialize<List<FoodItem>>(json, opts);
                if (items == null || items.Count == 0)
                {
                    Debug.WriteLine($"menuitems.json at {path} deserialized to no items.");
                    _allMenuItems = new List<FoodItem>();
                    RefreshGroups(null);
                    return;
                }

                _allMenuItems = items;
                RefreshGroups(null);
            }
            catch (JsonException jex)
            {
                MessageBox.Show($"menuitems.json contains invalid JSON: {jex.Message}", "JSON error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load menuitems.json: {ex.Message}", "Load error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Rebuild MenuGroups from _allMenuItems applying optional filter
        private void RefreshGroups(string query)
        {
            string q = string.IsNullOrWhiteSpace(query) || query == "Search items" ? null : query.Trim().ToLowerInvariant();

            var filtered = string.IsNullOrEmpty(q)
                ? _allMenuItems
                : _allMenuItems.Where(m =>
                    (m.Name ?? "").ToLowerInvariant().Contains(q)
                    || (m.Description ?? "").ToLowerInvariant().Contains(q)
                    || (m.Category ?? "").ToLowerInvariant().Contains(q)
                    || string.Join(" ", m.Tags ?? Array.Empty<string>()).ToLowerInvariant().Contains(q)
                  ).ToList();

            var groups = filtered
                .GroupBy(i => string.IsNullOrWhiteSpace(i.Category) ? "Uncategorized" : i.Category)
                // custom ordering: Pizza, Drink, Dessert, then others alphabetically
                .OrderBy(g =>
                {
                    var priority = new[] { "Pizza", "Drink", "Dessert" };
                    int idx = Array.IndexOf(priority, g.Key);
                    return idx == -1 ? int.MaxValue : idx;
                })
                .ThenBy(g => g.Key)
                .Select(g => new MenuGroup(g.Key) { Items = new ObservableCollection<FoodItem>(g.OrderBy(i => i.Name)) })
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
            if (sender is Button btn && btn.DataContext is FoodItem item)
            {
                // placeholder action — integrate with your cart later
                MessageBox.Show($"Added {item.Name} to cart (placeholder)", "Add to cart", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ========= Build Your Own Pizza overlay (existing unchanged logic) =========

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
            // TODO: add this built pizza to your cart data structure
            var overlay = GetNamedControl<Grid>("BuildPizzaOverlay");
            if (overlay != null)
                overlay.Visibility = Visibility.Collapsed;
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
                    // unselected
                    btn.Background = Brushes.White;
                    btn.Foreground = (Brush)FindResource("AccentRed");
                }
                else
                {
                    _buildToppings.Add(topping);
                    // selected
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

            if (sizeText == null || sauceText == null || crustText == null || toppingsPanel == null || totalText == null)
                return; // UI not yet loaded

            sizeText.Text = _buildSize ?? "-";
            sauceText.Text = _buildSauce ?? "-";
            crustText.Text = _buildCrust ?? "-";

            // toppings pills
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

            // price
            decimal total = 0m;
            if (_buildSize != null && _buildSizePrices.TryGetValue(_buildSize, out decimal basePrice))
            {
                total += basePrice;
            }
            total += _buildToppings.Count * BuildToppingPrice;

            totalText.Text = $"$ {total:0.00}";
        }
    }
}
