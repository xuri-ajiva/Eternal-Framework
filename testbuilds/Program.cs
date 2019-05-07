using Eternal.Ai;
using Eternal.Ai.Modle;
using Eternal.Drawing;
using Eternal.Net;
using Eternal.Utils;
using Eternal.Visualisation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using testbuilds.TestUtils;

namespace testbuilds {
    class Program {
        public static void Main(string[] args) {
            var p = new Program();
        }

        public Program() {
            ChoisObjekts ftp = new ChoisObjekts( "FtpTest", _Ftp );
            ChoisObjekts ims = new ChoisObjekts( "ImageScanTest", _ImageScan );
            ChoisObjekts ai = new ChoisObjekts( "AiTest", _Ai );
            ChoisObjekts task = new ChoisObjekts( "Testtest With Ftp", _Task );
            ChoisObjekts taskingo = new ChoisObjekts( "TaskInfoTests", _TaskInfo );

            Utils.SetupChoise( new ChoisObjekts[] { ftp, task, taskingo, ai, ims } );
            Console.WriteLine( "\n" );
            Console.WriteLine( "Press any Key to Exit!" );
            Console.ReadKey();
            Console.Clear();
            Thread.Sleep( 500 );
            Environment.Exit( 0 );
        }

        #region TaskTrack
        TaskInfo ti;
        TaskInfo.State _State = TaskInfo.State.None;
        public void _TaskInfo() {
            ti = new TaskInfo( "testtask", Tasktest );
            ti.Run();
        }
        public void Tasktest() {
            while (true) {
                ti._State = TaskInfo.State.critical;
                Thread.Sleep( 1000 );

                ti._State = TaskInfo.State.error;
                Thread.Sleep( 2000 );

                ti._State = TaskInfo.State.fail;
                Thread.Sleep( 2000 );

                ti._State = TaskInfo.State.None;
                Thread.Sleep( 2000 );

                ti._State = TaskInfo.State.ok;
                Thread.Sleep( 2000 );

                ti._State = TaskInfo.State.running;
                Thread.Sleep( 4000 );

                ti._State = TaskInfo.State.warning;
                Thread.Sleep( 2000 );
            }
        }
        private void _Task() {
            ti = new TaskInfo( " ", _Ftp );
            ti.Run();
        }
        #endregion

        #region FtpTest
        FtpClinet ftp;
        private void _Ftp() {
            var filename = "Roaming.rar";
            _State = TaskInfo.State.running;
            try {
                ftp = new FtpClinet( "ftp://127.0.0.1/" + filename, "test", Convert.ToBase64String( Encoding.UTF8.GetBytes( "test" ) ), filename, (int) Math.Pow( 2, 26 ) );
                Console.Write("\n  ");
                Thread up = new Thread( () => {
                    while (true) {
                        if (ti != null) {
                            ti._State = _State;
                            ti._Name = ftp._message;
                        } else {
                            Console.SetCursorPosition( 0, Console.CursorTop );
                            Console.Write( ftp._message +"                      ");
                        }
                    }
                } );
                up.Start();
                ftp.upload();
                Thread.Sleep( 10 );
                up.Abort();
                _State = TaskInfo.State.ok;

            } catch (System.Net.WebException) {
                _State = TaskInfo.State.warning;
            } catch (System.IO.IOException) {
                _State = TaskInfo.State.critical;
            } catch (Exception) {
                _State = TaskInfo.State.fail;
            }
        }
        #endregion

        #region AiTests
        MDouble m;
        AIMasterDouble ai;
        double[] lo = new double[4];

        public void SetAlgorythmos(double l1, double l2, double l3, double l4) {
            lo[0] = l1; lo[1] = l2; lo[2] = l3; lo[3] = l4;
        }
        RandomGenerator r = new RandomGenerator();
        public void SlotAutomatAlgo(MDouble m, bool ausgabe) {
            double bank = 10000;
            double cash = 100;
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
            m = new MDouble( 1000, 0, lo, SlotAutomatAlgo, 90 );
            ai = new AIMasterDouble( new MDouble[] { m } );
            Console.WriteLine( m.GetID );
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
            m = new MDouble( 10000, 0, lo, SlotAutomatAlgo, 1000 );
            double max = 0;
            for (int i = 0; i < 100; i++) {
                SlotAutomatAlgo( m, true );
                max = m.Variable > max ? m.Variable : max;
            }
            Console.WriteLine( "\ndone in " + ( DateTime.Now - t ) + "       Max:" + max );
            t = DateTime.Now;
            Console.WriteLine( "---------------FINISHED---------------" );
        }
        #endregion

        #region ImageScanTest
        ImageSearch IC = new ImageSearch();
        private void _ImageScan() {
            string Path_to_Bmp = "..\\..\\..\\example.png";

            bool result;
            Point point;
            TimeSpan TS;
            int Tolleranz = 50;
            ImageSearch.ImageScanMethode Ism = ImageSearch.ImageScanMethode.Fast;
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
