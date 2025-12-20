using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Encryption;

namespace PasswordManager
{
    public partial class PasswordManagerForm : Form
    {
        private string username;
        private string password;
        private string filePath;
        private string encryptionKey;
        private List<KeyValuePair<string, string[]>> accountData = new List<KeyValuePair<string, string[]>>();
        private Random random = new Random();
        private StringBuilder stringBuilder = new StringBuilder();
        private const int accountLimit = 200;

        private List<Panel> accountPanels = new List<Panel>();
        private List<Label> titleLabels = new List<Label>();
        private List<Label> accountLabels = new List<Label>();
        private List<Button> copyURLButtons = new List<Button>();
        private List<Button> copyAccountButtons = new List<Button>();
        private List<Button> copyPasswordButtons = new List<Button>();
        private List<Button> editButtons = new List<Button>();
        private List<Button> deleteButtons = new List<Button>();

        private int editingIndex;
        private int deletedItemIndex = -1;
        private string lastCopiedPassword = "";

        public PasswordManagerForm()
        {
            InitializeComponent();
        }

        private void FormMinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void FormQuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PasswordManagerForm_Load(object sender, EventArgs e)
        {
            // Find user's documents folder
            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PasswordManagerData");

            // Check if the user has used the application before
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
                LoginPanel.Visible = false;
                RegistrationPanel.Visible = true;
            }

            CreateAccountPlaceholders();
            ResetRegistrationUI();
            ResetLoginUI();
        }

