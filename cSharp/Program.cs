using System;
//using Newtonsoft.Json.Linq;

namespace cSharp;

internal class Program
{
    static void Main(string[] args)
    {

        Console.Write("Тебя зовут: ");
        string name = Console.ReadLine() ?? "";
        if (name == "")
        {
            name = "Инкогнито";
        }

        Console.Write("А твой возраст?\n");
        string ageLine = Console.ReadLine() ?? "";
        int realAge;
        if (!int.TryParse(ageLine, out realAge))
        {
            Console.WriteLine("Ты ввёл не число.");
        }
        else
        {
            if (realAge < 0)
            {
                Console.WriteLine("Ты явно ошибся.");
            }
            else
            {
                Console.WriteLine($"Здравствуй, {name}.");
                if (realAge < 18)
                {
                    Console.WriteLine("Тебе запрещён алкоголь и энергетики!");
                }
                if (realAge < 13)
                {
                    Console.WriteLine("Тебе нельзя материться!");
                }
                if (realAge >= 18)
                {
                    Console.WriteLine("Ты совершенолетний.");
                }
                if (realAge > 35)
                {
                    Console.WriteLine("Тебе можно избираться в президенты.");
                }
            }
        }

    }
}