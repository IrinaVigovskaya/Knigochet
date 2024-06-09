using Coursework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Diagnostics;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Eventing.Reader;

namespace Coursework.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Users> users = new List<Users>();
            List<Books> books = new List<Books>();
            List<Taken> taken = new List<Taken>();

            string connectionString = @"Data Source = C:\Work\Coursework\Knigochet.sqlite";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query_for_users = "SELECT * FROM users";
                string query_for_books = "SELECT * FROM books";
                string query_for_takens = "SELECT * FROM takens";

                using (SQLiteCommand command_users = new SQLiteCommand(query_for_users, connection))
                using (SQLiteDataReader reader_users = command_users.ExecuteReader())
                {
                    while (reader_users.Read())
                    {
                        users.Add(new Users
                        {
                            Id = Convert.ToInt32(reader_users["Id"]),
                            Name = reader_users["Name"].ToString(),
                            Address = reader_users["Address"].ToString(),
                            Email = reader_users["Email"].ToString()
                        });
                    }
                }

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
                   
                        using (SQLiteCommand command_takens = new SQLiteCommand(query_for_takens, connection))
                        using (SQLiteDataReader reader_takens = command_takens.ExecuteReader())
                        {
                            while (reader_takens.Read())
                            {
                                string date_;
                                if (DateTime.Parse((string)reader_takens["ReturnDate"]) < DateTime.Now)
                                {
                                    date_ = "Просрочено с " + reader_takens["ReturnDate"].ToString();
                                }
                                else
                                {
                                    date_ = reader_takens["ReturnDate"].ToString();
                                }
                                taken.Add(new Taken
                                {
                                    Id = Convert.ToInt32(reader_takens["Id"]),
                                    Username = reader_takens["Username"].ToString(),
                                    BookTitle = reader_takens["BookTitle"].ToString(),
                                    ReturnDate = date_,
                                });
                            }
                        }
                return View((books, users, taken));
            }
        }

        [HttpPost]
        public IActionResult TakeBook(Taken taken)
        {
            if (ModelState.IsValid)
            {
                string connectionString = @"Data Source = C:\Work\Coursework\Knigochet.sqlite";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string dateString = taken.ReturnDate.ToString().Split(' ')[0].Replace("-", ".");
                    string query = "INSERT INTO takens (Username, BookTitle, ReturnDate) VALUES ('" + taken.Username + "', '" + taken.BookTitle + "', '" + dateString + "');";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            return Redirect("Index");

        }

        [HttpPost]
        public IActionResult Delete_Take(Taken taken)
        {
            string connectionString = @"Data Source = C:\Work\Coursework\Knigochet.sqlite";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM takens WHERE id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", taken.Id);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            return Redirect("Index");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}