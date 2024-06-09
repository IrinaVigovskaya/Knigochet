using Coursework.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace Coursework.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            List<Books> books = new List<Books>();

            string connectionString = @"Data Source = C:\Work\Coursework\Knigochet.sqlite";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query_for_books = "SELECT * FROM books";

                using (SQLiteCommand command_books = new SQLiteCommand(query_for_books, connection))
                using (SQLiteDataReader reader_books = command_books.ExecuteReader())
                {
                    while (reader_books.Read())
                    {
                        books.Add(new Books
                        {
                            Id = Convert.ToInt32(reader_books["Id"]),
                            Title = reader_books["Title"].ToString(),
                            Author = reader_books["Author"].ToString()
                        });
                    }
                }
                return View(books);
            }
        }

        [HttpPost]
        public IActionResult Delete_Book(Books book)
        {
            string connectionString = @"Data Source = C:\Work\Coursework\Knigochet.sqlite";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM books WHERE id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", book.Id);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            return Redirect("Index");

        }

            [HttpPost]
        public IActionResult Add_Book(Books book)
        {
            if (ModelState.IsValid)
            {
                string connectionString = @"Data Source = C:\Work\Coursework\Knigochet.sqlite";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                       connection.Open();

                      string query = "INSERT INTO books (title, author) VALUES ('" + book.Title + "', '" + book.Author + "');";

                      using (var command = new SQLiteCommand(query, connection))
                       {
                            command.ExecuteNonQuery();
                       }

                        connection.Close();
                    }

                    return Redirect("Index");
            }
            return View("Index");
        }
    }

}
