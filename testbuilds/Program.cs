using Eternal.Ai;
using Eternal.Ai.Modle;
using Eternal.Drawing;
using Eternal.Forms;
using Eternal.Net;
using Eternal.Utils;
using Eternal.Visualisation;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using testbuilds.TestUtils;

namespace testbuilds {
    class Program {
        public static void Main(string[] args) {
            //new EternalFramework.EternalMain( 1 );
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( true );

            Console.CancelKeyPress += Cancle;

            new Program();
        }

        private static void Cancle(object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            Console.Clear();
        }

        private Program() {
            var exit = new ChoisObjekts( "Exit(0)", _Exit );
            var ftp = new ChoisObjekts( "FtpTest", _Ftp );
            var ims = new ChoisObjekts( "ImageScanTest", _ImageScan );
            var ai = new ChoisObjekts( "AiTest", _Ai );
            var task = new ChoisObjekts( "Testtest With Ftp", _Task );
            var taskingo = new ChoisObjekts( "TaskInfoTests", _TaskInfo );
            var gravigsform = new ChoisObjekts( "GraficsForm tests", _GraphicsForm );

            while (true) {

                Utils.SetupChoise( new[] { exit, ftp, task, taskingo, ai, ims, gravigsform } );
                Console.WriteLine( "\n" );
                Thread.Sleep( 1000 );
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void _Exit() {
            Console.WriteLine( "\nPress Any Key To Exit..." );
            var p = ParentProcessUtilities.GetParentProcess();
            if (p.ProcessName.ToLower() != "cmd")
                Console.ReadKey();
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
            Thread up = new Thread( () => { } );
            try {
                _ftp = new FtpClinet( "ftp://127.0.0.1/" + filename, "test", passwd, filename, (int) Math.Pow( 2, 26 ) );
                Console.Write( "\n  " );
                up = new Thread( () => {
                    while (true)
                        if (_ti != null) {
                            _ti._State = _state;
                            _ti._Name = _ftp.Message;
                        }
                        else {
                            Console.SetCursorPosition( 0, Console.CursorTop );
                            Console.Write( _ftp.Message + "                      " );
                        }
                } );
                up.Start();
                _ftp.Upload();
                Thread.Sleep( 10 );
                up.Abort();
                _state = TaskInfo.State.ok;

            }
            catch (System.Net.WebException) {
                _state = TaskInfo.State.warning;
            }
            catch (IOException) {
                _state = TaskInfo.State.critical;
            }
            catch (Exception) {
                _state = TaskInfo.State.fail;
            }
            up.Abort();
        }
        #endregion

        #region AiTests

        private ModuleBase _module;
        private AiMasterBase _aiMaster;
        private readonly dynamic[] _lo = new dynamic[4];
        private readonly RandomGenerator _r = new RandomGenerator();
        //private readonly Random _r = new Random();

        private void SetAlgorythmos(double l1, double l2, double l3, double l4/*, double l5*/) {
            _lo[0] = R( l1 ); _lo[1] = R( l2 ); _lo[2] = R( l3 ); _lo[3] = R( l4 );/* _lo[4] = r( l5 );*/
        }
        private void SetAlgorythmos(ModuleBase @base) {
            SetAlgorythmos( @base.ToChange[0], @base.ToChange[1], @base.ToChange[2], @base.ToChange[3]/*, @base.ToChange[4] */);
        }

        private int _startmoney;
        private void SlotAutomatAlgo(ModuleBase m, bool ausgabe) {
            double bank = 10000;
            double cash = _startmoney;
            var i = 0;
            var array = m.ToChange;

            while (cash >= 0 && bank > 0) {
                var g = _r.Next( 0, 100 );

                cash -= 2;
                bank += 2;

                if (g > 99) {
                    bank -= array[0];
                    cash += array[0];
                }
                else if (g > 90) {
                    bank -= array[1];
                    cash += array[1];
                }
                else if (g > 80) {
                    bank -= array[2];
                    cash += array[2];
                }
                else if (g > 70) {
                    bank -= array[3];
                    cash += array[3];
                }/*
                else if (g > 65) {
                    bank -= array[4];
                    cash += array[4];
                }*/

                for (var j = 0; j < array.Length; j++) {
                    array[j] = R( array[j] );
                }

                i += 1;
            }
            if (ausgabe) {
                cash = R( cash );
                bank = R( bank );
                Console.WriteLine( $"[{i}]: cach:{cash}, bank:{bank}" );
            }
            m.Variable = i;
        }

        private double R(double d) {
            var m = (decimal) d; return (double) ( Math.Truncate( m * 1000m ) / 1000m );
        }

        private void _Ai() {
            const bool showOutput = false;
            const double vMin = 0.01D;
            const double vMax = 0.02D;

            Console.Write( "Bitte startgeld Für Kunden Angeben:" );
            int.TryParse( Console.ReadLine(), out _startmoney );
            Console.Write( "Bitte anzahl der Trains angeben:" );
            int.TryParse( Console.ReadLine(), out var loops );
            Console.Write( "Bitte Thaget(int) angeben:" );
            double.TryParse( Console.ReadLine(), out var thaget );

            var tolleranz = (int) ( thaget / 5D );

            Console.WindowWidth = 153;

            Console.WriteLine( "-----------------SETUP----------------" );
            var t = DateTime.Now;
            SetAlgorythmos( 20, 11, 6, 4 );
            _module = new ModuleBase( 0, thaget, vMin, vMax, _lo, SlotAutomatAlgo, tolleranz );
            _aiMaster = new AiMasterBase( _module );
            Console.WriteLine( _module.GetId );
            Console.WriteLine( "done in " + ( DateTime.Now - t ) );

            Console.WriteLine( "---------------CALIBRATE--------------" );
            t = DateTime.Now;
            _aiMaster.Trainloop();
            SetAlgorythmos( _module );
            _module = new ModuleBase( 0, thaget, vMin, vMax, _lo, SlotAutomatAlgo, tolleranz * 2 );
            Console.WriteLine( "\ndone in " + ( DateTime.Now - t ) );
            Console.WriteLine( "---------------FINISHED---------------" );

            Console.WriteLine( "Press any Key to enter TrainLoop({0})...", loops );
            Console.ReadKey();
            Console.Clear();

            for (var i = 1; i < loops + 1; i++) {
                Console.SetCursorPosition( 0, 0 );
                Console.WriteLine( "---------------TRAINING---------------" );
                t = DateTime.Now;
                _aiMaster.Trainloop( showOutput );
                SetAlgorythmos( _module );
                _module = new ModuleBase( 0, thaget, vMin, vMax, _lo, SlotAutomatAlgo, tolleranz );
                Console.WriteLine( "\ndone in " + ( DateTime.Now - t ) );
                Console.WriteLine( "---------------FINISHED---------------" );
                ProgressBar( i, loops );
            }

            var maxFinalVaule = 0D;
            var maxRound = 0D;
            var maxTs = TimeSpan.MaxValue;

            foreach (var stat in _aiMaster.Stats) {
                if (maxFinalVaule < stat.Finalvaule)
                    maxFinalVaule = stat.Finalvaule;
                if (maxRound < stat.Round1)
                    maxRound = stat.Round1;
                if (maxTs < stat.Time1)
                    maxTs = stat.Time1;
            }

            Console.SetCursorPosition( 0, Console.CursorTop + 3 );
            Console.WriteLine( $"MaxRound:{maxRound}\nMaxVaule:{maxFinalVaule}\nMaxTimespan:{maxTs}" );

            Console.WriteLine( "\n\n-------------RUNNING TESTS------------" );
            Console.WriteLine( "Press any Key..." );
            Console.ReadKey();
            t = DateTime.Now;


            SetAlgorythmos( _module );
            _module = new ModuleBase( 0, thaget, vMin, vMax, _lo, SlotAutomatAlgo, tolleranz );

            double max = 0;
            for (var i = 0; i < 100; i++) {
                SlotAutomatAlgo( _module, true );
                max = _module.Variable > max ? _module.Variable : max;
            }

            Console.WriteLine( $"\ndone in {DateTime.Now - t}       Max:{max}" );
            Console.WriteLine( "---------------FINISHED---------------" );
        }

        private void ProgressBar(int i, int max) {

            var rundenanzeige = $"[{ i}/{ max}]";
            var v = Console.WindowWidth - 3;
            Console.SetCursorPosition( 1, Console.CursorTop );
            for (var j = 0; j < ( i / (double) max ) * v; j++) {
                Console.Write( "=" );
            }
            Console.Write( ">" );
            for (var j = i; j < ( v - ( i / (double) max ) * v ) - 1; j++) {
                Console.Write( " " );
            }

            try { Console.SetCursorPosition( 0, Console.CursorTop ); }
            catch {
                // ignored
            }

            Console.Write( "[" );
            try { Console.SetCursorPosition( v + 1, Console.CursorTop ); }
            catch {
                // ignored
            }

            Console.Write( "]" );
            try { Console.SetCursorPosition( ( v + 1 ) - rundenanzeige.Length, Console.CursorTop - 1 ); }
            catch {
                // ignored
            }

            Console.Write( rundenanzeige );
        }
        #endregion

        #region ImageScanTest

        ImageSearch _mainimageseatch = new ImageSearch();
        private void _ImageScan() {
            const string pathToBmp = "..\\..\\..\\example.png";

            const ImageSearch.ImageSearchMethode ism = ImageSearch.ImageSearchMethode.Fast;
            Console.WriteLine( Path.GetDirectoryName( pathToBmp ) );

            var imageSearch = new ImageSearch( new Bitmap( pathToBmp ), _mainimageseatch.CaptureScreen(), ism, true, 50 );

            Process.Start( pathToBmp );

            while (true) {

                imageSearch.SeatchIn( _mainimageseatch.CaptureScreen() );
                Console.WriteLine( imageSearch.Result );
                imageSearch.Clean();
                Thread.Sleep( 250 );
            }
        }
        #endregion

        #region GraphicsForm

        private static void _GraphicsForm() {

            Console.WriteLine( "Bitte wählen:\n[0]" + GraphicsForm.WindowType.Form + "\n[1]" + GraphicsForm.WindowType.Fullscreen + "\n[2]" + GraphicsForm.WindowType.OverlaySingleWindow );
            var chois = -1;
            while (!int.TryParse( Console.ReadLine(), out chois ) && chois <= 3 && chois >= 0) {

            }

            var name = "";
            var type = GraphicsForm.WindowType.Form;
            switch (chois) {
                case 0:
                    type = GraphicsForm.WindowType.Form;
                    break;
                case 1:
                    type = GraphicsForm.WindowType.Fullscreen;
                    break;
                case 2:
                    type = GraphicsForm.WindowType.OverlaySingleWindow;
                    Console.Write( "Bitte Fensternahme angeben:" );
                    name = Console.ReadLine();
                    break;
            }
            var graphicsForm = new GraphicsForm( 20, DrawAction, type, name );
            Application.Run( graphicsForm );
        }

        private static void DrawAction(Graphics g, int frame) {
            const int max = 100;
            const int scale = 8;
            var k = frame % max;
            for (var i = 0; i < max; i++) {
                for (var j = 0; j < max; j++) {
                    g.FillRectangle( new SolidBrush( Color.FromArgb( 255, i, j, k ) ), new Rectangle( i * scale, j * scale, scale, scale ) );
                }
            }
        }

        #endregion
    }
}
