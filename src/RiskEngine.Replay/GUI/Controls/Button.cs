using System;
using System.Numerics;
using Raylib_cs;

namespace RiskEngine.Replay.GUI.Controls;

public class Button
{
    public Rectangle Bounds { get; set; }
    public string Text { get; set; }
    public int FontSize { get; set; } = 18;

    // Farb-Styling (mit sinnvollen Defaults)
    public Color NormalColor { get; set; } = Color.DarkBlue;
    public Color HoverColor { get; set; } = Color.LightGray;
    public Color PressedColor { get; set; } = Color.Gray;
    public Color TextColor { get; set; } = Color.White;
    public Color HoverTextColor { get; set; } = Color.Black;
    public Color BorderColor { get; set; } = Color.White;

    // Zustände
    public bool IsHovered { get; private set; }

    // Event für entkoppelte Logik
    public event Action? OnClick;

    public Button(Rectangle bounds, string text)
    {
        Bounds = bounds;
        Text = text;
    }

    /// <summary>
    /// Aktualisiert den Zustand, zeichnet den Button und gibt true zurück, wenn geklickt wurde.
    /// </summary>
    public bool DrawAndCheck(Vector2 mousePos)
    {
        IsHovered = Raylib.CheckCollisionPointRec(mousePos, Bounds);
        bool isPressed = IsHovered && Raylib.IsMouseButtonDown(MouseButton.Left);
        bool isClicked = IsHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (isClicked)
        {
            OnClick?.Invoke();
        }

        // Farbe basierend auf Zustand wählen
        Color currentBtnColor = NormalColor;
        Color currentTextColor = TextColor;

        if (IsHovered)
        {
            currentBtnColor = isPressed ? PressedColor : HoverColor;
            currentTextColor = HoverTextColor;
        }

        // 1. Hintergrund & Rahmen
        Raylib.DrawRectangleRec(Bounds, currentBtnColor);
        Raylib.DrawRectangleLinesEx(Bounds, 1, BorderColor);

        // 2. Text zentrieren
        int textWidth = Raylib.MeasureText(Text, FontSize);
        float textX = Bounds.X + (Bounds.Width - textWidth) / 2f;
        float textY = Bounds.Y + (Bounds.Height - FontSize) / 2f;

        Raylib.DrawText(Text, (int)textX, (int)textY, FontSize, currentTextColor);

        return isClicked;
    }
}