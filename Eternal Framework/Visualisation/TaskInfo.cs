using System;
using System.Threading;

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
        private int _id;
        public bool IsRunning;

        private Thread _thread;
        private readonly Action _run;

        public TaskInfo(string Name, Action run, ConsoleColor NormalColor = ConsoleColor.White)
        {
            _Name = Name;
            _NormalClolor = NormalColor;
            _run = run;
            _thread = new Thread( () => {
                while (true) {
                    Thread.Sleep( 150 );
                    if (IsRunning)
                        Writestate();
                }
            } );
            _thread.Start();
        }
        private int _line;
        public virtual void Run(bool startgivenvoid = true) {
            Console.ForegroundColor = _NormalClolor;
            _line = Console.CursorTop;
            Console.Write( "[       ]: " + _Name );
            IsRunning = true;

            if (!startgivenvoid) return;
            _run();
            IsRunning = false;
            Writestate();
        }
        public virtual void Abort() {
            _thread.Abort();
            IsRunning = false;
            _Guid = Guid.Empty;
            _Name = string.Empty;
            _thread = null;
            _State = State.None;
        }
        public void Writestate() {

            var msg = "";

            Console.SetCursorPosition( 0, _line );
            Console.Write( "[       ]: " + _Name );
            Console.SetCursorPosition( 1, _line );

            switch (_State) {
                case State.None:
                    break;
                case State.running:
                    for (var i = 0; i < 7; i++) {
                        msg += i == _id ? "*" : " ";
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    _id++;
                    if (_id >= 7)
                        _id = 0;
                    break;
                case State.ok:
                    msg = ( "  OK!  " );
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case State.fail:
                    msg = ( " fai" + "le " );
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case State.error:
                    msg = ( " Error " );
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case State.warning:
                    msg = ( "Warning" );
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case State.critical:
                    Console.Beep( 2048, 100 );
                    msg = ( "*Error*" );
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }

            Console.Write( msg );
            Console.ForegroundColor = _NormalClolor;
        }
        public bool Equals(TaskInfo t1, TaskInfo t2) {
            return t1._Guid == t2._Guid;
        }

    }
}
