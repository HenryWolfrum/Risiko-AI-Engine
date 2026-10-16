namespace RiskEngine.Simulations;

internal class Program
{
    private static void Main(string[] args)
    {
        Arena.RunTournament(10_000, 4);
    }
}