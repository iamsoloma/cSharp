
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
            displayDate = PublisDate.Year.ToString(); //PublisDate.ToString();
        }
        return $"{displayAuthor}[{displayDate} год]: {displayName} - {displayGenre}";
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
        Library library = new Library();
        string[] menu = { "Добавить книгу", "Удалить книгу", "Показать все книги", "Найти книгу", "Сортировать книги", "Выход" };

        int selected = 0;
        ConsoleKeyInfo key;

        while (true)
        {
            Console.Clear();
            for (int i = 0; i < menu.Length; i++)
            {
                if (i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("> " + menu[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(" " + menu[i]);
                }
            }
            Console.WriteLine("\n(Используйте стрелки ↑↓ для навигации, Enter - выбор, Esc - выход)");
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.UpArrow && selected > 0)
            {
                selected--;
            }

            if (key.Key == ConsoleKey.DownArrow && selected < menu.Length - 1)
            {
                selected++;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                switch (selected)
                {
                    case 0: AddBookMenu(library); break;
                    case 1: DeleteBookMenu(library); break;
                    case 2: ShowAllBooks(library); break;
                    case 3: FindBooksMenu(library); break;
                    case 4: SortBooksMenu(library); break;
                    case 5: return;
                }
            }
            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
        static void AddBookMenu(Library library)
        {
            Console.Clear();
            Console.WriteLine("--- Добавление книги ---");
            Console.Write("Введите название книги (обязательно): ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Введите автора (Enter - пропустить): ");
            string author = Console.ReadLine() ?? "";

            Console.Write("Введите год издания (Enter - пропустить): ");
            DateTime date = DateTime.MinValue;
            string dateInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(dateInput))
            {
                if (int.TryParse(dateInput, out int year)) date = new DateTime(year, 1, 1);
                else Console.WriteLine("Неверный формат года. Книга будет добавлена без даты.");
            }

            Console.Write("Введите жанр (Enter - пропустить): ");
            string genre = Console.ReadLine() ?? "";

            try
            {
                library.Add(new Book { Name = name, Author = author, PublisDate = date, Genre = genre });
                Console.WriteLine("\nКнига успешно добавлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
            }
            Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        static void DeleteBookMenu(Library library)
        {
            Console.Clear();
            Console.WriteLine("--- Удаление книги ---");
            Console.Write("Введите название книги для удаления: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("Введите автора книги для удаления: ");
            string author = Console.ReadLine() ?? "";

            library.Delete(name, author);
            Console.WriteLine("\nОперация удаления завершена.");
            Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        static void FindBooksMenu(Library library)
        {
            Console.Clear();
            Console.WriteLine("--- Поиск книг ---");
            Console.Write("Искать по названию (Enter - пропустить): ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Искать по автору (Enter - пропустить): ");
            string author = Console.ReadLine() ?? "";

            Console.Write("Искать по году (Enter - пропустить): ");
            DateTime date = DateTime.MinValue;
            string dateInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(dateInput) && int.TryParse(dateInput, out int year))
            {
                date = new DateTime(year, 1, 1);
            }

            Console.Write("Искать по жанру (Enter - пропустить): ");
            string genre = Console.ReadLine() ?? "";

            List<Book> found = library.Find(name, author, date, genre);

            Console.WriteLine($"\n--- Найдено книг: {found.Count} ---");
            foreach (var book in found)
            {
                Console.WriteLine(book.ToString());
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        static void SortBooksMenu(Library library)
        {
            Console.Clear();
            Console.WriteLine("--- Сортировка книг ---");
            Console.WriteLine("По какому полю сортировать?");
            Console.WriteLine("1. Название");
            Console.WriteLine("2. Автор");
            Console.WriteLine("3. Год");
            Console.WriteLine("4. Жанр");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1": library.SortByName(); break;
                case "2": library.SortByAuthor(); break;
                case "3": library.SortByDate(); break;
                case "4": library.SortByGenre(); break;
                default:
                    Console.WriteLine("\nНеверный ввод.");
                    Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
                    Console.ReadKey();
                    return;
            }
            Console.WriteLine("\nСортировка выполнена.");
            Console.WriteLine("Нажмите любую клавишу для просмотра списка...");
            Console.ReadKey();
            ShowAllBooks(library);
        }

        static void ShowAllBooks(Library library)
        {
            Console.Clear();
            Book[] allBooks = library.GetAllBooks();
            Console.WriteLine($"--- Всего книг в библиотеке: {allBooks.Length} ---\n");

            if (allBooks.Length == 0)
            {
                Console.WriteLine("Библиотека пуста.");
            }
            else
            {
                foreach (var book in allBooks) Console.WriteLine(book.ToString());
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }
}