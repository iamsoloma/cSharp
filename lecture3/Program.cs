
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualBasic;

namespace cSharp;

internal class Program
{
    static void Main(string[] args)
    {
       string path = "lesson3_stock.txt";
       using (var writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            writer.WriteLine("книга,3");
            writer.WriteLine("книга,3");
            writer.WriteLine("книга,3");
        }
    }
}