using Coursework.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace Coursework.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            List<Users> users = new List<Users>();

            string connectionString = @"Data Source = C:\Work\Coursework\Knigochet.sqlite";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query_for_users = "SELECT * FROM users";

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
                return View(users);
            }
        }

        [HttpPost]
        public IActionResult Add_User(Users user)
        {
            if (ModelState.IsValid) {
                string connectionString = @"Data Source = C:\Work\Coursework\Knigochet.sqlite";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO users (name, address, email) VALUES ('" + user.Name + "', '" + user.Address + "', '" + user.Email + "');";

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

        [HttpPost]
        public IActionResult Delete_User(Users user)
        {
            string connectionString = @"Data Source = C:\Work\Coursework\Knigochet.sqlite";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM users WHERE id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            return Redirect("Index");

        }
    }


}
