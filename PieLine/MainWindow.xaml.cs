using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PieLine
{
    public partial class MainWindow : Window
    {
        // Menu cards for search filtering
        private readonly List<FrameworkElement> _menuCards = new List<FrameworkElement>();

        // Build-your-own pizza state
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

            // Register menu cards for search
            _menuCards.Add(CheeseCard);
            _menuCards.Add(PepperoniCard);
            _menuCards.Add(VeggieCard);

            _menuCards.Add(CokeCard);
            _menuCards.Add(SpriteCard);
            _menuCards.Add(FantaCard);

            _menuCards.Add(CookieCard);
            _menuCards.Add(CinnamonRollCard);
            _menuCards.Add(LavaCakeCard);

            // Initialize builder summary
            UpdateBuildPizzaSummary();
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
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchTextBox.Text;

            if (text == "Search items")
            {
                ShowAllCards();
                return;
            }

            string query = text.Trim().ToLower();

            if (string.IsNullOrEmpty(query))
            {
                ShowAllCards();
                return;
            }

            foreach (var card in _menuCards)
            {
                string tagText = (card.Tag as string) ?? "";
                bool match = tagText.ToLower().Contains(query);
                card.Visibility = match ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ShowAllCards()
        {
            foreach (var card in _menuCards)
            {
                card.Visibility = Visibility.Visible;
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

            BuildPizzaOverlay.Visibility = Visibility.Visible;
        }

        private void BuildPizzaClose_Click(object sender, RoutedEventArgs e)
        {
            BuildPizzaOverlay.Visibility = Visibility.Collapsed;
        }

        private void BuildPizzaAddToCart_Click(object sender, RoutedEventArgs e)
        {
            // TODO: add this built pizza to your cart data structure
            BuildPizzaOverlay.Visibility = Visibility.Collapsed;
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
            if (BuildPizzaSummarySizeText == null)
                return; // not loaded yet

            BuildPizzaSummarySizeText.Text = _buildSize ?? "-";
            BuildPizzaSummarySauceText.Text = _buildSauce ?? "-";
            BuildPizzaSummaryCrustText.Text = _buildCrust ?? "-";

            // toppings pills
            BuildPizzaSummaryToppingsPanel.Children.Clear();
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
                BuildPizzaSummaryToppingsPanel.Children.Add(pill);
            }

            // price
            decimal total = 0m;
            if (_buildSize != null && _buildSizePrices.TryGetValue(_buildSize, out decimal basePrice))
            {
                total += basePrice;
            }
            total += _buildToppings.Count * BuildToppingPrice;

            BuildPizzaSummaryTotalText.Text = $"$ {total:0.00}";
        }
    }
}
