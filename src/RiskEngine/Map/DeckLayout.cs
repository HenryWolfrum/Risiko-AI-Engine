namespace RiskEngine;

public class DeckLayout
{
    public CardType[] TerritoryToType { get; }

    // Pre-calculated Bitmasks für High-Performance Bitboard Operations
    public ulong InfantryMask { get; }
    public ulong CavalryMask { get; }
    public ulong ArtilleryMask { get; }
    public ulong JokerMask { get; }

    public DeckLayout(CardType[] territoryToType)
    {
        TerritoryToType = territoryToType;

        ulong infantry = 0UL;
        ulong cavalry = 0UL;
        ulong artillery = 0UL;
        ulong joker = 0UL;

        for (int id = 0; id < territoryToType.Length; id++)
        {
            ulong bit = 1UL << id;

            switch (territoryToType[id])
            {
                case CardType.Infantry:  infantry |= bit; break;
                case CardType.Cavalry:   cavalry |= bit; break;
                case CardType.Artillery: artillery |= bit; break;
                case CardType.Joker:  joker |= bit; break;
            }
        }

        InfantryMask = infantry;
        CavalryMask = cavalry;
        ArtilleryMask = artillery;
        JokerMask = joker;
    }
}