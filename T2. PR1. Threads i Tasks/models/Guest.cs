using T2_PR1.helperClasses;

namespace T2_PR1.models
{
    internal class Guest
    {
        //Private fields
        private const int _minThinkingTime = 500;
        private const int _maxThinkingTime = 1500;
        private const int _minEatingTime = 500;
        private const int _maxEatingTime = 2000;

        //Properties
        internal int Id { get; }
        internal bool IsThinking { get; set; }
        internal bool IsEating { get; set; }

        //Constructor
        internal Guest(int id)
        {
            Id = id;
            IsThinking = false;
            IsEating = false;
        }

        //Methods
        internal void Think()
        {
            var thinkingTime = MyMath.NextInt(_minThinkingTime, _maxThinkingTime);

            IsThinking = true;
            Console.WriteLine($"Guest {Id} is waiting for a chopstick.");
            Thread.Sleep(thinkingTime);
            IsThinking = false;
        }

        internal void Eat()
        {
            var eatingTime = MyMath.NextInt(_minEatingTime, _maxEatingTime);
            
            Console.WriteLine($"Guest {Id} is eating.");
            IsEating = true;
            Thread.Sleep(eatingTime);
            IsEating = false;
        }

        internal void ChooseChopstick(Chopstick chopstick)
        {
            if (chopstick.IsAvailable)
            {
                chopstick.IsAvailable = false;
                Console.WriteLine($"Guest {Id} has picked up chopstick {chopstick.Id}.");
            }
        }
    }
}
