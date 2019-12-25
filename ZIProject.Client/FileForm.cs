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

namespace ZIProject.Client
{
    public partial class FileForm : Form
    {
        private FileController fileController;

        public FileForm(ICloudStoreService cloudStoreClient)
        {
            this.fileController = new FileController(cloudStoreClient);
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
            fileController.LogoutUser();
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
        }

        private void buttonUploadFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogUpload.ShowDialog() == DialogResult.OK)
            {
                fileController.RequestFileUpload(openFileDialogUpload.FileName);
            }
        }

        private void buttonDownloadFile_Click(object sender, EventArgs e)
        {
            if (dataGridViewFiles.SelectedRows.Count > 0)
            {
                var fileInfo = (RemoteFileInfo)dataGridViewFiles.SelectedRows[0].DataBoundItem;

                if (folderBrowserDialogDownloadLocation.ShowDialog() == DialogResult.OK)
                {
                    fileController.RequestFileDownload(fileInfo.Name, folderBrowserDialogDownloadLocation.SelectedPath);
                }
            }
            else
            {
                MessageBox.Show("You have to select one of the uploaded files.");
            }
        }
    }
}
