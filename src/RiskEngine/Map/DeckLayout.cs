namespace RiskEngine;

public class DeckLayout
{
    public DeckLayout(CardType[] territoryToType)
    {
        TerritoryToType = territoryToType;

        var infantry = 0UL;
        var cavalry = 0UL;
        var artillery = 0UL;
        var joker = 0UL;

        for (var id = 0; id < territoryToType.Length; id++)
        {
            var bit = 1UL << id;

            switch (territoryToType[id])
            {
                case CardType.Infantry: infantry |= bit; break;
                case CardType.Cavalry: cavalry |= bit; break;
                case CardType.Artillery: artillery |= bit; break;
                case CardType.Joker: joker |= bit; break;
            }
        }

        InfantryMask = infantry;
        CavalryMask = cavalry;
        ArtilleryMask = artillery;
        JokerMask = joker;
    }

    public CardType[] TerritoryToType { get; }

    // Pre-calculated Bitmasks für High-Performance Bitboard Operations
    public ulong InfantryMask { get; }
    public ulong CavalryMask { get; }
    public ulong ArtilleryMask { get; }
    public ulong JokerMask { get; }
}