using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCollection
{
    public class SHA1Hasher
    {
        private readonly uint[] Hash = new uint[5] { 0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0 };

        public uint[] HashedUint { get; private set; } = null;
        public byte[] HashedBytes 
        { 
            get
            {
                if (HashedUint != null)
                    return CryptoHelpers.uintArrayToByteArray(HashedUint);
                return null;
            }
        }
        public string HashedString 
        {
            get
            {
                if (HashedUint != null)
                    return CryptoHelpers.byteArrayToString(HashedBytes);
                return null;
            }
        }

        public SHA1Hasher() { }

        public uint[] ComputeHash(byte[] data)
        {
            byte[] paddedData = PadInput(data);

            const int blockSize = 64;
            uint[] w;
            uint[] words;
            uint A, B, C, D, E, F;
            uint K0, K1, K2, K3;
            uint[] H = new uint[5];
            uint temp;

            H[0] = Hash[0];
            H[1] = Hash[1];
            H[2] = Hash[2];
            H[3] = Hash[3];
            H[4] = Hash[4];

            K0 = 0x5A827999;
            K1 = 0x6ED9EBA1;
            K2 = 0x8F1BBCDC;
            K3 = 0xCA62C1D6;

            for (int i = 0; i < paddedData.Length; i += blockSize)
            {//Process each block
                w = new uint[80];
                words = new uint[16];

                //Split into 16 words
                for (int j = i; j < (i + blockSize); j += 4)
                {
                    uint tempword = 0;
                    tempword += paddedData[j];
                    tempword = tempword << 8;

                    tempword += paddedData[j + 1];
                    tempword = tempword << 8;

                    tempword += paddedData[j + 2];
                    tempword = tempword << 8;

                    tempword += paddedData[j + 3];
                    words[(j % blockSize) / 4] = tempword;
                }

                for (int j = 0; j < 16; j++)
                {
                    w[j] = words[j];
                }

                for (int j = 16; j < 80; j++)
                {
                    w[j] = CryptoHelpers.Rotate(1, (w[j - 3] ^ w[j - 8] ^ w[j - 14] ^ w[j - 16]));
                }

                A = H[0];
                B = H[1];
                C = H[2];
                D = H[3];
                E = H[4];

                for (int j = 0; j < 20; j++)
                {
                    F = (B & C) | ((~B) & D);
                    temp = CryptoHelpers.Rotate(5, A) + F + E + K0 + w[j];
                    E = D;
                    D = C;
                    C = CryptoHelpers.Rotate(30, B);
                    B = A;
                    A = temp;
                }

                for (int j = 20; j < 40; j++)
                {
                    F = (B ^ C ^ D);
                    temp = CryptoHelpers.Rotate(5, A) + F + E + K1 + w[j];
                    E = D;
                    D = C;
                    C = CryptoHelpers.Rotate(30, B);
                    B = A;
                    A = temp;
                }

                for (int j = 40; j < 60; j++)
                {
                    F = (B & C) | (B & D) | (C & D);
                    temp = CryptoHelpers.Rotate(5, A) + F + E + K2 + w[j];
                    E = D;
                    D = C;
                    C = CryptoHelpers.Rotate(30, B);
                    B = A;
                    A = temp;
                }

                for (int j = 60; j < 80; j++)
                {
                    F = (B ^ C ^ D);
                    temp = CryptoHelpers.Rotate(5, A) + F + E + K3 + w[j];
                    E = D;
                    D = C;
                    C = CryptoHelpers.Rotate(30, B);
                    B = A;
                    A = temp;
                }

                H[0] += A;
                H[1] += B;
                H[2] += C;
                H[3] += D;
                H[4] += E;
            }

            HashedUint = H;
            return H;
        }

        private static byte[] PadInput(byte[] data)
        {
            int dataLen = data.Length;

            //Calculating the number of empty bytes that need to be padded
            long numOfBytesToPad = 64*((dataLen % 64 - 1) / 55) - (dataLen % 64) + 55;

            byte firstPad = 128;
            byte restByte = 0;

            byte[] result = new byte[dataLen + 1 + numOfBytesToPad + 8];

            //Move the data to result
            Array.Copy(data, 0, result, 0, dataLen);

            //Add the first byte (only leftmost bit is set)
            result[dataLen] = firstPad;

            //Add the needed number of empty bytes
            for (int i = dataLen + 1; i < dataLen + 1 + numOfBytesToPad; i++)
            {
                result[i] = restByte;
            }

            //Convert the data length to big endian 64bit integer
            byte[] dataLenForPad = BitConverter.GetBytes((ulong)dataLen * 8);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(dataLenForPad);
            }

            //Add the length at the end
            Array.Copy(dataLenForPad, 0, result, dataLen + 1 + numOfBytesToPad, dataLenForPad.Length);

            return result;
        }
    }
}
