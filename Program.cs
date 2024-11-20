using System;
using System.Collections.Generic;
using System.Linq;

public class Book
{
    public string Id { get; set; } // Уникальный идентификатор книги
    public string Title { get; set; } // Название книги
    public string Author { get; set; } // Автор книги
    public int YearPublished { get; set; } // Год издания

    public Book(string id, string title, string author, int yearPublished)
    {
        Id = id;
        Title = title;
        Author = author;
        YearPublished = yearPublished;
    }

    public override string ToString()
    {
        return $"{Title} by {Author} ({YearPublished}) [ID: {Id}]";
    }
}


public class Library
{
    private List<Book> Books { get; set; } // Список книг в библиотеке

    public Library()
    {
        Books = new List<Book>();
    }

    // Метод для добавления книги в библиотеку
    public void AddBook(Book book)
    {
        Books.Add(book);
        Console.WriteLine($"Книга '{book.Title}' добавлена в библиотеку.");
    }

    // Метод для удаления книги по идентификатору
    public void RemoveBook(string bookId)
    {
        var bookToRemove = Books.FirstOrDefault(b => b.Id == bookId);
        if (bookToRemove != null)
        {
            Books.Remove(bookToRemove);
            Console.WriteLine($"Книга '{bookToRemove.Title}' удалена из библиотеки.");
        }
        else
        {
            Console.WriteLine("Книга с указанным идентификатором не найдена.");
        }
    }

    // Метод для получения книги по идентификатору
    public Book GetBook(string bookId)
    {
        return Books.FirstOrDefault(b => b.Id == bookId);
    }

    // Метод для получения списка всех книг
    public List<Book> GetAllBooks()
    {
        return Books;
    }
}


class Program
{
    static void Main(string[] args)
    {
        Library library = new Library();

        // Добавляем книги в библиотеку
        library.AddBook(new Book("1", "1984", "George Orwell", 1949));
        library.AddBook(new Book("2", "To Kill a Mockingbird", "Harper Lee", 1960));
        library.AddBook(new Book("3", "The Great Gatsby", "F. Scott Fitzgerald", 1925));

        // Получаем и выводим все книги
        Console.WriteLine("Все книги в библиотеке:");
        foreach (var book in library.GetAllBooks())
        {
            Console.WriteLine(book);
        }

        // Удаляем книгу
        library.RemoveBook("2");

        // Получаем и выводим книгу по идентификатору
        var bookById = library.GetBook("1");
        if (bookById != null)
        {
            Console.WriteLine($"Найдена книга: {bookById}");
        }
        else
        {
            Console.WriteLine("Книга не найдена.");
        }
    }
}

