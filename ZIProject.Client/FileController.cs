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
        private static string CryptoDataLocation = "C:\\cryptodata.txt";
        //private CloudStoreServiceClient cloudStoreClient;
        private ICloudStoreService proxy;

        public FileController(ICloudStoreService cloudStoreClient)
        {
            this.proxy = cloudStoreClient;

            if (!File.Exists(CryptoDataLocation))
            {
                File.Create(CryptoDataLocation);
				OneTimePadCrypter otp = new OneTimePadCrypter(Encoding.Unicode.GetBytes("defaultsifra"));
				byte[] cryptoFileBytes = File.ReadAllBytes(CryptoDataLocation);
                byte[] encryptedCryptoFileBytes = otp.Encrypt(cryptoFileBytes);
                File.WriteAllBytes(CryptoDataLocation, encryptedCryptoFileBytes);
            }
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

        public void RequestFileUpload(string filePath, InputForms.CryptoChoice answer)
        {
            string fileName = Path.GetFileName(filePath);
            byte[] fileToSend = File.ReadAllBytes(filePath);

            SHA1Hasher sha1 = new SHA1Hasher();
            sha1.ComputeHash(fileToSend);
            string hashValue = sha1.HashedString;

            byte[] encryptedFile;
            switch (answer.Choice)
            {
                case InputForms.CryptoChoices.OneTimePad:
                    OneTimePadCrypter oneTimePadCrypter = new OneTimePadCrypter(answer.Key);
                    encryptedFile = oneTimePadCrypter.Encrypt(fileToSend);
                    break;
                case InputForms.CryptoChoices.TEA:
                    TEACrypter tea = new TEACrypter(answer.Key);
                    if (TEACrypter.CheckIfDataNeedsPadding(fileToSend))
                    {
                        System.Windows.Forms.MessageBox.Show("Your file had to be padded! Remember to choose depading when downloading the file!");
                        encryptedFile = tea.Encrypt(TEACrypter.PadData(fileToSend));
                    }
                    else
                    {
                        encryptedFile = tea.Encrypt(fileToSend);
                    }
                    break;
                default:
                    encryptedFile = fileToSend;
                    break;

            }

            string FileManagerServiceUrl = "http://localhost:56082/MyCloudStore/CloudStoreService.svc";
            var serviceUrl = string.Format($"{FileManagerServiceUrl}/UploadFile/{fileName}/{hashValue}/{encryptedFile.Length}");
            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
            request.Method = "POST";
            request.ContentType = "text/plain";

            System.IO.Stream reqStream = request.GetRequestStream();
            reqStream.Write(encryptedFile, 0, encryptedFile.Length);
            reqStream.Close();

            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            System.Windows.Forms.MessageBox.Show(string.Format("Client: Receive Response HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription));


            if (resp.StatusCode == HttpStatusCode.OK)
            {
                OneTimePadCrypter otp = new OneTimePadCrypter(Encoding.Unicode.GetBytes("defaultsifra"));
                byte[] cryptoFileBytes = File.ReadAllBytes(CryptoDataLocation);
                byte[] decryptedCryptoFileBytes = otp.Decrypt(cryptoFileBytes);
                File.WriteAllBytes(CryptoDataLocation, decryptedCryptoFileBytes);
                using (var stream = new FileStream(CryptoDataLocation, FileMode.Append))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine($"{filePath} ::: {answer.Choice.ToString()} ::: {Encoding.UTF8.GetString(answer.Key)} ::: {answer.Depad.ToString()}");
                    }
                }
                cryptoFileBytes = File.ReadAllBytes(CryptoDataLocation);
                byte[] encryptedCryptoFileBytes = otp.Encrypt(cryptoFileBytes);
                File.WriteAllBytes(CryptoDataLocation, encryptedCryptoFileBytes);
            }

            resp.Dispose();
        }

        public void RequestFileDownload(string fileName, string directoryPath, InputForms.CryptoChoice answer, string supposedFileHash)
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

                        //decrypt bytes
                        byte[] decryptedFile;
                        switch (answer.Choice)
                        {
                            case InputForms.CryptoChoices.OneTimePad:
                                OneTimePadCrypter oneTimePadCrypter = new OneTimePadCrypter(answer.Key);
                                decryptedFile = oneTimePadCrypter.Decrypt(fileBytes);
                                break;
                            case InputForms.CryptoChoices.TEA:
                                TEACrypter tea = new TEACrypter(answer.Key);
                                if (answer.Depad)
                                {
                                    System.Windows.Forms.MessageBox.Show("Your file was depaded.");
                                    decryptedFile = tea.Decrypt(TEACrypter.DepadData(fileBytes));
                                }
                                else
                                {
                                    decryptedFile = tea.Decrypt(fileBytes);
                                }
                                break;
                            default:
                                decryptedFile = fileBytes;
                                break;

                        }

                        //Compare hash
                        SHA1Hasher sha1 = new SHA1Hasher();
                        sha1.ComputeHash(decryptedFile);
                        string hashValue = sha1.HashedString;

                        if (hashValue != supposedFileHash)
                        {
                            System.Windows.Forms.MessageBox.Show("Your downloaded file's hash and its recorded hash before uploading do not match!");
                        }

                        //save them to a location
                        File.WriteAllBytes(Path.Combine(directoryPath, fileName), fileBytes);

                        //open with adequate application
                        System.Diagnostics.Process.Start(Path.Combine(directoryPath, fileName));

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
