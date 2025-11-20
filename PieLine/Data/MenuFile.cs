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
                ""image"": ""images/cheese.png""
                },
                {
                ""id"": ""pizza-pepperoni-001"",
                ""name"": ""Pepperoni Pizza"",
                ""category"": ""Pizza"",
                ""price"": 14.99,
                ""description"": ""Zesty tomato sauce, mozzarella cheese, and crisp slices of pepperoni."",
                ""tags"": [""Pepperoni"", ""Spicy""],
                ""image"": ""images/pepperoni.png""
                },
                {
                ""id"": ""drink-coke-001"",
                ""name"": ""Coke"",
                ""category"": ""Beverage"",
                ""price"": 1.99,
                ""description"": ""Chilled classic cola."",
                ""tags"": [""20 oz bottle""],
                ""image"": ""images/coke.png""
                }
            ]";
            var items = JsonSerializer.Deserialize<List<MenuItem>>(defaultJson, JsonOptions);
            return items ?? new List<MenuItem>();
        }
    }
}
