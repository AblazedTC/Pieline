using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PieLine
{
    public static class MenuFile
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menuitems.json");

        public static List<MenuItem> LoadMenuItems()
        {

            if (!File.Exists(FilePath))
            {
                var defaults = GetDefaultMenuItems();
                SaveMenuItems(defaults);
                return defaults;
            }

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                var defaults = GetDefaultMenuItems();
                SaveMenuItems(defaults);
                return defaults;
            }

            try
            {
                var items = JsonSerializer.Deserialize<List<MenuItem>>(json, JsonOptions);
                return items ?? new List<MenuItem>();
            }
            catch
            {
                var defaults = GetDefaultMenuItems();
                SaveMenuItems(defaults);
                return defaults;
            }
        }

        public static void SaveMenuItems(List<MenuItem> items)
        {
            string json = JsonSerializer.Serialize(items ?? new List<MenuItem>(), JsonOptions);
            File.WriteAllText(FilePath, json);
        }

        private static List<MenuItem> GetDefaultMenuItems()
        {
            const string defaultJson = @"[
                {
                ""id"": ""pizza-cheese-001"",
                ""name"": ""Cheese Pizza"",
                ""category"": ""Pizza"",
                ""price"": 12.99,
                ""description"": ""A classic cheese pizza topped with rich tomato sauce and melted mozzarella."",
                ""tags"": [""Mozzarella"", ""Vegetarian""],
                ""image"": ""Images/cheese.png""
                },
                {
                ""id"": ""pizza-pepperoni-001"",
                ""name"": ""Pepperoni Pizza"",
                ""category"": ""Pizza"",
                ""price"": 14.99,
                ""description"": ""Zesty tomato sauce, mozzarella cheese, and crisp slices of pepperoni."",
                ""tags"": [""Pepperoni"", ""Spicy""],
                ""image"": ""Images/pepperoni.png""
                },
                {
                ""Id"": ""pizza-veggie-001"",
                ""Name"": ""Veggie Pizza"",
                ""Category"": ""Pizza"",
                ""Price"": 14.99,
                ""Description"": ""A colorful medley of fresh bell peppers, onions, mushrooms, and olives atop our tomato sauce and mozzarella"",
                ""Tags"": [
                  ""Vegetarian"",
                  ""Mozzarella"",
                  ""Bell Peppers"",
                  ""Onions"",
                  ""Mushrooms"",
                  ""Olives""],
                ""Image"": ""Images/veggie.png""
                },
                {
                ""Id"": ""drink-Sprite-001"",
                ""Name"": ""Sprite"",
                ""Category"": ""Beverage"",
                ""Price"": 1.99,
                ""Description"": ""The classic lemon-lime soda."",
                ""Tags"": [
                    ""20 oz bottle""
                ],
                ""Image"": ""Images/sprite.png""
                },
                {
                ""Id"": ""drink-fanta-001"",
                ""Name"": ""Fanta"",
                ""Category"": ""Beverage"",
                ""Price"": 1.99,
                ""Description"": ""A soft drink with a tingly fruity taste."",
                ""Tags"": [
                    ""20 oz bottle""
                ],
                ""Image"": ""Images/fanta.png""
                },
                {
                ""id"": ""drink-coke-001"",
                ""name"": ""Coke"",
                ""category"": ""Beverage"",
                ""price"": 1.99,
                ""description"": ""Chilled classic cola."",
                ""tags"": [""20 oz bottle""],
                ""image"": ""Images/coke.png""
                },
                {
                ""Id"": ""dessert-chocchunkcookie-001"",
                ""Name"": ""Chocolate Chunk Cookie"",
                ""Category"": ""Dessert"",
                ""Price"": 6.99,
                ""Description"": ""A gooey chocolatey dessert"",
                ""Tags"": [
                    ""Cookie"",
                    ""Sweet"",
                    ""Chocolate""
                ],
                ""Image"": ""Images/ccc.png""
                },
                {
                ""Id"": ""dessert-cinnamonroll-001"",
                ""Name"": ""Cinnamon Roll"",
                ""Category"": ""Dessert"",
                ""Price"": 6.99,
                ""Description"": ""A gooey chocolatey dessert"",
                ""Tags"": [
                    ""Baked"",
                    ""Cinnamon""
                ],
                ""Image"": ""Images/cinnamonroll.png""
                },
                {
                ""Id"": ""dessert-lavacake-001"",
                ""Name"": ""Molten Lava Cake"",
                ""Category"": ""Dessert"",
                ""Price"": 6.99,
                ""Description"": ""A gooey chocolatey dessert"",
                ""Tags"": [
                    ""Chocolate"",
                    ""Cake"",
                    ""Baked""
                ],
                ""Image"": ""Images/lavacake.png""
                }
            ]";
            var items = JsonSerializer.Deserialize<List<MenuItem>>(defaultJson, JsonOptions);
            return items ?? new List<MenuItem>();
        }
    }
}
