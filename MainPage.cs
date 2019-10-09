using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CyberCrypt;

namespace CorCrypt
{
    public partial class CorCrypt : Form
    {
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
            passwordValue.Text = PASSWORD_VALUE_DEFAULT;
            GreyPasswordLabel();

            filePathErrorLabel.Visible = false;
            passwordErrorLabel.Visible = false;

            filePathsListBox.Items.Clear();

            toolStripProgressBar.Value = 0;

            toolStripStatusLabel.Text = "0 files";
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
                int count = 0;
                int index = 0;

                foreach (string file in filePathsListBox.Items)
                {
                    index = filePathsListBox.FindStringExact(file);

                    try
                    {
                        _AES256.DecryptFile(file, passwordValue.Text);
                    }
                    catch (Exception exception)
                    {
                        filePathsListBox.SetSelected(index, true);
                        MessageBox.Show(exception.Message + $"\nCannot cipher file '{ file }' @ index { index }", "Cannot Cipher File");
                        ResetForm();
                        return;
                    }

                    toolStripProgressBar.Value = (++count / filePathsListBox.Items.Count) * 100;
                }

                if (count == 0)
                {
                    MessageBox.Show($"Successfully decrypted file '{ filePathsListBox.Items[index].ToString() }'");
                }
                else
                {
                    MessageBox.Show($"Successfully decrypted files:\n" + GetAllFileFromListBox());
                }

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
                int count = 0;
                int index = 0;

                foreach (string file in filePathsListBox.Items)
                {
                    index = filePathsListBox.FindStringExact(file);

                    try
                    {
                        _AES256.EncryptFile(file, passwordValue.Text);
                    }
                    catch (Exception exception)
                    {
                        filePathsListBox.SetSelected(index, true);
                        MessageBox.Show(exception.Message + $"\nCannot cipher file '{ file }' @ index { index }", "Cannot Cipher File");
                        ResetForm();
                        return;
                    }

                    toolStripProgressBar.Value = (++count / filePathsListBox.Items.Count) * 100;
                }

                if (count == 0)
                {
                    MessageBox.Show($"Successfully encrypted file '{ filePathsListBox.Items[index].ToString() }'");
                }
                else
                {
                    MessageBox.Show($"Successfully encrypted files:\n" + GetAllFileFromListBox());
                }

                ResetForm();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetAllFileFromListBox()
        {
            string files = "";

            foreach (string file in filePathsListBox.Items)
            {
                files += $"{ file }\n";
            }

            return files;
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

                SetErrorLabel(passwordErrorLabel, error);

                return;
            }
        }

        private void RefreshStatusLabel(int count)
        {
            if (count == 1)
            {
                toolStripStatusLabel.Text = count + " file";
            }
            else
            {
                toolStripStatusLabel.Text = count + " files";
            }
        }

        private void RemoveFileButton_Click(object sender, EventArgs e)
        {
            filePathsListBox.Items.RemoveAt(filePathsListBox.SelectedIndex);
            RefreshStatusLabel(filePathsListBox.Items.Count);
        }

        private string[] SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open File...";
            openFileDialog.Filter = "All files (*.*)|*.*|CSV files (*.csv)|*.csv|Text files (*.txt)|*txt";
            //saveFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return new string[0];
            }

            return openFileDialog.FileNames;
        }

        private void SelectFile_Click(object sender, EventArgs e)
        {
            string[] files = SelectFile();

            if (!(files.Length == 0))
            {

                foreach (string file in files)
                {
                    if (!filePathsListBox.Items.Contains(file))
                    {
                        filePathsListBox.Items.Add(file);
                        RefreshStatusLabel(filePathsListBox.Items.Count);
                    }
                }
            }
        }

        private void SetErrorLabel(Label label, string error)
        {
            label.Text = error;
            label.Visible = true;
        }

        private Boolean ValidateForm()
        {
            Boolean formValid = true;

            if (filePathsListBox.Items.Count == 0)
            {
                string error = "Please choose at least one file.";

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
    }
}
