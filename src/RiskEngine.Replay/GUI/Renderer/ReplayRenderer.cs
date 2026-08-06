using RiskEngine.Replay;
using RiskEngine.Replay.GUI;
using RiskEngine.Replay.GUI.Renderer;

public sealed class ReplayRenderer
{
    private readonly TopBarRenderer _topBar;
    private readonly MapRenderer _map;

    public NavigationMode CurrentMode => _topBar.CurrentMode;

    public ReplayRenderer()
    {
        _topBar = new TopBarRenderer();
        _map = new MapRenderer();
    }

    public void Initialize()
    {
        _map.Initialize();
    }

    public TopBarAction Update()
    {
        return _topBar.Update();
    }

    public void Draw(ReplayLayout layout, ReplayHeader header, ReplayFrame frame, int frameIndex, int frameCount)
    {
        _topBar.Draw(layout.TopBar, header, frame, frameIndex, frameCount);
        _map.Draw(frame, layout.Map);
    }

    public void Shutdown()
    {
        _map.Shutdown();
    }
}