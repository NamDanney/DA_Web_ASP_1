namespace DA_Web.Helpers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Hash password using BCrypt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Hashed password</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        /// <summary>
        /// Verify password against hash
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <param name="hashedPassword">Hashed password from database</param>
        /// <returns>True if password matches</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generate random password
        /// </summary>
        /// <param name="length">Password length</param>
        /// <returns>Random password</returns>
        public static string GenerateRandomPassword(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Validate password strength
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>List of validation errors</returns>
        public static List<string> ValidatePasswordStrength(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password is required");
                return errors;
            }

            if (password.Length < 6)
                errors.Add("Password must be at least 6 characters long");

            if (password.Length > 255)
                errors.Add("Password cannot exceed 255 characters");

            if (!password.Any(char.IsUpper))
                errors.Add("Password must contain at least one uppercase letter");

            if (!password.Any(char.IsLower))
                errors.Add("Password must contain at least one lowercase letter");

            if (!password.Any(char.IsDigit))
                errors.Add("Password must contain at least one number");

            return errors;
        }
    }
}