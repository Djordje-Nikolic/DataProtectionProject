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
        internal static bool SaveFile(DatabaseInteraction.Models.FileInfo fileInfo, byte[] fileBytes)
        {
            try
            {
                string path = fileInfo.GetFilePath();
                CreateDirectoryIfNotExists(path);
                using (var newFileStream = File.Create(path))
                {
                    newFileStream.Write(fileBytes, 0, fileBytes.Length);
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

                //byte[] fileBytes = File.ReadAllBytes(path);

                //return new MemoryStream(fileBytes);
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving the file.", e);
            }
        }

        internal static bool RemoveFile(DatabaseInteraction.Models.FileInfo fileInfo)
        {
            try
            {
                string path = fileInfo.GetFilePath();

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void CreateDirectoryIfNotExists(string filePath)
        {
            var directory = new System.IO.FileInfo(filePath).Directory;
            if (directory == null) throw new Exception("Directory could not be determined for the filePath");

            Directory.CreateDirectory(directory.FullName);
        }

        public static byte[] ReadToEnd(System.IO.Stream stream)
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

        internal static long GetLength(Stream stream)
        {
            long length = -1;
            try
            {
                length = stream.Length;
            }
            catch (Exception) { }

            if (length != -1)
                return length;

            try
            {
                long originalPosition = 0;
                long totalBytesRead = 0;

                if (stream.CanSeek)
                {
                    originalPosition = stream.Position;
                    stream.Position = 0;
                }

                try
                {
                    byte[] readBuffer = new byte[4096];

                    int bytesRead;

                    while ((bytesRead = stream.Read(readBuffer, 0, 4096)) > 0)
                    {
                        totalBytesRead += bytesRead;
                    }

                }
                finally
                {
                    if (stream.CanSeek)
                    {
                        stream.Position = originalPosition;
                    }
                }

                length = totalBytesRead;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return length;
        }
    }
}