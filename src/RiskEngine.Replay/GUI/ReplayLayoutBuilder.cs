using Raylib_cs;

namespace RiskEngine.Replay.GUI;

public static class ReplayLayoutBuilder
{
    public static ReplayLayout Create()
    {
        int width = Raylib.GetScreenWidth();
        int height = Raylib.GetScreenHeight();

        const int topBarHeight = 80;
        const int playerWidth = 300;
        const int outerMargin = 20;

        return new ReplayLayout
        {
            TopBar = new Rectangle(0, 0, width, topBarHeight),

            Map = new Rectangle(outerMargin, topBarHeight, width - playerWidth - outerMargin * 2, height - topBarHeight - outerMargin),

            PlayerViewer = new Rectangle(width - playerWidth, topBarHeight, playerWidth, height - topBarHeight)
        };
    }
}