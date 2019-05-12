using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace testbuilds.TestUtils {
    class Utils {
        internal static void SetupChoise(ChoisObjekts[] choises) {
            for (var i = 0; i < choises.Length; i++) {
                Console.WriteLine( $"[{i}]: {choises[i]._Name}" );
            }
            var v = 0;
            while (!int.TryParse( Console.ReadLine(), out v )) {
                Console.Write( "Bitte Wählen[0 - " + choises.Length + "]:" );
            }
            choises[v].Avtivete();
        }
    }
    public class ChoisObjekts {
        public Guid guid { get; private set; }
        public string _Name { get; private set; }
        public Action _YourVoid { get; private set; }

        public ChoisObjekts(string Name, Action YourVoid) {
            guid = Guid.NewGuid();
            _Name = Name;
            _YourVoid = YourVoid;
        }

        public void Avtivete() {
            Console.WriteLine( "Choised: " + _Name );
            _YourVoid();
        }

        public bool Equals(ChoisObjekts c1, ChoisObjekts c2) {
            if (c1.guid == c2.guid) {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// A utility class to determine a process parent.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    public struct ParentProcessUtilities {
        // These members must match PROCESS_BASIC_INFORMATION
        public IntPtr Reserved1;
        public IntPtr PebBaseAddress;
        public IntPtr Reserved2_0;
        public IntPtr Reserved2_1;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;

        [DllImport( "ntdll.dll" )]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
            ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

        /// <summary>
        /// Gets the parent process of the current process.
        /// </summary>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess() {
            return GetParentProcess( Process.GetCurrentProcess().Handle );
        }

        /// <summary>
        /// Gets the parent process of specified process.
        /// </summary>
        /// <param name="id">The process id.</param>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess(int id) {
            Process process = Process.GetProcessById( id );
            return GetParentProcess( process.Handle );
        }

        /// <summary>
        /// Gets the parent process of a specified process.
        /// </summary>
        /// <param name="handle">The process handle.</param>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess(IntPtr handle) {
            ParentProcessUtilities pbi = new ParentProcessUtilities();
            int returnLength;
            int status = NtQueryInformationProcess( handle, 0, ref pbi, Marshal.SizeOf( pbi ), out returnLength );
            if (status != 0)
                throw new Win32Exception( status );

            try {
                return Process.GetProcessById( pbi.InheritedFromUniqueProcessId.ToInt32() );
            }
            catch (ArgumentException) {
                // not found
                return null;
            }
        }
    }
}
