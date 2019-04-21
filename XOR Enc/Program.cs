using System;
using System.IO;
using System.Text;
using XOR_Enc.utils;

namespace XOR_Enc
{
    internal class Program
    {
        public static string filename = "";
        protected static string chois = "";
        protected static string passwd = "";

        private static void Main(string[] args)
        {
            if(args.Length ==1)
            {
                filename = args[0];
                Console.WriteLine("passwd");
                passwd = Console.ReadLine();

                Console.WriteLine("Enc/Dec ? [e/d]");
                chois = Console.ReadLine();

                prog();
            }
            else if(args.Length == 3)
            {
                filename = args[0];
                passwd = args[1];
                chois = args[2];

                prog();
            }
            else
                Console.WriteLine("usage: "+Path.GetFileName( System.Reflection.Assembly.GetExecutingAssembly().Location)+" datei passwd [e/d]");

            Console.WriteLine("Finished!\nProgramm is closing...");
            Console.ReadKey();
        }

        private static void prog()
        {
            if (chois.ToLower() == "e")
                utils.crypt.AES_Encrypt(filename,
                    filename + ".enc",
                    Encoding.Unicode.GetBytes(passwd));
            else if (chois.ToLower() == "d")
                utils.crypt.AES_Decrypt(filename, (Path.GetExtension(filename) == ".enc") ? filename.Substring(0, filename.Length - 4) : "dec",
                    Encoding.Unicode.GetBytes(passwd));
            Console.WriteLine(Path.GetExtension(filename));
        }

        public static string EncryptDecrypt(string szPlainText, int szEncryptionKey)
        {
            StringBuilder szInputStringBuild = new StringBuilder(szPlainText);
            StringBuilder szOutStringBuild = new StringBuilder(szPlainText.Length);
            char Textch;
            for (int iCount = 0; iCount < szPlainText.Length; iCount++)
            {
                Textch = szInputStringBuild[iCount];
                Textch = (char)(Textch ^ szEncryptionKey);
                szOutStringBuild.Append(Textch);
            }
            return szOutStringBuild.ToString();
        }
    }
}
