namespace RiskEngine.Replay.GUI.Layout;

public struct ViewLayout
{
    public ViewSection[] Sections;

    public float ScreenWidth;
    public float ScreenHeight;

    public int SectionCount => Sections.Length;
}