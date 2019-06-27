using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Eternal.MathE;

namespace Eternal.Forms {
    partial class GraphicsForm {
        private IContainer components;

        protected override void Dispose(bool disposing) {
            // ReSharper disable once UseNullPropagation
            if ( disposing && ( this.components != null ) ) {
                this.components.Dispose();
            }

            base.Dispose( disposing );
        }

        private void InitializeComponent() {
            this.components = new Container();
            SuspendLayout();
            // 
            // GraphicsForm
            // 
            AutoScaleDimensions =  new SizeF( 6F, 13F );
            AutoScaleMode       =  AutoScaleMode.Font;
            ClientSize          =  new Size( 100, 100 );
            Margin              =  new Padding( 2, 2, 2, 2 );
            Name                =  "GraphicsForm";
            Text                =  "Form";
            this.Load                += ILoad;
            this.Closing             += IClosing;

            ResumeLayout( false );
        }

        private void IClosing(object sender, CancelEventArgs e) { Environment.Exit( 0 ); }
        
    }

    public partial class GraphicsForm : Form {
        public enum WindowType {
            Form,
            Fullscreen,
            OverlaySingleWindow
        }

        [DllImport( "user32.dll", SetLastError = true )] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport( "user32.dll", SetLastError = true )] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport( "user32.dll", SetLastError = true )]
        // ReSharper disable once UnusedMember.Local
        private static extern IntPtr FindWindow(string ipClassName, string ipWindowName);

        [DllImport( "user32.dll" )]
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
        private readonly string     _windowToOverlay;

        private RECT      _rect;
        private Rectangle _rectangle;
        private IntPtr    _hWnd = IntPtr.Zero;

        private readonly Color _transparentColor;
        private readonly bool  _frequentUpdate;

        private readonly DrawSource _drawSource;
        private          MicroTimer microTimer;

        public GraphicsForm(WindowType type, Color transparentColor, string windowToOverlay = "", bool frequentUpdate = false, int maxFps = 10) {
            this._type             = type;
            this._transparentColor = transparentColor;
            this._windowToOverlay  = windowToOverlay;
            this._frequentUpdate   = frequentUpdate;

            var tpf = ( (long) MicroStopwatch.µs / maxFps );

            InitializeComponent();
            this._drawSource = new DrawSource();

            this._drawSource.DrawAction += DrawSourceOnDrawAction;

            Controls.Add( this._drawSource );

            this.microTimer                   =  new MicroTimer();
            this.microTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler( IUpdate );

            this.microTimer.Interval = tpf;
        }

        private void DrawSourceOnDrawAction(Graphics g, int f) {
            var i = new DrawInfo( this._transparentColor, g, f );

            this.DrawUpdate?.Invoke( i );
        }

        public event Func<DrawInfo, DrawInfo> DrawUpdate;


        public void FireUpdate() { this._drawSource.Invoke( new Action( () => this._drawSource.Refresh() ) ); }

        // ReSharper disable once InconsistentNaming
        private void ILoad(object sender, EventArgs e) {
            BackColor       = this._transparentColor;
            TransparencyKey = BackColor;
            DoubleBuffered  = true;

            if ( this._type != WindowType.Form ) {
                ShowInTaskbar   = false;
                FormBorderStyle = FormBorderStyle.None;
                TopMost         = true;

                var initialStyle = GetWindowLong( Handle, -20 );
                SetWindowLong( Handle, -20, initialStyle | 0x80000 | 0x20 );

                Location = new Point( 0, 0 );
                Size     = Screen.PrimaryScreen.Bounds.Size;
            }
            else {
                this._drawSource.MouseMove += new MouseEventHandler( FormMouseMove );
                this._drawSource.MouseDown += new MouseEventHandler( FormMouseDown );

                ShowInTaskbar   = true;
                FormBorderStyle = FormBorderStyle.Sizable;
                TopMost         = false;
            }

            if ( this._type == WindowType.OverlaySingleWindow && this._windowToOverlay != "" ) {
                set_hWnd();
            }

            // this._updater.Start();
            this.microTimer.Enabled = true; // Start timer
        }

        // ReSharper disable once InconsistentNaming
        private void IUpdate(object sender, EventArgs e) {
            if ( this._type == WindowType.OverlaySingleWindow ) Overlay();

            if ( this._frequentUpdate ) FireUpdate();
        }

        private bool set_hWnd() {
            if ( this._windowToOverlay == "" ) return false;
            try {
                this._hWnd = FindWindow( null, this._windowToOverlay );
                if ( this._hWnd != IntPtr.Zero ) return true;

                this.microTimer.Enabled = false;
                throw new Exception( "Window Not Found!" );
            } catch {
                //  
            }

            return false;
        }

        private void Overlay() {
            if ( this._hWnd == IntPtr.Zero )
                if ( !set_hWnd() )
                    return;
            GetWindowRect( this._hWnd, out this._rect );
            this._rectangle.X      = this._rect.Left;
            this._rectangle.Y      = this._rect.Top;
            this._rectangle.Width  = this._rect.Right  - this._rect.Left + 1;
            this._rectangle.Height = this._rect.Bottom - this._rect.Top  + 1;

            Invoke( new Action( () => Size = new Size( this._rectangle.Width, this._rectangle.Height ) ) );
            Invoke( new Action( () => Top  = this._rect.Top ) );
            Invoke( new Action( () => Left = this._rect.Left ) );
        }

    #region Handler für das Verschieben der Form

        private Point _mousePosition;

        private void FormMouseDown(object sender, MouseEventArgs e) { this._mousePosition = new Point( -e.X, -e.Y ); }

        private void FormMouseMove(object sender, MouseEventArgs e) {
            // Wenn der Linke Button gedrückt ist
            if ( e.Button == MouseButtons.Left ) {
                // Maus-Position auf dem Control
                Point mousePos = MousePosition;

                // Verschiebt den Punkt um den angegebenden Betrag
                mousePos.Offset( this._mousePosition.X, this._mousePosition.Y );

                // Neue Position setzen
                Location = mousePos;
            }
        }

    #endregion

    #region public

        public DrawSource DrawSource => this._drawSource;

        public IntPtr HWnd => this._hWnd;

        public RECT Rect => this._rect;

        public Rectangle Rectangle => this._rectangle;

        public WindowType Type => this._type;

        public string WindowToOverlay => this._windowToOverlay;

        public IContainer Components => this.components;

        public Size WindowSize { get => Size; set => Size = value; }

    #endregion
    }

    public sealed class DrawSource : Panel {
        [DebuggerStepThrough]
        public DrawSource() {
            DoubleBuffered =  true;
            Dock           =  DockStyle.Fill;
            Location       =  new Point( 0, 0 );
            Name           =  "DrawSource";
            TabIndex       =  0;
            this.Paint     += IPaint;
        }

        // ReSharper disable once InconsistentNaming
        [DebuggerStepThrough]
        private void IPaint(object sender, PaintEventArgs e) {
            G = e.Graphics;
            G.Clear( BackColor );
            Frame += 1;

            this.DrawAction?.Invoke( G, Frame );
        }

        public event Action<Graphics, int> DrawAction;

        private int Frame { get; set; }

        private Graphics G { get; set; }
    }

    public class DrawInfo {
        private Graphics _g;
        private Color    _transparentColor;

        private int _frame;

        [DebuggerStepThrough]
        public DrawInfo(Color transparentColor, Graphics g = null, int frame = 0) {
            this._transparentColor = transparentColor;
            this._g                = g;
            this._frame            = frame;
        }

        public Graphics G {
            [DebuggerStepThrough] get => this._g;
            set => this._g = value;
        }

        public int Frame {
            [DebuggerStepThrough] get => this._frame;
            set => this._frame = value;
        }

        public Color TransparentColor {
            [DebuggerStepThrough] get => this._transparentColor;
            set => this._transparentColor = value;
        }
    }
}