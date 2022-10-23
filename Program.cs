using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadWar
{
    class Program
    {
        private const string badChar = "-\\|/";
        private static int hit = 0;
        private static int miss = 0;
        private static bool endGame = false;
        private static Random random = new Random();
        private static ManualResetEvent resetEvent = new ManualResetEvent(false);
        private static Semaphore semaphore = new Semaphore(3, 3);
        static void BadGuys()
        {
            resetEvent.WaitOne(5000);
            while (!endGame)
            {
                //условие для вероятности создания врага
                int chance = hit + miss + 20;

                int res = random.Next(0, 100);
                if (res < chance)
                {
                    Thread thread = new Thread(BadGuy);
                    thread.Start(random.Next(1, 20));
                }
                Thread.Sleep(1000);
            }
        }
        
        static void BadGuy(object randY)
        {
            int y = (int)randY;
            int x = (y % 2 == 0) ? 1 : Console.WindowWidth - 1;
            int direction = (y % 2 == 0) ? 1 : -1;

            //пока не ушел с экрана
            while (x != 0 && x != Console.WindowWidth)
            {
                ConsoleBuffer.WriteAt(x, y, badChar[x % 4].ToString());
                if (endGame)
                    return;

                bool hitme = false;
                for (int i = 0; i < 6; i++)
                {
                    hitme = ConsoleBuffer.GetAt(x, y) == '*';
                    if (hitme)
                        break;
                    Thread.Sleep(40);
                }
                if(hitme)
                {
                    ConsoleBuffer.WriteAt(x, y, " ");
                    Console.Beep();
                    Interlocked.Increment(ref hit);
                    UpdateScore();
                    return;
                }

                ConsoleBuffer.WriteAt(x, y, " ");
                x += direction;
            }
            Interlocked.Increment(ref miss);
            UpdateScore();

        }
        static void Bullet(object coord)
        {
            if (!semaphore.WaitOne(1))
            {
                return;
            }
            var xy = (Win32Manager.COORD)coord;
            while (xy.Y > 0)
            {
                ConsoleBuffer.WriteAt(xy.X, xy.Y, "*");
                Thread.Sleep(100);
                ConsoleBuffer.WriteAt(xy.X, xy.Y, " ");
                xy.Y--;
            }
            semaphore.Release();
        }

        static void UpdateScore()
        {
            Console.Title = $"Война потоков. Колличество попаданий: {hit}, колличество промахов: {miss}.";
            if (miss == 5)
            {
                endGame = true;
                ConsoleBuffer.WriteAt(Console.WindowWidth / 2, Console.WindowHeight / 2, "Game over");
            }
        }

        static void Main(string[] args)
        {
            
            Console.CursorVisible = false;

            int gunX = Console.WindowWidth / 2;
            int gunY = Console.WindowHeight - 1;

            ConsoleBuffer.WriteAt(gunX, gunY, "|");

            Thread badThread = new Thread(BadGuys) { IsBackground = true };
            badThread.Start();

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    resetEvent.Set();
                    if (gunX > 0)
                    {
                        ConsoleBuffer.WriteAt(gunX, gunY, " ");
                        gunX--;
                        ConsoleBuffer.WriteAt(gunX, gunY, "|");
                    }
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    resetEvent.Set();
                    if (gunX < Console.WindowWidth - 1)
                    {
                        ConsoleBuffer.WriteAt(gunX, gunY, " ");
                        gunX++;
                        ConsoleBuffer.WriteAt(gunX, gunY, "|");
                    }
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    Thread bulletThread = new Thread(Bullet);
                    Win32Manager.COORD cOORD = new Win32Manager.COORD();
                    cOORD.X = (short)gunX;
                    cOORD.Y = (short)(gunY - 1);
                    bulletThread.Start(cOORD);
                    Thread.Sleep(100);
                }
            }
            


            Console.ReadLine();
        }
    }
}
