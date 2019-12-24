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
        bool Register(string username, string password);

        [OperationContract]
        bool Login(string username, string password);

        [OperationContract]
        void UploadFile(System.IO.Stream fileStream);

        [OperationContract]
        System.IO.Stream DownloadFile(string fileName);

        [OperationContract]
        IEnumerable<RemoteFileInfo> GetAllFiles();
    }

    [DataContract]
    public class RemoteFileInfo
    {
        [DataMember]
        public int ID;

        [DataMember]
        public string Name;

        [DataMember]
        public string HashValue;
    }
}
