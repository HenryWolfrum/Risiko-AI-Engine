using Raylib_cs;

namespace RiskEngine.Layout.Replay.GUI;

public interface ISectionRenderer
{
    void Render(Rectangle bounds,ReplayUIContext context);
}