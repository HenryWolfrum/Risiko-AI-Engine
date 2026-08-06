using Raylib_cs;
using RiskEngine.State;

namespace RiskEngine.Replay.GUI.Renderer;

public sealed class ReplayRenderer
{
    public void Draw(ReplayFrame frame)
    {
        DrawHUD(frame);
    }

   
    private void DrawHUD(ReplayFrame frame)
    {
        int x = 20;
        int y = 20;
        const int lineHeight = 30;

        Raylib.DrawText($"Round: {frame.State.CurrentRound}", x, y, 20, Color.White);
        y += lineHeight;

        Raylib.DrawText($"Player: {frame.State.PlayerTurn}", x, y, 20, Color.White);
        y += lineHeight;

        Raylib.DrawText($"Phase: {frame.State.CurrentPhase}", x, y, 20, Color.White);
        y += lineHeight;

        if (frame.Action is GameAction action)
        {
            Raylib.DrawText($"Action: {action.Type}", x, y, 20, Color.White);
        }
        else
        {
            Raylib.DrawText("Action: <Initial State>", x, y, 20, Color.White);
        }
    }
    
}