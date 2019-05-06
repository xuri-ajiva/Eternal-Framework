
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests {
    class Program {
        static void Main(string[] args) {

            string testdate = "some string of daten";
            Console.WriteLine( testdate );
            var tmp = pack( testdate );
            Console.WriteLine( tmp );
            Console.WriteLine( restore( tmp ) );
            Console.ReadLine();

        }
        public static string Fullsize(string daten) {
            string result = "";
            int length = (int) daten.Length;

            for (int i = length.ToString().Length; i < 5; i++) {
                result += "0";
            }

            result += length.ToString();

            result += daten;
            return result;
        }
        public static string pack(string daten) {
            return "\\&" + daten.Length.ToString() + "\\$" + daten.Replace( "\\", "\\%" ) + "\\?";
        }
        public static string restore(string daten, string endstring = "\\?") {
            if (daten.Substring( 0, 2 ) != "\\&")
                throw new Exception( "No vailet String" );


            daten = daten.Substring( 2 );
            int Il = 0;
            int length = 0;
            for (int i = 0; i < long.MaxValue.ToString().Length; i++)
                if (daten.Substring( i, 2 ) == "\\$") {
                    Il = i;
                    length = int.Parse( daten.Substring( 0, i ) );
                    break;
                }

            string rlDaten = daten.Substring( Il + 2, length ).Replace( "\\%", "\\" );

            if (daten.Substring( Il + 2 + length, 2 ) == "\\?")
                Console.WriteLine( "vailet" );
            return rlDaten;
        }
    }
}
