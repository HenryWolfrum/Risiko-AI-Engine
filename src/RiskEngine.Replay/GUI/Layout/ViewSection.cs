using Raylib_cs;

namespace RiskEngine.Replay.GUI;

public class ViewSection
{
    public ViewSection(string name, Rectangle bounds, ISectionRenderer renderer)
    {
        Name = name;
        Bounds = bounds;
        Renderer = renderer;
    }

    public string Name { get; }
    public Rectangle Bounds { get; set; }
    public ISectionRenderer Renderer { get; set; }

    public void Draw(ReplayUIContext context)
    {
        Raylib.BeginScissorMode((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height);

        Renderer.Render(Bounds, context);

        Raylib.EndScissorMode();
    }
}