using RiskEngine.Replay.GUI;
using RiskEngine.Replay.Runner;
using RiskEngine.Replay.Setup;

namespace RiskEngine.Replay;

internal static class Program
{
    public static void Main()
    {
        // Configure replay
        ReplayHeader header = ReplaySetup.Create();

        // Resimulate and record the game
        Replay replay = ReplayRunner.Run(header);
        
        //Setup a player for replay
        ReplayPlayer player = new ReplayPlayer(replay);

        // Launch the replay viewer
        ReplayViewer.Run(player,1600,900);
    }
}