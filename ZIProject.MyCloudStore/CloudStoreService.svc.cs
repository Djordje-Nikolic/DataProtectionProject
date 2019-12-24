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

namespace ZIProject.MyCloudStore
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]  //is this needed?
    public class CloudStoreService : ICloudStoreService
    {
        //Instead of throwing exceptions I should be sending messages

        public IEnumerable<RemoteFileInfo> GetAllFiles()
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
                    tempFile.ID = file.ID;
                    tempFile.Name = file.Name;
                    tempFile.HashValue = file.HashValue;
                    result.Add(tempFile);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error fetching files.", e);
            }
            finally
            {
                context.Dispose();
            }

            return result;
        }

        public bool Login(string username, string password)
        {
            UserInfo userInfo = new UserInfo(username, password);

            try
            {
                using (var context = new DBContext())
                {
                    if (context.CheckUserCredentials(userInfo))
                    {
                        SessionManager.Instance.AddSession(GetChannelIdentification(), userInfo);
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
                throw new Exception("Error logging in.", e);
            }
        }

        public bool Register(string username, string password)
        {
            UserInfo userInfo = new UserInfo(username, password);

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
                throw new Exception("Error registering the user.", e);
            }
        }

        public void UploadFile(Stream fileStream)
        {
            SessionInfo sessionInfo = SessionManager.Instance.CheckForSession(GetChannelIdentification());
            if (sessionInfo == null || sessionInfo.LoggedInUser == null)
            {
                throw new InvalidOperationException("Cannot upload files when the user isn't logged in.");
            }

            NameValueCollection PostParameters = HttpUtility.ParseQueryString(new StreamReader(fileStream).ReadToEnd());
            string filename = PostParameters["filename"];
            string hashValue = PostParameters["hashvalue"];
            //var img = PostParameters["File"];

            var fileInfo = new DatabaseInteraction.Models.FileInfo(filename, hashValue);

            try
            {
                using (var context = new DBContext())
                {
                    context.TryAddFile(fileInfo, sessionInfo.LoggedInUser);
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
                        string path = file.GetFilePath();
                        return new FileStream(path, FileMode.Open);
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
    }
}
