using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZIProject.Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new LoginForm());

            //CryptionController cryptionController = new CryptionController(new MyCloudStore.RemoteUserInfo() { Username = "djole" });
            //byte[] fileBytes = File.ReadAllBytes("C:\\Users\\DjordjeNikolic\\Documents\\MyCloudStore\\DataStore\\3\\15");
            //byte[] decryptedBytes = cryptionController.DecryptFile(fileBytes, "15231539_1300630036655118_164251479_o.jpg");
            //File.WriteAllBytes("C:\\drivers2.jpg", decryptedBytes);

            CryptionController cryptionController = new CryptionController(new MyCloudStore.RemoteUserInfo() { Username = "djole" });
            byte[] fileBytes = File.ReadAllBytes("C:\\Users\\DjordjeNikolic\\Desktop\\Djole shit\\Cooking\\15591782_1328152383902883_659942100_n.jpg");
            byte[] encryptedBytes = cryptionController.EncryptFile("15591782_1328152383902883_659942100_n.jpg", fileBytes, new CryptoChoice() { Choice = CryptoCollection.CryptoChoices.OneTimePad, Key = Encoding.UTF8.GetBytes("sifra") });
            byte[] decryptedBytes = cryptionController.DecryptFile(fileBytes, "15591782_1328152383902883_659942100_n.jpg");
            File.WriteAllBytes("C:\\drivers2.jpg", decryptedBytes);
        }
    }
}
