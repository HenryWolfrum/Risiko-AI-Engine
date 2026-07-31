namespace RiskEngine.Simulations;

internal class Program
{
    private static void Main(string[] args)
    {
        // Wir starten für den Anfang mit 10.000 Spielen
        Arena.RunTournament(numberOfGames: 100, playerCount: 4);
    }
}