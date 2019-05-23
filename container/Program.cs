//#define pack

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;



/*
    Name : begin : ende : 

    */
namespace container {
    public class Program {
        public static string filename = "archife.pac";
        public static string[] files = new string[99];
        private static List<string> fs;// = new List<string>(files);

        public static int IntLength => int.MaxValue.ToString().Length;

        public Program(string[] args) {

            if (args.Length >= 3) {
                filename = args[1];
                fs = new List<string>( args );
                fs.RemoveRange( 0, 2 );
                switch (args[0]) {
                    case "p":
                        Pack();
                        break;
                    case "u":
                        Unpack();
                        break;
                    default:
                        PrintSyntax();
                        break;
                }
            }

            if (args.Length == 2) {
                filename = args[1];
                if (args[0].ToLower() == "u")
                    Unpack();
                else
                    PrintSyntax();

            }

            if (args.Length == 0) {
                PrintSyntax();
            }

            if (args.Length == 1) {
                if (args[0].ToLower() == ( "help" )) {
                    PrintSyntax();
                    Console.ReadKey();
                }
                Console.Write( "Packen Oder Entpacken? [p/u]:" );
                args = new[] { Console.ReadKey().KeyChar.ToString(), args[0] };
                Console.WriteLine();
            }
            if (args.Length >= 2) {
                switch (args[0]) {
                    case "p":
                        Console.Write( "Bitte AusgabeDatei angeben:" );
                        filename = Console.ReadLine();
                        fs = new List<string>( args );
                        fs.RemoveAt( 0 );
                        Pack();
                        break;
                    case "u":
                        filename = args[1];
                        Unpack();
                        break;
                    default:
                        PrintSyntax();
                        break;
                }
            }

            Console.WriteLine( "\nFinished!" );
            Console.ReadKey();

        }

        private void PrintSyntax() {
            Console.WriteLine( "\nArgument:   u        - Unpack" +
                               "\n            p        - Pack" +
                               "\n            file     - file to Pack" +
                               "\n            archive  - file to UnPack" +
                               "\n            Help     - This" +
                               "\n" +
                               "\nUsage:      File" +
                               "\n            [p/u] archive file(0) file(1) file(n)" +
                               "\n            u archive" );
        }

