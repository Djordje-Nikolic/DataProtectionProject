using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCollection
{
    public class TEACrypter
    {
        public static readonly int KeyLengthInBytes = 16;
        public uint[] Key { get; private set; }
        public static byte Pad { get; private set; }

        public TEACrypter(byte[] key)
        {
            if (key.Length != 16)
            {
                throw new ArgumentOutOfRangeException("Key needs to be 128 bits long (16 bytes)");
            }

            Pad = 0xFE;
            this.Key = CryptoHelpers.byteArrayToUintArray(key);
        }

        public static bool CheckIfDataNeedsPadding(byte[] data)
        {
            if (data.Length % 8 == 0)
                return false;
            return true;
        }

        public static byte[] PadData(byte[] data)
        {
            if (data.Length % 8 != 0)
            {
                int numOfPads = 8 - data.Length % 8;

                byte[] paddedData = new byte[data.Length + numOfPads];
                Array.Copy(data, 0, paddedData, 0, data.Length);

                for (int i = 0; i < numOfPads; i++)
                {
                    paddedData[data.Length + i] = Pad;
                }

                return paddedData;
            }

            return data;
        }

        public static byte[] DepadData(byte[] data)
        {
            int i = data.Length - 1;
            while (i >= 0 && data[i] == Pad)
            {
                i--;
            }

            byte[] depadedData = new byte[i + 1];

            Array.Copy(data, 0, depadedData, 0, i + 1);

            return depadedData;
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data.Length % 8 != 0)
                throw new ArgumentOutOfRangeException("Input data needs to be of a length that is a multiple of 8.");

            byte[] result = new byte[data.Length];
            byte[] tempBytes;
            uint[] temp = new uint[2];
            uint[] tempResult;
            for (int i = 0; i < data.Length; i += 8)
            {
                for (int j = i; j < (i + 8); j += 4)
                {
                    uint tempword = 0;
                    tempword += data[j];
                    tempword = tempword << 8;

                    tempword += data[j + 1];
                    tempword = tempword << 8;

                    tempword += data[j + 2];
                    tempword = tempword << 8;

                    tempword += data[j + 3];
                    temp[(j % 8) / 4] = tempword;
                }

                tempResult = Code(temp);
                tempBytes = CryptoHelpers.uintArrayToByteArray(tempResult);
                Array.Copy(tempBytes, 0, result, i, 8);
            }

            return result;
        }

        public byte[] Decrypt(byte[] data)
        {
            if (data.Length % 8 != 0)
                throw new ArgumentOutOfRangeException("Input data needs to be of a length that is a multiple of 8.");

            byte[] result = new byte[data.Length];
            byte[] tempBytes;
            uint[] temp = new uint[2];
            uint[] tempResult;
            for (int i = 0; i < data.Length; i += 8)
            {
                for (int j = i; j < (i + 8); j += 4)
                {
                    uint tempword = 0;
                    tempword += data[j];
                    tempword = tempword << 8;

                    tempword += data[j + 1];
                    tempword = tempword << 8;

                    tempword += data[j + 2];
                    tempword = tempword << 8;

                    tempword += data[j + 3];
                    temp[(j % 8) / 4] = tempword;
                }

                tempResult = Decode(temp);
                tempBytes = CryptoHelpers.uintArrayToByteArray(tempResult);
                Array.Copy(tempBytes, 0, result, i, 8);
            }

            return result;
        }

        private uint[] Code(uint[] doubleUint)
        {
            uint[] result = new uint[2];
            uint v0 = doubleUint[0];
            uint v1 = doubleUint[1];
            uint sum = 0;
            uint delta = 0x9E3779B9;

            uint k0 = Key[0];
            uint k1 = Key[1];
            uint k2 = Key[2];
            uint k3 = Key[3];

            for (int i = 0; i < 32; i++)
            {
                sum += delta;
                v0 += ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                v1 += ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
            }

            result[0] = v0;
            result[1] = v1;

            return result;
        }

        private uint[] Decode(uint[] doubleUint)
        {
            uint[] result = new uint[2];
            uint v0 = doubleUint[0];
            uint v1 = doubleUint[1];
            uint delta = 0x9E3779B9;
            uint sum = delta << 5;

            uint k0 = Key[0];
            uint k1 = Key[1];
            uint k2 = Key[2];
            uint k3 = Key[3];

            for (int i = 0; i < 32; i++)
            {
                v1 -= ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
                v0 -= ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                sum -= delta;
            }

            result[0] = v0;
            result[1] = v1;
            return result;
        }
    }
}
