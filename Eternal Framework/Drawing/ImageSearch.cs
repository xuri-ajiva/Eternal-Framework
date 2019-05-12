using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Eternal.Drawing {
    public class ImageSearch : EternalFramework.EternalMain {
        public enum ImageSearchMethode {
            Fast = 129093810,
            Slow = 329876329
        }

        private Bitmap _seatchForBitmap;
        private Bitmap _seatchInBitmap;
        private ImageSearchMethode _imageSearchMethode = ImageSearchMethode.Fast;
        private bool _found = false;
        private bool _joinThread = true;
        private TimeSpan _timeSpan = new TimeSpan();
        private Point _point = new Point();
        private int _toleranz = 10;
        private bool ctor;
        private string _result;

        public ImageSearch() {
        }

        public ImageSearch(Bitmap seatchForBitmap, Bitmap seatchInBitmap, ImageSearchMethode imageSearchMethode,
            bool joinThread = true, int toleranz = 10) {
            ctor = true;

            _seatchForBitmap = seatchForBitmap ?? throw new ArgumentNullException( nameof( seatchForBitmap ) );
            _seatchInBitmap = seatchInBitmap ?? throw new ArgumentNullException( nameof( seatchInBitmap ) );
            _imageSearchMethode = imageSearchMethode;
            _joinThread = joinThread;
            _toleranz = toleranz;
        }

        #region findFast

        public Point? FindBitmap_Fast(Bitmap haystack, Bitmap needle, int toleranz = 0) {
            if (null == haystack || null == needle) {
                return null;
            }
            if (haystack.Width < needle.Width || haystack.Height < needle.Height) {
                return null;
            }

            var haystackArray = GetPixelArray( haystack );
            var needleArray = GetPixelArray( needle );

            foreach (var firstLineMatchPoint in FindMatch( haystackArray.Take( haystack.Height - needle.Height ), needleArray[0], toleranz )) {
                if (IsNeedlePresentAtLocation( haystackArray, needleArray, firstLineMatchPoint, 1, toleranz )) {
                    return firstLineMatchPoint;
                }
            }

            return null;
        }

        private int[][] GetPixelArray(Bitmap bitmap) {
            var result = new int[bitmap.Height][];
            var bitmapData = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb );

            for (var y = 0; y < bitmap.Height; ++y) {
                result[y] = new int[bitmap.Width];
                Marshal.Copy( bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length );
            }

            bitmap.UnlockBits( bitmapData );

            return result;
        }

        private IEnumerable<Point> FindMatch(IEnumerable<int[]> haystackLines, int[] needleLine, int toleranz = 0) {
            var y = 0;
            foreach (var haystackLine in haystackLines) {
                for (int x = 0, n = haystackLine.Length - needleLine.Length; x < n; ++x) {
                    if (ContainSameElements( haystackLine, x, needleLine, 0, needleLine.Length, toleranz )) {
                        yield return new Point( x, y );
                    }
                }
                y += 1;
            }
        }

        private bool ContainSameElements(int[] first, int firstStart, int[] second, int secondStart, int length, int toleranz = 0) {
            for (var i = 0; i < length; ++i) {
                if (Math.Abs( first[i + firstStart] - second[i + secondStart] ) > toleranz) {
                    return false;
                }
            }
            return true;
        }

        private bool IsNeedlePresentAtLocation(int[][] haystack, int[][] needle, Point point, int alreadyVerified, int toleranz = 0) {
            //alreadyVerified -> skip
            for (var y = alreadyVerified; y < needle.Length; ++y) {
                if (!ContainSameElements( haystack[y + point.Y], point.X, needle[y], 0, needle.Length, toleranz )) {
                    return false;
                }
            }
            return true;
        }

        #endregion

        public Bitmap CaptureScreen() {
            var image = new Bitmap( Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb );
            var gfx = Graphics.FromImage( image );
            gfx.CopyFromScreen( Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy );
            return image;
        }

        public Bitmap CaptureScreen(int X, int Y, int Width, int Height) {
            var image = new Bitmap( Width, Height, PixelFormat.Format32bppArgb );
            var gfx = Graphics.FromImage( image );
            gfx.CopyFromScreen( X, Y, 0, 0, new Size( Width, Height ), CopyPixelOperation.SourceCopy );
            return image;
        }

        private Point? FindBitmap_Slow(Bitmap Searchin, Bitmap Searchfor, int toleranz = 0) {
            for (var outerX = 0; outerX < Searchin.Width - Searchfor.Width; outerX++) {
                for (var outerY = 0; outerY < Searchin.Height - Searchfor.Height; outerY++) {
                    for (var innerX = 0; innerX < Searchfor.Width; innerX++) {
                        for (var innerY = 0; innerY < Searchfor.Height; innerY++) {
                            var searchColor = Searchfor.GetPixel( innerX, innerY );
                            var withinColor = Searchin.GetPixel( outerX + innerX, outerY + innerY );

                            if (Math.Abs( searchColor.R - withinColor.R ) > toleranz ||
                                Math.Abs( searchColor.G - withinColor.G ) > toleranz ||
                                Math.Abs( searchColor.B - withinColor.B ) > toleranz ||
                                Math.Abs( searchColor.A - withinColor.A ) > toleranz) {
                                goto NotFound;
                            }
                        }
                    }
                    ///*point.X += Searchfor.Width / 2; // Set X to the middle of the bitmap.
                    ///*point.Y += Searchfor.Height / 2; // Set Y to the center of the bitmap.
                    return new Point( outerX, outerY );

                NotFound:
                    ;
                }
            }
            return null;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void Seatch(Bitmap SeatchFor, Bitmap SeatchIn) {
            _seatchInBitmap = SeatchIn;
            _seatchForBitmap = SeatchFor;
            Seatch();
        }
        public void SeatchIn(Bitmap SeatchIn) {
            _seatchInBitmap = SeatchIn;
            Seatch();
        }
        public void SeatchFor(Bitmap SeatchFor) {
            _seatchForBitmap = SeatchFor;
            Seatch();
        }
        public void Seatch() {
            if (!ctor) throw new NullReferenceException( "Use Constructor to create objekt!" );

            var k = DateTime.Now;

            Point? res = null;

            var seatch = new Thread( () => {
                res = _imageSearchMethode == ImageSearchMethode.Fast ? FindBitmap_Fast( _seatchInBitmap, _seatchForBitmap, _toleranz ) :
                    FindBitmap_Slow( _seatchInBitmap, _seatchForBitmap, _toleranz );
            } );
            seatch.Start();

            if (_joinThread)
                seatch.Join();

            if (res.HasValue) {
                _found = true;
                _point = new Point( res.Value.X, res.Value.Y );
            }
            else {
                _found = false;
                _point = new Point( 0, 0 );
            }

            _timeSpan = DateTime.Now - k;
            _result = $"Seatch returned: {_found},  X={_point.X}, Y={_point.Y}, TimeSpan={_timeSpan}";
        }

        public void Clean() {
            _found = false;
            _timeSpan = new TimeSpan();
            _point = new Point();
            _result = "";
        }

        #region only for lasys

        public string SeatchImageWithInfo(Bitmap _Searchin, Bitmap _Searchfor, ImageSearchMethode imageSearchMethode, int toleranz = 0, bool jointhread = true) {
            _seatchForBitmap = _Searchfor;
            _seatchInBitmap = _Searchin;
            _imageSearchMethode = imageSearchMethode;
            _toleranz = toleranz;
            _joinThread = jointhread;

            var tmp = ctor;
            if (tmp == false)
                ctor = true;
            Seatch();
            ctor = tmp;

            return $"Seatch returned: {_found},  X={_point.X}, Y={_point.Y}, TimeSpan={_timeSpan}";
        }

        public Point?[] SeatchImages(Bitmap[] _Searchfor, out string result, ImageSearchMethode imageSearchMethode, int toleranz = 0, bool jointhread = true) {
            var f = 0;
            var a = _Searchfor.Length;
            var dt = DateTime.Now;
            SeatchInBitmap = CaptureScreen();
            _imageSearchMethode = imageSearchMethode;
            _toleranz = toleranz;
            _joinThread = jointhread;

            var Presults = new Point?[a];

            var tmp = ctor;
            if (tmp == false)
                ctor = true;

            for (var i = 0; i < a; i++) {
                Seatch();

                Presults[i] = _point;
            }

            ctor = tmp;

            result = $"Found {f} from {a} images. TimeSpan:{( DateTime.Now - dt )}";

            return Presults;
        }

        public bool SeatchImage(Bitmap _Searchin, Bitmap _Searchfor, ImageSearchMethode imageSearchMethode, int toleranz = 0, bool jointhread = true) {
            _seatchForBitmap = _Searchfor;
            _seatchInBitmap = _Searchin;
            _imageSearchMethode = imageSearchMethode;
            _toleranz = toleranz;
            _joinThread = jointhread;

            var tmp = ctor;
            if (tmp == false)
                ctor = true;
            Seatch();
            ctor = tmp;

            return _found;
        }

        public bool ImageIsOnScreen(Bitmap _Searchfor, ImageSearchMethode imageSearchMethode) {
            return SeatchImage( CaptureScreen(), _Searchfor, imageSearchMethode );
        }

        #endregion

        #region Propertis


        public string Result => _result;

        public Bitmap SeatchForBitmap {
            get => _seatchForBitmap;
            set => _seatchForBitmap = value;
        }

        public Bitmap SeatchInBitmap {
            get => _seatchInBitmap;
            set => _seatchInBitmap = value;
        }

        public ImageSearchMethode imageSearchMethode {
            get => _imageSearchMethode;
            set => _imageSearchMethode = value;
        }

        public bool Found {
            get => _found;
            set => _found = value;
        }

        public bool JoinThread {
            get => _joinThread;
            set => _joinThread = value;
        }

        public TimeSpan TimeSpan {
            get => _timeSpan;
            set => _timeSpan = value;
        }

        public Point Point {
            get => _point;
            set => _point = value;
        }

        public int Toleranz {
            get => _toleranz;
            set => _toleranz = value;
        }

        #endregion
    }
}
