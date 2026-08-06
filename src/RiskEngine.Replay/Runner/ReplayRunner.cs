using RiskEngine.AI.Configuration;
using RiskEngine.AI.Factory;
using RiskEngine.Replay.Recording;
using RiskEngine.State;

namespace RiskEngine.Replay.Runner;

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