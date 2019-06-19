using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace overlaytests {
    internal class Program {
        public static bool draw    { get; set; }
        public static int  IUpdat  { get; set; }
        public static bool IUPDATE { get; set; }

        public static List<Rect> rectangles = new List<Rect>();
        public static List<Rect> MyList     = new List<Rect>();
        public static List<Rect> BaseList   = new List<Rect>();

        public static Form Overlay;

        const int whide  = 1450;
        const int heigth = 1350;

        const int iterations = 255;
        const int oSet       = 0;

        const int scale = 1;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            IUpdat  = 0;
            IUPDATE = false;

            var t = new Thread( () => ThreasStart() );
            //t.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            for ( var i = 0; i < whide; i += scale ) {
                for ( var j = 0; j < heigth; j += scale ) {
                    var a = (double) ( i - ( whide  / 2 ) ) / (double) ( (double) whide  / 4 );
                    var b = (double) ( j - ( heigth / 2 ) ) / (double) ( (double) heigth / 4 );
                    var c = new complex( a, b );
                    var z = new complex( 0, 0 );

                    var it = 0;

                    do {
                        it++;
                        z.Square();
                        z.Add( c );
                        if ( z.Magnitude() > 2.0 ) break;
                    } while ( it < iterations );

                    var color = Color.FromArgb( 100, it, it, it );

                    //color = it < iterations ? Color.Black : Color.White;

                    var rec   = new Rectangle( i + oSet, j + oSet, scale, scale );
                    var _rect = new Rect( rec, color, true, 0 );

                    BaseList.Add( _rect );
                }
            }

            rectangles = BaseList;

            IUpdat += 1000;
            //t.Start();
            Overlay = new Overlay();
            Application.Run( Overlay );
            Environment.Exit( 0 );
        }


        private static void ThreasStart() {
            while ( true ) {
                if ( rectangles.Count % 100 == 0 )
                    Thread.Sleep( 1 );

                var pos = reasources.GetCursorPosition();
                //if (MyList.Count > 300)
                //    MyList.RemoveAt( 0 );

                MyList = new List<Rect>();

                var c = new complex( pos.X - whide , pos.Y - heigth );
                var z = new complex( 0,     0 );

                var it         = 0;
                var iterations = 100;
                do {
                    it++;
                    z.Square();
                    z.Add( c );
                    if ( z.Magnitude() > 2.0 ) break;

                    var p = new Point( (int) z.a+whide/2, (int) z.b+ heigth / 2 );
                    Console.WriteLine( p.X + "    " + p.Y );

                    var color = Color.FromArgb( 255, 1, 1, 1 );
                    var rec   = new Rectangle( p, new Size( 10, 10 ) );
                    var _rect = new Rect( rec, color, true, 10 );

                    MyList.Add( _rect );
                } while ( it < iterations );

                rectangles = new List<Rect>(BaseList);
                rectangles.AddRange( MyList );
                IUpdat += 1;
                Thread.Sleep( 1 );
            }
        }
    }

    public class Rect {
        public Rectangle _rectangle { get; private set; }
        public Color     _color     { get; private set; }
        public bool      _fill      { get; private set; }
        public int       _size      { get; private set; }


        public Rect(Rectangle rec, Color color, bool fill, int size) {
            _size      = size;
            _fill      = fill;
            _color     = color;
            _rectangle = rec;
        }
    }

    public class reasources {
        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout( LayoutKind.Sequential )]
        public struct POINT {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point) {
                return new Point( point.X, point.Y );
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport( "user32.dll" )]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition() {
            POINT lpPoint;
            GetCursorPos( out lpPoint );
            //bool success = User32.GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }
    }
}