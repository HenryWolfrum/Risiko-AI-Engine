namespace RiskEngine;

public readonly struct ValidationResult
{
    public bool IsValid { get; }

    public GameError Error { get; }


    private ValidationResult(bool isValid, GameError error)
    {
        IsValid = isValid;
        Error = error;
    }


    //Full valid request
    public static ValidationResult Valid()
    {
        return new ValidationResult(true, GameError.None);
    }


    public static ValidationResult Invalid(GameError error)
    {
        return new ValidationResult(false, error);
    }
}