namespace RiskEngine.Rules;

public static class TurnInCardsRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action,DeckLayout deck, MapLayout map)
    {
        // Cards out of Range
        if (action.Card1 >= map.TerritoryCount || action.Card2 >= map.TerritoryCount ||action.Card3 >= map.TerritoryCount)
            return ValidationResult.Invalid(GameError.UnknownCard);

        //Does player own the cards
        var player = state.PlayerTurn;
        if (!CardHelper.PlayerHasCard(in state, player, action.Card1,deck) ||
            !CardHelper.PlayerHasCard(in state, player, action.Card2,deck) ||
            !CardHelper.PlayerHasCard(in state, player, action.Card3,deck))
            return ValidationResult.Invalid(GameError.CardNotOwned);

        //Get Card Types
        var cardType1 = deck.TerritoryToType[action.Card1];
        var cardType2 = deck.TerritoryToType[action.Card2];
        var cardType3 = deck.TerritoryToType[action.Card3];

        //At least one Joker is valid set
        if (cardType1 == CardType.Joker || cardType2 == CardType.Joker || cardType3 == CardType.Joker)
            return ValidationResult.Valid();

        //Three of a kind
        if (cardType1 == cardType2 && cardType2 == cardType3) return ValidationResult.Valid();

        //All different
        if (cardType1 != cardType2 && cardType1 != cardType3 && cardType2 != cardType3) return ValidationResult.Valid();

        //Invalid Card Set
        return ValidationResult.Invalid(GameError.InvalidCardSet);
    }
}