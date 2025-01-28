using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace PasswordManager
{
    public partial class PasswordManagerForm
    {
        // Method to load data from a file
        private bool GetData(string file)
        {
            try
            {
                accountData.Clear();

                // Read all lines from the file
                string[] lines = File.ReadAllLines(Path.Combine(filePath, file + ".txt"));

                if (lines.Length == 0) return false;

                // The first line contains the password
                password = lines[0];

                // Determine if settings are included
                int dataStartPoint = 1;
                if (bool.TryParse(lines[1], out _))
                {
                    LoadSettings(lines);
                    dataStartPoint = 5;
                }

                // Load account data
                LoadAccountData(lines, dataStartPoint);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Method to save data to a file
        private bool SaveData()
        {
            try
            {
                using (var fs = new FileStream(Path.Combine(filePath, username + ".txt"), FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine(password);
                    sw.WriteLine(SettingsIncludeNumbersCheckbox.Checked);
                    sw.WriteLine(SettingsIncludeSpecialCheckbox.Checked);
                    sw.WriteLine(SettingsMinPasswordInput.Text);
                    sw.WriteLine(SettingsMaxPasswordInput.Text);

                    foreach (var data in accountData)
                    {
                        sw.WriteLine(data.Key);
                        sw.WriteLine(data.Value[0]);
                        sw.WriteLine(data.Value[1]);
                        sw.WriteLine(data.Value[2]);
                        sw.WriteLine(data.Value[3]);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Method to delete a file
        private void DeleteFile(string fileName)
        {
            var path = Path.Combine(filePath, fileName + ".txt");

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        // Method to clear all data
        private void ClearData()
        {
            accountData.Clear();
            username = null;
            password = null;

            // Clear UI elements
            if (titleLabels.Any())
            {
                ClearUI();
            }
        }

        // Helper method to load settings from the file
        private void LoadSettings(string[] lines)
        {
            SettingsIncludeNumbersCheckbox.Checked = bool.Parse(lines[1]);
            SettingsIncludeSpecialCheckbox.Checked = bool.Parse(lines[2]);

            SettingsMinPasswordInput.Text = lines[3];
            SettingsMinPasswordInput.ForeColor = SettingsMinPasswordInput.Text != "16" ? Color.White : Color.Gray;
            SettingsMinPasswordInput_Leave(null, null);

            SettingsMaxPasswordInput.Text = lines[4];
            SettingsMaxPasswordInput.ForeColor = SettingsMaxPasswordInput.Text != "24" ? Color.White : Color.Gray;
            SettingsMaxPasswordInput_Leave(null, null);
        }

        // Helper method to load account data from the file
        private void LoadAccountData(string[] lines, int dataStartPoint)
        {
            for (int i = dataStartPoint; i < lines.Length; i += 5)
            {
                var accountEntry = new string[4];
                for (int j = 0; j < 4; j++)
                {
                    accountEntry[j] = i + j + 1 < lines.Length ? lines[i + j + 1] : "Corrupted";
                }

                accountData.Add(new KeyValuePair<string, string[]>(lines[i], accountEntry));
            }
        }
    }
}
