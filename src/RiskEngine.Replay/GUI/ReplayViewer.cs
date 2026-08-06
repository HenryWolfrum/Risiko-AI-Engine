using Raylib_cs;
using RiskEngine.Replay.GUI.Renderer;

namespace RiskEngine.Replay.GUI;

public sealed class ReplayViewer
{
    private readonly ReplayPlayer _player;
    private readonly ReplayRenderer _renderer;

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
        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            _player.NextEvent();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            _player.PreviousEvent();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            _player.NextPlayer();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            _player.PreviousPlayer();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.PageDown))
        {
            _player.NextRound();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.PageUp))
        {
            _player.PreviousRound();
        }
    }

    private void Draw()
    {
        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.DarkGray);

        _renderer.Draw(_player.CurrentFrame);

        Raylib.EndDrawing();
    }

    private void Shutdown()
    {
        Raylib.CloseWindow();
    }
}