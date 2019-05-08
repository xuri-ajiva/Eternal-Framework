using System;
using System.Security.Cryptography;

namespace Eternal.Utils {
    public class RandomGenerator {
        private readonly RNGCryptoServiceProvider _csp;

        public RandomGenerator() {
            _csp = new RNGCryptoServiceProvider();
        }

        public int Next(int minValue, int maxExclusiveValue) {
            if (minValue >= maxExclusiveValue) {
                throw new ArgumentOutOfRangeException( "minValue must be lower than maxExclusiveValue" );
            }

            var diff = (long) maxExclusiveValue - minValue;
            var upperBound = uint.MaxValue / diff * diff;

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
            var buffer = new byte[bytesNumber];
            _csp.GetBytes( buffer );
            return buffer;
        }
    }
    public static class StringSizer {

        public static string Size(string daten) {
            var finalData = daten.Replace( "\\", "\\%" ).
                Replace( "\n", "\\/" );

            return "\\&" + finalData.Length + "\\$" + finalData + "\n";
        }
        public static string UnSize(string daten, out int length) {
            if (daten.Substring( 0, 2 ) != "\\&")
                throw new Exception( "No vailet String" );


            daten = daten.Substring( 2 );
            var il = 0;
            length = 0;
            for (var i = 0; i < long.MaxValue.ToString().Length; i++)
                if (daten.Substring( i, 2 ) == "\\$") {
                    il = i;
                    length = int.Parse( daten.Substring( 0, i ) );
                    break;
                }

            var rlDaten = daten.Substring( il + 2, length ).Replace( "\\%", "\\" ).
                Replace( "\\/", "\n" );

            //if (daten.Substring( Il + 2 + length, 2 ) == "\n")
            //    Console.WriteLine( "vailet" );
            return rlDaten;
        }
        public static string Fullsize(string daten, int Rlength) {
            var result = "";
            var length = daten.Length;

            for (var i = length.ToString().Length; i < Rlength; i++) {
                result += "0";
            }

            result += length.ToString();

            result += daten;
            return result;
        }
    }
}
