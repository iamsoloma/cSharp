using System;
using System.IO;
using System.Text;

internal class Program
{
    static void Main(string []args)
    {
        string source = "lesson_source.bin";
        string copy = "lesson_copy.bin";

        File.WriteAllBytes(source, Encoding.UTF8.GetBytes("Привет!"));

        using (var reader = new FileStream(source, FileMode.Open, FileAccess.Read))
        using (var writer = new FileStream(copy, FileMode.Create, FileAccess.Write))
        {
            byte[] buffer = new byte[5];
            int count;

            while ((count = reader.Read(buffer, 0, buffer.Length))>0)
            {
                writer.Write(buffer, 0, count);
            }
        }

        Console.WriteLine(
            Convert.ToHexString(File.ReadAllBytes(source)) ==  Convert.ToHexString(File.ReadAllBytes(copy))
        );
    }
}