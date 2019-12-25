using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ZIProject.MyCloudStore;

namespace ZIProject.Client
{
    public class FileController
    {
        //private CloudStoreServiceClient cloudStoreClient;
        private ICloudStoreService proxy;

        public FileController(ICloudStoreService cloudStoreClient)
        {
            this.proxy = cloudStoreClient;
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

        public void LogoutUser()
        {
            proxy.Logout();
        }

        public void RequestFileUpload(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            byte[] fileToSend = File.ReadAllBytes(filePath);

            //calculate file hash TODO
            string hashValue = "hash1";

            //encrypt file bytes TODO

            string FileManagerServiceUrl = "http://localhost:56082/MyCloudStore/CloudStoreService.svc";
            var serviceUrl = string.Format($"{FileManagerServiceUrl}/UploadFile/{fileName}/{hashValue}/{fileToSend.Length}");
            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
            request.Method = "POST";
            request.ContentType = "text/plain";

            System.IO.Stream reqStream = request.GetRequestStream();
            reqStream.Write(fileToSend, 0, fileToSend.Length);
            reqStream.Close();

            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            System.Windows.Forms.MessageBox.Show(string.Format("Client: Receive Response HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription));
            resp.Dispose();
        }

        public void RequestFileDownload(string fileName, string directoryPath)
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

                        byte[] fileBytes = ReadToEnd(fileStream);

                        //decrypt bytes TODO

                        //save them to a location
                        File.WriteAllBytes(Path.Combine(directoryPath, fileName), fileBytes);

                        //open with adequate application TODO

                        fileStream.Close();
                        fileStream.Dispose();

                    }

                    System.Windows.Forms.MessageBox.Show(string.Format("Client: Receive Response HTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription));
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error downloading the file: " + e.GetFullMessage());
            }
        }

        private static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
