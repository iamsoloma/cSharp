using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace lab3;

public enum BuildingType
{
    Residential,
    NonResidential
}

[XmlInclude(typeof(ResidentialBuilding))]
[XmlInclude(typeof(NonResidentialBuilding))]
public abstract class Building
{
    public string Address { get; set; }
    public abstract double AverageCountOfUsers { get; }

    protected Building() { }

    protected Building(string address)
    {
        Address = address;
    }
}

public class ResidentialBuilding : Building
{
    public int CountOfFlats { get; set; }
    public int RoomsPerFlat { get; set; }

    public ResidentialBuilding() { }

    public ResidentialBuilding(string address, int countOfFlats, int roomsPerFlat) : base(address)
    {
        CountOfFlats = countOfFlats;
        RoomsPerFlat = roomsPerFlat;
    }

    private const double K = 1.3;
    public override double AverageCountOfUsers
    {
        get { return K * CountOfFlats * RoomsPerFlat; }
    }
}

public class NonResidentialBuilding : Building
{
    public int Area { get; set; }

    public NonResidentialBuilding() { }

    public NonResidentialBuilding(string address, int area) : base(address)
    {
        Area = area;
    }

    private const double K = 0.2;
    public override double AverageCountOfUsers => K * Area;
}

public class ManagementCompany
{
    private readonly List<Building> buildings = new List<Building>();

    public double CompanyAverageCountOfUsers { get; private set; }

    public void Add(Building building)
    {
        if (building == null || string.IsNullOrEmpty(building.Address))
        {
            throw new ArgumentException("Неправильный ввод данных здания");
        }

        buildings.Add(building);

        if (buildings.Count > 0)
        {
            CompanyAverageCountOfUsers = 0;
            foreach (var b in buildings)
            {
                CompanyAverageCountOfUsers += b.AverageCountOfUsers;
            }

            CompanyAverageCountOfUsers = CompanyAverageCountOfUsers / buildings.Count;
        }
    }

    public IEnumerable<Building> GetBuildings() { return buildings; }

    private class BuildingComparer : IComparer<Building>
    {
        public int Compare(Building x, Building y)
        {
            if (x == null || y == null) return 0;

            int avgUsersComp = y.AverageCountOfUsers.CompareTo(x.AverageCountOfUsers);
            if (avgUsersComp != 0) return avgUsersComp;

            int typeX = x is ResidentialBuilding ? 0 : 1;
            int typeY = y is ResidentialBuilding ? 0 : 1;
            int typeCompare = typeX.CompareTo(typeY);
            if (typeCompare != 0) return typeCompare;

            return string.Compare(x.Address, y.Address);
        }
    }

    public void Sort()
    {
        buildings.Sort(new BuildingComparer());
    }


    public void ToXML(string filename)
    {
        var serializer = new XmlSerializer(typeof(List<Building>));

        using (var stream = File.Create(filename))
        {
            serializer.Serialize(stream, buildings);
            stream.Flush();
        }
    }

    public static ManagementCompany FromXML(string filename)
    {
        var managmentCompany = new ManagementCompany();
        var serializer = new XmlSerializer(typeof(List<Building>));

        using (var stream = File.OpenRead(filename))
        {
            var readedBuildings = serializer.Deserialize(stream) as IEnumerable<Building>;
            if (readedBuildings != null)
            {
                foreach (var b in readedBuildings)
                {
                    managmentCompany.Add(b);
                }
            }
        }

        return managmentCompany;
    }
}

internal class Program
{
    static ManagementCompany company = new ManagementCompany();

