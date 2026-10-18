namespace RiskEngine.Simulations;

internal class Program
{
    private static void Main(string[] args)
    {
        Arena.Run(
            gamesPerScenario: 10_000,
            playerCount: 4,
            experimentSeed: 20_000);
    }
}