namespace RiskEngine.Exceptions;

/// <summary>
/// Basis-Klasse für alle gezielten und abgefangenen Fehler der Risk Engine.
/// </summary>
public abstract class RiskEngineException : Exception
{
    protected RiskEngineException(string message) : base(message) { }
    protected RiskEngineException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Wird geworfen, wenn die Engine in einen physikalisch/regeltechnisch unmöglichen Zustand gerät (z. B. 0 Truppen, Underflow).
/// </summary>
public class InvalidEngineStateException : RiskEngineException
{
    public InvalidEngineStateException(string message) : base(message) { }
}

/// <summary>
/// Wird geworfen, wenn ein Mutator/Executor eine unzulässige GameAction verarbeiten soll.
/// </summary>
public class InvalidGameActionException : RiskEngineException
{
    public InvalidGameActionException(string message) : base(message) { }
}