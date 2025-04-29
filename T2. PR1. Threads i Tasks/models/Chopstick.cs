namespace T2_PR1.models
{
    internal class Chopstick
    {
        //Properties
        internal int Id { get; }
        internal bool IsAvailable { get; set; }

        //Constructor
        internal Chopstick(int id)
        {
            Id = id;
            IsAvailable = true;
        }
    }
}
