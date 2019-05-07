using System;
using System.Ai;
using System.Ai.Modle;
using System.Collections.Generic;
using System.Drawing;
using System.Ftp;
using System.ImageScan;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Utils;
using System.Windows.Forms;
using testbuilds.TestUtils;

namespace testbuilds {
    class Program {
        void Main(string[] args) {

            ChoisObjekts ftp = new ChoisObjekts( "FtpTest", _Ftp );
            ChoisObjekts ims = new ChoisObjekts( "ImageScanTest", _ImageScan );
            ChoisObjekts ai = new ChoisObjekts( "AiTest", _Ai );

            Utils.SetupChoise( new ChoisObjekts[] { ftp, ai, ims } );

            Console.WriteLine( "Press any Key t Exit..." );
            Console.ReadLine();
        }

        #region FtpTest
        private void _Ftp() {
            var filename = "Roaming.rar";
            FtpClinet ftp = new FtpClinet( "ftp://127.0.0.1/" + filename, "test", Convert.ToBase64String( Encoding.UTF8.GetBytes( "test" ) ), filename, (int) Math.Pow( 2, 26 ) );
            ftp.upload();
        }
        #endregion

        #region AiTests
        double[] lo = new double[4];
        public void SetAlgorythmos(double l1, double l2, double l3, double l4) {
            lo[0] = l1; lo[1] = l2; lo[2] = l3; lo[3] = l4;
        }
        public void SlotAutomatAlgo(MDouble m, bool ausgabe) {
            double bank = 10000;
            double cash = 100;

            RandomGenerator r = new RandomGenerator();

            int i = 0;

            while (cash != 0 && bank > 0) {
                int g = r.Next( 0, 100 );

                cash -= 2;
                bank += 2;

                if (g > 99) {
                    bank -= lo[0];
                    cash += lo[0];
                } else if (g > 90) {
                    bank -= lo[1];
                    cash += lo[1];
                } else if (g > 80) {
                    bank -= lo[2];
                    cash += lo[2];
                } else if (g > 70) {
                    bank -= lo[3];
                    cash += lo[3];
                }
                if (cash < 0) {
                    cash = 0;
                }

                i += 1;
            }
            if (ausgabe) {
                Console.WriteLine( $"[{i}]: cach:{cash}, bank:{bank}" );
            }
            m.Variable = i;
        }
        private void _Ai() {

            int loops = 100;
            bool ShowOutput = false;
            double VMin = 0.01D;
            double VMax = 0.02D;
            Console.WindowWidth = 153;

            Console.WriteLine( "-----------------SETUP----------------" );
            var t = DateTime.Now;
            SetAlgorythmos( 22, 13, 5, 3 );
            MDouble m = new MDouble( 1000, 0, lo, SlotAutomatAlgo, 90 );
            System.Ai.AIMasterDouble ai = new AIMasterDouble( new MDouble[] { m } );
            Console.WriteLine( m.Guid );
            Console.WriteLine( "done in " + ( DateTime.Now - t ) );
            Console.WriteLine( "Press any Key to enter TrainLoop({0})...", loops );
            Console.ReadKey();
            Console.Clear();
            for (int i = 0; i < loops; i++) {
                Console.SetCursorPosition( 0, 0 );
                Console.WriteLine( "---------------TRAINING---------------" );
                t = DateTime.Now;
                ai.trainloop( m, VMin, VMax, ShowOutput );
                SetAlgorythmos( m.Doubles[0], m.Doubles[1], m.Doubles[2], m.Doubles[3] );
                m = new MDouble( 1000, 0, lo, SlotAutomatAlgo, 90 );
                Console.WriteLine( "\ndone in " + ( DateTime.Now - t ) );
                Console.WriteLine( "---------------FINISHED---------------" + $"                                                                                                          [{ i}/{ loops}]" );
                int v = Console.WindowWidth - 3;
                Console.Write( "[" );
                Console.SetCursorPosition( v + 1, Console.CursorTop );
                Console.Write( "]" );
                Console.SetCursorPosition( 1, Console.CursorTop );
                for (int j = 0; j < ( (double) i / (double) 100 ) * v; j++) {
                    Console.Write( "=" );
                }
                Console.Write( ">" );
                for (int j = i; j < ( v - ( (double) i / (double) 100 ) * v ) - 1; j++) {
                    Console.Write( " " );
                }
            }
            Console.WriteLine( "\n\n-------------RUNNING TESTS------------" );
            Console.WriteLine( "Press any Key..." );
            Console.ReadKey();
            t = DateTime.Now;
            SetAlgorythmos( m.Doubles[0], m.Doubles[1], m.Doubles[2], m.Doubles[3] );
            MDouble mn = new MDouble( 10000, 0, lo, SlotAutomatAlgo, 1000 );
            double max = 0;
            for (int i = 0; i < 100; i++) {
                SlotAutomatAlgo( mn, true );
                max = mn.Variable > max ? mn.Variable : max;
            }
            Console.WriteLine( "\ndone in " + ( DateTime.Now - t ) + "       Max:" + max );
            t = DateTime.Now;
            Console.WriteLine( "---------------FINISHED---------------" );
        }
        #endregion

        #region ImageScanTest
        private void _ImageScan() {
            string Path_to_Bmp = "..\\..\\..\\example.png";

            ImageScan IC = new ImageScan();

            bool result;
            Point point;
            TimeSpan TS;
            int Tolleranz = 50;
            ImageScan.ImageScanMethode Ism = ImageScan.ImageScanMethode.Fast;
            Console.WriteLine( Path.GetDirectoryName( Path_to_Bmp ) );
            var b = new Bitmap( Path_to_Bmp );

            while (true) {
                var str = IC.SertchImageWithInfo( IC.CaptureScreen(), b, out result, out point, out TS, Ism, Tolleranz );
                Console.WriteLine( str );
                Thread.Sleep( 250 );
            }
        }
        #endregion

    }
}
