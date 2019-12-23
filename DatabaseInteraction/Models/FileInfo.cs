using System;
using System.IO;

namespace ZIProject.DatabaseInteraction.Models
{
    public class FileInfo
    {
        public static readonly string FileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyCloudStore\\DataStore\\");
        public int ID { get; internal set; }
        public string Name { get; internal set; }
        public int UserID { get; internal set; }
        public string HashValue { get; internal set; }
        internal FileInfo() { ID = -1; UserID = -1; }
        public FileInfo(string name, string hashValue)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
            {
                throw new ArgumentOutOfRangeException("Entered name is invalid.");
            }

            if (string.IsNullOrWhiteSpace(hashValue) || hashValue.Length != 40)
            {
                throw new ArgumentOutOfRangeException("Entered hash value is invalid.");
            }

            ID = -1;
            Name = name;
            UserID = -1;
            HashValue = hashValue;
        }
        public string GetFilePath()
        {
            if (ID == -1 || UserID == -1)
            {
                throw new InvalidOperationException("File has to be initialized and attached to a specific user before it's path can be generated.");
            }

            if (!File.Exists(Path.Combine(FileDirectory, $"{UserID}\\")))
            {
                Directory.CreateDirectory(Path.Combine(FileDirectory, $"{UserID}\\"));
            }

            return Path.Combine(FileDirectory, $"{UserID}\\{ID}");
        }
    }
}
