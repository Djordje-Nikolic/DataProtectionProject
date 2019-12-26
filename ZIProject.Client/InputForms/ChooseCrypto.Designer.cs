namespace ZIProject.Client.InputForms
{
    partial class ChooseCrypto
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxCrypto = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.radioButtonOTP = new System.Windows.Forms.RadioButton();
            this.radioButtonTEA = new System.Windows.Forms.RadioButton();
            this.radioButtonELG = new System.Windows.Forms.RadioButton();
            this.checkBoxDepadChoice = new System.Windows.Forms.CheckBox();
            this.groupBoxCrypto.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(238, 397);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 25);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBoxCrypto
            // 
            this.groupBoxCrypto.Controls.Add(this.radioButtonELG);
            this.groupBoxCrypto.Controls.Add(this.radioButtonTEA);
            this.groupBoxCrypto.Controls.Add(this.radioButtonOTP);
            this.groupBoxCrypto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxCrypto.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCrypto.Name = "groupBoxCrypto";
            this.groupBoxCrypto.Size = new System.Drawing.Size(301, 258);
            this.groupBoxCrypto.TabIndex = 2;
            this.groupBoxCrypto.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(12, 292);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(160, 22);
            this.textBox1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 273);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Key:";
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOk.Location = new System.Drawing.Point(157, 397);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 25);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // radioButtonOTP
            // 
            this.radioButtonOTP.AutoSize = true;
            this.radioButtonOTP.Checked = true;
            this.radioButtonOTP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonOTP.Location = new System.Drawing.Point(93, 78);
            this.radioButtonOTP.Name = "radioButtonOTP";
            this.radioButtonOTP.Size = new System.Drawing.Size(127, 24);
            this.radioButtonOTP.TabIndex = 0;
            this.radioButtonOTP.TabStop = true;
            this.radioButtonOTP.Text = "One Time Pad";
            this.radioButtonOTP.UseVisualStyleBackColor = true;
            // 
            // radioButtonTEA
            // 
            this.radioButtonTEA.AutoSize = true;
            this.radioButtonTEA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonTEA.Location = new System.Drawing.Point(93, 116);
            this.radioButtonTEA.Name = "radioButtonTEA";
            this.radioButtonTEA.Size = new System.Drawing.Size(58, 24);
            this.radioButtonTEA.TabIndex = 1;
            this.radioButtonTEA.Text = "TEA";
            this.radioButtonTEA.UseVisualStyleBackColor = true;
            // 
            // radioButtonELG
            // 
            this.radioButtonELG.AutoSize = true;
            this.radioButtonELG.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonELG.Location = new System.Drawing.Point(93, 154);
            this.radioButtonELG.Name = "radioButtonELG";
            this.radioButtonELG.Size = new System.Drawing.Size(92, 24);
            this.radioButtonELG.TabIndex = 2;
            this.radioButtonELG.Text = "El Gamal";
            this.radioButtonELG.UseVisualStyleBackColor = true;
            // 
            // checkBoxDepadChoice
            // 
            this.checkBoxDepadChoice.AutoSize = true;
            this.checkBoxDepadChoice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxDepadChoice.Location = new System.Drawing.Point(207, 292);
            this.checkBoxDepadChoice.Name = "checkBoxDepadChoice";
            this.checkBoxDepadChoice.Size = new System.Drawing.Size(106, 20);
            this.checkBoxDepadChoice.TabIndex = 6;
            this.checkBoxDepadChoice.Text = "Depad data?";
            this.checkBoxDepadChoice.UseVisualStyleBackColor = true;
            this.checkBoxDepadChoice.Visible = false;
            // 
            // ChooseCrypto
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(325, 434);
            this.Controls.Add(this.checkBoxDepadChoice);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.groupBoxCrypto);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "ChooseCrypto";
            this.Text = "Choose the desired crypto algorithm";
            this.groupBoxCrypto.ResumeLayout(false);
            this.groupBoxCrypto.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxCrypto;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.RadioButton radioButtonELG;
        private System.Windows.Forms.RadioButton radioButtonTEA;
        private System.Windows.Forms.RadioButton radioButtonOTP;
        private System.Windows.Forms.CheckBox checkBoxDepadChoice;
    }
}