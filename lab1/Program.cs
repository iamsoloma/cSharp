
using System;
using System.Collections.Generic;

namespace cSharp;

public class Book
{
    public string Name { get; set; } //Обязательно
    public string Author { get; set; } //Необязательно
    public DateTime PublisDate { get; set; } //Необязательно
    public string Genre { get; set; } //Необязательно

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Name))
        {
            Name = "Инкогнито";
        }

        string PublishStroke;
        if (PublisDate == DateTime.MinValue)
        {
            PublishStroke = "когда-то";
        }
        else
        {
            PublishStroke = PublisDate.ToString();
        }

        if (string.IsNullOrEmpty(Genre))
        {
            Genre = "без жанра";
        }
        return $"{Author}[{PublishStroke}]: {Name} - {Genre}";
    }
}

public class Library
{
    private readonly List<Book> books = new List<Book>();
    public void Add(Book book)
    {
        if (book == null)
        {
            throw new ArgumentNullException(nameof(book));
        }

        if (string.IsNullOrEmpty(book.Name))
        {
            throw new ArgumentNullException(nameof(book));
        }

        books.Add(book);
    }

    public void Delete(string name, string author)
    {
        // циклом for надо перебарть все книги и удалить только при совпадении, иначе ничего.
    }

    public void Find(string name, string author, DateTime publishedAt, string genre)
    {
        //надо вернуть список без учёта регистра
    }
}

internal class Program
{
    static void Main(string[] args)
    {

        Console.Write("Привет мир!");

    }
}