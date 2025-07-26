using System;
using System.Collections.Generic;
using System.IO;

namespace LibraryCatalog
{
    class Book
    {
        public string Title;
        public string Author;
        public bool IsAvailable;

        public Book(string title, string author, bool isAvailable)
        {
            Title = title;
            Author = author;
            IsAvailable = isAvailable;
        }

        public override string ToString()
        {
            return $"{Title} by {Author} - {(IsAvailable ? "Available" : "Borrowed")}";
        }

        public string ToFileString()
        {
            return $"{Title},{Author},{IsAvailable}";
        }

        public static Book FromFileString(string line)
        {
            var parts = line.Split(',');
            return new Book(parts[0], parts[1], bool.Parse(parts[2]));
        }
    }

    class Library
    {
        private List<Book> catalog;
        private string filePath = "library.txt";

        public Library()
        {
            catalog = new List<Book>();
            LoadFromFile();
        }

        // Load books from file
        private void LoadFromFile()
        {
            if (!File.Exists(filePath)) return;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    catalog.Add(Book.FromFileString(line));
                }
            }
        }

        // Save books to file
        private void SaveToFile()
        {
            List<string> lines = new List<string>();
            foreach (var book in catalog)
            {
                lines.Add(book.ToFileString());
            }
            File.WriteAllLines(filePath, lines);
        }

        // Display all books
        public void DisplayBooks()
        {
            Console.WriteLine("\nCatalog:");
            foreach (var book in catalog)
            {
                Console.WriteLine(book);
            }
        }

        // Search for books by title
        public void SearchBook(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Search term cannot be empty.");
                return;
            }

            Console.WriteLine($"\nSearch results for '{title}':");

            bool found = false;

            foreach (var book in catalog)
            {
                if (book.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine(book);
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No books found matching that title.");
            }
        }


        // Borrow a book
        public void BorrowBook()
        {
            Console.WriteLine("\nAvailable Books:");

            List<Book> availableBooks = new List<Book>();
            foreach (var book in catalog)
            {
                if (book.IsAvailable)
                {
                    Console.WriteLine(book);
                    availableBooks.Add(book);
                }
            }

            if (availableBooks.Count == 0)
            {
                Console.WriteLine("No boooks are currently available.");
                return;
            }

            Console.Write("\nEnter title to borrw: ");
            string title = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Please enter a title.");
                return;
            }

            for (int i = 0; i < catalog.Count; i++)
            {
                if (catalog[i].Title.Equals(title, StringComparison.OrdinalIgnoreCase) && catalog[i].IsAvailable)
                {
                    catalog[i].IsAvailable = false;
                    Console.WriteLine($"You borrowed '{catalog[i].Title}'.");
                    SaveToFile();
                    return;
                }
            }
            Console.WriteLine("Book not found or already borrowed.");
        }

        // Return a book
        public void ReturnBook(string title)
        {
            for (int i = 0; i < catalog.Count; i++)
            {
                if (catalog[i].Title.Equals(title, StringComparison.OrdinalIgnoreCase) && !catalog[i].IsAvailable)
                {
                    catalog[i].IsAvailable = true;
                    Console.WriteLine($"You returned '{catalog[i].Title}'");
                    SaveToFile();
                    return;
                }
            }
            Console.WriteLine("Book not found or already available.");
        }
    }

    class Program
    {
        static void Main()
        {
            Library library = new Library();

            while (true)
            {
                Console.WriteLine("\nLibrary Menu:");
                Console.WriteLine("1. View All Books");
                Console.WriteLine("2. Search Book");
                Console.WriteLine("3. Borrow Book");
                Console.WriteLine("4. Return Book");
                Console.WriteLine("5. Exit");
                Console.Write("Enter choice: ");
                
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        library.DisplayBooks();
                        break;
                    case "2":
                        Console.Write("Enter title to search: ");
                        string searchTitle = Console.ReadLine();
                        library.SearchBook(searchTitle);
                        break;
                    case "3":
                        library.BorrowBook();
                        break;
                    case "4":
                        Console.Write("Enter title to return: ");
                        string returnTitle = Console.ReadLine();
                        library.ReturnBook(returnTitle);
                        break;
                    case "5":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
    }
}
