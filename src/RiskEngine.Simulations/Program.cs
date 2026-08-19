namespace RiskEngine.Simulations;

internal class Program
{
    private static void Main(string[] args)
    {
        Arena.RunTournament(numberOfGames: 10_000, playerCount: 4);
    }
}