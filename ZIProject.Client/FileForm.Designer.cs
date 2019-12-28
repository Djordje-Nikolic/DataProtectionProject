namespace ZIProject.Client
{
    partial class FileForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.buttonRefreshFiles = new System.Windows.Forms.Button();
            this.buttonUploadFile = new System.Windows.Forms.Button();
            this.buttonDownloadFile = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.openFileDialogUpload = new System.Windows.Forms.OpenFileDialog();
            this.dataGridViewFiles = new System.Windows.Forms.DataGridView();
            this.folderBrowserDialogDownloadLocation = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelLeftoverSpace = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Your uploaded files:";
            // 
            // buttonRefreshFiles
            // 
            this.buttonRefreshFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRefreshFiles.Location = new System.Drawing.Point(262, 424);
            this.buttonRefreshFiles.Name = "buttonRefreshFiles";
            this.buttonRefreshFiles.Size = new System.Drawing.Size(127, 27);
            this.buttonRefreshFiles.TabIndex = 2;
            this.buttonRefreshFiles.Text = "Refresh file list";
            this.buttonRefreshFiles.UseVisualStyleBackColor = true;
            this.buttonRefreshFiles.Click += new System.EventHandler(this.buttonRefreshFiles_Click);
            // 
            // buttonUploadFile
            // 
            this.buttonUploadFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUploadFile.Location = new System.Drawing.Point(528, 424);
            this.buttonUploadFile.Name = "buttonUploadFile";
            this.buttonUploadFile.Size = new System.Drawing.Size(127, 27);
            this.buttonUploadFile.TabIndex = 3;
            this.buttonUploadFile.Text = "Upload new file";
            this.buttonUploadFile.UseVisualStyleBackColor = true;
            this.buttonUploadFile.Click += new System.EventHandler(this.buttonUploadFile_Click);
            // 
            // buttonDownloadFile
            // 
            this.buttonDownloadFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDownloadFile.Location = new System.Drawing.Point(661, 424);
            this.buttonDownloadFile.Name = "buttonDownloadFile";
            this.buttonDownloadFile.Size = new System.Drawing.Size(127, 27);
            this.buttonDownloadFile.TabIndex = 4;
            this.buttonDownloadFile.Text = "Download file";
            this.buttonDownloadFile.UseVisualStyleBackColor = true;
            this.buttonDownloadFile.Click += new System.EventHandler(this.buttonDownloadFile_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLogout.Location = new System.Drawing.Point(16, 424);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(91, 27);
            this.buttonLogout.TabIndex = 5;
            this.buttonLogout.Text = "Logout";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // openFileDialogUpload
            // 
            this.openFileDialogUpload.Title = "Choose the file you wish to upload";
            // 
            // dataGridViewFiles
            // 
            this.dataGridViewFiles.AllowUserToAddRows = false;
            this.dataGridViewFiles.AllowUserToDeleteRows = false;
            this.dataGridViewFiles.AllowUserToResizeRows = false;
            this.dataGridViewFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFiles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewFiles.Location = new System.Drawing.Point(12, 61);
            this.dataGridViewFiles.MultiSelect = false;
            this.dataGridViewFiles.Name = "dataGridViewFiles";
            this.dataGridViewFiles.ReadOnly = true;
            this.dataGridViewFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewFiles.Size = new System.Drawing.Size(776, 357);
            this.dataGridViewFiles.TabIndex = 6;
            // 
            // folderBrowserDialogDownloadLocation
            // 
            this.folderBrowserDialogDownloadLocation.Description = "Choose where to download the selected file";
            // 
            // buttonRemove
            // 
            this.buttonRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemove.Location = new System.Drawing.Point(395, 424);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(127, 27);
            this.buttonRemove.TabIndex = 7;
            this.buttonRemove.Text = "Remove file";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(318, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "Your leftover allocated space on the server: ";
            // 
            // labelLeftoverSpace
            // 
            this.labelLeftoverSpace.AutoSize = true;
            this.labelLeftoverSpace.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLeftoverSpace.ForeColor = System.Drawing.Color.IndianRed;
            this.labelLeftoverSpace.Location = new System.Drawing.Point(332, 9);
            this.labelLeftoverSpace.Name = "labelLeftoverSpace";
            this.labelLeftoverSpace.Size = new System.Drawing.Size(0, 20);
            this.labelLeftoverSpace.TabIndex = 9;
            // 
            // FileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 479);
            this.Controls.Add(this.labelLeftoverSpace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.dataGridViewFiles);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.buttonDownloadFile);
            this.Controls.Add(this.buttonUploadFile);
            this.Controls.Add(this.buttonRefreshFiles);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FileForm";
            this.Text = "Files Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileForm_FormClosing);
            this.Load += new System.EventHandler(this.FileForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFiles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRefreshFiles;
        private System.Windows.Forms.Button buttonUploadFile;
        private System.Windows.Forms.Button buttonDownloadFile;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.OpenFileDialog openFileDialogUpload;
        private System.Windows.Forms.DataGridView dataGridViewFiles;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogDownloadLocation;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelLeftoverSpace;
    }
}