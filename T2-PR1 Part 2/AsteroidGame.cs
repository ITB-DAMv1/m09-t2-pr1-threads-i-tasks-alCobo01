using System.Reflection.PortableExecutable;

namespace T2_PR1_Part_2
{
    internal class AsteroidGame
    {
        //Constants
        private static readonly int renderFrequency = 20;
        private static readonly int logicFrequency = 50;
        private static readonly Random _rnd = new Random();
        private static readonly int width = Console.WindowWidth;
        private static readonly int height = Console.WindowHeight;

        private static int playerPosition = width / 2;
        private static List<(int x, int y)> asteroids = new List<(int x, int y)>();

        //Stats
        private int asteroidTickCount = 0;
        private static int numCollisions = 1;

        //Properties
        private CancellationTokenSource CancelTokenSrc { get; set; } 
        private CancellationToken CancellationToken { get; set; }

        //Constructor
        internal AsteroidGame(CancellationTokenSource cancelTokenSrc)
        {
            CancelTokenSrc = cancelTokenSrc;
            CancellationToken = CancelTokenSrc.Token;
        }

        //Methods
        internal async Task Run()
        {
            Task renderTask = Task.Run(Render);

            Task logicalTask = Task.Run(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    lock (asteroids)
                    {
                        asteroids.RemoveAll(a =>
                            a.y == height - 1 &&
                            a.x != playerPosition
                        );

                        if (CheckCollision())
                        {
                            Console.Beep();
                            numCollisions++;
                            playerPosition = width / 2;
                            asteroids.Clear();
                        }
                        else
                        {
                            asteroidTickCount++;
                            if (asteroidTickCount >= 10)
                            {
                                asteroidTickCount = 0;
                                MoveAsteroids();
                            }
                        }
                    }

                    await Task.Delay(logicFrequency);
                }
            });

            Task movePlayer = Task.Run(MovePlayer);

            Task spawnAsteroids = Task.Run(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    SpawnAsteroids();
                    await Task.Delay(500);
                }
            });

            await Task.WhenAny(renderTask, spawnAsteroids, logicalTask, movePlayer);

        }

        private async Task Render()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                Console.Clear();
                Console.SetCursorPosition(playerPosition, height - 1);
                Console.Write("^");

                lock (asteroids)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (var asteroid in asteroids)
                    {
                        Console.SetCursorPosition(asteroid.x, asteroid.y);
                        Console.Write("*");
                    }
                }

                await Task.Delay(renderFrequency);
            }
        }

        private async Task MovePlayer()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q) CancelTokenSrc.Cancel();
                if (key == ConsoleKey.A && playerPosition > 0) playerPosition--;
                else if (key == ConsoleKey.D && playerPosition < width - 1) playerPosition++;
            }
            
            await Task.Delay(logicFrequency);
        }

        private static void SpawnAsteroids()
        {
            int x = _rnd.Next(0, width);
            lock (asteroids)
            {
                asteroids.Add((x, 0));
            }
        }

        private static void MoveAsteroids()
        {
            lock (asteroids)
            {
                for (int i = 0; i < asteroids.Count; i++)
                {
                    asteroids = asteroids
                        .Select(a => (a.x, y: a.y + 1))
                        .Where(a => a.y < height)
                        .ToList();
                }
            }
        }

        private bool CheckCollision()
        {
            lock (asteroids)
            {
                return asteroids.Any(a =>
                    a.y == height - 1 &&
                    a.x == playerPosition);
            }
        }
    }
}
