
namespace RiskEngine.Replay.Console;

internal static class Program
{
    public static void Main()
    {
        int seed = ReadSeed();

        Console.WriteLine($"Using seed: {seed}");
    }

    private static int ReadSeed()
    {
        Console.Write("Seed: ");

        while (!int.TryParse(Console.ReadLine(), out int seed))
        {
            Console.Write("Please enter a valid integer: ");
        }

        return seed;
    }
}