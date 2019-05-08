using Eternal.Ai;
using Eternal.Ai.Modle;
using Eternal.Drawing;
using Eternal.Net;
using Eternal.Utils;
using Eternal.Visualisation;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using testbuilds.TestUtils;

namespace testbuilds {
    class Program {
        public static void Main(string[] args)
        {
           new Program();
        }

        public Program() {
            var ftp = new ChoisObjekts( "FtpTest", _Ftp );
            var ims = new ChoisObjekts( "ImageScanTest", _ImageScan );
            var ai = new ChoisObjekts( "AiTest", _Ai );
            var task = new ChoisObjekts( "Testtest With Ftp", _Task );
            var taskingo = new ChoisObjekts( "TaskInfoTests", _TaskInfo );

            Utils.SetupChoise( new[] { ftp, task, taskingo, ai, ims } );
            Console.WriteLine( "\n" );
            Console.WriteLine( "Press any Key to Exit!" );
            Console.ReadKey();
            Console.Clear();
            Thread.Sleep( 500 );
            Environment.Exit( 0 );
        }

        #region TaskTrack
        private TaskInfo _ti;
        TaskInfo.State _state = TaskInfo.State.None;
        public void _TaskInfo() {
            _ti = new TaskInfo( "testtask", Tasktest );
            _ti.Run();
        }
        public void Tasktest() {
            while (true) {
                _ti._State = TaskInfo.State.critical;
                Thread.Sleep( 1000 );

                _ti._State = TaskInfo.State.error;
                Thread.Sleep( 2000 );

                _ti._State = TaskInfo.State.fail;
                Thread.Sleep( 2000 );

                _ti._State = TaskInfo.State.None;
                Thread.Sleep( 2000 );

                _ti._State = TaskInfo.State.ok;
                Thread.Sleep( 2000 );

                _ti._State = TaskInfo.State.running;
                Thread.Sleep( 4000 );

                _ti._State = TaskInfo.State.warning;
                Thread.Sleep( 2000 );
            }
        }
        private void _Task() {
            _ti = new TaskInfo( " ", _Ftp );
            _ti.Run();
        }
        #endregion

        #region FtpTest
        private FtpClinet _ftp;
        private void _Ftp() {
            const string filename = "Roaming.rar";
            _state = TaskInfo.State.running;
            var passwd = Convert.ToBase64String( Encoding.UTF8.GetBytes( "test" ) );
            try {
                _ftp = new FtpClinet( "ftp://127.0.0.1/" + filename, "test", passwd, filename, (int) Math.Pow( 2, 26 ) );
                Console.Write( "\n  " );
                var up = new Thread( () => {
                    while (true)
                        if (_ti != null) {
                            _ti._State = _state;
                            _ti._Name = _ftp.Message;
                        } else {
                            Console.SetCursorPosition( 0, Console.CursorTop );
                            Console.Write( _ftp.Message + "                      " );
                        }
                } );
                up.Start();
                _ftp.Upload();
                Thread.Sleep( 10 );
                up.Abort();
                _state = TaskInfo.State.ok;

            } catch (System.Net.WebException) {
                _state = TaskInfo.State.warning;
            } catch (IOException) {
                _state = TaskInfo.State.critical;
            } catch (Exception) {
                _state = TaskInfo.State.fail;
            }
        }
        #endregion

        #region AiTests

        private MDouble _m;
        private AiMasterDouble _ai;
        private readonly double[] _lo = new double[4];

        public void SetAlgorythmos(double l1, double l2, double l3, double l4) {
            _lo[0] = l1; _lo[1] = l2; _lo[2] = l3; _lo[3] = l4;
        }
        RandomGenerator r = new RandomGenerator();
        public void SlotAutomatAlgo(MDouble m, bool ausgabe) {
            double bank = 10000;
            double cash = 100;
            var i = 0;

            while (cash <= 0 && bank > 0) {
                var g = r.Next( 0, 100 );

                cash -= 2;
                bank += 2;

                if (g > 99) {
                    bank -= _lo[0];
                    cash += _lo[0];
                } else if (g > 90) {
                    bank -= _lo[1];
                    cash += _lo[1];
                } else if (g > 80) {
                    bank -= _lo[2];
                    cash += _lo[2];
                } else if (g > 70) {
                    bank -= _lo[3];
                    cash += _lo[3];
                }

                i += 1;
            }
            if (ausgabe) {
                Console.WriteLine( $"[{i}]: cach:{cash}, bank:{bank}" );
            }
            m.Variable = i;
        }

