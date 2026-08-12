using System.Numerics;
using Raylib_cs;
using RiskEngine.Replay.GUI.Controls;

namespace RiskEngine.Replay.GUI;

public class ControlBarRenderer : ISectionRenderer
{
    // Button-Objekte einmalig beim Erstellen des Renderers instanziieren
    private readonly Button _prevButton = new(default, "<");
    private readonly Button _modeButton = new(default, "Phase");
    private readonly Button _nextButton = new(default, ">");

    public void Render(Rectangle bounds, ReplayUIContext context)
    {
        var player = context.Player;
        var frame = player.CurrentFrame;
        
        // Background
        Raylib.DrawRectangleRec(bounds, Color.DarkGray);
        Raylib.DrawRectangleLinesEx(bounds, 1, Color.Gray); // Subtle Border

        // Separate into three subareas
        float leftWidth = bounds.Width * 0.30f;
        float centerWidth = bounds.Width * 0.40f;
        float rightWidth = bounds.Width * 0.30f;

        Rectangle leftArea = new(bounds.X, bounds.Y, leftWidth, bounds.Height);
        Rectangle centerArea = new(bounds.X + leftWidth, bounds.Y, centerWidth, bounds.Height);
        Rectangle rightArea = new(bounds.X + leftWidth + centerWidth, bounds.Y, rightWidth, bounds.Height);

        // Render sub Areas
        RenderLeftMetaInfo(leftArea, player._replay.Header.Seed, player.CurrentFrameIndex, player.FrameCount);
        RenderCenterGameStatus(centerArea, frame);
        RenderRightControls(rightArea, context);
    }

    private static void RenderLeftMetaInfo(Rectangle bounds, int seed, int currentFrameIndex, int totalFrames)
    {
        // Middle Half of height
        float middleHalfY = bounds.Y + (bounds.Height * 0.25f);
        float lineSpacing = 22f;
        float paddingLeft = 20f;

        int fontSize = 18;
        Color textColor = Color.RayWhite;

        // Seed
        string seedText = $"Seed: {seed}";
        Raylib.DrawText(seedText, (int)(bounds.X + paddingLeft), (int)middleHalfY, fontSize, textColor);

        // Frame
        string frameText = $"Frame: {currentFrameIndex} / {totalFrames}";
        Raylib.DrawText(frameText, (int)(bounds.X + paddingLeft), (int)(middleHalfY + lineSpacing), fontSize, Color.LightGray);
    }

    private static void RenderCenterGameStatus(Rectangle bounds, ReplayFrame frame)
    {
        int fontSize = 20;

        // 1. Statuszeile in Segmente zerlegen
        string part1 = $"Round: {frame.State.CurrentRound}  |  Player: ";
        string part2 = $"{frame.State.PlayerTurn}";
        string part3 = $"  |  Phase: {frame.State.CurrentPhase}";

        // 2. Farbe des aktuellen Spielers ermitteln
        Color playerColor = ReplayViewer.GetPlayerColor(frame.State.PlayerTurn);

        // 3. Breiten für die Gesamtzentrierung messen
        int width1 = Raylib.MeasureText(part1, fontSize);
        int width2 = Raylib.MeasureText(part2, fontSize);
        int width3 = Raylib.MeasureText(part3, fontSize);

        int totalWidth = width1 + width2 + width3;

        // 4. Startkoordinaten berechnen
        float currentX = bounds.X + (bounds.Width - totalWidth) / 2f;
        float textY = bounds.Y + (bounds.Height - fontSize) / 2f;

        // 5. Segmente nacheinander rendern
        Raylib.DrawText(part1, (int)currentX, (int)textY, fontSize, Color.Gold);
        currentX += width1;

        Raylib.DrawText(part2, (int)currentX, (int)textY, fontSize, playerColor);
        currentX += width2;

        Raylib.DrawText(part3, (int)currentX, (int)textY, fontSize, Color.Gold);
    }

    private void RenderRightControls(Rectangle bounds, ReplayUIContext context)
    {
        Vector2 mousePos = Raylib.GetMousePosition();

        float buttonHeight = bounds.Height * 0.5f; 
        float buttonY = bounds.Y + (bounds.Height - buttonHeight) / 2f; 

        // Button dimensions
        float btnNavWidth = 40f;
        float btnModeWidth = 110f;
        float spacing = 10f;

        float totalControlWidth = (btnNavWidth * 2) + btnModeWidth + (spacing * 2);
        float startX = bounds.X + (bounds.Width - totalControlWidth) / 2f;

        // Update button positions & dynamic label
        _prevButton.Bounds = new Rectangle(startX, buttonY, btnNavWidth, buttonHeight);
    
        _modeButton.Bounds = new Rectangle(startX + btnNavWidth + spacing, buttonY, btnModeWidth, buttonHeight);
        _modeButton.Text = context.Mode.ToString(); // Displays "Frame", "Phase", "Player", or "Round"
    
        _nextButton.Bounds = new Rectangle(startX + btnNavWidth + btnModeWidth + (spacing * 2), buttonY, btnNavWidth, buttonHeight);

        // Render buttons & set trigger actions directly on context
        if (_prevButton.DrawAndCheck(mousePos))
        {
            context.PendingAction = ReplayAction.Previous;
        }

        if (_modeButton.DrawAndCheck(mousePos))
        {
            context.PendingAction = ReplayAction.ToggleMode;
        }

        if (_nextButton.DrawAndCheck(mousePos))
        {
            context.PendingAction = ReplayAction.Next;
        }
    }
}