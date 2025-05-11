namespace T2_PR1.Models
{
    internal class Chopstick
    {
        //Properties
        internal int Id { get; }
        internal bool IsAvailable { get; set; }
        internal object Lock { get; } = new object();

        //Constructor
        internal Chopstick(int id)
        {
            Id = id;
            IsAvailable = true;
        }
    }
}
