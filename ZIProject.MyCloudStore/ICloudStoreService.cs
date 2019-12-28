using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ZIProject.MyCloudStore
{
   
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICloudStoreService" in both code and config file together.
    [ServiceContract]
    public interface ICloudStoreService
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool Register(string username, string password);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped)]
        RemoteUserInfo Login(string username, string password);

        [OperationContract]
        void Logout();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UploadFile/{filename}/{hashValue}/{length}", BodyStyle = WebMessageBodyStyle.Bare)]
        void UploadFile(string filename, string hashValue, string length, System.IO.Stream fileStream);

        [OperationContract]
        [WebGet(UriTemplate = "DownloadFile/{fileName}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        System.IO.Stream DownloadFile(string fileName);

        [OperationContract]
        [WebGet(UriTemplate = "files", BodyStyle = WebMessageBodyStyle.Bare)]
        System.IO.Stream GetAllFiles();
    }

    public class RemoteFileInfo
    {
        public string Name { get; set; }
        public string HashValue { get; set; }
    }

    [DataContract]
    public class RemoteUserInfo
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public long LeftoverSpace { get; set; }
    }
}
