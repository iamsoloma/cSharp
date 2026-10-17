using System;

namespace cSharp;

public class String
{
    private readonly char[] value;

    public int Length { get { return value.Length; } }

    public String()
    {
        value = Array.Empty<char>();
    }

    public String(string text)
    {
        value = (text ?? string.Empty).ToCharArray();
    }

    public String(char[] chars)
    {
        if (chars == null)
        {
            value = Array.Empty<char>();
        }
        else
        {
            value = (char[])chars.Clone();
        }
    }

    public String(char c)
    {
        value = new char[1];
        value[0] = c;
    }

    public static implicit operator String(string v)
    {
        return new String(v);
    }

    public override string ToString()
    {
        return new string(value);
    }

    //получение по индексу
    public char this[int i]
    {
        get
        {
            if (i < 0 || i >= value.Length)
            {
                throw new IndexOutOfRangeException($"Индекс {i} вне длины строки {value.Length - 1}.");
            }
            return value[i];
        }
    }

    //добавление в конец строки
    public static String operator +(String s, char c)
    {
        if (s is null)
        {
            return new String(c);
        }
        char[] result = new char[s.value.Length + 1];
        Array.Copy(s.value, result, s.value.Length);
        result[result.Length - 1] = c;

        return new String(result);
    }

    public static String operator +(char c, String s)
    {
        if (s == string.Empty)
        {
            return new String(c);
        }
        char[] result = new char[s.value.Length + 1];
        result[0] = c;
        Array.Copy(s.value, 0, result, 1, s.value.Length);

        return new String(result);
    }

    //сложение строк
    public static String operator +(String s1, String s2)
    {
        if (s1 == null && s2 == null)
        {
            return new String();
        }
        if (s1 == null)
        {
            return s2;
        }
        if (s2 == null)
        {
            return s1;
        }

        char[] result = new char[s1.Length + s2.Length];
        Array.Copy(s1.value, result, s1.Length);
        Array.Copy(s2.value, 0, result, s1.Length, s2.Length);
        return new String(result);
    }

    //сравнение

    public bool Compare(String comparable, int index, int lenght = 0)
    {

        if (index < 0 || comparable is null || index > value.Length)
        {
            return false;
        }

        if (lenght < 0)
        {
            return false;
        }

        if (lenght == 0)
        {
            lenght = comparable.Length;
        }

        if (lenght > comparable.Length || lenght > value.Length - index)
        {
            return false;
        }

        for (int i = 0; i < lenght; i++)
        {
            if (value[i + index] != comparable.value[i])
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        if (obj is not String v) return false;

        return v.Length == value.Length && Compare(v, 0);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (char c in value)
        {
            hash = hash * 31 + c.GetHashCode();
        }
        return hash;
    }


    //Операторы сравнения
    public static bool operator ==(String left, String right)
    {
        if (left is null && right is null)
        {
            return true;
        }
        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(String left, String right)
    {
        return !(left == right);
    }

}

internal class Program
{
    static void Main(string[] args)
    {
        String s1 = new String("");
        String s2 = new String("");
        string[] menu = {
            "1. Задать новые значения для s1 и s2",
            "2. Добавить символ в конец s1 (оператор +)",
            "3. Сложить s1 и s2 (оператор +)",
            "4. Получить символ из s1 по индексу (индексатор [])",
            "5. Сравнить s1 и s2 (операторы == и !=)",
            "6. Проверить метод Compare (сравнение подстрок)",
            "7. Выход"
        };

        ConsoleKeyInfo q;
        int v = 0;
        Console.ForegroundColor = ConsoleColor.Green;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Текущее состояние объектов ===");
            Console.WriteLine($"s1 = \"{s1}\" (Длина: {s1.Length})");
            Console.WriteLine($"s2 = \"{s2}\" (Длина: {s2.Length})");
            Console.WriteLine(new string('-', 40));

            for (int i = 0; i < menu.Length; i++)
            {
                if (i == v)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("> ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("  ");
                }
                Console.WriteLine(menu[i]);
            }
            Console.ForegroundColor = ConsoleColor.Green;

            q = Console.ReadKey();

            if (q.Key == ConsoleKey.UpArrow && v != 0) v--;
            if (q.Key == ConsoleKey.DownArrow && v != menu.Length - 1) v++;

            if (q.Key == ConsoleKey.Enter)
            {
                switch (v)
                {
                    case 0: TaskSetStrings(ref s1, ref s2); break;
                    case 1: TaskAddChar(ref s1); break;
                    case 2: TaskConcat(ref s1, s2); break;
                    case 3: TaskGetIndex(s1); break;
                    case 4: TaskCompareStrings(s1, s2); break;
                    case 5: TaskCompareSubstring(s1, s2); break;
                    case 6: return; // Выход из программы
                }
            }

            if (q.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }

    static void TaskSetStrings(ref String s1, ref String s2)
    {
        Console.Clear();
        Console.Write("Введите текст для s1: ");
        s1 = Console.ReadLine() ?? ""; // Неявное преобразование string в наш класс String
        Console.Write("Введите текст для s2: ");
        s2 = Console.ReadLine() ?? "";
        Console.WriteLine("Значения обновлены.");
        Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    static void TaskAddChar(ref String s1)
    {
        Console.Clear();
        Console.Write("Введите один символ для добавления в конец s1: ");
        string input = Console.ReadLine() ?? "";
        if (!string.IsNullOrEmpty(input))
        {
            s1 = s1 + input[0];
            Console.WriteLine($"Успешно! Теперь s1 = \"{s1}\"");
        }
        else
        {
            Console.WriteLine("Символ не был введен.");
        }
        Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    static void TaskConcat(ref String s1, String s2)
    {
        Console.Clear();
        s1 = s1 + s2;
        Console.WriteLine($"Сложение выполнено. Результат записан в s1: \"{s1}\"");
        Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    static void TaskGetIndex(String s1)
    {
        Console.Clear();
        Console.Write($"Введите индекс для строки s1 (0 - {s1.Length - 1}): ");
        if (int.TryParse(Console.ReadLine(), out int index))
        {
            try
            {
                char c = s1[index];
                Console.WriteLine($"Символ по индексу {index}: '{c}'");
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат индекса.");
        }
        Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    static void TaskCompareStrings(String s1, String s2)
    {
        Console.Clear();
        Console.WriteLine($"Результат (s1 == s2)   : {s1 == s2}");
        Console.WriteLine($"Результат (s1 != s2)   : {s1 != s2}");
        Console.WriteLine($"Результат s1.Equals(s2): {s1.Equals(s2)}");
        Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    static void TaskCompareSubstring(String s1, String s2)
    {
        Console.Clear();
        Console.Write("Введите индекс начала сравнения в s1: ");
        if (int.TryParse(Console.ReadLine(), out int index))
        {
            Console.Write("Введите длину сравнения (0 - вся длина s2): ");
            if (int.TryParse(Console.ReadLine(), out int length))
            {
                bool result = s1.Compare(s2, index, length);
                Console.WriteLine($"Результат Compare(s2, {index}, {length}): {result}");
            } else
            {
                Console.WriteLine("Неверный формат длины.");
            }
        } else
        {
            Console.WriteLine("Неверный формат индекса.");
        }
        Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }
}