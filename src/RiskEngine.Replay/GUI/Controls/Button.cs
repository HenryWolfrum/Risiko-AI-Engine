namespace RiskEngine.Replay.GUI.Controls;

using Raylib_cs;


public sealed class Button
{
    public Rectangle Bounds { get; set; }

    public string Text { get; set; }

    public Button(Rectangle bounds, string text)
    {
        Bounds = bounds;
        Text = text;
    }

    public bool IsHovered => Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), Bounds);

    public bool IsClicked()
    {
        return IsHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);
    }

    public void Draw()
    {
        Color background = IsHovered ? Color.LightGray : Color.Gray;

        Raylib.DrawRectangleRec(Bounds, background);
        Raylib.DrawRectangleLinesEx(Bounds, 2, Color.Black);

        int fontSize = 20;

        int textWidth = Raylib.MeasureText(Text, fontSize);

        int x = (int)(Bounds.X + (Bounds.Width - textWidth) / 2);
        int y = (int)(Bounds.Y + (Bounds.Height - fontSize) / 2);

        Raylib.DrawText(Text, x, y, fontSize, Color.Black);
    }
}