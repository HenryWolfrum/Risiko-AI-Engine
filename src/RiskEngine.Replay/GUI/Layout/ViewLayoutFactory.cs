using Raylib_cs;

namespace RiskEngine.Replay.GUI;

public abstract class ViewLayoutFactory
{
    public static ViewLayout CreateDefault(float width, float height)
    {
         
        //ViewSections
        ViewSection[] sections = new ViewSection[3];
        
        //Bounds
        Rectangle[] bounds = new Rectangle[3];
        
        
        
        //1. ControlBar
        string controlBarName = "ControlBar";
        
        //ControlBar constants
        float ControlBarX = 0;
        float ControlBarY = 0;
        float ControlBarWidth = width;
        float ControlBarHeight = 0.25F * height;
        
        bounds[0] = new Rectangle(ControlBarX, ControlBarY, ControlBarWidth, ControlBarHeight);
        
        //Renderer
        ControlBarRenderer controlBarRenderer = new ControlBarRenderer();
        

        //2. Map
        string mapName = "Map";
        
        //Map Dimensions
        float mapX = 0;
        float mapY = ControlBarHeight;
        float mapWidth = 0.5F * width;
        float mapHeight = 0.75F * height;
        
        bounds[1]= new Rectangle(mapX,mapY,mapWidth,mapHeight);
        
        //Renderer
        MapRenderer mapRenderer = new MapRenderer();
        
        //3. Player Viewer
        string playerViewerName = "Player Viewer";
        
        //Player Viewer Dimensions
        float playerViewX = 0.5F * width;
        float playerViewY = ControlBarHeight;
        float playerViewWidth = 0.5F * width;
        float playerViewHeight = 0.75F * height;

        bounds[2] = new Rectangle(playerViewX, playerViewY, playerViewWidth, playerViewHeight);

        //Renderer
        PlayerViewerRenderer playerViewerRenderer = new PlayerViewerRenderer();


        //Sections assembly
        sections[0] = new ViewSection(controlBarName, bounds[0], controlBarRenderer);
        sections[1] = new ViewSection(mapName, bounds[1], mapRenderer);
        sections[2] = new ViewSection(playerViewerName, bounds[2], playerViewerRenderer);
        

        return new ViewLayout
        {
            ScreenWidth = width,
            ScreenHeight = height,
            Sections =  sections
        };
    }
}