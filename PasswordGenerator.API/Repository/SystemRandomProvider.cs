namespace PasswordGenerator.API.Repository
{
    public class SystemRandomProvider : IRandomProvider
    {
        private readonly Random _random = new();

        public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);

        public char GetRandomChar(string fromChars) =>
            fromChars[_random.Next(fromChars.Length)];
    }
}
