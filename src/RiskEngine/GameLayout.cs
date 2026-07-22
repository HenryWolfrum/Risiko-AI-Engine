namespace RiskEngine;

public sealed class GameLayout
{
    public MapLayout Map { get; }
    public DeckLayout Deck { get; }

    public EngineConfig Config { get; }

    public GameLayout(MapLayout mapLayout, DeckLayout deckLayout,EngineConfig engineConfig)
    {
        Map = mapLayout;
        Deck = deckLayout;
        Config = engineConfig;
    }
}