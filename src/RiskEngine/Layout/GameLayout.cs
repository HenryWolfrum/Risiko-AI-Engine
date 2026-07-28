namespace RiskEngine;

public sealed class GameLayout
{
    public GameLayout(MapLayout map, DeckLayout deck, EngineConfig config)
    {
        var result = GameLayoutValidator.Validate(map, deck, config);

        if (!result.IsValid)
        {
            throw new ArgumentException(
                $"Invalid GameLayout: {result.Error}", nameof(map));
        }

        Map = map;
        Deck = deck;
        Config = config;
    }

    /// <summary>
    /// Immutable map topology.
    /// </summary>
    public MapLayout Map { get; }

    /// <summary>
    /// Immutable territory card layout.
    /// </summary>
    public DeckLayout Deck { get; }

    /// <summary>
    /// Immutable engine configuration.
    /// </summary>
    public EngineConfig Config { get; }
}