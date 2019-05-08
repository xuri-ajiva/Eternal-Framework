using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XOR_Enc.utils
{
    class Crypt

    {
        public static void AES_Encrypt(string inputFile, string outputFile, byte[] passwordBytes)
        {
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var cryptFile = outputFile;
            var fsCrypt = new FileStream(cryptFile, FileMode.Create);

            var aes = new RijndaelManaged {KeySize = 256, BlockSize = 128};



            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Padding = PaddingMode.Zeros;

            aes.Mode = CipherMode.CBC;

            var cs = new CryptoStream(fsCrypt,
                 aes.CreateEncryptor(),
                CryptoStreamMode.Write);

            var fsIn = new FileStream(inputFile, FileMode.Open);

            int data;
            while ((data = fsIn.ReadByte()) != -1)
                cs.WriteByte((byte)data);


            fsIn.Close();
            cs.Close();
            fsCrypt.Close();

        }

        public static void AES_Decrypt(string inputFile, string outputFile, byte[] passwordBytes)
        {



            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var fsCrypt = new FileStream(inputFile, FileMode.Open);

            var aes = new RijndaelManaged();

            aes.KeySize = 256;
            aes.BlockSize = 128;


            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Padding = PaddingMode.Zeros;

            aes.Mode = CipherMode.CBC;

            var cs = new CryptoStream(fsCrypt,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read);

            var fsOut = new FileStream(outputFile, FileMode.Create);

            int data;
            while ((data = cs.ReadByte()) != -1)
                fsOut.WriteByte((byte)data);

            fsOut.Close();
            cs.Close();
            fsCrypt.Close();

        }
    }
    public class Datn
    {
        private string _front = "0x";
        private string _privstr = "00000000";

        public void Set(int index, int vaule)
        {
            _privstr = _privstr.Substring(0, _privstr.Length - index - 1) + vaule.ToString("X") + _privstr.Substring(_privstr.Length - index);
        }
        public string Get { get { return _front + _privstr; } }
        public string GetIndex(int index) { return _privstr.Substring(_privstr.Length - index - 1, 1); }
    }
}
