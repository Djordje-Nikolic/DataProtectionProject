using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ZIProject.MyCloudStore;
using CryptoCollection;
using System.Text;

namespace ZIProject.Client
{
    public class FileController
    {
        //private CloudStoreServiceClient cloudStoreClient;
        private ICloudStoreService proxy;
        private CryptionController cryptionController;

        public FileController(ICloudStoreService cloudStoreClient, CryptionController cryptionController)
        {
            this.proxy = cloudStoreClient;

            this.cryptionController = cryptionController;
        }

        public List<RemoteFileInfo> RequestAllUserFiles()
        {
            WebRequest request = WebRequest.Create("http://localhost:56082/MyCloudStore/CloudStoreService.svc/files");
            WebResponse ws = request.GetResponse();

            List<RemoteFileInfo> files;
            using (var rs = ws.GetResponseStream())
            {
                string result = new StreamReader(rs).ReadToEnd();
                files = JsonConvert.DeserializeObject<List<RemoteFileInfo>>(result);
            }
            return files;
        }

        public void RequestFileUpload(string filePath, CryptoChoice answer, bool replaceOnConflict = false)
        {
            string fileName = Path.GetFileName(filePath);
            byte[] fileBytes = File.ReadAllBytes(filePath);

            SHA1Hasher sha1 = new SHA1Hasher();
            sha1.ComputeHash(fileBytes);
            string hashValue = sha1.HashedString;

            byte[] encryptedBytes;
            try
            {
                encryptedBytes = cryptionController.EncryptFile(fileName, fileBytes, answer, replaceOnConflict);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Couldn't encrypt the file: " + e.Message);
            }

            HttpWebResponse resp = null;
            try
            {
                string FileManagerServiceUrl = "http://localhost:56082/MyCloudStore/CloudStoreService.svc";
                var serviceUrl = string.Format($"{FileManagerServiceUrl}/UploadFile/{fileName}/{hashValue}");
                var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
                request.Method = "POST";
                request.ContentType = "text/plain";

                System.IO.Stream reqStream = request.GetRequestStream();
                reqStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                reqStream.Close();

                resp = (HttpWebResponse)request.GetResponse();
                System.Windows.Forms.MessageBox.Show(string.Format("Client: Receive Response HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription));
            }
            catch (Exception e)
            {
                throw new Exception("Error uploading the file to the service: " + e.GetFullMessage());
            }
            finally
            {
                if (resp != null && resp.StatusCode != HttpStatusCode.OK)
                {
                    cryptionController.RemoveFileRecord(fileName);
                }

                resp.Dispose();
            }
        }

        public void RequestFileDownload(string fileName, string directoryPath, string supposedFileHash)
        {
            string FileManagerServiceUrl = "http://localhost:56082/MyCloudStore/CloudStoreService.svc";
            var serviceUrl = string.Format($"{FileManagerServiceUrl}/DownloadFile/{fileName}");
            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (var fileStream = response.GetResponseStream())
                    {
                        if (fileStream == null)
                        {
                            System.Windows.Forms.MessageBox.Show("File not recieved");
                            return;
                        }

                        byte[] fileBytes = fileStream.ReadToEnd();

                        //decrypt bytes
                        byte[] decryptedFile;
                        try
                        {
                            decryptedFile = cryptionController.DecryptFile(fileBytes, fileName);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error decrypting the downloaded file: " + e.Message);
                        }

                        //Compare hash (double check since the cryptionController already does this
                        SHA1Hasher sha1 = new SHA1Hasher();
                        sha1.ComputeHash(decryptedFile);
                        string hashValue = sha1.HashedString;

                        if (hashValue != supposedFileHash)
                        {
                            System.Windows.Forms.MessageBox.Show("Your downloaded file's hash and its recorded hash before uploading do not match!");
                        }

                        //save them to a location
                        File.WriteAllBytes(Path.Combine(directoryPath, fileName), decryptedFile);

                        //open with adequate application
                        System.Diagnostics.Process.Start(Path.Combine(directoryPath, fileName));
                    }

                    System.Windows.Forms.MessageBox.Show(string.Format("Client: Receive Response HTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription));
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error downloading the file: " + e.GetFullMessage());
            }
        }

        public void RequestRemoveFile(string fileName)
        {
            string FileManagerServiceUrl = "http://localhost:56082/MyCloudStore/CloudStoreService.svc";
            var serviceUrl = string.Format($"{FileManagerServiceUrl}/RemoveFile/{fileName}");
            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {              
                    System.Windows.Forms.MessageBox.Show(string.Format("Client: Receive Response HTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription));

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        cryptionController.RemoveFileRecord(fileName);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error removing the file: " + e.GetFullMessage());
            }
        }
        
    }
}
