namespace PasswordGenerator.API.Repository
{
    public interface IRandomProvider
    {
        int Next(int minValue, int maxValue);
        char GetRandomChar(string fromChars);
    }
}
