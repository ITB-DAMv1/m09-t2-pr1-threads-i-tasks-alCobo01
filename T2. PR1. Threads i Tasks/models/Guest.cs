﻿using System.Diagnostics;
using T2_PR1.HelperClasses;
using T2_PR1.Services;

namespace T2_PR1.Models
{
    internal class Guest
    {
        //Private fields
        private static readonly int _minThinkingTime = 500;
        private static readonly int _maxThinkingTime = 2000;
        private static readonly int _minEatingTime = 500;
        private static readonly int _maxEatingTime = 1000;
        private static readonly ConsoleColor[] _guestColors = { ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Red };
        
        private static readonly Stopwatch _hungerStopwatch = new Stopwatch();
        private static readonly Stopwatch _totalBlockedStopwatch = new Stopwatch();
        
        private static readonly object _consoleLock = new object();
        private static readonly object _statsLock = new object();


        private readonly ChopstickManager _chopstickManager;
        

        //Properties
        internal int Id { get; }
        internal int LeftChopstickId { get; }
        internal int RightChopstickId { get; }
        internal bool IsThinking { get; private set; }
        internal bool IsEating { get; private set; }
        internal bool IsHungry { get; private set; }
        private bool _isRunning { get; set; } = true;
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
            PrintColoredMessage("is thinking", ConsoleColor.DarkGreen);
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
            _isRunning = false;
        }

        private void PrintColoredMessage(string message, ConsoleColor backgroundColor)
        {
            // Use lock to ensure that only one thread can write to the console at a time
            // To prevent different colors at the same time
            lock (_consoleLock)
            {
                ConsoleColor originalForeground = Console.ForegroundColor;
                ConsoleColor originalBackground = Console.BackgroundColor;
                
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = TextColor;
                
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                Console.WriteLine($"[{timestamp}] Guest {Id} - {message}");
                
                // Ensure we restore the original colors
                Console.ForegroundColor = originalForeground;
                Console.BackgroundColor = originalBackground;
            }
        }

        internal Thread GenerateThread()
        {
            Thread thread = new Thread(() =>
            {
                _hungerStopwatch.Start();
                
                while (_isRunning)
                {
                    // Think (philosopher is not hungry)
                    Think();
                    
                    // Become hungry and try to get chopsticks
                    IsHungry = true;
                    _hungerStopwatch.Start();
                    _totalBlockedStopwatch.Start();
                    PrintColoredMessage("is hungry and waiting for chopsticks", ConsoleColor.DarkBlue);
                    
                    bool hasChopsticks = false;
                    while (!hasChopsticks && _isRunning)
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
                            
                            PrintColoredMessage($"Picked up chopsticks {LeftChopstickId} and {RightChopstickId}", ConsoleColor.DarkGray);
                            
                            Eat();
                            
                            // Release chopsticks
                            _chopstickManager.ReleaseChopsticks(LeftChopstickId, RightChopstickId);
                            PrintColoredMessage($"released chopsticks {LeftChopstickId} and {RightChopstickId}", ConsoleColor.DarkMagenta);
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
