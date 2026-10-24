using RiskEngine.Setup.Error;

namespace RiskEngine.Setup.Configuration;

public readonly record struct SetupValidationResult(SetupError Error)
{
    public bool IsValid => Error == SetupError.None;

    public static SetupValidationResult Valid() => new(SetupError.None);
    public static SetupValidationResult Invalid(SetupError error) => new(error);
}