using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;

namespace Restaurant
{
    public static class AppConfig
    {
        private static Dictionary<string, string> _config = new();

        public static string BundleDiscount => Get("BundleDiscount");
        public static string LowStockMultiplier => Get("LowStockMultiplier");
        public static string DeliveryFee => Get("DeliveryFee");
        public static string MinOrderForFreeDelivery => Get("MinOrderForFreeDelivery");
        public static string MinOrderForValueDiscount => Get("MinOrderForValueDiscount");
        public static string ValueDiscountPercentage => Get("ValueDiscountPercentage");
        public static string MinOrdersInTimeframeForDiscount => Get("MinOrdersInTimeframeForDiscount");
        public static string TimeframeForOrderCountDiscount => Get("TimeframeForOrderCountDiscount");
        public static string OrderCountDiscountPercentage => Get("OrderCountDiscountPercentage");

        static AppConfig()
        {
            var lines = File.ReadAllLines("config.txt");
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("="))
                    continue;

                var parts = line.Split('=', 2);
                _config[parts[0].Trim()] = parts[1].Trim();
            }
        }

        private static string Get(string key)
        {
            if (_config.TryGetValue(key, out var value))
                return value;
            throw new KeyNotFoundException($"Config key '{key}' not found.");
        }
    }
}
