namespace RiskEngine.Rules;

public static class TurnInCardsRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action, in DeckLayout deck)
    {
        // Cards out of Range
        if (action.Card1 >= EngineConstants.DEFAULT_TERRITORY_COUNT ||
            action.Card2 >= EngineConstants.DEFAULT_TERRITORY_COUNT ||
            action.Card3 >= EngineConstants.DEFAULT_TERRITORY_COUNT)
        {
            return ValidationResult.Invalid(GameError.UnknownCard);
        }

        //Does player own the cards
        byte player = state.PlayerTurn;
        if (!state.PlayerHasCard(player, action.Card1) ||
            !state.PlayerHasCard(player, action.Card2) ||
            !state.PlayerHasCard(player, action.Card3))
        {
            return ValidationResult.Invalid(GameError.CardNotOwned);
        }

        //Get Card Types
        CardType cardType1 = deck.TerritoryToType[action.Card1];
        CardType cardType2 = deck.TerritoryToType[action.Card2];
        CardType cardType3 = deck.TerritoryToType[action.Card3];

        //At least one Joker is valid set
        if (cardType1 == CardType.Joker || cardType2 == CardType.Joker || cardType3 == CardType.Joker)
        {
            return ValidationResult.Valid();
        }

        //Three of a kind
        if (cardType1 == cardType2 && cardType2 == cardType3)
        {
            return ValidationResult.Valid();
        }
        
        //All different
        if (cardType1 != cardType2 && cardType1 != cardType3 && cardType2 != cardType3)
        {
            return ValidationResult.Valid();
        }
        
        //Invalid Card Set
        return ValidationResult.Invalid(GameError.InvalidCardSet);
    }
}