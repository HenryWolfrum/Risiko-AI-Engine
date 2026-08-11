using Raylib_cs;
using RiskEngine.Replay.Recording;

namespace RiskEngine.Replay.GUI;

// Main script for organizing and running the replay view
public static class ReplayViewer
{
    public static void Run(ReplayPlayer player, int screenWidth, int screenHeight)
    {
        InitializeView(screenWidth, screenHeight);
        
        ViewLayout layout = ViewLayoutFactory.CreateDefault(screenWidth, screenHeight);
        
        ReplayMode currentMode = ReplayMode.Frame;

        if (!player.IsFirstFrame)
        {
            if (player.FrameCount == 0)
            {
                throw new ArgumentException("Replay player must have a frame count.");
            }

            // Go to initial frame
            player.JumpTo(0);
        }

        // Main game / render loop
        while (!Raylib.WindowShouldClose())
        {
            // 1. Context für den aktuellen Frame erstellen
            var context = new ReplayUIContext(player, currentMode);

            // 2. Render View (Button-Klicks setzen context.PendingAction)
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            RenderView(layout, context);

            Raylib.EndDrawing();

            // 3. Durch UI ausgelöste Aktionen verarbeiten
            currentMode = context.Mode;
            ReplayAction pendingAction = context.PendingAction;
            
            UpdateView(ref currentMode, ref pendingAction, player);
        }

        // Cleanup graphics context
        Raylib.CloseWindow();
    }

    public static void InitializeView(int screenWidth, int screenHeight)
    {
        // Set window flags (resizable window and anti-aliasing)
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.Msaa4xHint);
       
        Raylib.InitWindow(screenWidth, screenHeight, "Risk Engine - Replay Viewer");
        
        Raylib.SetTargetFPS(60);
    }
    
    /// <summary>
    /// Handles replay actions and interacts with the player accordingly.
    /// </summary>
    public static void UpdateView(ref ReplayMode mode, ref ReplayAction action, ReplayPlayer player)
    {
        if (action == ReplayAction.None) return;

        switch (action)
        {
            case ReplayAction.ToggleMode:
                // Cycle through step modes: Frame -> Phase -> Player -> Round -> Frame
                mode = mode switch
                {
                    ReplayMode.Frame  => ReplayMode.Phase,
                    ReplayMode.Phase  => ReplayMode.Player,
                    ReplayMode.Player => ReplayMode.Round,
                    _                 => ReplayMode.Frame
                };
                break;

            case ReplayAction.Next:
                // Jump forward based on current mode
                switch (mode)
                {
                    case ReplayMode.Frame:
                        player.NextFrame();
                        break;
                    case ReplayMode.Phase:
                        player.NextPhase();
                        break;
                    case ReplayMode.Player:
                        player.NextPlayer();
                        break;
                    case ReplayMode.Round:
                        player.NextRound();
                        break;
                }
                break;

            case ReplayAction.Previous:
                // Jump backward based on current mode
                switch (mode)
                {
                    case ReplayMode.Frame:
                        player.PreviousFrame();
                        break;
                    case ReplayMode.Phase:
                        player.PreviousPhase();
                        break;
                    case ReplayMode.Player:
                        player.PreviousPlayer();
                        break;
                    case ReplayMode.Round:
                        player.PreviousRound();
                        break;
                }
                break;
        }

        // Action completed, reset to None
        action = ReplayAction.None;
    }

    public static void RenderView(ViewLayout layout, ReplayUIContext context)
    {
        foreach (ViewSection section in layout.Sections)
        {
            section.Renderer.Render(section.Bounds, context);
        }
    }
}