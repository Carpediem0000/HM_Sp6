using System;
using System.Collections.Generic;
using System.Threading;

namespace HM_Sp6
{
    class Horse
    {
        private static object _lock = new object();

        private static Random _rnd = new Random();
        private static int _YPositionCursore = -1;
        private static bool _finish = false;
        private static int _numFinish = 0;

        private ManualResetEvent _resetEvent;


        private const int _XPositionCursore = 5;

        public int position;
        public int HoursFinish;
        public int HoursNumber;

        public Horse(ManualResetEvent manual)
        {
            lock (_lock)
            {
                _YPositionCursore++;
                HoursNumber = _YPositionCursore;
            }
            _resetEvent = manual;
            position = 0;
        }

        public void Race(object obj)
        {
            lock (_lock)
            {
                Console.SetCursorPosition(0, HoursNumber);
                Console.Write($"#{HoursNumber + 1}.");
            }

            while (position <= 20)
            {
                lock (_lock)
                {
                    if (position == 20)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        _numFinish++;
                        HoursFinish = _numFinish;
                    }
                    Console.SetCursorPosition(_XPositionCursore, HoursNumber);
                    Console.Write("|");
                    for (int i = 0; i < position; i++)
                    {
                        Console.Write("-");
                    }
                    if (position == 20)
                    {
                        lock (_lock)
                        {
                            Console.SetCursorPosition(_XPositionCursore + position, HoursNumber);
                            Console.WriteLine($"| Finish #{HoursFinish}!");
                        }
                    }
                }
                Thread.Sleep(_rnd.Next(1000, 2000));
                position++;
            }

            if (!_finish)
            {
                Console.SetCursorPosition(0, _YPositionCursore + 1);
                Console.WriteLine($"Horse #{HoursNumber + 1} WIN");
            }
            _finish = true;

            _resetEvent.Set();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            const int ThreadCount = 10;

            Random rnd = new Random();

            ManualResetEvent[] manualResets = new ManualResetEvent[ThreadCount];
            List<Horse> horses = new List<Horse>();

            for (int i = 0; i < ThreadCount; i++)
            {
                manualResets[i] = new ManualResetEvent(false);
                horses.Add(new Horse(manualResets[i]));
                ThreadPool.QueueUserWorkItem(horses[i].Race);
            }

            Console.CursorVisible = false;
            WaitHandle.WaitAll(manualResets);
            Console.CursorVisible = true;

            Console.ReadKey();
        }
    }
}
