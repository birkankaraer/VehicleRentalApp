using VehicleRentalApp.Models;

namespace VehicleRentalApp.Data
{
    public class InMemoryUserStore
    {
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "admin", Password = "123456", Role = "Admin" },
            new User { Id = 2, Username = "user", Password = "123456", Role = "User" }
        };

        public User ValidateUser(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