        /*=====================================================*\
        |                    Add Account UI                     |
        \*=====================================================*/
        private void AddAccountTitleInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(AddAccountTitleInput, "Title");
        }

        private void AddAccountTitleInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(AddAccountTitleInput, "Title");
        }

        private void AddAccountURLInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(AddAccountURLInput, "URL");
        }

        private void AddAccountURLInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(AddAccountURLInput, "URL");
        }

        private void AddAccountNameInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(AddAccountNameInput, "Username");
        }

        private void AddAccountNameInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(AddAccountNameInput, "Username");
        }

        private void AddAccountPasswordInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(AddAccountPasswordInput, "Password");
        }

        private void AddAccountPasswordInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(AddAccountPasswordInput, "Password");
        }

        private void AddAccountNotesInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(AddAccountNotesInput, "...");
        }

        private void AddAccountNotesInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(AddAccountNotesInput, "...");
        }

        private void HandleInputEnter(TextBox input, string placeholder)
        {
            if (input.ForeColor == Color.Gray)
            {
                input.Clear();
                input.ForeColor = Color.White;
            }
        }

        private void ResetAddAccountUI()
        {
            ResetInputField(AddAccountTitleInput, "Title", true);
            ResetInputField(AddAccountURLInput, "URL", true);
            ResetInputField(AddAccountNameInput, "Username", true);
            ResetInputField(AddAccountPasswordInput, "Password", true);
            ResetInputField(AddAccountNotesInput, "...", true);
            AddAccountFeedback.Text = "";
        }

        private void ResetInputField(TextBox input, string placeholder, bool isForced = false)
        {
            if (string.IsNullOrWhiteSpace(input.Text) || isForced)
            {
                input.Text = placeholder;
                input.ForeColor = Color.Gray;
            }
        }

        /*=====================================================*\
        |                      Add Account                      |
        \*=====================================================*/
        private void AddAccountButton_Click(object sender, EventArgs e)
        {
            AccountsPanel.Visible = false;
            AccountSearchPanel.Visible = false;
            SettingsButton.Enabled = false;
            LogoutButton.Enabled = false;

            ResetAddAccountUI();

            AddAccountPanel.Visible = true;
            AddAccountButton.Enabled = false;
        }

        private void AddAccountRandomButton_Click(object sender, EventArgs e)
        {
            AddAccountPasswordInput.Text = CreateRandomString();
            AddAccountPasswordInput.ForeColor = Color.White;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (AddAccountTitleInput.Text == "" || AddAccountTitleInput.ForeColor == Color.Gray)
            {
                AddAccountFeedback.Text = "Title is empty!";
                return;
            }

            accountData.Add(new KeyValuePair<string, string[]>(
                Encrypter.Encrypt(AddAccountTitleInput.Text, encryptionKey),
                new string[] {
                    (AddAccountURLInput.ForeColor == Color.Gray) ? Encrypter.Encrypt("", encryptionKey) : Encrypter.Encrypt(AddAccountURLInput.Text, encryptionKey),
                    (AddAccountNameInput.ForeColor == Color.Gray) ? Encrypter.Encrypt("", encryptionKey) : Encrypter.Encrypt(AddAccountNameInput.Text, encryptionKey),
                    (AddAccountPasswordInput.ForeColor == Color.Gray) ? Encrypter.Encrypt("", encryptionKey) : Encrypter.Encrypt(AddAccountPasswordInput.Text, encryptionKey),
                    (AddAccountNotesInput.ForeColor == Color.Gray) ? Encrypter.Encrypt("", encryptionKey) : Encrypter.Encrypt(AddAccountNotesInput.Text, encryptionKey),
                }
            ));

            SaveData();

            AddAccountPanel.Visible = false;
            AddAccountButton.Enabled = true;
            SettingsButton.Enabled = true;
            LogoutButton.Enabled = true;
            AccountsPanel.Visible = true;
            AccountSearchPanel.Visible = true;

            DisplayAccounts();
        }

        private void CancelAddButton_Click(object sender, EventArgs e)
        {
            AddAccountPanel.Visible = false;
            AddAccountButton.Enabled = true;
            SettingsButton.Enabled = true;
            LogoutButton.Enabled = true;
            AccountsPanel.Visible = true;
            AccountSearchPanel.Visible = true;
        }

        /*=====================================================*\
        |                    Edit Account UI                    |
        \*=====================================================*/
        private void EditAccountTitleInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(EditAccountTitleInput, "Title");
        }

        private void EditAccountTitleInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(EditAccountTitleInput, "Title");
        }

        private void EditAccountURLInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(EditAccountURLInput, "URL");
        }

        private void EditAccountURLInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(EditAccountURLInput, "URL");
        }

        private void EditAccountNameInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(EditAccountNameInput, "Username");
        }

        private void EditAccountNameInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(EditAccountNameInput, "Username");
        }

        private void EditAccountPasswordInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(EditAccountPasswordInput, "Password");
        }

        private void EditAccountPasswordInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(EditAccountPasswordInput, "Password");
        }

        private void EditAccountNotesInput_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(EditAccountNotesInput, "...");
        }

        private void EditAccountNotesInput_Leave(object sender, EventArgs e)
        {
            ResetInputField(EditAccountNotesInput, "...");
        }

        private void ResetEditAccountUI()
        {
            ResetInputField(EditAccountTitleInput, "Title");
            ResetInputField(EditAccountURLInput, "URL");
            ResetInputField(EditAccountNameInput, "Username");
            ResetInputField(EditAccountPasswordInput, "Password");
            ResetInputField(EditAccountNotesInput, "...");
            EditAccountFeedback.Text = "";
        }

        /*=====================================================*\
        |                     Edit Account                      |
        \*=====================================================*/
        private void EditAccountButton_Click(object sender, EventArgs e)
        {
            if (EditAccountTitleInput.Text == "" || EditAccountTitleInput.ForeColor == Color.Gray)
            {
                EditAccountFeedback.Text = "Title is empty!";
                return;
            }

            accountData.RemoveAt(editingIndex);
            accountData.Insert(editingIndex, new KeyValuePair<string, string[]>(
                Encrypter.Encrypt(EditAccountTitleInput.Text, encryptionKey),
                new string[] {
                    (EditAccountURLInput.ForeColor == Color.Gray) ? Encrypter.Encrypt("", encryptionKey) : Encrypter.Encrypt(EditAccountURLInput.Text, encryptionKey),
                    (EditAccountNameInput.ForeColor == Color.Gray) ? Encrypter.Encrypt("", encryptionKey) : Encrypter.Encrypt(EditAccountNameInput.Text, encryptionKey),
                    (EditAccountPasswordInput.ForeColor == Color.Gray) ? Encrypter.Encrypt("", encryptionKey) : Encrypter.Encrypt(EditAccountPasswordInput.Text, encryptionKey),
                    (EditAccountNotesInput.ForeColor == Color.Gray) ? Encrypter.Encrypt("", encryptionKey) : Encrypter.Encrypt(EditAccountNotesInput.Text, encryptionKey),
                }
            ));
            SaveData();

            EditAccountPanel.Visible = false;
            AddAccountButton.Enabled = true;
            SettingsButton.Enabled = true;
            LogoutButton.Enabled = true;
            AccountsPanel.Visible = true;
            AccountSearchPanel.Visible = true;

            DisplayAccounts();
        }

        private void EditAccountRandomButton_Click(object sender, EventArgs e)
        {
            EditAccountPasswordInput.Text = CreateRandomString();
            EditAccountPasswordInput.ForeColor = Color.White;
        }

        private void CancelEditAccountButton_Click(object sender, EventArgs e)
        {
            EditAccountPanel.Visible = false;
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

        /*=====================================================*\
        |                    Delete Account                     |
        \*=====================================================*/
        private void CancelDeleteAccountButton_Click(object sender, EventArgs e)
        {
            ConfirmDeletePanel.Visible = false;
            AddAccountButton.Enabled = true;
            SettingsButton.Enabled = true;
            LogoutButton.Enabled = true;

            if (accountData.Count >= accountLimit)
            {
                AddAccountButton.Enabled = false;
            }

            deletedItemIndex = -1;
        }

        private void DeleteAccountButton_Click(object sender, EventArgs e)
        {
            accountData.RemoveAt(deletedItemIndex);
            ConfirmDeletePanel.Visible = false;
            AddAccountButton.Enabled = true;
            SettingsButton.Enabled = true;
            LogoutButton.Enabled = true;
            DisplayAccounts();
            SaveData();
            deletedItemIndex = -1;
        }

        /*=====================================================*\
        |                       Search Bar                      |
        \*=====================================================*/
        private void AccountSearchBar_Enter(object sender, EventArgs e)
        {
            HandleInputEnter(AccountSearchBar, "Search...");
        }

        private void AccountSearchBar_Leave(object sender, EventArgs e)
        {
            ResetInputField(AccountSearchBar, "Search...");
        }

        private void AccountSearchBar_KeyUp(object sender, KeyEventArgs e)
        {
            if (AccountSearchBar.ForeColor != Color.Gray)
            {
                for (int index = 0; index < accountLimit; index++)
                {
                    if (index >= accountData.Count) break;
                    accountPanels[index].Visible = titleLabels[index].Text.ToLower().Contains(AccountSearchBar.Text.ToLower());
                }
            }
        }

        /*=====================================================*\
        |                    Clear Clipboard                    |
        \*=====================================================*/
        private void ClearClipboardTimer_Tick(object sender, EventArgs e)
        {
            if (Clipboard.GetText() == lastCopiedPassword)
            {
                Clipboard.Clear();
                ClearClipboardTimer.Stop();
            }

            lastCopiedPassword = "";
        }
    }
}
