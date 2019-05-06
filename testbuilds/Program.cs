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
        static void Main(string[] args) {
            _Ftp f = new _Ftp();
            _Ai a = new _Ai();
            _Imagescan i = new _Imagescan( "..\\..\\..\\example.png" );

            Utils.SetupChoise( new ChoisObjekts[] { f, a, i } );

            Console.WriteLine( "Press any Key t Exit..." );
            Console.ReadLine();
        }
    }

    public class _Ftp : ChoisObjekts {
        public _Ftp() : base( "Ftp Tests" ) {
        }
        public override void Avtivete() {
            base.Avtivete();
            Ftp();
        }
        private void Ftp() {
            var filename = "Roaming.rar";
            FtpClinet ftp = new FtpClinet( "ftp://127.0.0.1/" + filename, "test", Convert.ToBase64String( Encoding.UTF8.GetBytes( "test" ) ), filename, (int) Math.Pow( 2, 26 ) );
            ftp.upload();
        }
    }

    public class _Ai : ChoisObjekts {
        public _Ai() : base( "Ai Tests" ) {
        }
        public override void Avtivete() {
            base.Avtivete();
            Ai();
        }

        private static void Ai() {

            Console.WriteLine( "-----------------SETUP----------------" );
            var t = DateTime.Now;
            Algorythmos a = new Algorythmos( 20, 10, 5, 3 );
            MDouble m = new MDouble( 1000, 0, a.lo, a, 90 );
            System.Ai.AIMasterDouble ai = new AIMasterDouble( new MDouble[] { m } );
            Console.WriteLine( m.Guid );
            Console.WriteLine( "done in " + ( DateTime.Now - t ) );
            Console.WriteLine( "---------------TRAINING---------------" );
            Console.WriteLine( "Press any Key..." );
            Console.ReadKey();
            t = DateTime.Now;
            ai.trainloop( m );
            Console.WriteLine( "\ndone in " + ( DateTime.Now - t ) );
            Console.WriteLine( "---------------FINISHED---------------" );
            Console.WriteLine( "\n\n-------------RUNNING TESTS------------" );
            Console.WriteLine( "Press any Key..." );
            Console.ReadKey();
            t = DateTime.Now;
            Algorythmos n = new Algorythmos( m.Doubles[0], m.Doubles[1], m.Doubles[2], m.Doubles[3] );
            MDouble mn = new MDouble( 10000, 0, a.lo, a, 1000 );
            for (int i = 0; i < 100; i++) {
                n.OnMainDouble( mn, true );
            }
            Console.WriteLine( "\ndone in " + ( DateTime.Now - t ) );
            t = DateTime.Now;
            Console.WriteLine( "---------------FINISHED---------------" );
        }
    }

    public class _Imagescan : ChoisObjekts {
        private string ptb;
        public _Imagescan(string PathToBitmap) : base( "ImageScan Tests" ) {
            ptb = PathToBitmap;
        }
        public override void Avtivete() {
            base.Avtivete();
            Scan();
        }

        private void Scan() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( true );


            string Path_to_Bmp = ptb;

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
    }

    internal class Algorythmos : Algos {
        public Algorythmos(double l1, double l2, double l3, double l4) {
            lo[0] = l1; lo[1] = l2; lo[2] = l3; lo[3] = l4;
        }

        public double[] lo = new double[4];

        public override void OnMainDouble(MDouble m, bool ausgabe = false) {
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
    }
}
