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
        private UserController userController = null;
        private ICloudStoreService proxy;

        public LoginForm()
        {
            InitializeComponent();

            WebChannelFactory<ICloudStoreService> factory = new WebChannelFactory<ICloudStoreService>(new Uri("http://localhost:56082/MyCloudStore/CloudStoreService.svc"));
            proxy = factory.CreateChannel();

            userController = new UserController(proxy, this);
        }

        public async void TransitionToFileForm()
        {
            await Task.Delay(2500);

            var fileForm = new FileForm(proxy, userController);
            fileForm.Location = this.Location;
            fileForm.StartPosition = FormStartPosition.Manual;
            fileForm.FormClosing += delegate { this.Show(); };
            fileForm.Show();

            labelStatus.Text = "";
            this.Hide();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxUsername.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                labelStatus.Text = "Username and password cannot be nothing.";
                return;
            }

            try
            {
                if (userController.Login(textBoxUsername.Text, textBoxPassword.Text))
                {
                    labelStatus.Text = "Login successful. Transitioning...";
                }
                else
                {
                    labelStatus.Text = "Login unsuccessfull.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetFullMessage(), "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (userController.Register(textBoxUsername.Text, textBoxPassword.Text))
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
                MessageBox.Show(ex.GetFullMessage(), "Registration error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
