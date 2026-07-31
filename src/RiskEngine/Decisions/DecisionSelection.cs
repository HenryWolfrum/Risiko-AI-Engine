/// <summary>
/// Represents the decision selected by an algorithm or user.
/// Contains the index of the chosen option within the current DecisionSpace
/// and the chosen parameter value (if applicable).
/// </summary>
public readonly struct DecisionSelection
{
    public ushort OptionIndex { get; }
    public byte ChosenParameter { get; }

    public DecisionSelection(ushort optionIndex, byte chosenParameter = 0)
    {
        OptionIndex = optionIndex;
        ChosenParameter = chosenParameter;
    }
}