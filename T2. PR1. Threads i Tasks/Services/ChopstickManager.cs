using T2_PR1.Models;

namespace T2_PR1.Services
{
    internal class ChopstickManager
    {
        //Properties
        internal Chopstick[] Chopsticks { get; } = new Chopstick[5];

        //Constructor
        internal ChopstickManager()
        {
            for (int i = 0; i < Chopsticks.Length; i++)
            {
                Chopsticks[i] = new Chopstick(i);
            }
        }

        //Methods
        internal bool TryGetChopsticks(int guestId, int leftChopstickId, int rightChopstickId)
        {
            // To prevent deadlock, we have to acquire the minId chopstick first
            int firstId = Math.Min(leftChopstickId, rightChopstickId);
            int secondId = Math.Max(leftChopstickId, rightChopstickId);
            
            Chopstick firstChopstick = Chopsticks[firstId];
            Chopstick secondChopstick = Chopsticks[secondId];
            
            bool acquired = false;
            
            // Try to acquire the first chopstick
            if (Monitor.TryEnter(firstChopstick.Lock, 0))
            {
                try
                {
                    // Try to acquire the second chopstick
                    if (Monitor.TryEnter(secondChopstick.Lock, 0))
                    {
                        try
                        {
                            // Successfully acquired both chopsticks
                            firstChopstick.IsAvailable = false;
                            secondChopstick.IsAvailable = false;
                            acquired = true;
                        }
                        catch
                        {
                            Monitor.Exit(secondChopstick.Lock);
                        }
                    }
                }
                finally
                {
                    // If we couldn't acquire the second chopstick, release the first
                    if (!acquired)
                    {
                        Monitor.Exit(firstChopstick.Lock);
                    }
                }
            }
            
            return acquired;
        }
        
        internal void ReleaseChopsticks(int leftChopstickId, int rightChopstickId)
        {
            // Release in the same order as acquisition to maintain consistency
            int firstId = Math.Min(leftChopstickId, rightChopstickId);
            int secondId = Math.Max(leftChopstickId, rightChopstickId);
            
            Chopstick firstChopstick = Chopsticks[firstId];
            Chopstick secondChopstick = Chopsticks[secondId];
            
            // Mark as available again
            firstChopstick.IsAvailable = true;
            secondChopstick.IsAvailable = true;
            
            // Release locks
            Monitor.Exit(secondChopstick.Lock);
            Monitor.Exit(firstChopstick.Lock);
        }
    }
}
