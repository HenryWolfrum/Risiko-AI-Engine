using Raylib_cs;

namespace RiskEngine.Replay.GUI;

public interface ISectionRenderer
{
    void Render(Rectangle bounds, ReplayPlayer player);
}