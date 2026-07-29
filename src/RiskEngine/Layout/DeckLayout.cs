using System.Numerics;
using RiskEngine.State;

public class DeckLayout
{
    public DeckLayout(CardType[] territoryToType)
    {
        TerritoryToType = territoryToType;

        ulong infantry = 0;
        ulong cavalry = 0;
        ulong artillery = 0;
        ulong joker = 0;

        for (int id = 0; id < territoryToType.Length; id++)
        {
            ulong bit = 1UL << id;

            switch (territoryToType[id])
            {
                case CardType.Infantry:
                    infantry |= bit;
                    break;

                case CardType.Cavalry:
                    cavalry |= bit;
                    break;

                case CardType.Artillery:
                    artillery |= bit;
                    break;

                case CardType.Joker:
                    joker |= bit;
                    break;
            }
        }

        InfantryMask = infantry;
        CavalryMask = cavalry;
        ArtilleryMask = artillery;
        JokerMask = joker;

        AllCardsMask = infantry | cavalry | artillery | joker;
    }

    public CardType[] TerritoryToType { get; }

    public byte CardCount => (byte)TerritoryToType.Length;

    public byte JokerCount => (byte)BitOperations.PopCount(JokerMask);

    public byte TerritoryCardCount => (byte)(CardCount - JokerCount);

    public ulong InfantryMask { get; }
    public ulong CavalryMask { get; }
    public ulong ArtilleryMask { get; }
    public ulong JokerMask { get; }

    public ulong AllCardsMask { get; }
}