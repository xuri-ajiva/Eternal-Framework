using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace overlaytests {
    internal class Program {
        public static bool draw { get; set; }
        public static int IUpdat { get; set; }
        public static bool IUPDATE { get; set; }

        public static List<Rect> rectangles = new List<Rect>();

        public static Form Overlay;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            IUpdat = 0;
            IUPDATE = false;

            Thread t = new Thread( () => ThreasStart() );
            t.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            Overlay = new Overlay();
            Application.Run( Overlay );
        }

        private static void ThreasStart() {
            while (true) {
                //while (IUpdat > 0)
                //    Thread.Sleep(1);
                if (rectangles.Count % 100 == 0)
                    Thread.Sleep( 1 );
                var r1 = new Random();
                var r2 = new Random();
                var r3 = new Random();
                var r4 = new Random();

                Color color = Color.FromArgb( r1.Next( 256 ), r2.Next( 256 ), r3.Next( 256 ) );
                Rectangle rec = new Rectangle( r4.Next( 0, Screen.PrimaryScreen.Bounds.Width ), r1.Next( 0, Screen.PrimaryScreen.Bounds.Height ), 10, 10 );
                Rect _rect = new Rect( rec, color, /*r2.Next(0, 2) == 1 ? true : false*/true, r3.Next( 0, 10 ) );

                rectangles.Add( _rect );

                IUpdat += 1;
                if (rectangles.Count % 100 == 0)
                    Console.WriteLine( IUpdat.ToString() + " : " + ( rectangles.Count ).ToString() );
            }
        }
    }

    public class Rect {
        public Rectangle _rectangle { get; private set; }
        public Color _color { get; private set; }
        public bool _fill { get; private set; }
        public int _size { get; private set; }


        public Rect(Rectangle rec, Color color, bool fill, int size) {
            _size = size;
            _fill = fill;
            _color = color;
            _rectangle = rec;
        }
    }
}
