
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tests {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine( Console.OutputEncoding );
            //Console.OutputEncoding = Encoding.GetEncoding( 1252 );
            Console.WriteLine( Console.OpenStandardError() );
            Console.WriteLine(
@"  	0 	1 	2 	3 	4 	5 	6 	7 	8 	9 	A 	B 	C 	D 	E 	F
U+250x 	─ 	━ 	│ 	┃ 	┄ 	┅ 	┆ 	┇ 	┈ 	┉ 	┊ 	┋ 	┌ 	┍ 	┎ 	┏
U+251x 	┐ 	┑ 	┒ 	┓ 	└ 	┕ 	┖ 	┗ 	┘ 	┙ 	┚ 	┛ 	├ 	┝ 	┞ 	┟
U+252x 	┠ 	┡ 	┢ 	┣ 	┤ 	┥ 	┦ 	┧ 	┨ 	┩ 	┪ 	┫ 	┬ 	┭ 	┮ 	┯
U+253x 	┰ 	┱ 	┲ 	┳ 	┴ 	┵ 	┶ 	┷ 	┸ 	┹ 	┺ 	┻ 	┼ 	┽ 	┾ 	┿
U+254x 	╀ 	╁ 	╂ 	╃ 	╄ 	╅ 	╆ 	╇ 	╈ 	╉ 	╊ 	╋ 	╌ 	╍ 	╎ 	╏
U+255x 	═ 	║ 	╒ 	╓ 	╔ 	╕ 	╖ 	╗ 	╘ 	╙ 	╚ 	╛ 	╜ 	╝ 	╞ 	╟
U+256x 	╠ 	╡ 	╢ 	╣ 	╤ 	╥ 	╦ 	╧ 	╨ 	╩ 	╪ 	╫ 	╬ 	╭ 	╮ 	╯
U+257x 	╰ 	╱ 	╲ 	╳ 	╴ 	╵ 	╶ 	╷ 	╸ 	╹ 	╺ 	╻ 	╼ 	╽ 	╾ 	╿ " );
            /*Console.WriteLine( @"
            ┌─┬┐  ╔═╦╗  ╓─╥╖  ╒═╤╕
            │ ││  ║ ║║  ║ ║║  │ ││
            ├─┼┤  ╠═╬╣  ╟─╫╢  ╞═╪╡
            └─┴┘  ╚═╩╝  ╙─╨╜  ╘═╧╛
            ┌───────────────────┐
            │  ╔═══╗ Some Text  │▒
            │  ╚═╦═╝ in the box │▒
            ╞═╤══╩══╤═══════════╡▒
            │ ├──┬──┤           │▒
            │ └──┴──┘           │▒
            └───────────────────┘▒
             ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
" );*/
            Console.ReadLine();
            //testtask t = new testtask( "Test",ConsoleColor.Yellow );
            //t.Run();
            //Console.WriteLine( "E" );
            //Console.ReadLine();
        }
    }

    public class testtask : Task {
        public testtask(string Name, ConsoleColor NormalColor = ConsoleColor.White) : base( Name, NormalColor ) {
            Console.WriteLine( "Taskcreatet!" );
        }
        public override void Run() {
            base.Run();
            while (true) {
                _State = State.critical;
                Thread.Sleep( 1000 );

                _State = State.error;
                Thread.Sleep( 2000 );

                _State = State.fail;
                Thread.Sleep( 2000 );

                _State = State.None;
                Thread.Sleep( 2000 );

                _State = State.ok;
                Thread.Sleep( 2000 );

                _State = State.running;
                Thread.Sleep( 4000 );

                _State = State.warning;
                Thread.Sleep( 2000 );
            }
        }
    }


    public class Task {
        Guid _Guid = Guid.NewGuid();
        public string _Name { get; private set; }
        public enum State {
            None,
            running,
            ok,
            fail,
            error,
            warning,
            critical
        }
        public State _State = State.None;
        public ConsoleColor _NormalClolor = ConsoleColor.White;
        int id = 0;
        public bool IsRunning = false;
        public Task(string Name, ConsoleColor NormalColor = ConsoleColor.White) {
            _Name = Name;
            _NormalClolor = NormalColor;
            var t = new Thread( () => {
                while (true) {
                    Thread.Sleep( 200 );
                    if (IsRunning)
                        Writestate();
                }
            } );
            t.Start();
        }
        public virtual void Run() {
            Console.ForegroundColor = _NormalClolor;
            Console.Write( "[       ]: " + _Name );
            IsRunning = true;

        }
        public void Writestate() {
            Console.SetCursorPosition( 1, Console.CursorTop );

            var Msg = "";

            switch (_State) {
                case State.None:
                    break;
                case State.running:
                    for (var i = 0; i < 7; i++) {
                        Msg += i == id ? "*" : " ";
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    id++;
                    if (id >= 7)
                        id = 0;
                    break;
                case State.ok:
                    Msg = ( "  OK!  " );
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case State.fail:
                    Msg = ( " faile " );
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case State.error:
                    Msg = ( " Error " );
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case State.warning:
                    Msg = ( "Warning" );
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case State.critical:
                    Console.Beep( 2048, 100 );
                    Msg = ( "*Error*" );
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    break;
            }
            Console.Write( Msg );
            Console.ForegroundColor = _NormalClolor;
        }
        public bool Equals(Task t1, Task t2) {
            return t1._Guid == t2._Guid;
        }
    }
}
