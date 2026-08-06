using Raylib_cs;
using RiskEngine.Replay;
using RiskEngine.Replay.GUI;
using RiskEngine.Replay.GUI.Renderer;

public sealed class ReplayViewer
{
    private readonly ReplayPlayer _player;
    private readonly ReplayRenderer _renderer;

    private ReplayLayout _layout;

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

        _layout = ReplayLayoutBuilder.Create();

        _renderer.Initialize();
    }

    private void Shutdown()
    {
        _renderer.Shutdown();
        Raylib.CloseWindow();
    }

    private void Update()
    {
        // Falls später Fenstergröße geändert werden kann
        _layout = ReplayLayoutBuilder.Create();

        TopBarAction action = _renderer.Update();

        switch (action)
        {
            case TopBarAction.Next:
                ExecuteNext();
                break;

            case TopBarAction.Previous:
                ExecutePrevious();
                break;
        }
    }

    private void Draw()
    {
        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.DarkGray);

        _renderer.Draw(
            _layout,
            _player._replay.Header,
            _player.CurrentFrame,
            _player.CurrentFrameIndex,
            _player.FrameCount);

        Raylib.EndDrawing();
    }

    private void ExecuteNext(){switch (_renderer.CurrentMode){case NavigationMode.Event:_player.NextEvent();break;

            case NavigationMode.Phase:
                _player.NextPhase();
                break;

            case NavigationMode.Player:
                _player.NextPlayer();
                break;

            case NavigationMode.Round:
                _player.NextRound();
                break;
        }
    }
    private void ExecutePrevious()
    {
        switch (_renderer.CurrentMode)
        {
            case NavigationMode.Event:
                _player.PreviousEvent();
                break;

            case NavigationMode.Phase:
                _player.PreviousPhase();
                break;

            case NavigationMode.Player:
                _player.PreviousPlayer();
                break;

            case NavigationMode.Round:
                _player.PreviousRound();
                break;
        }
    }
}