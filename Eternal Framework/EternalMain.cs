//#undef DEBUG

using System;
using System.Drawing;
using System.Text;
using System.Threading;

namespace EternalFramework {
    public class EternalMain {
        private readonly Guid _guid = Guid.NewGuid();
        private static bool _used;
        private static readonly string EndodetSplash = Eternal.privatevar.privatestaff.buildbase64();



        public const string SoftwareName = "EternalMain";
        public EternalMain() {
#if DEBUG
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (!_used) {
                Console.WriteLine();
                Console.WriteLine( "   ┌─────────────────────────────────────────────╖" );
                Console.WriteLine( "   │ You are using EternalMain in DEBUG version! ║" );
                Console.WriteLine( "   ╘═════════════════════════════════════════════╝" );
                Console.WriteLine();

            }
            Console.ForegroundColor = c;
            _used = true;
#else
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (!_used) {
                Console.WriteLine();
                Console.WriteLine( $"   ┌────────────────────────────────────────────────────╖" );
                Console.WriteLine( $"   │ You are using EternalMain Framework version 1.0.0! ║" );
                Console.WriteLine( $"   ╘════════════════════════════════════════════════════╝" );
                Console.WriteLine();
            }
            Console.ForegroundColor = c;
            _used = true;
#endif
        }

        public EternalMain(int id) {
            WriteSplash();

            Thread t = new Thread( () => { while (true) try { Console.SetBufferSize( Console.WindowWidth, Console.WindowHeight ); } catch {/* ignored*/} } );
            //t.Start();
        }

        private static void WriteSplash() {
            var cs = new Size( Console.WindowWidth, Console.WindowHeight );

            const int width = 106;
            const int height = 38;

            var c = Console.ForegroundColor;

            Console.SetWindowSize( width, height );
            Console.SetBufferSize( width, height );
            Console.SetCursorPosition( 0, 2 );

            var spl = Encoding.UTF8.GetString( Convert.FromBase64String( EndodetSplash ) );


            foreach (var cu in spl) {
                switch (cu) {
                    case '`':
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case '+':
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case '#':
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case '@':
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case '.':
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case ',':
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case ';':
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case ':':
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.Write( cu );
            }


            Thread.Sleep( 2000 );
            Console.Clear();
            Console.SetWindowSize( cs.Width, cs.Height );
            Console.SetBufferSize( cs.Width, cs.Height );

            Console.ForegroundColor = c;
        }

        public string GetId => _guid.ToString();
    }
}
