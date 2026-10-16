using RiskEngine.Layout.AI.Factory;
using RiskEngine.Layout.Replay.Recording;
using RiskEngine.Layout.AI.Configuration;
using RiskEngine.Layout;

namespace RiskEngine.Layout.Replay.Runner;

public static  class ReplayRunner
{
    //Simulates the game and creates a replay
    public static Replay Run(ReplayHeader header)
    {
        //Create Players
        IRiskPlayer[] players = PlayerFactory.Create(header.PlayerConfigs);

        //Create a recorder
        ReplayRecorder recorder = new ReplayRecorder(header);

        //Run the Game with created objects
        GameRunner.PlayGame(header.Layout, players, header.Seed, recorder);

        //Return the Replay
        return recorder.Build();
    }
}