
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

    private class NameComparer : IComparer<Book>
    {
        public int Compare(Book a, Book b)
        {
            return string.Compare(
                a.Name, b.Name,
                StringComparison.CurrentCulture
            );
        }
    }

    private class AuthorComparer : IComparer<Book>
    {
        public int Compare(Book a, Book b)
        {
            return string.Compare(
                a.Author, b.Author,
                StringComparison.CurrentCulture
            );
        }
    }

    private class GenreComparer : IComparer<Book>
    {
        public int Compare(Book a, Book b)
        {
            return string.Compare(
                a.Genre, b.Genre,
                StringComparison.CurrentCulture
            );
        }
    }

    private class DateComparer : IComparer<Book>
    {
        public int Compare(Book a, Book b)
        {
            return a.PublisDate.CompareTo(b.PublisDate);
        }
    }

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

    public void SortByName()
    {
        if (books.Count == 0)
        {
            return;
        }
        books.Sort(new NameComparer());
    }

    public void SortByAuthor()
    {
        if (books.Count == 0)
        {
            return;
        }
        books.Sort(new AuthorComparer());
    }

    public void SortByGenre()
    {
        if (books.Count == 0)
        {
            return;
        }
        books.Sort(new GenreComparer());
    }

    public void SortByDate()
    {
        if (books.Count == 0)
        {
            return;
        }
        books.Sort(new DateComparer());
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