using System;
using System.Collections.Generic;
using System.Data.SQLite;

public class Book
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int YearPublished { get; set; }

    public Book(string id, string title, string author, int yearPublished)
    {
        Id = id;
        Title = title;
        Author = author;
        YearPublished = yearPublished;
    }
}

public class BookManager
{
    private List<Book> books = new List<Book>();
    private int nextId = 1;
    private const string ConnectionString = "Data Source=books.db;Version=3;";

    public BookManager()
    {
        InitializeDatabase();
        LoadBooksFromDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    YearPublished INTEGER NOT NULL
                )";
            using (var command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    private void LoadBooksFromDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string selectQuery = "SELECT Id, Title, Author, YearPublished FROM Books";
            using (var command = new SQLiteCommand(selectQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var book = new Book(reader["Id"].ToString(), reader["Title"].ToString(), reader["Author"].ToString(), Convert.ToInt32(reader["YearPublished"]));
                    books.Add(book);
                    nextId = Math.Max(nextId, Convert.ToInt32(reader["Id"]) + 1);
                }
            }
        }
    }

    public void AddBook(string title, string author, int yearPublished)
    {
        var book = new Book(nextId.ToString(), title, author, yearPublished);
        books.Add(book);
        SaveBookToDatabase(book);
        nextId++;
    }

    private void SaveBookToDatabase(Book book)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO Books (Title, Author, YearPublished) VALUES (@Title, @Author, @YearPublished)";
            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Title", book.Title);
                command.Parameters.AddWithValue("@Author", book.Author);
                command.Parameters.AddWithValue("@YearPublished", book.YearPublished);
                command.ExecuteNonQuery();
            }
        }
    }

    public void RemoveBook(string bookId)
    {
        books.RemoveAll(b => b.Id == bookId);
        RemoveBookFromDatabase(bookId);
    }

    private void RemoveBookFromDatabase(string bookId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string deleteQuery = "DELETE FROM Books WHERE Id = @Id";
            using (var command = new SQLiteCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", bookId);
                command.ExecuteNonQuery();
            }
        }
    }

    public Book GetBook(string bookId)
    {
        return books.Find(b => b.Id == bookId);
    }

    public List<Book> GetAllBooks()
    {
        return new List<Book>(books);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BookManager manager = new BookManager();
        bool running = true;

        while (running)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Добавить книгу");
            Console.WriteLine("2. Найти книгу по ID");
            Console.WriteLine("3. Показать все книги");
            Console.WriteLine("4. Удалить книгу по ID");
            Console.WriteLine("5. Выход");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите название книги: ");
                    string title = Console.ReadLine();
                    Console.Write("Введите имя автора: ");
                    string author = Console.ReadLine();
                    Console.Write("Введите год публикации: ");
                    int yearPublished;
                    while (!int.TryParse(Console.ReadLine(), out yearPublished))
                    {
                        Console.Write("Пожалуйста, введите корректный год публикации: ");
                    }
                    manager.AddBook(title, author, yearPublished);
                    Console.WriteLine("Книга добавлена.");
                    break;

                case "2":
                    Console.Write("Введите ID книги для поиска: ");
                    string searchId = Console.ReadLine();
                    Book foundBook = manager.GetBook(searchId);
                    if (foundBook != null)
                    {
                        Console.WriteLine($"Найдена книга: {foundBook.Title} автор: {foundBook.Author} ({foundBook.YearPublished})");
                    }
                    else
                    {
                        Console.WriteLine($"Книга с ID '{searchId}' не найдена.");
                    }
                    break;

                case "3":
                    Console.WriteLine("Все книги:");
                    DisplayBooks(manager.GetAllBooks());
                    break;

                case "4":
                    Console.Write("Введите ID книги для удаления: ");
                    string removeId = Console.ReadLine();
                    manager.RemoveBook(removeId);
                    Console.WriteLine($"Книга с ID '{removeId}' удалена.");
                    break;

                case "5":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Некорректный выбор. Пожалуйста, выберите действие из меню.");
                    break;
            }
        }
    }

    private static void DisplayBooks(List<Book> books)
    {
        foreach (var book in books)
        {
            Console.WriteLine($"{book.Id}: {book.Title} автор: {book.Author} ({book.YearPublished})");
        }
    }
}
