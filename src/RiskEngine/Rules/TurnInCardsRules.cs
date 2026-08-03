using System.Runtime.CompilerServices;

namespace RiskEngine.State.Rules;

public static class TurnInCardsRules
{
    public static ValidationResult Validate(in GameState state, in GameAction action,DeckLayout deck)
    {
        // Cards out of Range
        if (action.Card1 >= deck.CardCount || action.Card2 >= deck.CardCount ||action.Card3 >= deck.CardCount)
            return ValidationResult.Invalid(EngineError.UnknownCard);

        //Does player own the cards
        var player = state.PlayerTurn;
        if (!CardHelper.PlayerHasCard(in state, player, action.Card1,deck) ||
            !CardHelper.PlayerHasCard(in state, player, action.Card2,deck) ||
            !CardHelper.PlayerHasCard(in state, player, action.Card3,deck))
            return ValidationResult.Invalid(EngineError.CardNotOwned);

        //Get Card Types
        var cardType1 = deck.TerritoryToType[action.Card1];
        var cardType2 = deck.TerritoryToType[action.Card2];
        var cardType3 = deck.TerritoryToType[action.Card3];

        //Check if deck is valid
        if (!IsValidSet(cardType1,cardType2,cardType3))
        {
            return ValidationResult.Invalid(EngineError.InvalidCardSet);
        }
        
        return ValidationResult.Valid();
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidSet(CardType type1, CardType type2, CardType type3)
    {
        if (type1 == CardType.Joker ||
            type2 == CardType.Joker ||
            type3 == CardType.Joker)
            return true;

        if (type1 == type2 &&
            type2 == type3)
            return true;

        return type1 != type2 &&
               type1 != type3 &&
               type2 != type3;
    }
}