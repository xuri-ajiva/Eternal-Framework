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
            //t.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            int whide = 400;
            int heigth = 400;

            int iterations = 255;
            int oSet = 100;

            for (int i = 0; i < whide; i++) {
                for (int j = 0; j < heigth; j++) {

                    double a = (double)( i - ( whide / 2 ) ) / (double) ( (double) whide / 4 );
                    double b = (double)( j - ( heigth / 2 ) ) / (double) ( (double) heigth / 4 );
                    complex c = new complex( a, b );
                    complex z = new complex( 0, 0 );

                    int it = 0;

                    do {
                        it++;
                        z.Square();
                        z.Add( c );
                        if (z.Magnitude() > 2.0) break;
                    } while (it < iterations);
                    
                    Color color = Color.FromArgb( 255, it, it, it );

                    //color = it < iterations ? Color.Black : Color.White;

                    Rectangle rec = new Rectangle( i + oSet, j + oSet, 1, 1 );
                    Rect _rect = new Rect( rec, color, true, 0 );

                    rectangles.Add( _rect );
                }
            }


            IUpdat += 1000;

            Overlay = new Overlay();
            Application.Run( Overlay );
        }

        private static void ThreasStart() {
            while (true) {
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
