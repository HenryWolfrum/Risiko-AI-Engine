using Raylib_cs;
using RiskEngine.Replay.Recording;

namespace RiskEngine.Replay.GUI;

// Main script for organizing the replay view
public static class ReplayViewer
{
    public static void Run(ReplayPlayer player,int screenWidth,int screenHeight)
    {

        InitializeView(screenWidth, screenHeight);
        
        ViewLayout layout = ViewLayoutFactory.CreateDefault(screenWidth, screenHeight);

        if (!player.IsFirstFrame)
        {
            if (player.FrameCount == 0)
            {
                throw new ArgumentException("Replay player must have a frame count.");
            }

            //Go to intial frame
            player.JumpTo(0);
        }

        // Main game / render loop
        while (!Raylib.WindowShouldClose())
        {
            //1. Update
            UpdateView();
            
            // 2. Render
            Raylib.BeginDrawing();

            RenderView(layout,player);

            Raylib.EndDrawing();
        }

        // Cleanup graphics context
        Raylib.CloseWindow();
    }

    public static void InitializeView(int screenWidth, int screenHeight)
    {
        
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.Msaa4xHint);
       
        Raylib.InitWindow(screenWidth, screenHeight, "Risk Engine - Replay Viewer");
        
        Raylib.SetTargetFPS(60);
    }

    public static void UpdateView()
    {
        
    }
    public static void RenderView(ViewLayout layout,ReplayPlayer player){

        foreach (ViewSection section in layout.Sections)
        {
            section.Renderer.Render(section.Bounds,player);
        }
    }
}