using System;
using System.IO;
using System.Text;
using XOR_Enc.utils;

namespace XOR_Enc
{
    internal class Program
    {
        public static string Filename = "";
        protected static string Chois = "";
        protected static string Passwd = "";

        private static void Main(string[] args)
        {
            if(args.Length ==1)
            {
                Filename = args[0];
                Console.WriteLine("passwd");
                Passwd = Console.ReadLine();

                Console.WriteLine("Enc/Dec ? [e/d]");
                Chois = Console.ReadLine();

                Prog();
            }
            else if(args.Length == 3)
            {
                Filename = args[0];
                Passwd = args[1];
                Chois = args[2];

                Prog();
            }
            else
                Console.WriteLine("usage: "+Path.GetFileName( System.Reflection.Assembly.GetExecutingAssembly().Location)+" datei passwd [e/d]");

            Console.WriteLine("Finished!\nProgramm is closing...");
            Console.ReadKey();
        }

        private static void Prog()
        {
            if (Chois.ToLower() == "e")
                utils.Crypt.AES_Encrypt(Filename,
                    Filename + ".enc",
                    Encoding.Unicode.GetBytes(Passwd));
            else if (Chois.ToLower() == "d")
                utils.Crypt.AES_Decrypt(Filename, (Path.GetExtension(Filename) == ".enc") ? Filename.Substring(0, Filename.Length - 4) : "dec",
                    Encoding.Unicode.GetBytes(Passwd));
            Console.WriteLine(Path.GetExtension(Filename));
        }

        public static string EncryptDecrypt(string szPlainText, int szEncryptionKey)
        {
            var szInputStringBuild = new StringBuilder(szPlainText);
            var szOutStringBuild = new StringBuilder(szPlainText.Length);
            char textch;
            for (var iCount = 0; iCount < szPlainText.Length; iCount++)
            {
                textch = szInputStringBuild[iCount];
                textch = (char)(textch ^ szEncryptionKey);
                szOutStringBuild.Append(textch);
            }
            return szOutStringBuild.ToString();
        }
    }
}
