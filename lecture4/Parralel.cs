using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace lecture4;

internal static class Parralel
{
    private static async Task Main()
    {
        //Паралельно
        Console.WriteLine("Паралельно");
        var clock = Stopwatch.StartNew();
        Task<int> book = GetPriceAsync("книга", 800, 100);
        Task<int> lamp = GetPriceAsync("лампа", 500, 200);
        Task<int> cup = GetPriceAsync("чашка", 300, 300);
        int[] prices = await Task.WhenAll(book, lamp, cup);
        Console.WriteLine($"Цены: {string.Join(", ", prices)}; сумма: {prices.Sum()}");
        Console.WriteLine($"Секунд: {clock.Elapsed.TotalSeconds:F2}");

        //Последовательно
        Console.WriteLine("Последовательно");
        clock = Stopwatch.StartNew();

        int b = await GetPriceAsync("книга", 800, 100);
        int l = await GetPriceAsync("лампа", 500, 200);
        int c = await GetPriceAsync("чашка", 300, 300);

        prices = [b, l, c];

        Console.WriteLine($"Цены: {string.Join(", ", prices)}; сумма: {prices.Sum()}");
        Console.WriteLine($"Секунд: {clock.Elapsed.TotalSeconds:F2}");
    }


    private static async Task<int> GetPriceAsync(string name, int delayMs, int price)
    {
        await Task.Delay(delayMs);
        Console.WriteLine($"Готово: {name}");
        return price;
    }
}