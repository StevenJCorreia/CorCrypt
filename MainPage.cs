using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CorCrypt
{
    public partial class CorCrypt : Form
    {
        private string FILE_PATH_VALUE_DEFAULT = "Enter File Path";
        private string PASSWORD_VALUE_DEFAULT = "Enter Password";

        public CorCrypt()
        {
            InitializeComponent();

            filePathErrorLabel.Visible = false;
            passwordErrorLabel.Visible = false;
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string projectRepo = "https://github.com/StevenJCorreia/CorCrypt";
            Process.Start(projectRepo);
        }

        private void ResetForm()
        {
            filePathValue.Text = FILE_PATH_VALUE_DEFAULT;
            GreyFilePathLabel();
            passwordValue.Text = PASSWORD_VALUE_DEFAULT;
            GreyPasswordLabel();

            filePathErrorLabel.Visible = false;
            passwordErrorLabel.Visible = false;
        }

        private void DarkenFilePathLabel()
        {
            filePathValue.Text = "";
            filePathValue.ForeColor = Color.Black;
        }

        private void DarkenPasswordLabel()
        {
            passwordValue.Text = "";
            passwordValue.PasswordChar = '*';
            passwordValue.ForeColor = Color.Black;
        }

        private void DecryptButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }
            else
            {
                // TODO - Decrypt

                // TODO - Messagebox result

                ResetForm();
            }
        }

        private void EncryptButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }
            else
            {
                // TODO - Encrypt

                // TODO - Messagebox result

                ResetForm();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FilePathValue_Enter(object sender, EventArgs e)
        {
            if (filePathValue.Text == FILE_PATH_VALUE_DEFAULT)
            {
                DarkenFilePathLabel();
            }
        }

        private void FilePathValue_Leave(object sender, EventArgs e)
        {
            if (filePathValue.Text == "")
            {
                GreyFilePathLabel();
            }
        }

        private void GreyFilePathLabel()
        {
            filePathValue.Text = FILE_PATH_VALUE_DEFAULT;
            filePathValue.ForeColor = Color.Silver;
        }

        private void GreyPasswordLabel()
        {
            passwordValue.Text = PASSWORD_VALUE_DEFAULT;
            passwordValue.PasswordChar = '\0';
            passwordValue.ForeColor = Color.Silver;
        }

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectFile();
        }

        private void PasswordValue_Enter(object sender, EventArgs e)
        {
            if (passwordValue.Text == PASSWORD_VALUE_DEFAULT)
            {
                DarkenPasswordLabel();
            }
        }

        private void PasswordValue_Leave(object sender, EventArgs e)
        {
            if (passwordValue.Text == "")
            {
                GreyPasswordLabel();
            }
        }

        private string SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Open File...";
            openFileDialog.Filter = "All files (*.*)|*.*|CSV files (*.csv)|*.csv|Text files (*.txt)|*txt";
            //saveFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return "";
            }

            return openFileDialog.FileName; // TODO - Make this return string[] for multiple file functionality
        }

        private void SelectFile_Click(object sender, EventArgs e)
        {
            string filePath = SelectFile();

            if (!(filePath.Length == 0))
            {
                filePathValue.Text = filePath;
                filePathValue.ForeColor = Color.Black;
            }
        }

        private Boolean ValidateForm()
        {
            Boolean formValid = true;

            if (filePathValue.Text == FILE_PATH_VALUE_DEFAULT)
            {
                string error = "Please enter a file.";

                SetErrorLabel(filePathErrorLabel, error);

                formValid = false;
            }

            if (passwordValue.Text == PASSWORD_VALUE_DEFAULT)
            {
                string error = "Please enter a password.";

                SetErrorLabel(passwordErrorLabel, error);

                formValid = false;
            }

            return formValid;
        }

        private void SetErrorLabel(Label label, string error)
        {
            label.Text = error;
            label.Visible = true;
        }

        private void PasswordValue_TextChanged(object sender, EventArgs e)
        {
            if (passwordValue.Text == PASSWORD_VALUE_DEFAULT || passwordValue.Text.Length == 0)
            {
                return;
            }

            passwordErrorLabel.Visible = false;

            // Add more exception handling for weak passwords
            if (passwordValue.Text.Length < 8)
            {
                string error = "Weak password detected.";

                // TODO - Make look more like a warning than an error
                SetErrorLabel(passwordErrorLabel, error);

                return;
            }
        }
    }
}
