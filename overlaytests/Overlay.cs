using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace overlaytests {
    public partial class Overlay : Form {


        [DllImport( "user32.dll", SetLastError = true )]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport( "user32.dll", SetLastError = true )]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport( "user32.dll", SetLastError = true )]
        static extern IntPtr FindWindow(string IpClassName, string IpWindowName);

        public Overlay() {
            InitializeComponent();
        }

        public void IUpdate(object sender, EventArgs e) {
            //Source.Update();
            if (Program.IUpdat > 0) {
                Display.Refresh();
                Program.IUpdat = 0;
                Program.IUPDATE = false;
            }
        }

        private void ILoad(object sender, EventArgs e) {
            var r = new Random();

            BackColor = Color.DeepPink;
            TransparencyKey = BackColor;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            DoubleBuffered = true;
            ShowInTaskbar = false;

            var initialStyle = GetWindowLong( Handle, -20 );
            SetWindowLong( Handle, -20, initialStyle | 0x80000 | 0x20 );

            Location = new Point( 0, 0 );
            Size = Screen.PrimaryScreen.Bounds.Size;

            Updater.Start();
        }
    }

    public class Source : Panel {
        private Graphics g;
        public Brush brush = new SolidBrush( Color.Black );

        public Source() {
            DoubleBuffered = true;
            Dock = System.Windows.Forms.DockStyle.Fill;
            Location = new System.Drawing.Point( 0, 0 );
            Name = "Source";
            TabIndex = 0;
            Paint += new System.Windows.Forms.PaintEventHandler( IPaint );
        }

        private void IPaint(object sender, PaintEventArgs e) {
            if (Program.draw) {
                g = e.Graphics;
                g.Clear( BackColor );

                for (var i = 0; i < Program.rectangles.Count; i++) {
                    var r = Program.rectangles[i];

                    var p = new Pen( r._color, r._size );

                    Brush b = new SolidBrush( r._color );

                    if (r._fill)
                        g.FillRectangle( b, r._rectangle );
                    if (!r._fill)
                        g.DrawRectangle( p, r._rectangle );
                }
            }
            else
                Program.draw = true;
        }
    }

}
