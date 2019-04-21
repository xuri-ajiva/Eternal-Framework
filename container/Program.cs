using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace container
{
    internal class Program
    {
        public static string filename = "archife.pac";
        public static string[] files = new string[] { "..\\..\\..\\hallo welt.txt", "..\\..\\..\\Global.sln" };
        private static List<string> fs;// = new List<string>(files);

        public static int l { get { return int.MaxValue.ToString().Length; } }

        private static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Debug version!");
            //#warning Using Debug
#endif

            string choise = "";

            if (args.Length > 0)
                choise = args[0];

            ask:
            {
                if (choise == "")
                {
                    Console.WriteLine("p/u");
                    choise = Console.ReadLine().ToLower();
                }

                if (choise == "p")
                {
                    fs = new List<string>(args);
                    if (fs.Count > 0)
                    {
                        fs.RemoveAt(0);
                        pack();
                    }
                    else Console.WriteLine("Bitte Dateien zum verpacken angeben!");
                }
                else if (choise == "u")
                {
                    if (args.Length == 2)
                        filename = args[1];
                    else Console.WriteLine("Bitte archief angeben!");
                    Unpack();
                }
                else
                    goto ask;
                for (int i = 0; i < 6; i++)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("Programm exit in " + i.ToString());
                    Thread.Sleep(500);
                }
                //Console.ReadKey();
            }
        }

        private static void pack()
        {
            List<byte> contend = new List<byte>();

            string header = MakeVailed(fs.Count.ToString()); ;
            foreach (string s in fs)
            {
                var tmp = File.ReadAllBytes(s);
                foreach (byte i in tmp)
                {
                    contend.Add(i);
                }

                var info = MakeVailed(tmp.Length.ToString()) + Path.GetFileName(s);
                header += Fullsize(info);
            }

            var head = Encoding.UTF8.GetBytes(Fullsize(header));

            var final = new byte[contend.Count + head.Length];

            int t = 0;
            for (int i = t; i < head.Length; i++, t++)
            {
                final[t] = head[t];
            }

            for (int i = 0; i < contend.Count; i++, t++)
            {
                final[t] = contend[i];
            }

            Console.WriteLine(Encoding.UTF8.GetString(final));

            File.WriteAllBytes(filename, final);
        }
        private static void Unpack()
        {
            List<byte> contend = new List<byte>(File.ReadAllBytes(filename));


            //Console.WriteLine(Encoding.UTF8.GetString(final));

            var ascy = Encoding.UTF8.GetString(contend.ToArray());

            int headlength = int.Parse(ascy.Substring(0, l));

            var head = ascy.Substring(l * 2, headlength - l);
            int filescount = int.Parse(ascy.Substring(l, l));
            ascy = "";

            for (int m = 0; m < (headlength + l); m++)
            {
                contend.RemoveAt(0);
            }

            headlength -= l;

            //File.WriteAllBytes("aio.txt", contend.ToArray());

            int t = 0;
            int c = 0;
            for (int i = 0; i < filescount; i++)
            {
                var itemlength = int.Parse(head.Substring(t, l));
                t += l;
                var filesize = int.Parse(head.Substring(t, l));
                t += l;

                var file = contend.GetRange(c, filesize);
                c += filesize;

                var filename = head.Substring(t, itemlength - l);

                File.WriteAllBytes(filename, file.ToArray());
                t += itemlength - l;
            }
        }

        public static string ReverseSize(string daten)
        {
            string result = "";
            short length = (short)daten.Length;

            for (int i = length.ToString().Length; i < l; i++)
            {
                result += "0";
            }

            result += length.ToString();

            result += daten;
            return result;
        }

        public static string Fullsize(string daten)
        {
            string result = "";
            short length = (short)daten.Length;

            for (int i = length.ToString().Length; i < l; i++)
            {
                result += "0";
            }

            result += length.ToString();

            result += daten;
            return result;
        }
        public static string MakeVailed(string size)
        {
            string result = "";

            for (int i = size.Length; i < l; i++)
            {
                result += "0";
            }

            result += size;
            return result;
        }
    }
}
