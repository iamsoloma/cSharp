
using System;
using System.Collections.Generic;

namespace cSharp;

internal class Program
{
    static void Main(string[] args)
    {
        List<int> prices = new List<int> { 100, 250, 75 };
        List<string> labels = new List<string> { "тетрадь", "ручка", "папка" };

        prices.Add(50);
        labels.Add("замаска");

        for (int i = prices.Count - 1; i >= 0; i--)
        {
            Console.WriteLine($"{labels[i]}: {prices[i]}");
        }
    }
}