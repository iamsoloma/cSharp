using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Select
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] menu = { "Пункт 1", "Пункт 2", "Пункт 3", "Пункт 4" };
            ConsoleKeyInfo q;
            int v = 0;
            Console.ForegroundColor = ConsoleColor.Green;

            while (true)
            {
                Console.Clear();
                for (int i = 0; i < menu.Length; i++)
                {
                    if (i == v) Console.Write("> ");
                    else Console.Write("  ");
                    Console.WriteLine(menu[i]);
                }

                q = Console.ReadKey();

                if (q.Key == ConsoleKey.UpArrow && v != 0) v--;
                if (q.Key == ConsoleKey.DownArrow && v != menu.Length - 1) v++;
                if (q.Key == ConsoleKey.Enter) // Обработка выбора пункта меню
                {
                    switch (v)
                    {
                        case 0:
                            Task1();
                            break;
                        case 1:
                            Task2();
                            break;
                        case 2:
                            Task3();
                            break;
                        case 3:
                            Task4();
                            break;
                    }
                }
                if (q.Key == ConsoleKey.Escape) // Выход по Escape
                {
                    break;
                }
            }
        }

        static void Task1()
        {
            Console.Clear();
            Console.WriteLine("Выполняется Задача 1");
            Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        static void Task2()
        {
            Console.Clear();
            Console.WriteLine("Выполняется Задача 2");
            Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        static void Task3()
        {
            Console.Clear();
            Console.WriteLine("Выполняется Задача 3");
            Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        static void Task4()
        {
            Console.Clear();
            Console.WriteLine("Выполняется Задача 4");
            Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }
}