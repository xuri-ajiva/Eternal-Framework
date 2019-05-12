using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Eternal.Forms {
    partial class GraphicsForm {
        private System.ComponentModel.IContainer components;

        protected override void Dispose(bool disposing) {
            // ReSharper disable once UseNullPropagation
            if (disposing && ( components != null )) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
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
        private struct RECT {
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

        private readonly DrawSource _drawSource;

        private int _fps;
        private void FpsUpdater() {
            while (true) {
                Thread.Sleep( 1000 );
                try {

                    MethodInvoker del = delegate { Text = $"FPS:{_fps}"; };
                    Invoke( del );
                }
                catch { //ig
                }

                _fps = 0;
            }
        }

        public GraphicsForm(int maxFps, Action<Graphics, int> drawAction, WindowType type, string windowToOverlay = "", bool fpscounter = true) {
            _type = type;
            _windowToOverlay = windowToOverlay;

            var maxFps1 = maxFps;
            var tpf = ( (float) 1000 / maxFps1 );
            _tpfInt = Math.DivRem( 1000, maxFps1, out _ );
            _tpFoset = _rest += tpf - _tpfInt;

            InitializeComponent();
            _drawSource = new DrawSource( drawAction ?? throw new ArgumentNullException( nameof( drawAction ) ) );

            Controls.Add( _drawSource );

            if (!fpscounter) return;
            var fpsThread = new Thread( FpsUpdater );
            fpsThread.Start();
        }

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
                var hWnd2 = FindWindow( null, _windowToOverlay );
                GetWindowRect( hWnd2, out _rect );


                _rectangle.X = _rect.Left;
                _rectangle.Y = _rect.Top;
                _rectangle.Width = _rect.Right - _rect.Left + 1;
                _rectangle.Height = _rect.Bottom - _rect.Top + 1;

                Size = new Size( _rectangle.Width, _rectangle.Height );
                Top = _rect.Top;
                Left = _rect.Left;
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

            _fps++;
            _drawSource.Refresh();
        }
    }

    public sealed class DrawSource : Panel {
        public DrawSource(Action<Graphics, int> drawAction) {
            DrawAction = drawAction;

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

            DrawAction( G, Frame );
        }

        private Action<Graphics, int> DrawAction { get; }

        private int Frame { get; set; }

        private Graphics G { get; set; }
    }
}
