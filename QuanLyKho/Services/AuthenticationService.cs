using QuanLyKho.Models;

namespace QuanLyKho.Services
{
    public class AuthenticationService
    {
        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin";

        public bool Authenticate(string username, string password)
        {
            return username == AdminUsername && password == AdminPassword;
        }
    }
}
