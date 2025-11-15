using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PieLine
{
    public static class UserFile
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");

        public static List<User> LoadUsers()
        {
            if (!File.Exists(FilePath))
                return new List<User>();

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<User>();

            try
            {
                var users = JsonSerializer.Deserialize<List<User>>(json);
                return users ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        public static void SaveUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions{WriteIndented = true});
            File.WriteAllText(FilePath, json);
        }

        public static User? FindUserByPhone(string phoneNumberDigits)
        {
            var users = LoadUsers();
            return users.FirstOrDefault(u => u.PhoneNumber == phoneNumberDigits);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length <= 8)
                return false;

            bool hasDigit = password.Any(char.IsDigit);
            bool hasUpper = password.Any(char.IsUpper);

            return hasDigit && hasUpper;
        }

        public static bool TryAddUser(string fullName, string email, string phoneNumberDigits, string password, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                errorMessage = "Error: Invalid name, please enter a valid name, then try again.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(phoneNumberDigits) || phoneNumberDigits.Length != 10)
            {
                errorMessage = "Error: Invalid phone number, please enter a valid phone number then try again.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
            {
                errorMessage = "Error: Invalid email address, please enter a valid email address then try again.";
                return false;
            }

            if (!IsValidPassword(password))
            {
                errorMessage = "Error: Passwords must contain a number 0-9, capital letter A-Z, and be more then 8 characters.";
                return false;
            }

            var users = LoadUsers();
            if (users.Any(u => u.PhoneNumber == phoneNumberDigits || u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "Error: User already exists, this phone number or email have already been used.";
                return false;
            }

            users.Add(new User{FullName = fullName, Email = email, PhoneNumber = phoneNumberDigits, Password = password});

            SaveUsers(users);
            errorMessage = string.Empty;
            return true;
        }

        public static bool TryResetPassword(string phoneNumberDigits, string email, string newPassword, out string errorMessage)
        {
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.PhoneNumber == phoneNumberDigits);

            if (user == null)
            {
                errorMessage = "Error: Invalid phone number, please enter a valid phone number then try again.";
                return false;
            }

            if (!user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Error: Invalid email address, please enter a valid email address then try again.";
                return false;
            }

            if (!IsValidPassword(newPassword))
            {
                errorMessage = "Error: Passwords must contain a number 0-9, capital letter A-Z, and be more then 8 characters.";
                return false;
            }

            user.Password = newPassword;
            SaveUsers(users);

            errorMessage = string.Empty;
            return true;
        }
    }
}
