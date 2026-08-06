using Raylib_cs;
using RiskEngine.Replay.GUI.Controls;
using RiskEngine.State;

namespace RiskEngine.Replay.GUI.Renderer;

public sealed class TopBarRenderer
{
    private const int Height = 80;
    private const int TextY = 30;

    private readonly Button _previousButton;
    private readonly Button _nextButton;
    private readonly Button _modeButton;
    
    public NavigationMode CurrentMode => _mode;
    
    private NavigationMode _mode = NavigationMode.Event;

    public TopBarRenderer()
    {
        _modeButton = new Button(
            new Rectangle(850, 20, 120, 40),
            CurrentMode.ToString());

        _previousButton = new Button(
            new Rectangle(1000, 20, 120, 40),
            "Previous");

        _nextButton = new Button(
            new Rectangle(1140, 20, 120, 40),
            "Next");
    }
    
    private void ChangeMode()
    {
        _mode = _mode switch
        {
            NavigationMode.Event => NavigationMode.Phase,
            NavigationMode.Phase => NavigationMode.Player,
            NavigationMode.Player => NavigationMode.Round,
            NavigationMode.Round => NavigationMode.Event,
            _ => NavigationMode.Event
        };

        _modeButton.Text = _mode.ToString();
    }
    
    
    public TopBarAction Update()
    {
        if (_modeButton.IsClicked())
        {
            ChangeMode();
            return TopBarAction.ChangeMode;
        }

        if (_previousButton.IsClicked())
        {
            return TopBarAction.Previous;
        }

        if (_nextButton.IsClicked())
        {
            return TopBarAction.Next;
        }

        return TopBarAction.None;
    }

    public void Draw(ReplayHeader header, ReplayFrame frame, int frameIndex, int frameCount)
    {
        // Background
        Rectangle topBar = new Rectangle(0, 0, Raylib.GetScreenWidth(), Height);

        Raylib.DrawRectangleRec(topBar, Color.DarkGray);

        Raylib.DrawRectangleLinesEx(topBar, 2, Color.Black);

        // Metadata
        Raylib.DrawText($"Seed: {header.Seed}", 20, TextY, 20, Color.White);

        Raylib.DrawText($"Frame: {frameIndex}/{frameCount}", 180, TextY, 20, Color.White);

        Raylib.DrawText($"Round: {frame.State.CurrentRound}", 380, TextY, 20, Color.White);

        Raylib.DrawText($"Player: {frame.State.PlayerTurn + 1}", 520, TextY, 20, Color.White);

        Raylib.DrawText($"Phase: {frame.State.CurrentPhase}", 650, TextY, 20, Color.White);

        // Buttons
        _modeButton.Draw();
        _previousButton.Draw();
        _nextButton.Draw();
    }
}