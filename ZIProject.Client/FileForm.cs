using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZIProject.MyCloudStore;
using ZIProject.Client.InputForms;

namespace ZIProject.Client
{
    public partial class FileForm : Form
    {
        private FileController fileController;
        private UserController userController;

        public FileForm(ICloudStoreService cloudStoreClient, UserController userController)
        {
            this.userController = userController;
            CryptionController cryptionController = new CryptionController(userController.RemoteUserInfo);

            this.fileController = new FileController(cloudStoreClient, cryptionController);
            InitializeComponent();

            dataGridViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void FileForm_Load(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            Logout();
            this.Close();
        }

        private void Logout()
        {
            userController.LogoutUser();
        }

        private void FileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logout();
        }

        private void buttonRefreshFiles_Click(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void RefreshFileList()
        {
            dataGridViewFiles.AutoGenerateColumns = true;
            dataGridViewFiles.DataSource = fileController.RequestAllUserFiles();
            dataGridViewFiles.Refresh();

            userController.RefreshUserInfo();
            labelLeftoverSpace.Text = userController.RemoteUserInfo.LeftoverSpace.ToString() + " bytes";
        }

        private void buttonUploadFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogUpload.ShowDialog() == DialogResult.OK)
            {
                ChooseCrypto chooseCrypto = new ChooseCrypto(DataDirection.Upload);
                if (chooseCrypto.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        fileController.RequestFileUpload(openFileDialogUpload.FileName, chooseCrypto.Answer);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.GetFullMessage(), "Upload error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }

        private void buttonDownloadFile_Click(object sender, EventArgs e)
        {
            if (dataGridViewFiles.SelectedRows.Count > 0)
            {
                var fileInfo = (RemoteFileInfo)dataGridViewFiles.SelectedRows[0].DataBoundItem;

                if (folderBrowserDialogDownloadLocation.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        fileController.RequestFileDownload(fileInfo.Name, folderBrowserDialogDownloadLocation.SelectedPath, fileInfo.HashValue);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.GetFullMessage(), "Download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("You have to select one of the uploaded files.");
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (dataGridViewFiles.SelectedRows.Count > 0)
            {
                var fileInfo = (RemoteFileInfo)dataGridViewFiles.SelectedRows[0].DataBoundItem;

                try
                {
                    fileController.RequestRemoveFile(fileInfo.Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.GetFullMessage(), "Removal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("You have to select one of the uploaded files.");
            }
        }
    }
}
