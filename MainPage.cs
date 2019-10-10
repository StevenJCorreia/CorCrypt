using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CorCrypt.Models;
using CyberCrypt;

//TODO - Make the "weak password" label a linklabel that either brings the user to an online password generator or a new form where i'll generate it for them...

namespace CorCrypt
{
    public partial class CorCrypt : Form
    {
        private List<File> fileList = new List<File>();
        private string PASSWORD_VALUE_DEFAULT = "Enter Password";

        public CorCrypt()
        {
            InitializeComponent();
            
            filePathErrorLabel.Visible = false;
            passwordErrorLabel.Visible = false;

            RefreshListBox();
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

            fileList.Clear();

            RefreshListBox();

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

                foreach (File file in fileList)
                {
                    index = filePathsListBox.FindStringExact(file.fullFilePath);

                    try
                    {
                        _AES256.DecryptFile(file.fullFilePath, passwordValue.Text);
                    }
                    catch (Exception exception)
                    {
                        filePathsListBox.SetSelected(index, true);
                        MessageBox.Show(exception.Message + $"\nCannot cipher file '{ file.fullFilePath }' @ index { index }", "Cannot Cipher File");
                        ResetForm();
                        return;
                    }

                    toolStripProgressBar.Value = (++count / fileList.Count) * 100;
                }

                if (count == 0)
                {
                    MessageBox.Show($"Successfully decrypted file '{ fileList[index].fullFilePath }'");
                }
                else
                {
                    MessageBox.Show($"Successfully decrypted files:\n" + GetAllFilesFromListBox());
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

                foreach (File file in fileList)
                {
                    index = filePathsListBox.FindStringExact(file.fullFilePath);

                    try
                    {
                        _AES256.EncryptFile(file.fullFilePath, passwordValue.Text);
                    }
                    catch (Exception exception)
                    {
                        filePathsListBox.SetSelected(index, true);
                        MessageBox.Show(exception.Message + $"\nCannot cipher file '{ file.fullFilePath }' @ index { index }", "Cannot Cipher File");
                        ResetForm();
                        return;
                    }

                    toolStripProgressBar.Value = (++count / fileList.Count) * 100;
                }

                if (count == 0)
                {
                    MessageBox.Show($"Successfully encrypted file '{ fileList[index].fullFilePath }'");
                }
                else
                {
                    MessageBox.Show($"Successfully encrypted files:\n" + GetAllFilesFromListBox());
                }

                ResetForm();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetAllFilesFromListBox()
        {
            string files = "";

            foreach (File file in fileList)
            {
                files += $"{ file.fullFilePath }\n";
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

        private void RefreshListBox()
        {
            filePathsListBox.DataSource = null;
            filePathsListBox.DataSource = fileList;
            filePathsListBox.DisplayMember = "fileName";
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
            fileList.RemoveAt(filePathsListBox.SelectedIndex);

            RefreshStatusLabel(fileList.Count);

            RefreshListBox();
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
            File[] files = File.StringToFile_Array(SelectFile());

            if (!(files.Length == 0))
            {

                foreach (File file in files)
                {
                    if (!fileList.Contains(file))
                    {
                        // Same file name but within different directory
                        if (File.DuplicateFileName(files, file.fileName))
                        {
                            file.fileName += $" ({ file.fullFilePath })"; // TODO - Fix accessor functionality

                            fileList.Add(file);
                        }
                        else
                        {
                            fileList.Add(file);
                        }

                        RefreshStatusLabel(fileList.Count);

                        RefreshListBox();
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

            if (fileList.Count == 0)
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
