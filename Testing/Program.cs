using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptoCollection;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] input = new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66 };
            SHA1Hasher sha1 = new SHA1Hasher();
            var sha11 = new SHA1CryptoServiceProvider();

            byte[] hash1 = sha11.ComputeHash(input);
            uint[] hash2 = sha1.ComputeHash(input);

            Console.WriteLine(CryptoHelpers.byteArrayToString(hash1));
            Console.WriteLine(sha1.HashedString);

            string data = "pricam nesto";
            sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
            string hashBefore = sha1.HashedString;
            byte[] key = new byte[16] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76 };
            string key2 = Encoding.UTF8.GetString(key);
            TEACrypter tEACrypter = new TEACrypter(Encoding.UTF8.GetBytes(key2));
            var resultBytes = TEACrypter.DepadData(tEACrypter.Decrypt(tEACrypter.Encrypt(TEACrypter.PadData(Encoding.UTF8.GetBytes(data)))));
            string result = Encoding.UTF8.GetString(resultBytes);
            sha1.ComputeHash(Encoding.UTF8.GetBytes(result));
            string hashAfter = sha1.HashedString;

            if (hashAfter != hashBefore)
                Console.WriteLine("Something wrong");
        }
    }
}
