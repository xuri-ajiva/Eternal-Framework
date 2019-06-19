using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Eternal.Utils {
    class Utils {
        public static void ax(ConsoleColor colo, ConsoleColor Sugestet = ConsoleColor.DarkMagenta) {
            pref( colo, Sugestet );
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write( " + " );
            Console.ForegroundColor = colo;
        }

        public static void sx(ConsoleColor colo, ConsoleColor Sugestet = ConsoleColor.DarkMagenta) {
            pref( colo, Sugestet );
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write( " - " );
            Console.ForegroundColor = colo;
        }

        public static void pref(ConsoleColor colo, ConsoleColor Sugestet = ConsoleColor.DarkMagenta) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write( "[" );
            Console.ForegroundColor = Sugestet;
            Console.Write( "SERVER" );
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write( "]" );
            Console.ForegroundColor = colo;
        }

        public static void sessions(int session, ConsoleColor colo) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write( "Session:" );
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write( session );
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write( "" );
            Console.ForegroundColor = colo;
        }
    }

    public class RandomGenerator {
        private readonly RNGCryptoServiceProvider _csp;

        public RandomGenerator() { _csp = new RNGCryptoServiceProvider(); }

        public int Next(int minValue, int maxExclusiveValue) {
            if ( minValue >= maxExclusiveValue ) {
                throw new ArgumentOutOfRangeException( "minValue must be lower than maxExclusiveValue" );
            }

            var diff       = (long) maxExclusiveValue - minValue;
            var upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do {
                ui = GetRandomUDouble();
            } while ( ui >= upperBound );

            return (int) ( minValue + ( ui % diff ) );
        }

        private uint GetRandomUDouble() {
            var randomBytes = GenerateRandomBytes( sizeof(uint) );
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
            var finalData = daten.Replace( "\\", "\\%" ).Replace( "\n", "\\/" );

            return "\\&" + finalData.Length + "\\$" + finalData + "\n";
        }

        public static string UnSize(string daten, out int length) {
            if ( daten.Substring( 0, 2 ) != "\\&" ) throw new Exception( "No vailet String" );

            daten = daten.Substring( 2 );
            var il = 0;
            length = 0;
            for ( var i = 0; i < long.MaxValue.ToString().Length; i++ )
                if ( daten.Substring( i, 2 ) == "\\$" ) {
                    il     = i;
                    length = int.Parse( daten.Substring( 0, i ) );
                    break;
                }

            var rlDaten = daten.Substring( il + 2, length ).Replace( "\\%", "\\" ).Replace( "\\/", "\n" );

            //if (daten.Substring( Il + 2 + length, 2 ) == "\n")
            //    Console.WriteLine( "vailet" );
            return rlDaten;
        }

        public static string Fullsize(string daten, int Rlength) {
            var result = "";
            var length = daten.Length;

            for ( var i = length.ToString().Length; i < Rlength; i++ ) {
                result += "0";
            }

            result += length.ToString();

            result += daten;
            return result;
        }
    }

    public static class StringList {
        public static string MakeList(string[] dtn) {
            var returns = dtn.Aggregate( "", (current, s) => current + ( s.Replace( "\\", "\\%" ).Replace( "\n", "\\/" ) + "\\=" ) );

            returns += "\\?";
            return "\\&" + dtn.Length + "\\$" + returns + "\n";
        }

        public static string[] UnMakeList(string dtn, out int length, bool writeOutput = true) {
            var counts = 0;
            var wo     = writeOutput;
            var c      = 0;

            try {
                if ( dtn.Substring( c, 2 ) == "\\&" )
                    counts++;
                else
                    throw new Exception( "No valet String" );

                c += 2;

                var returns = new List<string>();

                var anzahl = GetNextIntInString( dtn.Substring( c, int.MaxValue.ToString().Length ) );
                c += anzahl.ToString().Length;

                if ( dtn.Substring( c, 2 ) == "\\$" )
                    counts++;
                else
                    throw new Exception( "Error:!!!" );

                c += 2;

                for ( var i = 0; i < anzahl; i++ ) {
                    var element = GetElementNext( dtn.Substring( c ), "\\=" );

                    if ( wo ) Console.WriteLine( element );

                    returns.Add( element );
                    c += ( element.Length );

                    if ( dtn.Substring( c, 2 ) == "\\=" )
                        counts++;
                    else
                        throw new Exception( "Error:!!!" );
                    c += ( 2 );
                }

                if ( dtn.Substring( c, 2 ) == "\\?" )
                    counts++;
                else
                    throw new Exception( "Error:!!!" );
                c += 2;

                if ( dtn.Substring( c, 1 ) == "\n" )
                    counts++;
                else
                    throw new Exception( "Error:!!!" );
                c += 1;

                var mx = anzahl + 4;
                if ( wo )
                    Console.WriteLine( $"Finished:[{counts}/{mx}] " +
                                       ( ( counts == mx )
                                           ? "Perfect!"
                                           : ( counts == mx - 1 )
                                               ? "Good"
                                               : "We Do not get all =[" ) );

                //if (dtn.Substring( Il + 2 + length, 2 ) == "\n")
                //    Console.WriteLine( "vailet" );

                var ret = new string[returns.Count];
                for ( var i = 0; i < ret.Length; i++ ) {
                    ret[i] = returns[i].Replace( "\\%", "\\" ).Replace( "\\/", "\n" );
                }

                length = c;
                return ret;
            } catch (Exception e) {
                if ( wo ) Console.WriteLine( $"Error @[{counts}]: {e.Message}!" );
                length = c;
                return new string[] { };
            }
        }

        private static string GetElementNext(string dtn, string strEnd) {
            var length = -99;
            for ( var i = 0; i < dtn.Length; i++ ) {
                if ( dtn.Substring( i, 2 ) == strEnd ) {
                    length = i;
                    break;
                }
            }

            return dtn.Substring( 0, length );
        }

        public static int GetNextIntInString(string v) {
            int returns;
            var i = 1;
            for ( ; i < v.Length; i++ )
                if ( !int.TryParse( v.Substring( 0, i ), out returns ) )
                    break;
            int.TryParse( v.Substring( 0, i - 1 ), out returns );
            return returns;
        }
    }
}