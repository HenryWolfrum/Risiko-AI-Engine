namespace RiskEngine.Setup.Error;

public sealed class SetupException : Exception
{
    public SetupError Error { get; }

    public SetupException(SetupError error) : base($"Invalid game configuration: {error}")
    {
        Error = error;
    }
}