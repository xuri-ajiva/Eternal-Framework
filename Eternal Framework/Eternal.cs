//#undef DEBUG

using System;

namespace EternalFramework {
    public class Eternal {
        private readonly Guid _guid = Guid.NewGuid();
        private static bool _used;
        public const string SoftwareName = "Eternal";
        public Eternal() {
#if DEBUG
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (!_used) {
                Console.WriteLine();
                Console.WriteLine( "   ┌────────────────────────────────────────╖" );
                Console.WriteLine( "   │ You are using Eternal in Used version! ║" );
                Console.WriteLine( "   ╘════════════════════════════════════════╝" );
                Console.WriteLine();

            }
            Console.ForegroundColor = c;
            _used = true;
#else
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (!_used) {
                Console.WriteLine();
                Console.WriteLine( $"   ┌────────────────────────────────────────────────╖" );
                Console.WriteLine( $"   │ You are using Eternal Framework version 1.0.0! ║" );
                Console.WriteLine( $"   ╘════════════════════════════════════════════════╝" );
                Console.WriteLine();
            }
            Console.ForegroundColor = c;
            _used = true;
#endif
        }
        public string GetId => _guid.ToString();
    }
}
