using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZIProject.MyCloudStore;

namespace ZIProject.Client
{
    public class UserController
    {
        private ICloudStoreService proxy;
        private LoginForm loginForm = null;
        public RemoteUserInfo RemoteUserInfo { get; private set; }

        public UserController(ICloudStoreService cloudStoreService, LoginForm form)
        { 
            this.proxy = cloudStoreService;
            this.loginForm = form;
        }

        public bool Login(string username, string password)
        {
            try
            {
                RemoteUserInfo = proxy.Login(username, password);

                if (RemoteUserInfo != null)
                {
                    loginForm.TransitionToFileForm();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Register(string username, string password) => proxy.Register(username, password);

        public void LogoutUser()
        {
            proxy.Logout();
        }
    }
}
