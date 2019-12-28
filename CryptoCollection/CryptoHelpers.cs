using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CryptoCollection
{
    public enum CryptoChoices
    {
        OneTimePad,
        TEA,
        ElGamal
    }
    public static class CryptoHelpers
    {
        public static uint Rotate(int numOfBits, uint word)
        {
            uint output = (word << numOfBits | word >> (32 - numOfBits));
            return output;
        }

        public static string byteArrayToString(byte[] input)
        {
            StringBuilder tempst = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
                tempst.Append(input[i].ToString("X2"));
            return tempst.ToString();
        }

        public static uint[] byteArrayToUintArray(byte[] input)
        {
            uint[] result = new uint[input.Length / 4];
            for (int j = 0; j < input.Length; j += 4)
            {
                uint tempword = 0;
                tempword += input[j];
                tempword = tempword << 8;

                tempword += input[j + 1];
                tempword = tempword << 8;

                tempword += input[j + 2];
                tempword = tempword << 8;

                tempword += input[j + 3];
                result[j / 4] = tempword;
            }
            return result;
        }

        public static byte[] uintArrayToByteArray(uint[] input)
        {
            byte[] bytes = new byte[input.Length * 4];
            byte[] temp;
            for (int i = 0; i < input.Length; i++)
            {
                temp = BitConverter.GetBytes(input[i]);
                Array.Reverse(temp);
                Array.Copy(temp, 0, bytes, i * 4, 4);
            }
            return bytes;
        }

        public static byte[] ReadAllBytes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File doesn't exist.");
            }

            byte[] fileBytes = null;
            using (var fileStream = File.OpenRead(filePath))
            {
                using (var binaryReader = new BinaryReader(fileStream, Encoding.UTF8))
                {
                    fileBytes = binaryReader.ReadBytes((int) fileStream.Length);
                }
            }

            return fileBytes;
        }

        public static void WriteAllBytes(string filePath, byte[] fileBytes)
        {
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8))
                {
                    binaryWriter.Write(fileBytes, 0, fileBytes.Length);
                }
            }
        }

        public static byte[] GenerateRandomByteArray(long length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException();

            Random random = new Random();
            byte[] result = new byte[length];
            random.NextBytes(result);

            return result;
        }
    }
}
