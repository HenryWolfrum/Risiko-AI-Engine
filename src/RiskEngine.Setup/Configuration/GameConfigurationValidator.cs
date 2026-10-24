using RiskEngine.AI.Configuration;
using RiskEngine.Layout;
using RiskEngine.Setup.Error;

namespace RiskEngine.Setup.Configuration;

public static class GameConfigurationValidator
{
    public static SetupValidationResult Validate(GameLayout layout, IReadOnlyList<PlayerConfiguration>? playerConfigurations)
    {
        if (playerConfigurations is null)
            return SetupValidationResult.Invalid(SetupError.PlayerConfigurationsMissing);

        if (playerConfigurations.Count != layout.Config.PlayerCount)
            return SetupValidationResult.Invalid(SetupError.PlayerConfigurationCountMismatch);

        foreach (var playerConfiguration in playerConfigurations)
            if (playerConfiguration is null)
                return SetupValidationResult.Invalid(SetupError.PlayerConfigurationEntryMissing);

        return SetupValidationResult.Valid();
    }
}