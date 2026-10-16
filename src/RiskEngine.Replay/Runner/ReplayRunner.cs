using RiskEngine.AI.Factory;
using RiskEngine.Core;
using RiskEngine.Player;
using RiskEngine.Replay.Recording;

namespace RiskEngine.Replay.Runner;

public static class ReplayRunner
{
    //Simulates the game and creates a replay
    public static Replay Run(ReplayHeader header)
    {
        //Create Players
        IRiskPlayer[] players = PlayerFactory.Create(header.PlayerConfigs);

        //Create a recorder
        var recorder = new ReplayRecorder(header);

        //Run the Game with created objects
        GameRunner.PlayGame(header.Layout, players, header.Seed, recorder);

        //Return the Replay
        return recorder.Build();
    }
}