        private void Pack() {

            Console.WriteLine( "Creating Header..." );
            var headList = new string[fs.Count];

            for (var i = 0; i < fs.Count; i++) {
                var s = fs[i];
                Console.WriteLine( "Processing Header for file:" + s );
                FileInfo f = new FileInfo( s );
                headList[i] = ( MakeList( new[] { f.Name, f.Length.ToString() } ) );
            }

            var header = MakeList( headList );
            var head = Encoding.UTF8.GetBytes( header.Length + header );
            Console.WriteLine();

            //contend
            var contend = new List<byte>();
            foreach (var s in fs) {

                Console.WriteLine( "Processing contend of file:" + s );

                var tmp = File.ReadAllBytes( s );
                foreach (var i in tmp) {
                    contend.Add( i );
                }
            }
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

            var hlength = GetNextInt( Encoding.UTF8.GetString( contend.GetRange( 0, 30 ).ToArray() ) );

            var ascy = Encoding.UTF8.GetString( contend.GetRange( hlength.ToString().Length, hlength ).ToArray() );
            var Elements = UnMakeList( ascy, out var leng, false );

            ascy = "";

            hlength += hlength.ToString().Length;

            Console.WriteLine( "Isolating contend..." );

            contend = contend.GetRange( hlength, contend.Count - hlength );
            // File.WriteAllBytes( "tmp", contend.ToArray() );

            var cn = 0;
            for (int i = 0; i < Elements.Length; i++) {
                var e = Elements[i];

                var ElementTemp = UnMakeList( e, out _, false );
                Console.WriteLine( $"Extracting File: {ElementTemp[0]}, Length: {ElementTemp[1]}" );
                var itemlemgth = int.Parse( ElementTemp[1] );

                File.WriteAllBytes( ElementTemp[0], contend.GetRange( cn, itemlemgth ).ToArray() );
                cn += itemlemgth;
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

        public static string MakeList(string[] daten) {
            string returns = "";

            foreach (var s in daten) {
                returns += s.Replace( "\\", "\\%" ).Replace( "\n", "\\/" ) + "\\=";
            }

            returns += "\\?";
            return "\\&" + daten.Length + "\\$" + returns + "\n";
        }
        public static string[] UnMakeList(string daten, out int length, bool WriteOutput = true) {
            int counts = 0;
            var wo = WriteOutput;
            int c = 0;

            try {

                if (daten.Substring( c, 2 ) == "\\&")
                    counts++;
                else throw new Exception( "No valet String" );

                c += 2;

                List<string> returns = new List<string>();

                var anzahl = GetNextInt( daten.Substring( c, int.MaxValue.ToString().Length ) );
                c += anzahl.ToString().Length;

                if (daten.Substring( c, 2 ) == "\\$")
                    counts++;
                else throw new Exception( "Error:!!!" );

                c += 2;

                for (var i = 0; i < anzahl; i++) {
                    var element = getElementNext( daten.Substring( c ), "\\=" );

                    if (wo)
                        Console.WriteLine( element );

                    returns.Add( element );
                    c += ( element.Length );

                    if (daten.Substring( c, 2 ) == "\\=")
                        counts++;
                    else throw new Exception( "Error:!!!" );
                    c += ( 2 );
                }

                if (daten.Substring( c, 2 ) == "\\?")
                    counts++;
                else throw new Exception( "Error:!!!" );
                c += 2;

                if (daten.Substring( c, 1 ) == "\n")
                    counts++;
                else throw new Exception( "Error:!!!" );
                c += 1;

                var mx = anzahl + 4;
                if (wo)
                    Console.WriteLine( $"Finished:[{counts}/{mx}] " + ( ( counts == mx ) ? "Perfect!" : ( counts == mx - 1 ) ? "Good" : "We Do not get all =[" ) );

                //if (daten.Substring( Il + 2 + length, 2 ) == "\n")
                //    Console.WriteLine( "vailet" );

                var ret = new string[returns.Count];
                for (int i = 0; i < ret.Length; i++) {
                    ret[i] = returns[i].Replace( "\\%", "\\" ).Replace( "\\/", "\n" );
                }

                length = c;
                return ret;
            }
            catch (Exception e) {
                if (wo)
                    Console.WriteLine( $"Error @[{counts}]: {e.Message}!" );
                length = c;
                return new string[] { };
            }
        }

        private static string getElementNext(string daten, string ende) {
            int length = -99;
            for (int i = 0; i < daten.Length; i++) {
                if (daten.Substring( i, 2 ) == ende) {
                    length = i; break;
                }
            }

            return daten.Substring( 0, length );
        }

        private static int GetNextInt(string v) {
            var returns = -9999;
            var i = 1;
            for (; i < v.Length; i++)
                if (!int.TryParse( v.Substring( 0, i ), out returns )) break;
            int.TryParse( v.Substring( 0, i - 1 ), out returns );
            return returns;
        }

        static int Main(string[] args) {


            //#if DEBUG
            //            Console.WriteLine( "Debug version!" );
            //#if pack
            //            args = new string[] { "p", @"D:\Sortiert\Cracked Games\Risk.of.Rain.2.v04.04.2019.rar", @"C:\Users\Xuri\Source\repos//\Eternal Framework\_Old.zip" };
            //#else
            //            args = new string[] { "u", filename };
            //#endif
            //            //#warning Using Debug
            //#endif

            var program = new Program( args );

            return 0;
        }
        static void test() {
            var x = MakeList( new[] { "TestData1", "data2", "3§", "4" } );
            x += "sdhahjkhdkahsdkhadsk";
            Console.WriteLine( x );
            var m = UnMakeList( x, out var l, false );
            Console.WriteLine( l );
            foreach (var s in m) {
                Console.WriteLine( s );
            }

            Console.WriteLine( x.Substring( 0, l ) + "" );
            Console.WriteLine( x );

            Console.Read();
            Environment.Exit( 0 );

        }
    }
}
