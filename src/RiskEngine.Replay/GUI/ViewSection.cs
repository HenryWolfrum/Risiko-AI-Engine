using Raylib_cs;
using RiskEngine.Replay.GUI;

namespace RiskEngine.Replay.GUI;

public class ViewSection
{
    public string Name { get; }
    public Rectangle Bounds { get; set; }
    public ISectionRenderer Renderer { get; set; }

    public ViewSection(string name, Rectangle bounds, ISectionRenderer renderer)
    {
        Name = name;
        Bounds = bounds;
        Renderer = renderer;
    }

    public void Draw(ReplayPlayer player)
    {
        Raylib.BeginScissorMode((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height);

        Renderer.Render(Bounds, player);

        Raylib.EndScissorMode();
    }
}