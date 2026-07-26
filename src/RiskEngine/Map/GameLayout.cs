namespace RiskEngine;

public sealed class GameLayout
{
    public GameLayout(MapLayout mapLayout, DeckLayout deckLayout, EngineConfig engineConfig)
    {
        //Configuration Validation check
        if (mapLayout.TerritoryNames.Length != engineConfig.TerritoryCount ||
            deckLayout.TerritoryToType.Length - 2 != engineConfig.TerritoryCount)
            throw new ArgumentException(
                $"LAYOUT MISMATCH: Config expects {engineConfig.TerritoryCount} Territories, " +
                $"Map has {mapLayout.TerritoryNames.Length}, Deck has {deckLayout.TerritoryToType.Length}.");


        Map = mapLayout;
        Deck = deckLayout;
        Config = engineConfig;
    }

    //Topology
    public MapLayout Map { get; }

    //Territory -> CardType Mapping
    public DeckLayout Deck { get; }

    //PlayerCount, TerritoryCount, ...
    public EngineConfig Config { get; }
}