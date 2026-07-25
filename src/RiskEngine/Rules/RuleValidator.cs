namespace RiskEngine;

public static class RuleValidator
{
    public static bool Validate(
        in GameState state,
        in GameAction action)
    {
        if (!IsActionAllowedInPhase(state.CurrentPhase, action.Type))
        {
            return false;
        }

        switch (action.Type)
        {
            case ActionType.PlaceTroops:
                // return ValidatePlaceTroops(state, action);

            case ActionType.Attack:
                // return ValidateAttack(state, action);

            case ActionType.Fortify:
                // return ValidateFortify(state, action);

            case ActionType.TurnInCards:
                // return ValidateTradeCards(state, action);

            case ActionType.EndTurn:
              //  return ValidateEndTurn(state, action);

            default:
                return false;
        }
    }


    //Action must be compatible with Phase
    private static bool IsActionAllowedInPhase(GamePhase phase, ActionType action)
    {
        switch (phase)
        {
            //Reinforce Phase
            case GamePhase.Reinforce:

                if (action == ActionType.PlaceTroops)
                    return true;

                if (action == ActionType.TurnInCards)
                    return true;

                if (action == ActionType.EndTurn)
                    return true;

                return false;

            //Attack Phase
            case GamePhase.Attack:

                if (action == ActionType.Attack)
                    return true;

                if (action == ActionType.EndTurn)
                    return true;

                return false;

            //Fortify Phase
            case GamePhase.Fortify:

                if (action == ActionType.Fortify)
                    return true;

                if (action == ActionType.EndTurn)
                    return true;

                return false;


            default:
                return false;
        }
    }
}