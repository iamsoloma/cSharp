
using System;
using System.Collections.Generic;
using System.Linq;

namespace cSharp;

public class Book
{
    public string Name { get; set; } //Обязательно
    public string Author { get; set; } //Необязательно
    public DateTime PublisDate { get; set; } //Необязательно
    public string Genre { get; set; } //Необязательно

    public override string ToString()
    {

        string displayAuthor = string.IsNullOrEmpty(Author) ? "Неизвестный автор" : Author;
        string displayName = string.IsNullOrEmpty(Name) ? "Инкогнито" : Name;
        string displayGenre = string.IsNullOrEmpty(Genre) ? "без жанра" : Genre;

        string displayDate;
        if (PublisDate == DateTime.MinValue)
        {
            displayDate = "когда-то";
        }
        else
        {
            displayDate = PublisDate.ToString();
        }
        return $"{displayAuthor}[{displayDate}]: {displayName} - {displayGenre}";
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
            throw new ArgumentNullException(nameof(book), "Название книги не может быть пустым!");
        }

        books.Add(book);
    }

    public void Delete(string name, string author)
    {
        // циклом for надо перебарть все книги и удалить только при совпадении, иначе ничего.
        for (int i = books.Count - 1; i >= 0; i--)
        {
            if (books[i].Name.ToLower() == name.ToLower() && books[i].Author.ToLower() == author.ToLower())
            {
                books.RemoveAt(i);
            }
        }
    }

    public List<Book> Find(string name, string author, DateTime publishedAt, string genre)
    {
        //надо вернуть список без учёта регистра
        List<Book> result = new List<Book>();
        foreach (var book in books)
        {
            bool matchName = string.IsNullOrEmpty(name) || (book.Name != null && book.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
            bool matchAuthor = string.IsNullOrEmpty(author) || (book.Author != null && book.Author.IndexOf(author, StringComparison.OrdinalIgnoreCase) >= 0);
            bool matchGenre = string.IsNullOrEmpty(genre) || (book.Genre != null && book.Genre.IndexOf(genre, StringComparison.OrdinalIgnoreCase) >= 0);
            bool matchDate = publishedAt == DateTime.MinValue || book.PublisDate == publishedAt;

            if (matchName && matchAuthor && matchGenre && matchDate)
            {
                result.Add(book);
            }
        }
        return result;
    }

    public Book[] GetAllBooks()
    {
        return books.ToArray();
    }
}

internal class Program
{
    static void Main(string[] args)
    {

        Console.Write("Привет мир!");

    }
}