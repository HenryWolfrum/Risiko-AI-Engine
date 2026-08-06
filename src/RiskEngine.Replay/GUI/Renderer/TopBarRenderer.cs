using Raylib_cs;
using RiskEngine.Replay.GUI.Controls;
using RiskEngine.State;

namespace RiskEngine.Replay.GUI.Renderer;

public sealed class TopBarRenderer
{
    private const int FontSize = 20;

    private const int ButtonWidth = 120;
    private const int ButtonHeight = 40;
    private const int ButtonSpacing = 20;

    private readonly Button _previousButton;
    private readonly Button _nextButton;
    private readonly Button _modeButton;

    private NavigationMode _mode = NavigationMode.Event;

    public NavigationMode CurrentMode => _mode;

    public TopBarRenderer()
    {
        // Position wird später durch das Layout gesetzt
        _modeButton = new Button(new Rectangle(), _mode.ToString());
        _previousButton = new Button(new Rectangle(), "Previous");
        _nextButton = new Button(new Rectangle(), "Next");
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
            return TopBarAction.Previous;

        if (_nextButton.IsClicked())
            return TopBarAction.Next;

        return TopBarAction.None;
    }

    public void Draw(
        Rectangle area,
        ReplayHeader header,
        ReplayFrame frame,
        int frameIndex,
        int frameCount)
    {
        // Hintergrund
        Raylib.DrawRectangleRec(area, Color.DarkGray);
        Raylib.DrawRectangleLinesEx(area, 2, Color.Black);

        float right = area.X + area.Width;
        float centerY = area.Y + area.Height / 2f;

        int textY = (int)(centerY - FontSize / 2f);
        float buttonY = centerY - ButtonHeight / 2f;

        // Metadaten
        Raylib.DrawText(
            $"Seed: {header.Seed}",
            (int)area.X + 20,
            textY,
            FontSize,
            Color.White);

        Raylib.DrawText(
            $"Frame: {frameIndex}/{frameCount}",
            (int)area.X + 180,
            textY,
            FontSize,
            Color.White);

        Raylib.DrawText(
            $"Round: {frame.State.CurrentRound}",
            (int)area.X + 380,
            textY,
            FontSize,
            Color.White);

        Raylib.DrawText(
            $"Player: {frame.State.PlayerTurn + 1}",
            (int)area.X + 520,
            textY,
            FontSize,
            Color.White);

        Raylib.DrawText(
            $"Phase: {frame.State.CurrentPhase}",
            (int)area.X + 650,
            textY,
            FontSize,
            Color.White);

        // Buttons relativ zum Layout positionieren
        _modeButton.Bounds = new Rectangle(
            right - 3 * ButtonWidth - 2 * ButtonSpacing,
            buttonY,
            ButtonWidth,
            ButtonHeight);

        _previousButton.Bounds = new Rectangle(
            right - 2 * ButtonWidth - ButtonSpacing,
            buttonY,
            ButtonWidth,
            ButtonHeight);

        _nextButton.Bounds = new Rectangle(
            right - ButtonWidth,
            buttonY,
            ButtonWidth,
            ButtonHeight);

        // Zeichnen
        _modeButton.Draw();
        _previousButton.Draw();
        _nextButton.Draw();
    }
}