using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Eternal.Forms {
    partial class GraphicsForm {
        private IContainer components;

        protected override void Dispose(bool disposing) {
            // ReSharper disable once UseNullPropagation
            if (disposing && ( components != null )) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        private void InitializeComponent() {
            components = new Container();
            // ReSharper disable once RedundantNameQualifier
            _updater = new System.Windows.Forms.Timer( components );
            SuspendLayout();
            // 
            // Updater
            // 
            _updater.Interval = 1;
            _updater.Tick += IUpdate;
            // 
            // GraphicsForm
            // 
            AutoScaleDimensions = new SizeF( 8F, 16F );
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size( 500, 500 );
            Name = "GraphicsForm";
            Text = "Form";
            Load += ILoad;
            Closing += IClosing;
            ResumeLayout( false );

        }

        private void IClosing(object sender, CancelEventArgs e) {
            Environment.Exit( 0 );
        }

        // ReSharper disable once RedundantNameQualifier
        private System.Windows.Forms.Timer _updater;
    }
    public partial class GraphicsForm : Form {
        public enum WindowType {
            Form,
            Fullscreen,
            OverlaySingleWindow
        }

        [DllImport( "user32.dll", SetLastError = true )]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport( "user32.dll", SetLastError = true )]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport( "user32.dll", SetLastError = true )]
        // ReSharper disable once UnusedMember.Local
        private static extern IntPtr FindWindow(string ipClassName, string ipWindowName);[DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout( LayoutKind.Sequential )]
        // ReSharper disable once InconsistentNaming
        public struct RECT {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }

        private readonly WindowType _type;
        private readonly int _tpfInt;
        private readonly float _tpFoset;
        private readonly string _windowToOverlay;

        private RECT _rect;
        private Rectangle _rectangle;
        private IntPtr _hWnd = IntPtr.Zero;

        private readonly DrawSource _drawSource;

        private int _fps;
        private void FpsUpdater() {
            while (true) {
                Thread.Sleep( 1000 );
                if (_type == WindowType.OverlaySingleWindow) return;
                try {

                    MethodInvoker del = delegate { Text = $"FPS:{_fps}"; };
                    Invoke( del );
                }
                catch { //ig
                }

                _fps = 0;
            }
        }

        public GraphicsForm(int maxFps, WindowType type, string windowToOverlay = "", bool fpscounter = true) {
            _type = type;
            _windowToOverlay = windowToOverlay;

            var maxFps1 = maxFps;
            var tpf = ( (float) 1000 / maxFps1 );
            _tpfInt = Math.DivRem( 1000, maxFps1, out _ );
            _tpFoset = _rest += tpf - _tpfInt;

            InitializeComponent();
            _drawSource = new DrawSource();

            _drawSource.DrawAction += DrawSourceOnDrawAction;

            Controls.Add( _drawSource );

            if (!fpscounter) return;
            var fpsThread = new Thread( FpsUpdater );
            fpsThread.Start();
        }

        private void DrawSourceOnDrawAction(Graphics arg1, int arg2) {
            DrawUpdate?.Invoke( arg1, arg2 );
        }

        public event Action<Graphics, int> DrawUpdate;


        // ReSharper disable once InconsistentNaming
        private void ILoad(object sender, EventArgs e) {

            BackColor = Color.FromArgb( 220, 126, 204 );
            TransparencyKey = BackColor;
            DoubleBuffered = true;

            if (_type != WindowType.Form) {
                ShowInTaskbar = false;
                FormBorderStyle = FormBorderStyle.None;
                TopMost = true;

                var initialStyle = GetWindowLong( Handle, -20 );
                SetWindowLong( Handle, -20, initialStyle | 0x80000 | 0x20 );

                Location = new Point( 0, 0 );
                Size = Screen.PrimaryScreen.Bounds.Size;
            }

            if (_type == WindowType.OverlaySingleWindow && _windowToOverlay != "") {
                set_hWnd();
            }

            _updater.Start();
        }

        private float _rest;
        // ReSharper disable once InconsistentNaming
        private void IUpdate(object sender, EventArgs e) {
            _rest += _tpFoset;
            var bonus = 0;
            if (_rest >= 1)
                bonus += 1;
            _rest -= bonus;
            _updater.Interval = _tpfInt + bonus;
            if(bonus>0)
                Console.WriteLine( bonus );

            if (_type == WindowType.OverlaySingleWindow)
                Overlay();

            _fps++;
            _drawSource.Refresh();
        }

        private bool set_hWnd() {
            if (_windowToOverlay == "") return false;
            try {
                _hWnd = FindWindow( null, _windowToOverlay );
                if (_hWnd != IntPtr.Zero) return true;

                Console.WriteLine( "Window Not Found!" );
                Application.Exit();
            }
            catch {
                //  
            }
            return false;
        }

        private void Overlay() {

            if (_hWnd == IntPtr.Zero) if (!set_hWnd()) return;

            GetWindowRect( _hWnd, out _rect );
            _rectangle.X = _rect.Left;
            _rectangle.Y = _rect.Top;
            _rectangle.Width = _rect.Right - _rect.Left + 1;
            _rectangle.Height = _rect.Bottom - _rect.Top + 1;

            Size = new Size( _rectangle.Width, _rectangle.Height );
            Top = _rect.Top;
            Left = _rect.Left;
        }

        #region public

        public DrawSource DrawSource => _drawSource;

        public int Fps => _fps;

        public IntPtr HWnd => _hWnd;

        public RECT Rect => _rect;

        public Rectangle Rectangle => _rectangle;

        public float Rest => _rest;

        public int TpfInt => _tpfInt;

        public float TpFoset => _tpFoset;

        public WindowType Type => _type;

        // ReSharper disable once RedundantNameQualifier
        public System.Windows.Forms.Timer Updater => _updater;

        public string WindowToOverlay => _windowToOverlay;

        public IContainer Components => components;

        #endregion
    }

    public sealed class DrawSource : Panel {
        public DrawSource() {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            Location = new Point( 0, 0 );
            Name = "DrawSource";
            TabIndex = 0;
            Paint += IPaint;
        }

        // ReSharper disable once InconsistentNaming
        private void IPaint(object sender, PaintEventArgs e) {
            G = e.Graphics;
            G.Clear( BackColor );
            Frame += 1;

            DrawAction?.Invoke( G, Frame );
        }

        public event Action<Graphics, int> DrawAction;

        private int Frame { get; set; }

        private Graphics G { get; set; }
    }
}
