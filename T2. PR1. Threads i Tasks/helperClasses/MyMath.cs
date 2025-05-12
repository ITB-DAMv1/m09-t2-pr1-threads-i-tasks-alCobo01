namespace T2_PR1.HelperClasses
{
    public static class MyMath
    {
        private static Random _random = new Random();
        public static int NextInt(int min, int max) => _random.Next(min, max + 1);
    }
}
