using RiskEngine.AI.Configuration;
using RiskEngine.Layout;
using RiskEngine.Setup.Configuration;
using RiskEngine.Setup.Error;

namespace RiskEngine.Setup.Builder;

public sealed class GameConfigurationBuilder
{
    private readonly List<PlayerConfiguration> _players = new();
    private GameLayout? _layout;
    private ulong? _seed;

    public GameConfigurationBuilder WithSeed(ulong seed)
    {
        _seed = seed;
        return this;
    }

    public GameConfigurationBuilder WithLayout(GameLayout layout)
    {
        _layout = layout;
        return this;
    }

    public GameConfigurationBuilder AddPlayer(PlayerConfiguration player)
    {
        ArgumentNullException.ThrowIfNull(player);
        _players.Add(player);
        return this;
    }

    public GameConfiguration Build()
    {
        // "is not { } x" funktioniert für Struct und Klasse gleichermaßen
        if (_seed is not { } seed)
            throw new SetupException(SetupError.SeedNotSet);

        if (_layout is not { } layout)
            throw new SetupException(SetupError.LayoutNotSet);

        // Der Konstruktor von GameConfiguration prüft Spielerzahl usw.
        return new GameConfiguration(seed, layout, _players);
    }
    
    
}