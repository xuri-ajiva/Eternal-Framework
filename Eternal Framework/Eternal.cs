//#undef DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalFramework {
    public class Eternal {
        private Guid _Guid = Guid.NewGuid();
        private static bool Used = false;
        public const string SoftwareName = "Eternal";
        public Eternal() {
#if DEBUG
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (!Used) {
                Console.WriteLine();
                Console.WriteLine( $"   ┌────────────────────────────────────────╖" );
                Console.WriteLine( $"   │ You are using Eternal in Used version! ║" );
                Console.WriteLine( $"   ╘════════════════════════════════════════╝" );
                Console.WriteLine();

            }
            Console.ForegroundColor = c;
            Used = true;
#else
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (!Used) {
                Console.WriteLine();
                Console.WriteLine( $"   ┌────────────────────────────────────────────────╖" );
                Console.WriteLine( $"   │ You are using Eternal Framework version 1.0.0! ║" );
                Console.WriteLine( $"   ╘════════════════════════════════════════════════╝" );
                Console.WriteLine();
            }
            Console.ForegroundColor = c;
            Used = true;
#endif
        }
        public string GetID => _Guid.ToString();
    }
}
