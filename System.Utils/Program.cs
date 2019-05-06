using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System.Utils {
    public class RandomGenerator {
        private readonly RNGCryptoServiceProvider csp;

        public RandomGenerator() {
            csp = new RNGCryptoServiceProvider();
        }

        public int Next(int minValue, int maxExclusiveValue) {
            if (minValue >= maxExclusiveValue) {
                throw new ArgumentOutOfRangeException( "minValue must be lower than maxExclusiveValue" );
            }

            long diff = (long) maxExclusiveValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do {
                ui = GetRandomUDouble();
            } while (ui >= upperBound);
            return (int) ( minValue + ( ui % diff ) );
        }

        private uint GetRandomUDouble() {
            var randomBytes = GenerateRandomBytes( sizeof( uint ) );
            return BitConverter.ToUInt32( randomBytes, 0 );
        }

        private byte[] GenerateRandomBytes(int bytesNumber) {
            byte[] buffer = new byte[bytesNumber];
            csp.GetBytes( buffer );
            return buffer;
        }
    }
    public static class StringSizer {

        public static string Size(string daten) {
            string finalData = daten.Replace( "\\", "\\%" ).
                Replace( "\n", "\\/" );

            return "\\&" + finalData.Length.ToString() + "\\$" + finalData + "\n";
        }
        public static string UnSize(string daten, out int length) {
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
    }
}
