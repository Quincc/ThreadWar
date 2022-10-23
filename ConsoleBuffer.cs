using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadWar
{
    public static class ConsoleBuffer
    {
        private static IntPtr consoleHandle;
        private static object obj = new object();

        static ConsoleBuffer()
        {
            consoleHandle = Win32Manager.GetStdHandle(-11);
        }

        public static void WriteAt(int x, int y, string value)
        {
            Win32Manager.COORD coord = new Win32Manager.COORD()
            {
                X = (short)x,
                Y = (short)y
            };

            lock (obj)
            {
                Win32Manager.WriteConsoleOutputCharacter(consoleHandle, value, value.Length, coord, out _);
            }
        }

        public static char GetAt(int x, int y)
        {
            Win32Manager.COORD coord = new Win32Manager.COORD()
            {
                X = (short)x,
                Y = (short)y
            };

            lock (obj)
            {
                StringBuilder sb = new StringBuilder(1);
                Win32Manager.ReadConsoleOutputCharacter(consoleHandle, sb, 1, coord, out _);
                return sb[0];
            }
        }
    }
}
