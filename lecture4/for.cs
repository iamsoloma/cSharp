using System;
using System.Threading.Tasks;

namespace lecture4;

internal static class For
{
    private static void Main()
    {
        int[] numbers = { 120, 121, 127, 128, 129, 130 };
        int[] results = new int[numbers.Length];
        Parallel.For(0, numbers.Length, i =>
        {
            results[i] = CountDivisors(numbers[i]);
        });
        Console.WriteLine(string.Join(", ", results)); // 16, 3, 2, 8, 4, 8
    }


    private static int CountDivisors(int n)
    {
        int count = 0;
        for (int d = 1; d <= n; d++)
            if (n % d == 0) count++;
        return count;
    }
}