using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZIProject.Client.InputForms;
using ZIProject.Client.Properties;
using CryptoCollection;

namespace ZIProject.Client
{
    public struct CryptoChoice
    {
        public byte[] Key;
        public CryptoChoices Choice;
        public bool Depad;
    }

    public class CryptionController
    {
        private string IdentificationUsername = null;

        public CryptionController(MyCloudStore.RemoteUserInfo userInfo)
        {
            if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.Username))
                throw new ArgumentOutOfRangeException("User info can't be null and username can't be an empty string.");

            IdentificationUsername = userInfo.Username;

            if (!File.Exists(Settings.Default.CryptionDataFilePath))
            {
                var fileStream = File.Create(Settings.Default.CryptionDataFilePath);
                fileStream.Dispose();

                byte[] fileBytes = ReadDataFile();
                byte[] encryptedBytes = EncryptDataFile(fileBytes);

                WriteDataFile(encryptedBytes);
            }
        }

        public byte[] EncryptFile(string fileName, byte[] fileBytes, CryptoChoice settings, bool replaceRecordIfConflict = false)
        {
            if (ReadRecord(fileName) != null)
            {
                if (replaceRecordIfConflict)
                {
                    RemoveRecord(fileName);
                }
                else
                {
                    throw new InvalidOperationException("The record for a file with this name already exists. If you want to replace it, pass another bool as an argument to the method.");
                }
            }

            SHA1Hasher sha1 = new SHA1Hasher();
            sha1.ComputeHash(fileBytes);
            string fileHash = sha1.HashedString;

            byte[] encryptedBytes;

            switch (settings.Choice)
            {
                case CryptoChoices.OneTimePad:
                    OneTimePadCrypter otp = new OneTimePadCrypter(settings.Key);
                    encryptedBytes = otp.Encrypt(fileBytes);
                    break;
                case CryptoChoices.TEA:
                    TEACrypter tea = new TEACrypter(settings.Key);
                    if (TEACrypter.CheckIfDataNeedsPadding(fileBytes))
                    {
                        fileBytes = TEACrypter.PadData(fileBytes);
                        settings.Depad = true;
                    }
                    encryptedBytes = tea.Encrypt(fileBytes);
                    break;
                default:
                    throw new InvalidOperationException("Cryption settings are not valid: Desired cryption algorithm doesn't exist.");
            }

            AddRecord(fileName, settings, fileHash);

            return encryptedBytes;
        }

        public byte[] DecryptFile(byte[] encryptedBytes, string fileName)
        {
            var foundRecord = ReadRecord(fileName);

            if (foundRecord == null)
            {
                throw new ArgumentException("There is no record for this file.");
            }

            CryptoChoice settings = foundRecord.Item1;
            string originalFileHash = foundRecord.Item2;

            byte[] fileBytes;
            switch (settings.Choice)
            {
                case CryptoChoices.OneTimePad:
                    OneTimePadCrypter otp = new OneTimePadCrypter(settings.Key);
                    fileBytes = otp.Decrypt(encryptedBytes);
                    break;
                case CryptoChoices.TEA:
                    TEACrypter tea = new TEACrypter(settings.Key);
                    fileBytes = tea.Decrypt(encryptedBytes);
                    if (settings.Depad)
                    {
                        fileBytes = TEACrypter.DepadData(fileBytes);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Cryption settings are not valid: Desired cryption algorithm doesn't exist.");
            }

            SHA1Hasher sha1 = new SHA1Hasher();
            sha1.ComputeHash(fileBytes);

            if (originalFileHash != sha1.HashedString)
            {
                throw new Exception("The hash value of the decrypted file and the original file hash value do not match. Access denied.");
            }

            return fileBytes;
        }

        internal byte[] DecryptFileExplicit(byte[] encryptedBytes, CryptoChoice settings)
        {
            byte[] fileBytes;
            switch (settings.Choice)
            {
                case CryptoChoices.OneTimePad:
                    OneTimePadCrypter otp = new OneTimePadCrypter(settings.Key);
                    fileBytes = otp.Decrypt(encryptedBytes);
                    break;
                case CryptoChoices.TEA:
                    TEACrypter tea = new TEACrypter(settings.Key);
                    fileBytes = tea.Decrypt(encryptedBytes);
                    if (settings.Depad)
                    {
                        fileBytes = TEACrypter.DepadData(fileBytes);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Cryption settings are not valid: Desired cryption algorithm doesn't exist.");
            }

            return fileBytes;
        }

        public void RemoveFileRecord(string fileName) => RemoveRecord(fileName);

        #region Records manipulation
        private Tuple<CryptoChoice,string> ReadRecord(string fileName)
        {
            byte[] pattern = GenerateBytePattern(fileName);

            byte[] encryptedDataFile = ReadDataFile();
            byte[] fileBytes = DecryptDataFile(encryptedDataFile);

            int recordStartIndex = FindPattern(fileBytes, pattern);

            if (recordStartIndex != -1)
            {
                int recordEnd = FindRecordEnd(fileBytes, recordStartIndex);

                byte[] recordBytes = fileBytes.Skip(recordStartIndex).Take(recordEnd - recordStartIndex).ToArray();

                string record = Encoding.UTF8.GetString(recordBytes);

                string[] recordParameters = record.Split(':');
                if (recordParameters.Length != 6)
                {
                    throw new Exception("Record is improperly stored.");
                }

                string fileHash = recordParameters[2];

                CryptoChoices choice;
                Enum.TryParse(recordParameters[3], out choice);

                byte[] key = Encoding.UTF8.GetBytes(recordParameters[4]);

                bool depad;
                Boolean.TryParse(recordParameters[5], out depad);

                return new Tuple<CryptoChoice, string> (new CryptoChoice() { Choice = choice, Key = key, Depad = depad }, fileHash);
            }

            return null;
        }
        private void AddRecord(string fileName, CryptoChoice settings, string fileHash)
        {
            byte[] encryptedDataFile = ReadDataFile();
            byte[] fileBytes = DecryptDataFile(encryptedDataFile);

            SHA1Hasher sha1 = new SHA1Hasher();
            sha1.ComputeHash(Encoding.UTF8.GetBytes(IdentificationUsername));
            SHA1Hasher sha2 = new SHA1Hasher();
            sha2.ComputeHash(Encoding.UTF8.GetBytes(fileName));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{sha2.HashedString}:{sha1.HashedString}:{fileHash}:{settings.Choice.ToString()}:{Encoding.UTF8.GetString(settings.Key)}:{settings.Depad}");
            byte[] recordBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());

            byte[] newFileBytes = new byte[fileBytes.Length + recordBytes.Length];
            Array.Copy(fileBytes, 0, newFileBytes, 0, fileBytes.Length);
            Array.Copy(recordBytes, 0, newFileBytes, fileBytes.Length, recordBytes.Length);

            encryptedDataFile = EncryptDataFile(newFileBytes);

            WriteDataFile(encryptedDataFile);
        }
        private void RemoveRecord(string fileName)
        {
            byte[] pattern = GenerateBytePattern(fileName);

            byte[] encryptedDataFile = ReadDataFile();
            byte[] fileBytes = DecryptDataFile(encryptedDataFile);

            int recordStartIndex = FindPattern(fileBytes, pattern);

            if (recordStartIndex != -1)
            {
                int recordEnd = FindRecordEnd(fileBytes, recordStartIndex);

                byte[] newFileBytes = new byte[fileBytes.Length - (recordEnd - recordStartIndex)];
                Array.Copy(fileBytes, 0, newFileBytes, 0, recordStartIndex);
                Array.Copy(fileBytes, recordEnd, newFileBytes, recordStartIndex, fileBytes.Length - recordEnd);

                encryptedDataFile = EncryptDataFile(newFileBytes);

                WriteDataFile(encryptedDataFile);
            }
        }
        private int FindRecordEnd(byte[] fileBytes, int recordIndex)
        {
            if (recordIndex >= fileBytes.Length)
                throw new ArgumentOutOfRangeException();

            int foundAt = -1;

            byte[] newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
            byte[] falseBytes = Encoding.UTF8.GetBytes(false.ToString());
            byte[] trueBytes = Encoding.UTF8.GetBytes(true.ToString());

            foundAt = FindPattern(fileBytes, newLineBytes, recordIndex);
            if (foundAt == -1)
            {
                foundAt = FindPattern(fileBytes, falseBytes, recordIndex);

                if (foundAt == -1)
                {
                    foundAt = FindPattern(fileBytes, trueBytes, recordIndex);

                    if (foundAt == -1)
                    {
                        throw new InvalidOperationException("No record starting at given index.");
                    }

                    foundAt += trueBytes.Length;
                }
                else
                {
                    foundAt += falseBytes.Length;
                }
            }
            else
            {
                foundAt += newLineBytes.Length;
            }

            return foundAt;
        }
        private int FindPattern(byte[] fileBytes, byte[] pattern, int startingIndex = 0)
        {
            if (startingIndex > fileBytes.Length - pattern.Length)
                throw new ArgumentOutOfRangeException();

            int foundAt = -1;

            for (int j,i = startingIndex; i < fileBytes.Length - pattern.Length; i++)
            {
                for (j = 0; j < pattern.Length; j++)
                {
                    if (pattern[j] != fileBytes[i + j])
                        break;
                }

                if (j == pattern.Length)
                {
                    foundAt = i;
                    break;
                }
            }

            return foundAt;
        }
        private byte[] GenerateBytePattern(string fileName)
        {
            SHA1Hasher sha1 = new SHA1Hasher();
            sha1.ComputeHash(Encoding.UTF8.GetBytes(IdentificationUsername));
            SHA1Hasher sha2 = new SHA1Hasher();
            sha2.ComputeHash(Encoding.UTF8.GetBytes(fileName));

            return Encoding.UTF8.GetBytes(sha2.HashedString + ":" + sha1.HashedString);
        }
        #endregion

        #region Data File Methods
        private byte[] ReadDataFile()
        {
            return File.ReadAllBytes(Settings.Default.CryptionDataFilePath);
        }
        private void WriteDataFile(byte[] fileBytes)
        {
            File.WriteAllBytes(Settings.Default.CryptionDataFilePath, fileBytes);
        }
        private byte[] DecryptDataFile(byte[] encryptedBytes)
        {
            byte[] fileBytes;

            switch (Settings.Default.DataFileCryption)
            {
                case CryptoChoices.OneTimePad:
                    OneTimePadCrypter otp = new OneTimePadCrypter(Encoding.UTF8.GetBytes(Settings.Default.CryptionDataKey));
                    fileBytes = otp.Decrypt(encryptedBytes);
                    break;
                case CryptoChoices.TEA:
                    TEACrypter tea = new TEACrypter(Encoding.UTF8.GetBytes(Settings.Default.CryptionDataKey));
                    fileBytes = tea.Decrypt(encryptedBytes);
                    if (Settings.Default.DataFilePadded)
                    {
                        fileBytes = TEACrypter.DepadData(fileBytes);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Cryption settings are not valid: Desired cryption algorithm doesn't exist.");
            }

            return fileBytes;
        }
        private byte[] EncryptDataFile(byte[] fileBytes)
        {
            byte[] encryptedBytes;
            switch (Settings.Default.DataFileCryption)
            {
                case CryptoChoices.OneTimePad:
                    OneTimePadCrypter otp = new OneTimePadCrypter(Encoding.UTF8.GetBytes(Settings.Default.CryptionDataKey));                   
                    encryptedBytes = otp.Encrypt(fileBytes);
                    break;
                case CryptoChoices.TEA:
                    TEACrypter tea = new TEACrypter(Encoding.UTF8.GetBytes(Settings.Default.CryptionDataKey));
                    if (TEACrypter.CheckIfDataNeedsPadding(fileBytes))
                    {
                        fileBytes = TEACrypter.PadData(fileBytes);
                        Settings.Default.DataFilePadded = true;
                        Settings.Default.Save();
                    }
                    encryptedBytes = tea.Encrypt(fileBytes);
                    break;
                default:
                    throw new InvalidOperationException("Cryption settings are not valid: Desired cryption algorithm doesn't exist.");
            }

            return encryptedBytes;
        }
        private void DeleteDataFile(bool doThrow = true)
        {
            string path = Settings.Default.CryptionDataFilePath;
            if (path != null && File.Exists(path))
                File.Delete(path);
            else if (doThrow)
                throw new InvalidOperationException("Couldn't delete the data file.");
        }
        #endregion

        #region Data File Settings

        internal void ChangeDataFileLocation(string newPath)
        {
            byte[] fileBytes = null;
            string oldPath = null;
            try
            {
                oldPath = Settings.Default.CryptionDataFilePath;
                fileBytes = ReadDataFile();

                DeleteDataFile();

                Settings.Default.CryptionDataFilePath = newPath;
                WriteDataFile(fileBytes);
            }
            catch (Exception e)
            {
                if (fileBytes != null)
                {
                    DeleteDataFile(false);
                    Settings.Default.CryptionDataFilePath = oldPath;
                    WriteDataFile(fileBytes);
                }

                throw new InvalidOperationException("Couldn't change the data file location: " + e.GetFullMessage());
            }
            finally
            {
                Settings.Default.Save();
            }

        }

        internal void ChangeDataFileEncryption(CryptoChoices newChoice, string newKey)
        {
            byte[] encryptedBytes = null;
            byte[] fileBytes = null;
            byte[] newEncryptedBytes = null;
            CryptoChoices oldChoice = Settings.Default.DataFileCryption;
            string oldKey = Settings.Default.CryptionDataKey;
            bool oldDepad = Settings.Default.DataFilePadded;
            try
            {
                encryptedBytes = ReadDataFile();
                fileBytes = DecryptDataFile(encryptedBytes);

                Settings.Default.CryptionDataKey = newKey;
                Settings.Default.DataFileCryption = newChoice;

                newEncryptedBytes = EncryptDataFile(fileBytes);
                WriteDataFile(newEncryptedBytes);
            }
            catch (Exception e)
            {
                if (newEncryptedBytes != null)
                {
                    Settings.Default.DataFilePadded = oldDepad;
                    Settings.Default.CryptionDataKey = oldKey;
                    Settings.Default.DataFileCryption = oldChoice;
                }

                throw new InvalidOperationException("Couldn't change the data file location: " + e.GetFullMessage());
            }
            finally
            {
                Settings.Default.Save();
            }
        }

        #endregion
    }
}
