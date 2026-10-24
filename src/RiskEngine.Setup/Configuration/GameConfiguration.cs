using RiskEngine.AI.Configuration;
using RiskEngine.Layout;
using RiskEngine.Setup.Error;

namespace RiskEngine.Setup.Configuration;

public sealed record GameConfiguration
{
    public ulong Seed {get; }
    public GameLayout Layout { get; }
    public IReadOnlyList<PlayerConfiguration> PlayerConfigurations { get; }

    public GameConfiguration(ulong seed, GameLayout layout, IReadOnlyList<PlayerConfiguration> playerConfigurations)
    {
        var result = GameConfigurationValidator.Validate(layout, playerConfigurations);
        if (!result.IsValid)
            throw new SetupException(result.Error);

        Seed = seed;
        Layout = layout;
        PlayerConfigurations = playerConfigurations.ToArray();
    }
}