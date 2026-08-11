using System.Numerics;
using Raylib_cs;

namespace RiskEngine.Replay.GUI;

public class ControlBarRenderer : ISectionRenderer
{
    // Beispiel-Zustand für den Step-Modus (Phase, Frame, Turn etc.)
    private string _currentMode = "Phase";

    public void Render(Rectangle bounds, ReplayPlayer player)
    {
        var frame = player.CurrentFrame;
        
        // 1. Hintergrund der ControlBar zeichnen
        Raylib.DrawRectangleRec(bounds, Color.DarkGray);
        Raylib.DrawRectangleLinesEx(bounds, 1, Color.Gray); // Subtle Border

        // 2. Aufteilung in 3 Subbereiche
        float leftWidth = bounds.Width * 0.30f;
        float centerWidth = bounds.Width * 0.40f;
        float rightWidth = bounds.Width * 0.30f;

        Rectangle leftArea = new(bounds.X, bounds.Y, leftWidth, bounds.Height);
        Rectangle centerArea = new(bounds.X + leftWidth, bounds.Y, centerWidth, bounds.Height);
        Rectangle rightArea = new(bounds.X + leftWidth + centerWidth, bounds.Y, rightWidth, bounds.Height);

        // Subbereiche rendern
        RenderLeftMetaInfo(leftArea,player._replay.Header.Seed,player.CurrentFrameIndex,player.FrameCount);
        RenderCenterGameStatus(centerArea, frame);
        RenderRightControls(rightArea);
    }

    private static void RenderLeftMetaInfo(Rectangle bounds,int seed, int currentFrameIndex, int totalFrames)
    {
        // Mittlere Hälfte der Höhe berechnen (Top Padding = 25% der Höhe)
        float middleHalfY = bounds.Y + (bounds.Height * 0.25f);
        float lineSpacing = 22f;
        float paddingLeft = 20f;

        int fontSize = 18;
        Color textColor = Color.RayWhite;

        // Zeile 1: Seed
        string seedText = $"Seed: {seed}";
        Raylib.DrawText(seedText, (int)(bounds.X + paddingLeft), (int)middleHalfY, fontSize, textColor);

        // Zeile 2: Frame x/y
        string frameText = $"Frame: {currentFrameIndex} / {totalFrames}";
        Raylib.DrawText(frameText, (int)(bounds.X + paddingLeft), (int)(middleHalfY + lineSpacing), fontSize, Color.LightGray);
    }

    private static void RenderCenterGameStatus(Rectangle bounds, ReplayFrame frame)
    {
        int fontSize = 20;

        // Text zusammenbauen
        string statusText = $"Round: {frame.State.CurrentRound}  |  Player: {frame.State.PlayerTurn}  |  Phase: {frame.State.CurrentPhase}";
        
        // Zentrieren in der mittleren Area
        int textWidth = Raylib.MeasureText(statusText, fontSize);
        float textX = bounds.X + (bounds.Width - textWidth) / 2f;
        float textY = bounds.Y + (bounds.Height - fontSize) / 2f;

        Raylib.DrawText(statusText, (int)textX, (int)textY, fontSize, Color.Gold);
    }

    private void RenderRightControls(Rectangle bounds)
    {
        Vector2 mousePos = Raylib.GetMousePosition();

        float buttonHeight = bounds.Height * 0.5f; // 50% der Bar-Höhe
        float buttonY = bounds.Y + (bounds.Height - buttonHeight) / 2f; // Vertikal zentriert

        // Breiten für < , Mode , >
        float btnNavWidth = 40f;
        float btnModeWidth = 110f;
        float spacing = 10f;

        float totalControlWidth = (btnNavWidth * 2) + btnModeWidth + (spacing * 2);
        float startX = bounds.X + (bounds.Width - totalControlWidth) / 2f;

        Rectangle prevBtn = new(startX, buttonY, btnNavWidth, buttonHeight);
        Rectangle modeBtn = new(startX + btnNavWidth + spacing, buttonY, btnModeWidth, buttonHeight);
        Rectangle nextBtn = new(startX + btnNavWidth + btnModeWidth + (spacing * 2), buttonY, btnNavWidth, buttonHeight);

        // Buttons Zeichnen & Interaktion
        if (DrawButton(prevBtn, "<", mousePos))
        {
            // TODO: Event/Action für Previous
        }

        if (DrawButton(modeBtn, _currentMode, mousePos))
        {
            // Modus umschalten
            _currentMode = _currentMode == "Phase" ? "Frame" : "Phase";
        }

        if (DrawButton(nextBtn, ">", mousePos))
        {
            // TODO: Event/Action für Next
        }
    }

    private static bool DrawButton(Rectangle rect, string text, Vector2 mousePos)
    {
        bool isHovered = Raylib.CheckCollisionPointRec(mousePos, rect);
        bool isClicked = isHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);

        Color btnColor = isClicked ? Color.Gray : (isHovered ? Color.LightGray : Color.DarkBlue);
        Color textColor = isHovered ? Color.Black : Color.White;

        Raylib.DrawRectangleRec(rect, btnColor);
        Raylib.DrawRectangleLinesEx(rect, 1, Color.White);

        int fontSize = 18;
        int textWidth = Raylib.MeasureText(text, fontSize);
        float textX = rect.X + (rect.Width - textWidth) / 2f;
        float textY = rect.Y + (rect.Height - fontSize) / 2f;

        Raylib.DrawText(text, (int)textX, (int)textY, fontSize, textColor);

        return isClicked;
    }
}