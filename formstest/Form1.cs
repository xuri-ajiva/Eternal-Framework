using overlaytests;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace formstest {
    public partial class test : Form {
        public test() {
            InitializeComponent();
        }

        private int frame = 0;

        private Bitmap bm = new Bitmap( 255, 255 );
        private const float MaxFps = 120f;


        private void Form1_Load(object sender, EventArgs e) {

            pictureBox1.Dock = DockStyle.Fill;
            //pictureBox1.Size = new Size( 255, 255 );
            //pictureBox1.Location = new Point( 10, 10 );

            DoubleBuffered = true;

            Genbitmap();

            var fps = 0;
            var update = new Thread( () => {

                while (true) {
                    fps += 1;
                    frame += 1;

                    pictureBox1.Image = Program.Calculate_Mandelbrot( pictureBox1.Width, pictureBox1.Height );

                    //pictureBox1.Image = OnUpdate( frame, 255, 255 );
                    Thread.Sleep( (int) ((float) 1000 /  MaxFps ) );
                }
            } );
            update.Start();
            var fpsupdate = new Thread( () => {
                while (true) {
                    Thread.Sleep( 1000 );

                    MethodInvoker del = delegate { Text = $"FPS:{fps}"; };
                    Invoke( del );
                    fps = 0;
                }
            } );
            fpsupdate.Start();
        }

        private void Genbitmap() {
            for (var i = 0; i < 255; i++) {
                for (var j = 0; j < 255; j++) {
                    bm.SetPixel( i, j, calculatecolor( i, j, frame ) );
                }
            }
        }

        private Bitmap OnUpdate(int frame, int Width, int Height) {
            if (true) {
                bm = new Bitmap( Width, Height );

                for (var i = 0; i < 255; i++) {
                    for (var j = 0; j < 255; j++) {
                        bm.SetPixel( i, j, calculatecolor( i, j, frame ) );
                        //bm.SetPixel( i, j, Color.AntiqueWhite );
                    }
                }
            }

            return bm;
        }

        private Color calculatecolor(int i, int j, int frame) {
            var c = Color.FromArgb( 255, i, j, frame % 255 );
            return c;
        }

        private void button1_Click(object sender, EventArgs e) {
            pictureBox1.Image = Program.Calculate_Mandelbrot( pictureBox1.Width, pictureBox1.Height );
        }

        public static void wait(int milliseconds) {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) => {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled) {
                Application.DoEvents();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            Environment.Exit( 0 );
        }
    }
}
