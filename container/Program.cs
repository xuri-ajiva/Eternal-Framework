﻿//#define pack

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace container {
    internal class Program {
        public static string filename = "archife.pac";
        public static string[] files = new string[99];
        private static List<string> fs;// = new List<string>(files);

        public static int l => int.MaxValue.ToString().Length;

        private static void Main(string[] args) {
#if DEBUG
            Console.WriteLine( "Debug version!" );
#if pack
            args = new string[] { "p", @"C:\Users\Private\source\repos\Global\Global.sln", @"C:\Users\Private\source\repos\Global\hallo welt.txt", @"C:\Users\Private\Desktop\Roaming - Kopie.rar" };
#else
            args = new string[] { "u", filename };
#endif
            //#warning Using Debug
#endif

            string choise = "";

            if (args.Length > 0)
                choise = args[0];

            ask:
            {
                if (choise == "") {
                    Console.WriteLine( "p/u" );
                    choise = Console.ReadLine().ToLower();
                }

                if (choise == "p") {
                    Console.WriteLine( "Packing..." );
                    fs = new List<string>( args );
                    if (fs.Count > 0) {
                        fs.RemoveAt( 0 );
                        pack();
                    } else Console.WriteLine( "Bitte Dateien zum verpacken angeben!" );
                } else if (choise == "u") {
                    if (args.Length == 2)
                        filename = args[1];
                    else Console.WriteLine( "Bitte archief angeben!" );
                    Console.WriteLine( "UnPacking..." );
                    Unpack();
                } else
                    goto ask;

                for (int i = 5; i > 0; i--) {
                    Console.SetCursorPosition( 0, Console.CursorTop );
                    Console.Write( "Programm exit in " + i.ToString() );
                    Thread.Sleep( 500 );
                }
                //Console.ReadKey();
            }
        }

        private static void pack() {
            List<byte> contend = new List<byte>();

            Console.WriteLine( "Creating Header..." );

            string header = "";
            foreach (string s in fs) {

                Console.WriteLine( "Processing file:" + s );

                var tmp = File.ReadAllBytes( s );
                foreach (byte i in tmp) {
                    contend.Add( i );
                }

                var info = MakeVailed( tmp.Length.ToString() ) + Path.GetFileName( s );
                header += pack( info );
            }
            var head = Encoding.UTF8.GetBytes( Fullsize( pack( header ).Replace( "\n", "" ) ) );

            var final = new byte[contend.Count + head.Length];

            Console.WriteLine( "Creating binery..." );

            int t = 0;
            for (int i = t; i < head.Length; i++, t++) {
                final[t] = head[t];
            }

            for (int i = 0; i < contend.Count; i++, t++) {
                final[t] = contend[i];
            }

            //Console.WriteLine( Encoding.UTF8.GetString( final ) );
            Console.WriteLine( "Writing all to file" );
            File.WriteAllBytes( filename, final );

            Console.WriteLine( "Cleaning..." );
            final = new byte[1];
            contend = new List<byte>();
            head = new byte[1];
            header = "";
        }
        private static void Unpack() {
            List<byte> contend = new List<byte>( File.ReadAllBytes( filename ) );

            Console.WriteLine( "reading header..." );

            int hlength = int.Parse( Encoding.UTF8.GetString( contend.GetRange( 0, 20 ).ToArray() ).Substring( 0, l ) );

            var ascy = Encoding.UTF8.GetString( contend.GetRange( l, hlength ).ToArray() );
            int len;
            string head = restore( ascy, out len );

            ascy = "";
            int fullheadlength = ( len + 2 + head.Length.ToString().Length + 2 ) + l;

            Console.WriteLine( "Isolating contend..." );

            contend = contend.GetRange( fullheadlength, contend.Count - fullheadlength );


            var infos = head.Split( '\n' );

            int c = 1;
            for (int i = 0; i < infos.Length - 1; i++) {
                string p = restore( infos[i], out int length );
                var filename = p.Substring( l );
                int filesize = int.Parse( p.Substring( 0, l ) );

                Console.WriteLine( "Processing file:" + filename );

                File.WriteAllBytes( filename, contend.GetRange( c, filesize ).ToArray() );
                c += filesize;
            }
            Console.WriteLine( "Cleaning..." );
            contend = new List<byte>();
            ascy = "";
        }

        public static string ReverseSize(string daten) {
            string result = "";
            short length = (short) daten.Length;

            for (int i = length.ToString().Length; i < l; i++) {
                result += "0";
            }

            result += length.ToString();

            result += daten;
            return result;
        }

        public static string pack(string daten) {
            string finalData = daten.Replace( "\\", "\\%" ).
                Replace( "\n", "\\/" );

            return "\\&" + finalData.Length.ToString() + "\\$" + finalData + "\n";
        }
        public static string restore(string daten, out int length) {
            if (daten.Substring( 0, 2 ) != "\\&")
                throw new Exception( "No vailet String" );


            daten = daten.Substring( 2 );
            int Il = 0;
            length = 0;
            for (int i = 0; i < long.MaxValue.ToString().Length; i++)
                if (daten.Substring( i, 2 ) == "\\$") {
                    Il = i;
                    length = int.Parse( daten.Substring( 0, i ) );
                    break;
                }

            string rlDaten = daten.Substring( Il + 2, length ).Replace( "\\%", "\\" ).
                Replace( "\\/", "\n" );

            //if (daten.Substring( Il + 2 + length, 2 ) == "\n")
            //    Console.WriteLine( "vailet" );
            return rlDaten;
        }

        public static string Fullsize(string daten) {
            string result = "";
            int length = (int) daten.Length;

            for (int i = length.ToString().Length; i < l; i++) {
                result += "0";
            }

            result += length.ToString();

            result += daten;
            return result;
        }

        public static string MakeVailed(string size) {
            string result = "";

            for (int i = size.Length; i < l; i++) {
                result += "0";
            }

            result += size;
            return result;
        }
    }
}