    static void Main(string[] args)
    {
        string[] menu = {
            "1. Добавить объект недвижимости (Ручной ввод)",
            "2. Упорядочить и вывести все объекты",
            "3. Вывести первые 3 объекта",
            "4. Вывести последние 4 адреса",
            "5. Показать среднее по компании",
            "6. Сохранить в XML",
            "7. Загрузить из XML",
            "8. Выход"
        };

        ConsoleKeyInfo q;
        int v = 0;

        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=== Управляющая компания ===");
            Console.ResetColor();

            for (int i = 0; i < menu.Length; i++)
            {
                if (i == v) Console.Write("> ");
                else Console.Write("  ");
                Console.WriteLine(menu[i]);
            }

            q = Console.ReadKey();

            if (q.Key == ConsoleKey.UpArrow && v != 0) v--;
            if (q.Key == ConsoleKey.DownArrow && v != menu.Length - 1) v++;
            if (q.Key == ConsoleKey.Enter)
            {
                Console.Clear();
                switch (v)
                {
                    case 0: TaskAddBuilding(); break;
                    case 1: Task1(); break;
                    case 2: Task2(); break;
                    case 3: Task3(); break;
                    case 4: Task4(); break;
                    case 5: Task5(); break;
                    case 6: Task6(); break;
                    case 7: return;
                }

                if (v != 7) // Не ждем клавишу при выходе
                {
                    Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                    Console.ReadKey();
                }
            }
            if (q.Key == ConsoleKey.Escape) break;
        }
    }

    // Пункт ручного добавления
    static void TaskAddBuilding()
    {
        Console.WriteLine("=== Добавление нового объекта ===");
        Console.WriteLine("1. Жилое строение");
        Console.WriteLine("2. Нежилое строение");
        Console.Write("Выберите тип (1 или 2): ");

        string typeChoice = Console.ReadLine();

        if (typeChoice != "1" && typeChoice != "2")
        {
            Console.WriteLine("Неверный тип. Возврат в меню.");
            return;
        }

        Console.Write("Введите адрес: ");
        string address = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(address))
        {
            Console.WriteLine("Адрес не может быть пустым. Возврат в меню.");
            return;
        }

        try
        {
            if (typeChoice == "1")
            {
                int flats = ReadPositiveInt("Введите количество квартир: ");
                int rooms = ReadPositiveInt("Введите количество комнат в квартире (тип квартиры): ");

                var resBuilding = new ResidentialBuilding(address, flats, rooms);
                company.Add(resBuilding);
                Console.WriteLine($"\nУспешно добавлено жилое строение по адресу: {address}");
            }
            else
            {
                int area = ReadPositiveInt("Введите площадь строения: ");

                var nonResBuilding = new NonResidentialBuilding(address, area);
                company.Add(nonResBuilding);
                Console.WriteLine($"\nУспешно добавлено нежилое строение по адресу: {address}");
            }

            // Сразу показываем обновленное среднее по компании (демонстрация п.4)
            Console.WriteLine($"Текущее среднее количество жильцов/работников по компании: {company.CompanyAverageCountOfUsers:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении: {ex.Message}");
        }
    }

    static int ReadPositiveInt(string prompt)
    {
        int result;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (int.TryParse(input, out result) && result > 0)
            {
                return result;
            }
            Console.WriteLine("Ошибка! Введите целое число больше нуля.");
        }
    }

    static void Task1()
    {
        if (!CheckData()) return;

        company.Sort();
        Console.WriteLine("Тип строения\t| Адрес\t\t\t| Среднее кол-во");
        Console.WriteLine(new string('-', 60));

        foreach (var b in company.GetBuildings())
        {
            string type = b is ResidentialBuilding ? "Жилое" : "Нежилое";
            Console.WriteLine($"{type,-14} | {b.Address,-20} | {b.AverageCountOfUsers:F2}");
        }
    }

    static void Task2()
    {
        if (!CheckData()) return;

        company.Sort();
        Console.WriteLine("=== Первые 3 объекта ===");
        Console.WriteLine("Тип строения\t| Адрес\t\t\t| Среднее кол-во");
        Console.WriteLine(new string('-', 60));

        foreach (var b in company.GetBuildings().Take(3))
        {
            string type = b is ResidentialBuilding ? "Жилое" : "Нежилое";
            Console.WriteLine($"{type,-14} | {b.Address,-20} | {b.AverageCountOfUsers:F2}");
        }
    }

    static void Task3()
    {
        if (!CheckData()) return;

        company.Sort();
        var list = company.GetBuildings().ToList();
        int skipCount = Math.Max(0, list.Count - 4);

        Console.WriteLine("=== Последние 4 объекта ===");
        Console.WriteLine("Тип строения\t| Адрес\t\t\t| Среднее кол-во");
        Console.WriteLine(new string('-', 60));

        foreach (var b in list.Skip(skipCount))
        {
            string type = b is ResidentialBuilding ? "Жилое" : "Нежилое";
            Console.WriteLine($"{type,-14} | {b.Address,-20} | {b.AverageCountOfUsers:F2}");
        }
    }

    static void Task4()
    {
        if (!CheckData()) return;
        Console.WriteLine($"Среднее количество жильцов/работников по компании: {company.CompanyAverageCountOfUsers:F2}");
    }

    static void Task5()
    {
        if (!CheckData()) return;
        try
        {
            company.ToXML("buildings.xml");
            Console.WriteLine("Данные успешно сохранены в файл buildings.xml");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
        }
    }

    static void Task6()
    {
        try
        {
            company = ManagementCompany.FromXML("buildings.xml");
            Console.WriteLine("Данные успешно загружены из файла buildings.xml");
            Console.WriteLine($"Загружено объектов: {company.GetBuildings().Count()}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Ошибка: Файл buildings.xml не найден. Сначала сохраните данные.");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Ошибка: Некорректный формат XML-файла (файл поврежден или имеет неверную структуру).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Неизвестная ошибка при чтении: {ex.Message}");
        }
    }

    static bool CheckData()
    {
        if (!company.GetBuildings().Any())
        {
            Console.WriteLine("Список объектов пуст. Сначала добавьте данные (Пункт 1).");
            return false;
        }
        return true;
    }
}