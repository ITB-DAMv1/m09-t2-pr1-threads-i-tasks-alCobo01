using System.Diagnostics;
using T2_PR1.Models;
using T2_PR1.Services;

namespace T2_PR1
{
    public class Program
    {
        private const int NumGuests = 5;
        private const int SimulationTime = 30000;
        
        public static void Main()
        {
            Console.WriteLine("=== DINING PHILOSOPHERS PROBLEM ===\n");
            Console.WriteLine("Each philosopher must think, get chopsticks, eat, and release chopsticks.");
            Console.WriteLine("The solution prevents deadlocks and starvation (no philosopher waits >15s).");
            Console.WriteLine($"Simulation will run for {SimulationTime/1000} seconds.\n");
            
            // Initialize chopstick manager
            ChopstickManager chopstickManager = new ChopstickManager();
            
            // Initialize guests (philosophers)
            Guest[] guests = new Guest[NumGuests];
            Thread[] threads = new Thread[NumGuests];
            
            for (int i = 0; i < NumGuests; i++)
            {
                // Each guest has chopsticks to their left and right
                // For guest i, right chopstick is i, left chopstick is (i+1) % NumGuests
                int rightChopstick = i;
                int leftChopstick = (i + 1) % NumGuests;
                
                guests[i] = new Guest(i, rightChopstick, leftChopstick, chopstickManager);
                threads[i] = guests[i].GenerateThread();
            }
            
            // Start the simulation
            Stopwatch simulationTimer = new Stopwatch();
            simulationTimer.Start();
            
            // Start all threads
            foreach (var thread in threads)
            {
                thread.Start();
            }
            
            // Wait for simulation time or until a philosopher starves
            bool simulationCompleted = true;
            while (simulationTimer.ElapsedMilliseconds < SimulationTime)
            {
                // Check if any thread has terminated (due to starvation)
                if (threads.Any(t => !t.IsAlive))
                {
                    simulationCompleted = false;
                    break;
                }
                
                Thread.Sleep(100); // Check every 100ms if any guest has died
            }
            
            // Stop all guests
            foreach (var guest in guests)
            {
                guest.Stop();
            }
            
            // Wait for threads to finish
            foreach (var thread in threads)
            {
                if (thread.IsAlive)
                {
                    thread.Join(1000); // Wait up to 1 second for each thread
                }
            }
            
            // Display result
            if (simulationCompleted)
            {
                Console.WriteLine("\n✅ Simulation completed successfully without deadlocks or starvation!");
            }
            else
            {
                Console.WriteLine("\n❌ Simulation ended early due to starvation (a philosopher waited >15s).");
            }
            
            // Save and display statistics
            StatsManager statsManager = new StatsManager();
            statsManager.PrintStats(guests);
            statsManager.SaveStats(guests);
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
