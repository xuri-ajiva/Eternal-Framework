using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System.ImageScan {
    public class ImageScan {
        public enum ImageScanMethode {
            Fast = 129093810,
            Slow = 329876329
        }

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
                Runtime.InteropServices.Marshal.Copy( bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length );
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
            //we already know that "alreadyVerified" lines already match, so skip them
            for (var y = alreadyVerified; y < needle.Length; ++y) {
                if (!ContainSameElements( haystack[y + point.Y], point.X, needle[y], 0, needle.Length, toleranz )) {
                    return false;
                }
            }
            return true;
        }

        public Bitmap CaptureScreen() {
            var image = new Bitmap( Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb );
            var gfx = Graphics.FromImage( image );
            gfx.CopyFromScreen( Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy );
            return image;
        }

        private void _Thread(Bitmap Sertchin, Bitmap Sertchfor, out bool R, out Point point, out TimeSpan TimeSpan, ImageScanMethode imageScanMethode, int toleranz = 0) {

            var k = DateTime.Now;

            var res = imageScanMethode == ImageScanMethode.Fast ? FindBitmap_Fast( Sertchin, Sertchfor, toleranz ) : FindBitmap_Slow( Sertchin, Sertchfor, toleranz );

            R = false;
            point = new Point( 0, 0 );
            if (res.HasValue) {
                R = ( res.Value.X == 0 && res.Value.Y == 0 ) ? false : true;
                point = new Point( res.Value.X, res.Value.Y );
            }
            TimeSpan = DateTime.Now - k;
        }

        public string SertchImageWithInfo(Bitmap _Sertchin, Bitmap _Sertchfor, ImageScanMethode imageScanMethode, int toleranz = 0, bool jointhread = true) {
            var __r = false;
            TimeSpan __TimeSpan;
            var Point = new Point( 0, 0 );

            return SertchImageWithInfo( _Sertchin, _Sertchfor, out __r, out Point, out __TimeSpan, imageScanMethode, toleranz, jointhread );
        }

        public string SertchImageWithInfo(Bitmap _Sertchfor, ImageScanMethode imageScanMethode, int toleranz = 0, bool jointhread = true) {
            var __r = false;
            TimeSpan __TimeSpan;
            var Point = new Point( 0, 0 );

            return SertchImageWithInfo( CaptureScreen(), _Sertchfor, out __r, out Point, out __TimeSpan, imageScanMethode, toleranz, jointhread );
        }

        public string SertchImageWithInfo(Bitmap _Sertchin, Bitmap _Sertchfor, out bool _R, out Point point, out TimeSpan _TimeSpan, ImageScanMethode imageScanMethode, int toleranz = 0, bool jointhread = true) {
            var __r = false;
            var __TimeSpan = TimeSpan.MinValue;
            var Point = new Point( 0, 0 );

            var t = new Threading.Thread( () => _Thread( _Sertchin, _Sertchfor, out __r, out Point, out __TimeSpan, imageScanMethode, toleranz ) );
            t.Start();
            if (jointhread)
                t.Join();
            _R = __r;
            point = Point;
            _TimeSpan = __TimeSpan;

            return $"Thread returned: {_R},  X={point.X}, Y={point.Y}, TimeSpan(Millisec)={__TimeSpan.Milliseconds}";
        }

        public Point?[] SertchImagesOnScreen(Bitmap[] _Sertchfor, out string result, out TimeSpan _TimeSpan, ImageScanMethode imageScanMethode, int toleranz = 0, bool jointhread = true) {
            var f = 0;
            var a = _Sertchfor.Length;
            var __r = false;
            var __TimeSpan = TimeSpan.MinValue;
            var point = new Point( 0, 0 );
            var _Sertchin = CaptureScreen();

            var Presults = new Point?[a];

            for (var i = 0; i < a; i++) {
                point = Point.Empty;
                Presults[i] = Point.Empty;

                var t = new Thread( () => _Thread( _Sertchin, _Sertchfor[i], out __r, out point, out __TimeSpan, imageScanMethode, toleranz ) );
                t.Start();
                t.Join();
                if (point != Point.Empty && point != new Point( 0, 0 )) {
                    f += 1;
                    Presults[i] = point;
                }
            }

            _TimeSpan = __TimeSpan;

            result = $"Found {f} from {a} images TimeSpan(Millisec)={__TimeSpan.Milliseconds}";

            return Presults;
        }

        public bool SertchImage(Bitmap _Sertchin, Bitmap _Sertchfor, ImageScanMethode imageScanMethode, int toleranz = 0, bool jointhread = true) {
            var __r = false;
            TimeSpan __TimeSpan;
            var Point = new Point( 0, 0 );

            SertchImageWithInfo( _Sertchin, _Sertchfor, out __r, out Point, out __TimeSpan, imageScanMethode, toleranz, jointhread );

            return __r;
        }

        public bool ImageIsOnScreen(Bitmap _Sertchfor, ImageScanMethode imageScanMethode, int toleranz = 0, bool jointhread = true) {
            return SertchImage( CaptureScreen(), _Sertchfor, imageScanMethode, toleranz, jointhread );
        }

        public Bitmap CaptureScreen(int X, int Y, int Width, int Height) {
            var image = new Bitmap( Width, Height, PixelFormat.Format32bppArgb );
            var gfx = Graphics.FromImage( image );
            gfx.CopyFromScreen( X, Y, 0, 0, new Size( Width, Height ), CopyPixelOperation.SourceCopy );
            return image;
        }

        public Point? FindBitmap_Slow(Bitmap Sertchin, Bitmap Sertchfor, int toleranz = 0) {
            Point point;
            for (var outerX = 0; outerX < Sertchin.Width - Sertchfor.Width; outerX++) {
                for (var outerY = 0; outerY < Sertchin.Height - Sertchfor.Height; outerY++) {
                    for (var innerX = 0; innerX < Sertchfor.Width; innerX++) {
                        for (var innerY = 0; innerY < Sertchfor.Height; innerY++) {
                            var searchColor = Sertchfor.GetPixel( innerX, innerY );
                            var withinColor = Sertchin.GetPixel( outerX + innerX, outerY + innerY );

                            if (Math.Abs( searchColor.R - withinColor.R ) > toleranz ||
                                Math.Abs( searchColor.G - withinColor.G ) > toleranz ||
                                Math.Abs( searchColor.B - withinColor.B ) > toleranz ||
                                Math.Abs( searchColor.A - withinColor.A ) > toleranz) {
                                goto NotFound;
                            }
                        }
                    }
                    point = new Point( outerX, outerY );
                    ///*point.X += Sertchfor.Width / 2; // Set X to the middle of the bitmap.
                    ///*point.Y += Sertchfor.Height / 2; // Set Y to the center of the bitmap.
                    return point;

                    NotFound:
                    continue;
                }
            }
            point = Point.Empty;
            return point;
        }
    }
}
