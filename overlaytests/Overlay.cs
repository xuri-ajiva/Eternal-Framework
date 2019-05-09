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

            this.BackColor = Color.DeepPink;
            this.TransparencyKey = this.BackColor;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;

            int initialStyle = GetWindowLong( this.Handle, -20 );
            SetWindowLong( this.Handle, -20, initialStyle | 0x80000 | 0x20 );

            this.Location = new Point( 0, 0 );
            this.Size = Screen.PrimaryScreen.Bounds.Size;

            Updater.Start();
        }
    }

    public class Source : Panel {
        private Graphics g;
        public Brush brush = new SolidBrush( Color.Black );

        public Source() {
            this.DoubleBuffered = true;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Location = new System.Drawing.Point( 0, 0 );
            this.Name = "Source";
            this.TabIndex = 0;
            this.Paint += new System.Windows.Forms.PaintEventHandler( this.IPaint );

            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //UpdateStyles();
        }
        private void IPaint(object sender, PaintEventArgs e) {
            if (Program.draw) {
                g = e.Graphics;
                g.Clear( this.BackColor );

                for (int i = 0; i < Program.rectangles.Count; i++) {
                    Rect r = Program.rectangles[i];

                    Pen p = new Pen( r._color, r._size );

                    Brush b = new SolidBrush( r._color );

                    if (r._fill)
                        g.FillRectangle( b, r._rectangle );
                    if (!r._fill)
                        g.DrawRectangle( p, r._rectangle );
                }
            } else
                Program.draw = true;
        }
    }

}
