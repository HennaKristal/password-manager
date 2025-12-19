using Encryption;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PasswordManager
{
    public partial class PasswordManagerForm
    {
        /*=====================================================*\
        |                      Settings UI                      |
        \*=====================================================*/

        private void SettingsUsernameInput_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SettingsUsernameInput.Text))
            {
                SettingsUsernameInput.Text = username;
            }
        }

        private void SettingsCurrentPasswordInput_Enter(object sender, EventArgs e)
        {
            if (SettingsCurrentPasswordInput.ForeColor == Color.Gray)
            {
                SettingsCurrentPasswordInput.Clear();
            }

            SettingsCurrentPasswordInput.PasswordChar = '*';
            SettingsCurrentPasswordInput.ForeColor = Color.White;
        }

        private void SettingsCurrentPasswordInput_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SettingsCurrentPasswordInput.Text))
            {
                SettingsCurrentPasswordInput.PasswordChar = '\0';
                SettingsCurrentPasswordInput.Text = "Current password";
                SettingsCurrentPasswordInput.ForeColor = Color.Gray;
            }
        }

        private void SettingsNewPasswordInput_Enter(object sender, EventArgs e)
        {
            if (SettingsNewPasswordInput.ForeColor == Color.Gray)
            {
                SettingsNewPasswordInput.Clear();
            }

            SettingsNewPasswordInput.PasswordChar = '*';
            SettingsNewPasswordInput.ForeColor = Color.White;
        }

        private void SettingsNewPasswordInput_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SettingsNewPasswordInput.Text))
            {
                SettingsNewPasswordInput.PasswordChar = '\0';
                SettingsNewPasswordInput.Text = "New password";
                SettingsNewPasswordInput.ForeColor = Color.Gray;
            }
        }

        private void SettingsConfirmPasswordInput_Enter(object sender, EventArgs e)
        {
            if (SettingsConfirmPasswordInput.ForeColor == Color.Gray)
            {
                SettingsConfirmPasswordInput.Clear();
            }

            SettingsConfirmPasswordInput.PasswordChar = '*';
            SettingsConfirmPasswordInput.ForeColor = Color.White;
        }

        private void SettingsConfirmPasswordInput_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SettingsConfirmPasswordInput.Text))
            {
                SettingsConfirmPasswordInput.PasswordChar = '\0';
                SettingsConfirmPasswordInput.Text = "Confirm new password";
                SettingsConfirmPasswordInput.ForeColor = Color.Gray;
            }
        }

        private void SettingsMinPasswordInput_Enter(object sender, EventArgs e)
        {
            if (SettingsMinPasswordInput.ForeColor == Color.Gray)
            {
                SettingsMinPasswordInput.Clear();
            }

            SettingsMinPasswordInput.ForeColor = Color.White;
        }

        private void SettingsMinPasswordInput_Leave(object sender, EventArgs e)
        {
            SettingsFeedback.Text = "";

            if (string.IsNullOrWhiteSpace(SettingsMinPasswordInput.Text))
            {
                SettingsMinPasswordInput.Text = (int.Parse(SettingsMaxPasswordInput.Text) < 16) ? SettingsMaxPasswordInput.Text : "16";
                SettingsMinPasswordInput.ForeColor = string.IsNullOrWhiteSpace(SettingsMaxPasswordInput.Text) || SettingsMaxPasswordInput.Text == "16" ? Color.Gray : Color.White;
            }
            else if (int.Parse(SettingsMinPasswordInput.Text) < 8)
            {
                SettingsMinPasswordInput.Text = "8";
                SettingsFeedback.Text = "Password minimum length cannot be lower than 8!";
            }

            if (int.Parse(SettingsMinPasswordInput.Text) > int.Parse(SettingsMaxPasswordInput.Text))
            {
                SettingsMinPasswordInput.Text = SettingsMaxPasswordInput.Text;
                SettingsFeedback.Text = "Password minimum length cannot be higher than password maximum length!";
            }
        }

        private void SettingsMinPasswordInput_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                // Remove any non-numeric characters
                string newText = new string(textBox.Text.Where(char.IsDigit).ToArray());

                // If the text has changed, update the TextBox
                if (newText != textBox.Text)
                {
                    int selectionStart = textBox.SelectionStart;
                    textBox.Text = newText;
                    textBox.SelectionStart = selectionStart > 0 ? selectionStart - 1 : 0;
                }
            }
        }

        private void SettingsMaxPasswordInput_Enter(object sender, EventArgs e)
        {
            if (SettingsMaxPasswordInput.ForeColor == Color.Gray)
            {
                SettingsMaxPasswordInput.Clear();
            }

            SettingsMaxPasswordInput.ForeColor = Color.White;
        }

        private void SettingsMaxPasswordInput_Leave(object sender, EventArgs e)
        {
            SettingsFeedback.Text = "";

            if (string.IsNullOrWhiteSpace(SettingsMaxPasswordInput.Text))
            {
                SettingsMaxPasswordInput.Text = (int.Parse(SettingsMinPasswordInput.Text) > 24) ? SettingsMinPasswordInput.Text : "24";
                SettingsMaxPasswordInput.ForeColor = string.IsNullOrWhiteSpace(SettingsMinPasswordInput.Text) || SettingsMinPasswordInput.Text == "24" ? Color.Gray : Color.White;
            }
            else if (int.Parse(SettingsMaxPasswordInput.Text) > 50)
            {
                SettingsMaxPasswordInput.Text = "50";
                SettingsFeedback.Text = "Password maximum length cannot be higher than 50!";
            }

            if (int.Parse(SettingsMaxPasswordInput.Text) < int.Parse(SettingsMinPasswordInput.Text))
            {
                SettingsMaxPasswordInput.Text = SettingsMinPasswordInput.Text;
                SettingsFeedback.Text = "Password maximum length cannot be lower than password minimum length!";
            }
        }

        private void SettingsMaxPasswordInput_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                // Remove any non-numeric characters
                string newText = new string(textBox.Text.Where(char.IsDigit).ToArray());

                // If the text has changed, update the TextBox
                if (newText != textBox.Text)
                {
                    int selectionStart = textBox.SelectionStart;
                    textBox.Text = newText;
                    textBox.SelectionStart = selectionStart > 0 ? selectionStart - 1 : 0;
                }
            }
        }

        private void ResetSettingsUI()
        {
            SettingsUsernameInput.Text = username;
            SettingsUsernameInput.ForeColor = Color.White;

            SettingsCurrentPasswordInput.Text = "Current password";
            SettingsCurrentPasswordInput.ForeColor = Color.Gray;
            SettingsCurrentPasswordInput.PasswordChar = '\0';

            SettingsNewPasswordInput.Text = "New password";
            SettingsNewPasswordInput.ForeColor = Color.Gray;
            SettingsNewPasswordInput.PasswordChar = '\0';

            SettingsConfirmPasswordInput.Text = "Confirm new password";
            SettingsConfirmPasswordInput.ForeColor = Color.Gray;
            SettingsConfirmPasswordInput.PasswordChar = '\0';

            SettingsFeedback.Text = "";
        }

        private void SettingsShowCurrentPassword_Click(object sender, EventArgs e)
        {
            SettingsCurrentPasswordInput.PasswordChar = '\0';
        }

        private void SettingsShowNewPassword_Click(object sender, EventArgs e)
        {
            SettingsNewPasswordInput.PasswordChar = '\0';
        }

        private void SettingsShowConfirmNewPassword_Click(object sender, EventArgs e)
        {
            SettingsConfirmPasswordInput.PasswordChar = '\0';
        }

        /*=====================================================*\
        |                       Settings                        |
        \*=====================================================*/
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            AccountsPanel.Visible = false;
            AccountSearchPanel.Visible = false;
            AddAccountButton.Enabled = false;
            SettingsButton.Enabled = false;
            LogoutButton.Enabled = false;
            ResetSettingsUI();
            SettingsPanel.Visible = true;
        }

        private void SaveSettingsButton_Click(object sender, EventArgs e)
        {
            if (!ValidateUsername(SettingsUsernameInput.Text))
            {
                SettingsFeedback.Text = "Invalid username, please change the name before saving!";
                return;
            }

            if (File.Exists(Path.Combine(filePath, SettingsUsernameInput.Text.ToLower() + ".txt")) && SettingsUsernameInput.Text.ToLower() != username)
            {
                SettingsFeedback.Text = "This username is already in use!";
                return;
            }

            // Change username and password
            if (IsNewPasswordEntered())
            {
                if (Encrypter.ComputeSHA256(SettingsCurrentPasswordInput.Text) != password)
                {
                    SettingsFeedback.Text = "Current password was wrong, new password was not saved!";
                    return;
                }

                if (!ValidatePassword(SettingsNewPasswordInput.Text, SettingsConfirmPasswordInput.Text))
                {
                    SettingsFeedback.Text = "Invalid password, please make sure password and confirm password match and they are both at least 8 characters long and contain both capital and lower letters!";
                    return;
                }

                UpdateUsernameAndPassword();
            }
            else // Change only username
            {
                UpdateUsername();
            }

            SaveData();

            CloseSettingsPanel();
        }

        private void CancelSettingsButton_Click(object sender, EventArgs e)
        {
            CloseSettingsPanel();
        }

        private bool IsNewPasswordEntered()
        {
            return !string.IsNullOrWhiteSpace(SettingsNewPasswordInput.Text) && SettingsNewPasswordInput.ForeColor == Color.White ||
                   !string.IsNullOrWhiteSpace(SettingsConfirmPasswordInput.Text) && SettingsConfirmPasswordInput.ForeColor == Color.White;
        }

        private void UpdateUsernameAndPassword()
        {
            if (SettingsUsernameInput.Text.ToLower() != username)
            {
                DeleteFile(username);
                username = SettingsUsernameInput.Text.ToLower();
            }

            if (accountData.Any())
            {
                List<KeyValuePair<string, string[]>> tempData = Encrypter.DecryptData(accountData, encryptionKey);
                encryptionKey = SettingsNewPasswordInput.Text;
                password = Encrypter.ComputeSHA256(SettingsNewPasswordInput.Text);
                accountData = Encrypter.EncryptData(tempData, encryptionKey);
            }
            else
            {
                encryptionKey = SettingsNewPasswordInput.Text;
                password = Encrypter.ComputeSHA256(SettingsNewPasswordInput.Text);
            }
        }

        private void UpdateUsername()
        {
            if (SettingsUsernameInput.Text.ToLower() != username)
            {
                DeleteFile(username);
                username = SettingsUsernameInput.Text.ToLower();
            }
        }

        private void CloseSettingsPanel()
        {
            SettingsPanel.Visible = false;
            AddAccountButton.Enabled = true;
            SettingsButton.Enabled = true;
            LogoutButton.Enabled = true;
            AccountsPanel.Visible = true;
            AccountSearchPanel.Visible = true;

            if (accountData.Count >= accountLimit)
            {
                AddAccountButton.Enabled = false;
            }
        }
    }
}
