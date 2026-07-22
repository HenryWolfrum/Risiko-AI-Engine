namespace RiskEngine;

public sealed class GameLayout
{
    public MapLayout Map { get; }
    public DeckLayout Deck { get; }

    public GameLayout(MapLayout mapLayout, DeckLayout deckLayout)
    {
        Map = mapLayout;
        Deck = deckLayout;
    }
}