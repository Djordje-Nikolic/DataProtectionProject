using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZIProject.DatabaseInteraction.Models;

namespace ZIProject.MyCloudStore
{
    public class FileManager
    {
        internal static bool SaveFile(DatabaseInteraction.Models.FileInfo fileInfo, Stream fileStream, long fileSize)
        {
            try
            {
                string path = fileInfo.GetFilePath();
                CreateDirectoryIfNotExists(path);
                using (var newFileStream = File.Create(path))
                {
                    fileStream.CopyTo(newFileStream);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        internal static Stream RetrieveFile(DatabaseInteraction.Models.FileInfo fileInfo)
        {
            try
            {
                string path = fileInfo.GetFilePath();

                var fileName = Path.GetFileName(path);
                return File.OpenRead(path);
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving the file.", e);
            }
        }

        private static void CreateDirectoryIfNotExists(string filePath)
        {
            var directory = new System.IO.FileInfo(filePath).Directory;
            if (directory == null) throw new Exception("Directory could not be determined for the filePath");

            Directory.CreateDirectory(directory.FullName);
        }
    }
}