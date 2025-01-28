using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace PasswordManager
{
    public partial class PasswordManagerForm
    {
        // Validates the username to contain only alphanumeric characters and underscores
        private bool ValidateUsername(string username)
        {
            return Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
        }

        // Validates the password against various criteria
        private bool ValidatePassword(string password, string confirmationPassword)
        {
            if (password.Length < 8) return false;
            if (password == password.ToLower()) return false;
            if (password == password.ToUpper()) return false;
            if (password != confirmationPassword) return false;

            return true;
        }

        // Creates a random string based on specified criteria
        private string CreateRandomString()
        {
            int minLength = int.Parse(SettingsMinPasswordInput.Text);
            int maxLength = int.Parse(SettingsMaxPasswordInput.Text) + 1;
            int length = random.Next(minLength, maxLength);

            string validCharacters = "";


            if (SettingsIncludeBasicCheckbox.Checked)
            {
                validCharacters += "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }


            if (SettingsIncludeNumbersCheckbox.Checked)
            {
                validCharacters += "0123456789";
            }

            if (SettingsIncludeSpecialCheckbox.Checked)
            {
                validCharacters += "+-_-!?=:#%&/~*§<>()[]{}€$£@";
            }

            if (string.IsNullOrEmpty(validCharacters))
            {
                return "";
            }

            stringBuilder.Clear();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    stringBuilder.Append(validCharacters[(int)(num % (uint)validCharacters.Length)]);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
