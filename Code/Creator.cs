using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using Encryption;
using System.Diagnostics;

namespace PasswordManager
{
    public partial class PasswordManagerForm
    {
        private void CreateAccountPlaceholders()
        {
            // Prevent creating new elements if they already exist
            if (accountPanels.Any()) { return; }

            // Create placeholders for UI elements
            for (int i = 0; i < accountLimit; i++)
            {
                Panel accountPanel = CreatePanel(760, 120);
                Label titleLabel = CreateLabel(accountPanel, 20, 20, 320, 25, 12, Color.White);
                Label accountLabel = CreateLabel(accountPanel, 20, 45, 320, 25, 10, Color.Gainsboro);
                Button copyURLButton = CreateButton(accountPanel, 20, 75, 90, 30, "URL", i);
                Button copyAccountButton = CreateButton(accountPanel, 120, 75, 90, 30, "Account", i);
                Button copyPasswordButton = CreateButton(accountPanel, 220, 75, 90, 30, "Password", i);
                Button editButton = CreateButton(accountPanel, 340, 75, 90, 30, "Edit", i);
                Button deleteButton = CreateButton(accountPanel, 440, 75, 90, 30, "Delete", i);
                accountPanels.Add(accountPanel);
                titleLabels.Add(titleLabel);
                accountLabels.Add(accountLabel);
                copyURLButtons.Add(copyURLButton);
                copyAccountButtons.Add(copyAccountButton);
                copyPasswordButtons.Add(copyPasswordButton);
                editButtons.Add(editButton);
                deleteButtons.Add(deleteButton);
            }
        }

        public Panel CreatePanel(int width, int height)
        {
            Panel panel = new Panel();
            AccountsPanel.Controls.Add(panel);
            panel.Visible = false;
            panel.Width = width;
            panel.Height = height;
            return panel;
        }

        public Label CreateLabel(Panel parent, int x, int y, int width, int height, int fontSize, Color color)
        {
            Label label = new Label();
            parent.Controls.Add(label);
            label.Visible = false;
            label.Left = x;
            label.Top = y;
            label.Width = width;
            label.Height = height;
            label.ForeColor = color;
            label.Font = new Font("Sans Serif", fontSize);
            return label;
        }

        public Button CreateButton(Panel parent, int x, int y, int width, int height, string text, int listIndex)
        {
            Button button = new Button();
            parent.Controls.Add(button);
            button.Visible = false;
            button.Left = x;
            button.Top = y;
            button.Width = width;
            button.Height = height;
            button.Font = new Font("Sans Serif", 8);
            button.FlatStyle = FlatStyle.Flat;
            button.TabStop = false;
            button.Cursor = Cursors.Hand;
            button.TabIndex = 0;
            button.Text = text;
            button.Click += (sender, EventArgs) => { Button_Click(sender, EventArgs, listIndex); };

            button.ForeColor = Color.FromArgb(225, 225, 225);
            if (text == "Delete")
            {
                button.FlatAppearance.BorderColor = Color.Red;
                button.FlatAppearance.MouseOverBackColor = Color.Red;
                button.FlatAppearance.MouseDownBackColor = Color.Red;
            }
            else if (text == "Edit")
            {
                button.FlatAppearance.BorderColor = Color.FromArgb(255, 116, 81);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 116, 81);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 116, 81);
            }
            else
            {
                button.FlatAppearance.BorderColor = Color.FromArgb(72, 101, 129);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(72, 101, 129);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(72, 101, 129);
            }

