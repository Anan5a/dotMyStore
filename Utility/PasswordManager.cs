namespace Utility
{
    using BCrypt.Net;

    public class PasswordManager
    {
        private readonly int cost = 12;
        public string HashPassword(string password)
        {
            return BCrypt.HashPassword(password, workFactor: cost);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Verify(password, hashedPassword);
        }
    }

}
