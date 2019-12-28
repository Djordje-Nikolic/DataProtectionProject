using System;
using System.Windows.Forms;
using CryptoCollection;

namespace ZIProject.Client.InputForms
{
    public enum DataDirection
    {
        Upload,
        Download
    }

    public partial class ChooseCrypto : Form
    {
        public CryptoChoice Answer;
        public ChooseCrypto(DataDirection dir)
        {
            InitializeComponent();

            if (dir == DataDirection.Download)
            {
                Text = "Choose a decryption algorithm";
                checkBoxDepadChoice.Visible = true;
            }
            else if (dir == DataDirection.Upload)
            {
                Text = "Choose an encryption algorithm";
                checkBoxDepadChoice.Visible = false;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (radioButtonOTP.Checked)
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("You have to enter a key!", "Invalid pad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                Answer = new CryptoChoice();
                Answer.Choice = CryptoChoices.OneTimePad;
                Answer.Key = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);
                this.DialogResult = DialogResult.OK;
                return;
            }

            if (radioButtonTEA.Checked)
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("You have to enter a key!", "Invalid key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                if (System.Text.Encoding.UTF8.GetBytes(textBox1.Text).Length != 16)
                {
                    MessageBox.Show("Entered key has to be 16 bytes long!", "Invalid key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                Answer = new CryptoChoice();
                Answer.Choice = CryptoChoices.TEA;
                Answer.Key = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);

                if (checkBoxDepadChoice.Visible)
                    Answer.Depad = checkBoxDepadChoice.Checked;

                this.DialogResult = DialogResult.OK;
                return;
            }

            if (radioButtonELG.Checked)
            {
                this.DialogResult = DialogResult.None;
                return;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