            return button;
        }

        protected void Button_Click(object sender, EventArgs e, int index)
        {
            Button button = sender as Button;

            if (button.Text == "URL")
            {
                var url = Encrypter.Decrypt(accountData[index].Value[0], encryptionKey);

                if (string.IsNullOrWhiteSpace(url))
                {
                    return;
                }

                bool isWebUrl = url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("http://", StringComparison.OrdinalIgnoreCase);

                if (isWebUrl)
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    };

                    Process.Start(processInfo);
                }
                else
                {
                    Clipboard.SetText(url);
                }
            }
            else if (button.Text == "Account")
            {
                var account = Encrypter.Decrypt(accountData[index].Value[1], encryptionKey);
                if (!string.IsNullOrEmpty(account))
                {
                    Clipboard.SetText(account);
                }
            }
            else if (button.Text == "Password")
            {
                var password = Encrypter.Decrypt(accountData[index].Value[2], encryptionKey);
                if (!string.IsNullOrEmpty(password))
                {
                    lastCopiedPassword = password;
                    Clipboard.SetText(lastCopiedPassword);
                    ClearClipboardTimer.Stop();
                    ClearClipboardTimer.Start();
                }
            }
            else if (button.Text == "Delete")
            {
                deletedItemIndex = index;
                AddAccountButton.Enabled = false;
                SettingsButton.Enabled = false;
                LogoutButton.Enabled = false;
                ConfirmDeleteLabel.Text = $"Are you sure you want to delete account: {Encrypter.Decrypt(accountData[deletedItemIndex].Key, encryptionKey)}?\nThis action is permanent and cannot be undone!";
                ConfirmDeletePanel.Visible = true;
            }
            else if (button.Text == "Edit")
            {
                AccountsPanel.Visible = false;
                AccountSearchPanel.Visible = false;
                AddAccountButton.Enabled = false;
                SettingsButton.Enabled = false;
                LogoutButton.Enabled = false;
                ConfirmDeletePanel.Visible = false;
                EditAccountPanel.Visible = true;

                EditAccountTitleInput.Text = Encrypter.Decrypt(accountData[index].Key, encryptionKey);
                EditAccountURLInput.Text = Encrypter.Decrypt(accountData[index].Value[0], encryptionKey);
                EditAccountNameInput.Text = Encrypter.Decrypt(accountData[index].Value[1], encryptionKey);
                EditAccountPasswordInput.Text = Encrypter.Decrypt(accountData[index].Value[2], encryptionKey);
                EditAccountNotesInput.Text = Encrypter.Decrypt(accountData[index].Value[3], encryptionKey);

                EditAccountTitleInput.ForeColor = Color.White;
                EditAccountURLInput.ForeColor = Color.White;
                EditAccountNameInput.ForeColor = Color.White;
                EditAccountPasswordInput.ForeColor = Color.White;
                EditAccountNotesInput.ForeColor = Color.White;
                ResetEditAccountUI();
                editingIndex = index;
            }
        }

        private void DisplayAccounts()
        {
            ClearUI();

            // Insert new data into UI
            int index = 0;
            foreach (var data in accountData)
            {
                titleLabels[index].Text = Encrypter.Decrypt(data.Key, encryptionKey);
                accountLabels[index].Text = Encrypter.Decrypt(data.Value[1], encryptionKey);

                accountPanels[index].Visible = true;
                titleLabels[index].Visible = true;
                accountLabels[index].Visible = true;
                copyURLButtons[index].Visible = true;
                copyAccountButtons[index].Visible = true;
                copyPasswordButtons[index].Visible = true;
                editButtons[index].Visible = true;
                deleteButtons[index].Visible = true;

                if (index + 1 >= accountLimit)
                {
                    AddAccountButton.Enabled = false;
                    break;
                }

                index++;
            }

            // Filter by search bar
            AccountSearchBar_KeyUp(null, null);
        }

        private void ClearUI()
        {
            for (int index = 0; index < accountLimit; index++)
            {
                titleLabels[index].Text = "";
                accountLabels[index].Text = "";

                accountPanels[index].Visible = false;
                titleLabels[index].Visible = false;
                accountLabels[index].Visible = false;
                copyURLButtons[index].Visible = false;
                copyAccountButtons[index].Visible = false;
                copyPasswordButtons[index].Visible = false;
                editButtons[index].Visible = false;
                deleteButtons[index].Visible = false;
            }
        }
    }
}