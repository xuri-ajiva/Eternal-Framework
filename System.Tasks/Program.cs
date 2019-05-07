using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Tasks {

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

        private Thread _Thread;

        public Task(string Name, ConsoleColor NormalColor = ConsoleColor.White) {
            _Name = Name;
            _NormalClolor = NormalColor;
            _Thread = new Thread( () => {
                while (true) {
                    Thread.Sleep( 200 );
                    if (IsRunning)
                        Writestate();
                }
            } );
            _Thread.Start();
        }
        public virtual void Run(Action Void) {
            Console.ForegroundColor = _NormalClolor;
            Console.Write( "[       ]: " + _Name );
            IsRunning = true;
            Void();
        }
        public virtual void Abort() {
            _Thread.Abort();
            IsRunning = false;
            _Guid = Guid.Empty;
            _Name = string.Empty;
            _Thread = null;
            _State = State.None;
        }
        public void Writestate() {
            Console.SetCursorPosition( 1, Console.CursorTop );

            string Msg = "";

            switch (_State) {
                case State.None:
                    break;
                case State.running:
                    for (int i = 0; i < 7; i++) {
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
