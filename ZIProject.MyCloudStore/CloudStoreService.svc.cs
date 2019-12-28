using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ZIProject.DatabaseInteraction.Models;
using ZIProject.DatabaseInteraction.DBTools;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using System.ServiceModel.Channels;
using System.ServiceModel.Activation;
using Newtonsoft.Json;
using System.Text;
using System.ServiceModel.Web;
using CryptoCollection;

namespace ZIProject.MyCloudStore
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]  //is this needed?
    public class CloudStoreService : ICloudStoreService
    {
        //Instead of throwing exceptions I should be sending messages

        public Stream GetAllFiles()
        {
            SessionInfo sessionInfo = SessionManager.Instance.CheckForSession(GetChannelIdentification());
            if (sessionInfo == null || sessionInfo.LoggedInUser == null)
            {
                throw new InvalidOperationException("No logged in user!");
            }

            DBContext context = new DBContext();

            List<RemoteFileInfo> result = new List<RemoteFileInfo>();
            try
            {
                RemoteFileInfo tempFile;
                List<DatabaseInteraction.Models.FileInfo> fileInfos = context.GetAllFiles(sessionInfo.LoggedInUser).ToList();
                foreach (var file in fileInfos)
                {
                    tempFile = new RemoteFileInfo();
                    tempFile.Name = file.Name;
                    tempFile.HashValue = file.HashValue;
                    result.Add(tempFile);
                }
            }
            catch (Exception e)
            {
                //Shouldnt throw exceptions
                //throw new Exception("Error fetching files.", e);
            }
            finally
            {
                context.Dispose();
            }

            string toreturn = JsonConvert.SerializeObject(result);
            byte[] resultBytes = Encoding.UTF8.GetBytes(toreturn);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            return new MemoryStream(resultBytes);
        }

        public RemoteUserInfo Login(string username, string password)
        {
            RemoteUserInfo answer = null;

            SHA1Hasher sha1 = new SHA1Hasher();
            sha1.ComputeHash(Encoding.Unicode.GetBytes(password));
            UserInfo userInfo = new UserInfo(username, sha1.HashedString);           

            try
            {
                using (var context = new DBContext())
                {
                    if (context.CheckUserCredentials(userInfo))
                    {
                        SessionManager.Instance.AddSession(GetChannelIdentification(), userInfo);
                        answer = new RemoteUserInfo();
                        answer.Username = userInfo.Username;
                        answer.LeftoverSpace = userInfo.LeftoverSpace;
                    }
                }
            }
            catch (Exception e)
            {
                //Shouldnt throw exceptions
                //throw new Exception("Error logging in.", e);
            }

            return answer;
        }

        public bool Register(string username, string password)
        {
            SHA1Hasher sha1 = new SHA1Hasher();
            sha1.ComputeHash(Encoding.Unicode.GetBytes(password));
            UserInfo userInfo = new UserInfo(username, sha1.HashedString);

            try
            {
                using (var context = new DBContext())
                {
                    if (context.TryAddUser(userInfo) != -1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                //Shouldnt throw exceptions
                //throw new Exception("Error registering the user.", e);
            }

            return false;
        }

        //should be changed so that file length is programatically determined
        public void UploadFile(string filename, string hashValue, string length, Stream fileStream)
        {
            SessionInfo sessionInfo = SessionManager.Instance.CheckForSession(GetChannelIdentification());
            if (sessionInfo == null || sessionInfo.LoggedInUser == null)
            {
                throw new InvalidOperationException("Cannot upload files when the user isn't logged in.");
            }

            var fileInfo = new DatabaseInteraction.Models.FileInfo(filename, hashValue);

            try
            {
                using (var context = new DBContext())
                {
                    //Determine file size
                    long fileSize = int.Parse(length);

                    //Check if user has leftover space for this file (TODO)
                    if (sessionInfo.LoggedInUser.LeftoverSpace < fileSize)
                    {
                        throw new InvalidOperationException("User doesn't have enough leftover allocated space to upload the desired file.");
                    }

                    //Add the file record to the database
                    context.TryAddFile(fileInfo, sessionInfo.LoggedInUser);

                    //Update user leftover space
                    context.UpdateUserLeftoverSpace(sessionInfo.LoggedInUser.ID, sessionInfo.LoggedInUser.LeftoverSpace - fileSize);

                    try
                    {
                        //Upload file
                        if (!FileManager.SaveFile(fileInfo, fileStream, fileSize))
                        {
                            //If file upload fails, revert changes in the database
                            context.RemoveFile(fileInfo.ID);
                            context.UpdateUserLeftoverSpace(sessionInfo.LoggedInUser.ID, sessionInfo.LoggedInUser.LeftoverSpace + fileSize);
                        }
                    }
                    catch (Exception e)
                    {
                        //If file upload fails, revert changes in the database
                        context.RemoveFile(fileInfo.ID);
                        context.UpdateUserLeftoverSpace(sessionInfo.LoggedInUser.ID, sessionInfo.LoggedInUser.LeftoverSpace + fileSize);

                        throw new Exception("File manager error.", e);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error adding a new file.", e);
            }
        }

        public Stream DownloadFile(string fileName)
        {
            SessionInfo sessionInfo = SessionManager.Instance.CheckForSession(GetChannelIdentification());
            if (sessionInfo == null || sessionInfo.LoggedInUser == null)
            {
                throw new InvalidOperationException("Cannot download files when the user isn't logged in.");
            }

            try
            {
                using (var context = new DBContext())
                {
                    DatabaseInteraction.Models.FileInfo file = context.CheckForFile(fileName, sessionInfo.LoggedInUser.ID);

                    if (file != null)
                    {
                        //WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
                        WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";

                        Stream result = FileManager.RetrieveFile(file);

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error accessing a file.", e);
            }
        }

        private string GetChannelIdentification()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties properties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string address = string.Empty;

            //http://www.simosh.com/article/ddbggghj-get-client-ip-address-using-wcf-4-5-remoteendpointmessageproperty-in-load-balanc.html
            if (properties.Keys.Contains(HttpRequestMessageProperty.Name))
            {
                HttpRequestMessageProperty endpointLoadBalancer = properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                if (endpointLoadBalancer != null && endpointLoadBalancer.Headers["X-Forwarded-For"] != null)
                    address = endpointLoadBalancer.Headers["X-Forwarded-For"];
            }

            if (string.IsNullOrEmpty(address))
            {
                return $"{endpoint.Address}:{endpoint.Port}";
            }
            else
            {
                return address;
            }


        }

        public void Logout()
        {
            SessionManager.Instance.RemoveSession(GetChannelIdentification());
        }
    }
}
