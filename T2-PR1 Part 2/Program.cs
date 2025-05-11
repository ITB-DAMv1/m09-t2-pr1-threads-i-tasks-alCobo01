using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace T2_PR1_Part_2
{
    public class Program
    {
        private static readonly int width = 200;
        private static readonly int height = 100;

        private static int playerPosition = width / 2;
        private static List<(int x, int y)> asteroids = new List<(int x, int y)>();

        private static CancellationTokenSource cts = new CancellationTokenSource();

        public static async Task Main()
        {
            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);
            Console.CursorVisible = false;

            var cancellationToken = cts.Token;

        }
       
        private async static Task MovePlayer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q) token.Canc
                else if (key == ConsoleKey.RightArrow && playerPosition < width - 1)
                {
                    playerPosition++;
                }
            }
            Console.Clear();
            Console.SetCursorPosition(playerPosition, height - 1);
            Console.Write("A");
            await Task.Delay(100, token);
            
        }

    }
}
