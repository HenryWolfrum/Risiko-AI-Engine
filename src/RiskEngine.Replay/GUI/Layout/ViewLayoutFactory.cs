using Raylib_cs;
using RiskEngine.Replay.GUI.Renderer;

namespace RiskEngine.Replay.GUI.Layout;

public abstract class ViewLayoutFactory
{
    public static ViewLayout CreateDefault(float width, float height)
    {
        //ViewSections
        var sections = new ViewSection[3];

        //Bounds
        var bounds = new Rectangle[3];


        //1. ControlBar
        var controlBarName = "ControlBar";

        //ControlBar constants
        float ControlBarX = 0;
        float ControlBarY = 0;
        var ControlBarWidth = width;
        var ControlBarHeight = 0.25F * height;

        bounds[0] = new Rectangle(ControlBarX, ControlBarY, ControlBarWidth, ControlBarHeight);

        //Renderer
        var controlBarRenderer = new ControlBarRenderer();


        //2. Map
        var mapName = "Map";

        //Map Dimensions
        float mapX = 0;
        var mapY = ControlBarHeight;
        var mapWidth = 0.5F * width;
        var mapHeight = 0.75F * height;

        bounds[1] = new Rectangle(mapX, mapY, mapWidth, mapHeight);

        //Renderer
        var mapRenderer = new MapRenderer();

        //3. Player Viewer
        var playerViewerName = "Player Viewer";

        //Player Viewer Dimensions
        var playerViewX = 0.5F * width;
        var playerViewY = ControlBarHeight;
        var playerViewWidth = 0.5F * width;
        var playerViewHeight = 0.75F * height;

        bounds[2] = new Rectangle(playerViewX, playerViewY, playerViewWidth, playerViewHeight);

        //Renderer
        var playerViewerRenderer = new PlayerViewerRenderer();


        //Sections assembly
        sections[0] = new ViewSection(controlBarName, bounds[0], controlBarRenderer);
        sections[1] = new ViewSection(mapName, bounds[1], mapRenderer);
        sections[2] = new ViewSection(playerViewerName, bounds[2], playerViewerRenderer);


        return new ViewLayout
        {
            ScreenWidth = width,
            ScreenHeight = height,
            Sections = sections
        };
    }
}