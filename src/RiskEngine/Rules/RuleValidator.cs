using RiskEngine.Validation;

namespace RiskEngine;

public static class RuleValidator
{
    //Check if action is legal in state context
    public static ValidationResult Validate(in GameState state, in GameAction action, GameLayout game)
    {
        ValidationResult phaseResult = IsActionAllowedInPhase(state.CurrentPhase, action.Type);

        //Illegal actions in context must not be executed
        if (!phaseResult.IsValid)
        {
            return phaseResult;
        }

        //Check Action according to context rules
        switch (action.Type)
        {
            case ActionType.Reinforce:
                return ReinforceRules.Validate(state, action);

            case ActionType.Attack:
                return AttackRules.Validate(state, action,game.Map);

            case ActionType.Fortify:
                return FortifyRules.Validate(state, action, game.Map);

            case ActionType.TurnInCards:
                return ValidationResult.Valid();

            case ActionType.EndTurn:
                return ValidationResult.Valid();

            default:
                return ValidationResult.Invalid(GameError.InvalidAction);
        }
    }


    //Action must be compatible with Phase
    private static ValidationResult IsActionAllowedInPhase(GamePhase phase, ActionType action)
    {
        switch (phase)
        {
            case GamePhase.Reinforce:

                if (action == ActionType.Reinforce)
                    return ValidationResult.Valid();

                if (action == ActionType.TurnInCards)
                    return ValidationResult.Valid();

                if (action == ActionType.EndTurn)
                    return ValidationResult.Valid();

                return ValidationResult.Invalid(GameError.ActionNotAllowedInPhase);


            case GamePhase.Attack:

                if (action == ActionType.Attack)
                    return ValidationResult.Valid();

                if (action == ActionType.EndTurn)
                    return ValidationResult.Valid();

                return ValidationResult.Invalid(GameError.ActionNotAllowedInPhase);


            case GamePhase.Fortify:

                if (action == ActionType.Fortify)
                    return ValidationResult.Valid();

                if (action == ActionType.EndTurn)
                    return ValidationResult.Valid();

                return ValidationResult.Invalid(GameError.ActionNotAllowedInPhase);


            default:

                return ValidationResult.Invalid(GameError.ActionNotAllowedInPhase);
        }
    }
}