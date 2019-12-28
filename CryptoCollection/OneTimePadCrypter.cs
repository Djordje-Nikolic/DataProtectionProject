using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCollection
{
    public class OneTimePadCrypter
    {
        public byte[] Pad { get; private set; }

        public OneTimePadCrypter(byte[] pad)
        {
            this.Pad = pad;
        }

        public byte[] Encrypt(byte[] data)
        {
            byte[] result = new byte[data.Length];
            for (int j,i = 0; i < data.Length; i++)
            {
                j = i % Pad.Length;
                result[i] = (byte)((uint)data[i] ^ (uint)Pad[j]);
            }
            return result;
        }

        public byte[] Decrypt(byte[] data)
        {
            var result = new byte[data.Length];
            for (int j,i = 0; i < data.Length; i++)
            {
                j = i % Pad.Length;
                result[i] = (byte)((uint)data[i] ^ (uint)Pad[j]);
            }
            return result;
        }
    }
}
