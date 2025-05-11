using System.Diagnostics;
using T2_PR1.HelperClasses;
using T2_PR1.Services;

namespace T2_PR1.Models
{
    internal class Guest
    {
        //Private fields
        private const int _minThinkingTime = 500;
        private const int _maxThinkingTime = 2000;
        private const int _minEatingTime = 500;
        private const int _maxEatingTime = 1000;
        private readonly ConsoleColor[] _guestColors = { ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.Green, ConsoleColor.Red };
        private readonly ChopstickManager _chopstickManager;
        private readonly Stopwatch _hungerStopwatch = new Stopwatch();
        private readonly Stopwatch _totalBlockedStopwatch = new Stopwatch();
        private readonly object _statsLock = new object();
        private bool _running = true;

        //Properties
        internal int Id { get; }
        internal int LeftChopstickId { get; }
        internal int RightChopstickId { get; }
        internal bool IsThinking { get; private set; }
        internal bool IsEating { get; private set; }
        internal bool IsHungry { get; private set; }
        internal int MealCount { get; private set; }
        internal long MaxHungerTime { get; private set; }
        internal long TotalBlockedTime { get; private set; }
        internal ConsoleColor TextColor => _guestColors[Id % _guestColors.Length];

        //Constructor
        internal Guest(int id, int rightChopstick, int leftChopstick, ChopstickManager chopstickManager)
        {
            Id = id;
            LeftChopstickId = leftChopstick;
            RightChopstickId = rightChopstick;
            _chopstickManager = chopstickManager;
            IsThinking = false;
            IsEating = false;
            IsHungry = false;
            MealCount = 0;
            MaxHungerTime = 0;
            TotalBlockedTime = 0;
        }

        //Methods
        internal void Think()
        {
            var thinkingTime = MyMath.NextInt(_minThinkingTime, _maxThinkingTime);

            IsThinking = true;
            PrintColoredMessage("is thinking", ConsoleColor.Blue);
            Thread.Sleep(thinkingTime);
            IsThinking = false;
        }

        internal void Eat()
        {
            var eatingTime = MyMath.NextInt(_minEatingTime, _maxEatingTime);
            
            IsEating = true;
            PrintColoredMessage("is eating", ConsoleColor.Green);
            Thread.Sleep(eatingTime);
            IsEating = false;
            
            lock (_statsLock)
            {
                MealCount++;
                _hungerStopwatch.Reset();
                IsHungry = false;
            }
        }

        internal void Stop()
        {
            _running = false;
        }

        private void PrintColoredMessage(string message, ConsoleColor backgroundColor)
        {
            ConsoleColor originalForeground = Console.ForegroundColor;
            ConsoleColor originalBackground = Console.BackgroundColor;
            
            Console.ForegroundColor = TextColor;
            Console.BackgroundColor = backgroundColor;
            
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.Write($"[{timestamp}] Guest {Id} ");
            Console.WriteLine(message);
            
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;
        }

        internal Thread GenerateThread()
        {
            Thread thread = new Thread(() =>
            {
                _hungerStopwatch.Start();
                
                while (_running)
                {
                    // Think (philosopher is not hungry)
                    Think();
                    
                    // Become hungry and try to get chopsticks
                    IsHungry = true;
                    _hungerStopwatch.Start();
                    _totalBlockedStopwatch.Start();
                    PrintColoredMessage("is hungry and waiting for chopsticks", ConsoleColor.Yellow);
                    
                    bool hasChopsticks = false;
                    while (!hasChopsticks && _running)
                    {
                        // Check hunger time - if over 15 seconds, we have starvation
                        if (_hungerStopwatch.ElapsedMilliseconds > 15000)
                        {
                            PrintColoredMessage("HAS BEEN STARVING FOR TOO LONG (>15s)!", ConsoleColor.Red);
                            return; // Exit the thread
                        }
                        
                        // Try to get chopsticks
                        hasChopsticks = _chopstickManager.TryGetChopsticks(Id, LeftChopstickId, RightChopstickId);
                        
                        if (hasChopsticks)
                        {
                            // Got both chopsticks
                            _totalBlockedStopwatch.Stop();
                            lock (_statsLock)
                            {
                                TotalBlockedTime += _totalBlockedStopwatch.ElapsedMilliseconds;
                                _totalBlockedStopwatch.Reset();
                                
                                long currentHungerTime = _hungerStopwatch.ElapsedMilliseconds;
                                if (currentHungerTime > MaxHungerTime)
                                {
                                    MaxHungerTime = currentHungerTime;
                                }
                            }
                            
                            PrintColoredMessage($"picked up chopsticks {LeftChopstickId} and {RightChopstickId}", ConsoleColor.DarkYellow);
                            
                            // Eat
                            Eat();
                            
                            // Release chopsticks
                            _chopstickManager.ReleaseChopsticks(LeftChopstickId, RightChopstickId);
                            PrintColoredMessage($"released chopsticks {LeftChopstickId} and {RightChopstickId}", ConsoleColor.DarkCyan);
                        }
                        else
                        {
                            // Couldn't get chopsticks, wait a bit and try again
                            Thread.Sleep(100);
                        }
                    }
                }
            });

            thread.Name = $"Guest_{Id}";
            return thread;
        }
    }
}
