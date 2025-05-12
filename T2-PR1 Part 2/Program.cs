using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace T2_PR1_Part_2
{
    public class Program
    {
        /// Constants for the console window size
        private static readonly int _width = 200;
        private static readonly int _height = 100;
        private static CancellationTokenSource cts = new CancellationTokenSource();

        public static async Task Main()
        {
            Console.SetWindowSize(_width, _height);
            Console.SetBufferSize(_width, _height);
            Console.CursorVisible = false;

            AsteroidGame game = new AsteroidGame(cts);
            await game.Run();

        }
    }
}
