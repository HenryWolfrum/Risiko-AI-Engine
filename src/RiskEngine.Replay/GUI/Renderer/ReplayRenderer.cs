using RiskEngine.Replay;
using RiskEngine.Replay.GUI.Renderer;

public sealed class ReplayRenderer
{
    private readonly TopBarRenderer _topBar;
    private readonly MapRenderer _map;

    public ReplayRenderer()
    {
        _topBar = new TopBarRenderer();
        _map = new MapRenderer();
    }

    public void Draw(ReplayHeader header, ReplayFrame frame, int frameIndex, int frameCount)
    {
        _topBar.Draw(header, frame, frameIndex, frameCount);
        
    }
}