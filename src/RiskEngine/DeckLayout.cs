namespace RiskEngine;


public class DeckLayout
{

   public CardType[] TerritoryToType { get; }


    public DeckLayout(CardType[] territoryToType)
    {
        TerritoryToType = territoryToType;
    }


}
