using System;
//using Newtonsoft.Json.Linq;

namespace cSharp;

internal class Program
{
    static void Main(string[] args)
    {

        Console.Write("Тебя зовут: ");
        string name = Console.ReadLine();

        Console.WriteLine($"{name}, здарова!");
        //Console.ReadKey();

        /*JObject o = JObject.FromObject(new
        {
            Name = name,
            Addres = "Мухосраново"
        });
        Console.WriteLine(o.ToString());*/
    }
}