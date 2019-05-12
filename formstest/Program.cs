using overlaytests;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace formstest {
    static class Program {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new test() );
        }
        public static Bitmap Calculate_Mandelbrot(int Whide, int Heigth) {
            var bmap = new Bitmap( Whide, Heigth );

            var iterations = 255;

            for (var i = 0; i < Whide; i++) {
                for (var j = 0; j < Heigth; j++) {
                    bmap.SetPixel( i, j, Color.Red );
                }
            }

            var zom = 1D;
            var rangewWhide = (double) Whide / 2;
            var rangehHeigth = (double) Heigth / 2;

            for (var i = -rangewWhide; i < rangewWhide / zom; i += 1 / zom) {
                for (var j = -rangehHeigth; j < rangehHeigth / zom; j += 1 / zom) {


                    if (false) {
                        if (i <= Whide / 3 || i >= Whide - Whide / 3) continue;
                        if (j >= Heigth - Heigth / 2) continue;
                    }

                    //var a = ( i - ( (double) whide / 2 ) ) / ( (double) whide / 4 );
                    //var b = ( j - ( (double) heigth / 2 ) ) / ( (double) heigth / 4 );
                    var a = i / ( (double) Whide / 4 );
                    var b = j / ( (double) Heigth / 4 ); ;

                    var c = new complex( a, b );
                    var z = new complex( 0, 0 );

                    var it = 0;

                    do {
                        it++;
                        z.Square();
                        z.Add( c );
                        if (z.Magnitude() > 2.0) break;
                    } while (it < iterations);

                    var color = Color.FromArgb( 255, it, it, it );
                    var x = (int) ( ( i + rangewWhide ) * zom );
                    if (x < 0)
                        x = 0;
                    if (x >= Whide)
                        x = Whide - 1;

                    var y = (int) ( ( j + rangehHeigth ) * zom );
                    if (y < 0)
                        y = 0;
                    if (y >= Heigth)
                        y = Heigth - 1;

                    bmap.SetPixel( x, y, color );

                }
            }
            return bmap;
        }
    }
}
