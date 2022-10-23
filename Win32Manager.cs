using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreadWar
{
    public static class Win32Manager
    {
        [DllImport("Kernel32", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("Kernel32", SetLastError = true)]
        public static extern bool ReadConsoleOutputCharacter(IntPtr hConsoleOutput,
            [Out] StringBuilder lpCharacter, uint nLength, COORD dwReadCoord,
            out uint lpNumberOfCharsRead);

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        [DllImport("Kernel32", SetLastError = true)]
        public static extern bool WriteConsoleOutputCharacter(IntPtr hConsoleOutput,
           string lpCharacter, int nLength, COORD dwWriteCoord,
           out uint lpNumberOfCharsWritten);
    }
}
