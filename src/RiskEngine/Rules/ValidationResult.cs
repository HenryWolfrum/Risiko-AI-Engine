namespace RiskEngine.State;

public readonly struct ValidationResult
{
    public bool IsValid { get; }

    public EngineError Error { get; }


    private ValidationResult(bool isValid, EngineError error)
    {
        IsValid = isValid;
        Error = error;
    }


    //Full valid request
    public static ValidationResult Valid()
    {
        return new ValidationResult(true, EngineError.None);
    }


    public static ValidationResult Invalid(EngineError error)
    {
        return new ValidationResult(false, error);
    }
}