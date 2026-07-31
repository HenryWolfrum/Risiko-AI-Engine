/// <summary> Zero-allocation view for card combination decision metadata. </summary>
public readonly struct CardTripleData
{
    public byte Card1 { get; }
    public byte Card2 { get; }
    public byte Card3 { get; }

    public CardTripleData(byte card1, byte card2, byte card3)
    {
        Card1 = card1;
        Card2 = card2;
        Card3 = card3;
    }
}