using RiskEngine.Replay.Recording;
using RiskEngine.Setup;

namespace RiskEngine.Replay.Runner;

public static class ReplayRunner
{
    //Simulates the game and creates a replay
    public static Replay Run(ReplayHeader header)
    {
        //Create a recorder
        var recorder = new ReplayRecorder(header);

        //Run the game; the recorder observes it
        GameLauncher.Play(header.Configuration, recorder);

        //Return the Replay
        return recorder.Build();
    }
}