        private void _Ai() {

            const int loops = 100;
            const bool showOutput = false;
            const double vMin = 0.01D;
            const double vMax = 0.02D;
            Console.WindowWidth = 153;

            Console.WriteLine( "-----------------SETUP----------------" );
            var t = DateTime.Now;
            SetAlgorythmos( 22, 13, 5, 3 );
            _m = new MDouble( 1000, 0, _lo, SlotAutomatAlgo, 90 );
            _ai = new AiMasterDouble( new[] { _m } );
            Console.WriteLine( _m.GetId );
            Console.WriteLine( "done in " + ( DateTime.Now - t ) );
            Console.WriteLine( "Press any Key to enter TrainLoop({0})...", loops );
            Console.ReadKey();
            Console.Clear();
            for (var i = 0; i < loops; i++) {
                Console.SetCursorPosition( 0, 0 );
                Console.WriteLine( "---------------TRAINING---------------" );
                t = DateTime.Now;
                _ai.Trainloop( _m, vMin, vMax, showOutput );
                SetAlgorythmos( _m.Doubles[0], _m.Doubles[1], _m.Doubles[2], _m.Doubles[3] );
                _m = new MDouble( 1000, 0, _lo, SlotAutomatAlgo, 90 );
                Console.WriteLine( "\ndone in " + ( DateTime.Now - t ) );
                Console.WriteLine( "---------------FINISHED---------------" + $"                                                                                                          [{ i}/{ loops}]" );
                var v = Console.WindowWidth - 3;
                Console.Write( "[" );
                Console.SetCursorPosition( v + 1, Console.CursorTop );
                Console.Write( "]" );
                Console.SetCursorPosition( 1, Console.CursorTop );
                for (var j = 0; j < ( i / (double) 100 ) * v; j++) {
                    Console.Write( "=" );
                }
                Console.Write( ">" );
                for (var j = i; j < ( v - ( i / (double) 100 ) * v ) - 1; j++) {
                    Console.Write( " " );
                }
            }
            Console.WriteLine( "\n\n-------------RUNNING TESTS------------" );
            Console.WriteLine( "Press any Key..." );
            Console.ReadKey();
            t = DateTime.Now;
            SetAlgorythmos( _m.Doubles[0], _m.Doubles[1], _m.Doubles[2], _m.Doubles[3] );
            _m = new MDouble( 10000, 0, _lo, SlotAutomatAlgo, 1000 );
            double max = 0;
            for (var i = 0; i < 100; i++) {
                SlotAutomatAlgo( _m, true );
                max = _m.Variable > max ? _m.Variable : max;
            }
            Console.WriteLine($"\ndone in {DateTime.Now - t}       Max:{max}");
            Console.WriteLine( "---------------FINISHED---------------" );
        }
        #endregion

        #region ImageScanTest

        private readonly ImageSearch _ic = new ImageSearch();
        private void _ImageScan() {
            const string pathToBmp = "..\\..\\..\\example.png";

            const int tolleranz = 50;
            const ImageSearch.ImageScanMethode ism = ImageSearch.ImageScanMethode.Fast;
            Console.WriteLine( Path.GetDirectoryName( pathToBmp ) );
            var b = new Bitmap( pathToBmp );

            while (true) {
                var str = _ic.SertchImageWithInfo( _ic.CaptureScreen(), b, out _, out _, out _, ism, tolleranz );
                Console.WriteLine( str );
                Thread.Sleep( 250 );
            }
        }
        #endregion

    }
}
