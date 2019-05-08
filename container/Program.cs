//#define pack

using Eternal.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace container {
    public class Program {
        public static string filename = "archife.pac";
        public static string[] files = new string[99];
        private static List<string> fs;// = new List<string>(files);

        public static int IntLength => int.MaxValue.ToString().Length;

        public Program(string[] args) {

            var choise = "";

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

                for (var i = 5; i > 0; i--) {
                    Console.SetCursorPosition( 0, Console.CursorTop );
                    Console.Write( "Programm exit in " + i.ToString() );
                    Thread.Sleep( 500 );
                }
                //Console.ReadKey();
            }
        }

        private void pack() {
            var contend = new List<byte>();

            Console.WriteLine( "Creating Header..." );

            var header = "";
            foreach (var s in fs) {

                Console.WriteLine( "Processing file:" + s );

                var tmp = File.ReadAllBytes( s );
                foreach (var i in tmp) {
                    contend.Add( i );
                }

                var info = MakeVailed( tmp.Length.ToString() ) + Path.GetFileName( s );
                header += StringSizer.Size( info );
            }
            var head = Encoding.UTF8.GetBytes( StringSizer.Fullsize( StringSizer.Size( header ).Replace( "\n", "" ), IntLength ) );

            var final = new byte[contend.Count + head.Length];

            Console.WriteLine( "Creating binery..." );

            var t = 0;
            for (var i = t; i < head.Length; i++, t++) {
                final[t] = head[t];
            }

            for (var i = 0; i < contend.Count; i++, t++) {
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
        private void Unpack() {
            var contend = new List<byte>( File.ReadAllBytes( filename ) );

            Console.WriteLine( "reading header..." );

            var hlength = int.Parse( Encoding.UTF8.GetString( contend.GetRange( 0, 20 ).ToArray() ).Substring( 0, IntLength ) );

            var ascy = Encoding.UTF8.GetString( contend.GetRange( IntLength, hlength ).ToArray() );
            int len;
            var head = StringSizer.UnSize( ascy, out len );

            ascy = "";
            var fullheadlength = ( len + 2 + head.Length.ToString().Length + 2 ) + IntLength;

            Console.WriteLine( "Isolating contend..." );

            contend = contend.GetRange( fullheadlength, contend.Count - fullheadlength );


            var infos = head.Split( '\n' );

            var c = 1;
            for (var i = 0; i < infos.Length - 1; i++) {
                var p = StringSizer.UnSize( infos[i], out var length );
                var filename = p.Substring( IntLength );
                var filesize = int.Parse( p.Substring( 0, IntLength ) );

                Console.WriteLine( "Processing file:" + filename );

                File.WriteAllBytes( filename, contend.GetRange( c, filesize ).ToArray() );
                c += filesize;
            }
            Console.WriteLine( "Cleaning..." );
            contend = new List<byte>();
            ascy = "";
        }

        public string ReverseSize(string daten) {
            var result = "";
            var length = (short) daten.Length;

            for (var i = length.ToString().Length; i < IntLength; i++) {
                result += "0";
            }

            result += length.ToString();

            result += daten;
            return result;
        }

        public string MakeVailed(string size) {
            var result = "";

            for (var i = size.Length; i < IntLength; i++) {
                result += "0";
            }

            result += size;
            return result;
        }

        static int Main(string[] args) {
#if DEBUG
            Console.WriteLine( "Debug version!" );
#if pack
            args = new string[] { "p", @"C:\Users\Private\source\repos\Global\Global.sln", @"C:\Users\Private\source\repos\Global\hallo welt.txt", @"C:\Users\Private\Desktop\Roaming - Kopie.rar" };
#else
            args = new string[] { "u", filename };
#endif
            //#warning Using Debug
#endif

            var program = new Program( args );

            return 0;
        }
    }
}
