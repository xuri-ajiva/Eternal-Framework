using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eternal.Visualisation {
   public class TaskInfo : EternalFramework.Eternal {
        Guid _Guid = Guid.NewGuid();
        public string _Name { get; set; }
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
        private Action _run;

        public TaskInfo(string Name, Action run, ConsoleColor NormalColor = ConsoleColor.White) : base() {
            _Name = Name;
            _NormalClolor = NormalColor;
            _run = run;
            _Thread = new Thread( () => {
                while (true) {
                    Thread.Sleep( 150 );
                    if (IsRunning)
                        Writestate();
                }
            } );
            _Thread.Start();
        }
        private int line = 0;
        public virtual void Run(bool startgivenvoid = true) {
            Console.ForegroundColor = _NormalClolor;
            line = Console.CursorTop;
            Console.Write( "[       ]: " + _Name );
            IsRunning = true;

            if (startgivenvoid) {
                _run();
                IsRunning = false;
                Writestate();
            }
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

            string Msg = "";

            Console.SetCursorPosition( 0, line );
            Console.Write( "[       ]: " + _Name );
            Console.SetCursorPosition( 1, line );

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
        public bool Equals(TaskInfo t1, TaskInfo t2) {
            return t1._Guid == t2._Guid;
        }

    }
}
