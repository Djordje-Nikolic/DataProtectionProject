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
                var sum = (int)data[i] + (int)Pad[j];
                if (sum > 255)
                    sum -= 255;
                result[i] = (byte)sum;
            }
            return result;
        }

        public byte[] Decrypt(byte[] data)
        {
            var result = new byte[data.Length];
            for (int j,i = 0; i < data.Length; i++)
            {
                j = i % Pad.Length;
                var dif = (int)data[i] - (int)Pad[j];
                if (dif < 0)
                    dif += 255;
                result[i] = (byte)dif;
            }
            return result;
        }
    }
}
