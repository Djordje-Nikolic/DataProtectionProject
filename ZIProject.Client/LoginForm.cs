using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel.Web;
using ZIProject.MyCloudStore;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ZIProject.Client
{
    public partial class LoginForm : Form
    {
        ICloudStoreService proxy = null;

        public LoginForm()
        {
            InitializeComponent();

            WebChannelFactory<ICloudStoreService> factory = new WebChannelFactory<ICloudStoreService>(new Uri("http://localhost:56082/MyCloudStore/CloudStoreService.svc"));
            proxy = factory.CreateChannel();
        }

        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxUsername.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                labelStatus.Text = "Username and password cannot be nothing.";
                return;
            }

            try
            {
                if (proxy.Login(textBoxUsername.Text, textBoxPassword.Text))
                {
                    labelStatus.Text = "Login successfull. Transitioning...";

                    await Task.Delay(2500);

                    var fileForm = new FileForm(proxy);
                    fileForm.Location = this.Location;
                    fileForm.StartPosition = FormStartPosition.Manual;
                    fileForm.FormClosing += delegate { this.Show(); };
                    fileForm.Show();

                    labelStatus.Text = "";
                    this.Hide();
                }
                else
                {
                    labelStatus.Text = "Login unsuccessfull.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetFullMessage(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxUsername.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                labelStatus.Text = "Username and password cannot be nothing.";
                return;
            }

            try
            {
                if (proxy.Register(textBoxUsername.Text, textBoxPassword.Text))
                {
                    labelStatus.Text = "Registration successful. You can now login.";
                }
                else
                {
                    labelStatus.Text = "Registration unsuccessfull.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetFullMessage(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
