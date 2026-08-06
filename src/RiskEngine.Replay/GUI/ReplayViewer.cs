using Raylib_cs;
using RiskEngine.Replay.GUI.Renderer;

namespace RiskEngine.Replay.GUI;

public sealed class ReplayViewer
{
    private readonly ReplayPlayer _player;
    private readonly ReplayRenderer _renderer;
    private NavigationMode _navigationMode = NavigationMode.Event;

    public ReplayViewer(Replay replay)
    {
        _player = new ReplayPlayer(replay);
        _renderer = new ReplayRenderer();
    }

    public void Run()
    {
        Initialize();

        while (!Raylib.WindowShouldClose())
        {
            Update();
            Draw();
        }

        Shutdown();
    }

    private void Initialize()
    {
        Raylib.InitWindow(1600, 900, "Risk Replay Viewer");
        Raylib.SetTargetFPS(60);
    }

    private void Update()
    {
        switch (_navigationMode)
        {
            case NavigationMode.Event:
                _player.NextEvent();
                break;

            case NavigationMode.Player:
                _player.NextPlayer();
                break;

            case NavigationMode.Phase:
                _player.NextPhase();
                break;

            case NavigationMode.Round:
                _player.NextRound();
                break;
        }
    }

    private void Draw()
    {
        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.DarkGray);

        _renderer.Draw(_player._replay.Header, _player.CurrentFrame, _player.CurrentFrameIndex, _player.FrameCount);

        Raylib.EndDrawing();
    }

    private void Shutdown()
    {
        Raylib.CloseWindow();
    }
}