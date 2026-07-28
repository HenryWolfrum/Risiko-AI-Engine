using RiskEngine.Rules;
using RiskEngine.Validation;

namespace RiskEngine;

/// <summary>
/// Central validation entry point for all player actions.
/// Ensures that actions are legal in the current phase and game context.
/// </summary>
public static class RuleValidator
{
    /// <summary>
    /// Validates an action against the current game state and active rules.
    /// </summary>
    public static ValidationResult Validate(in GameState state, in GameAction action, GameLayout game)
    {
        // First check if the action type is allowed in the current phase
        var phaseResult = IsActionAllowedInPhase(state.CurrentPhase, action.Type);


        // Illegal actions must never reach the mutation layer
        if (!phaseResult.IsValid)
            return phaseResult;


        // Skip and end actions are phase control commands.
        // They do not require additional rule validation.
        if (action.Type == ActionType.SkipPhase || action.Type == ActionType.EndTurn)
        {
            return ValidationResult.Valid();
        }


        // Validate action according to its specific rule set
        return action.Type switch
        {
            ActionType.Reinforce => ReinforceRules.Validate(in state, in action,game.Map),
            
            ActionType.Attack => AttackRules.Validate(in state, in action, game.Map),

            ActionType.Fortify => FortifyRules.Validate(in state, in action, game.Map),

            ActionType.TurnInCards => TurnInCardsRules.Validate(in state, in action, game.Deck),

           ActionType.Conquer => ConquerRules.Validate(in state, in action,game.Map),
            
            _ => ValidationResult.Invalid(EngineError.InvalidAction)
        };
    }


    /// <summary>
    /// Checks whether an action type is allowed during the current phase.
    /// </summary>
    private static ValidationResult IsActionAllowedInPhase(GamePhase phase, ActionType action)
    {
        return phase switch
        {
            // Player must finish card handling before reinforcement
            GamePhase.CardTurnIn => action switch
            {
                ActionType.TurnInCards => ValidationResult.Valid(),

                _ => ValidationResult.Invalid(EngineError.ActionNotAllowedInPhase)
            },


            // Player must place all reinforcement troops
            GamePhase.Reinforce => action switch
            {
                ActionType.Reinforce => ValidationResult.Valid(),

                _ => ValidationResult.Invalid(EngineError.ActionNotAllowedInPhase)
            },


            // Player may attack multiple times or skip attacking
            GamePhase.Attack => action switch
            {
                ActionType.Attack => ValidationResult.Valid(),

                ActionType.SkipPhase => ValidationResult.Valid(),

                ActionType.EndTurn => ValidationResult.Valid(),

                _ => ValidationResult.Invalid(EngineError.ActionNotAllowedInPhase)
            },


            // Conquer phase is mandatory after a successful conquest
            GamePhase.Conquer => action switch
            { 
                ActionType.Conquer => ValidationResult.Valid(),

                _ => ValidationResult.Invalid(EngineError.ActionNotAllowedInPhase)
            },


            // Fortification is optional
            GamePhase.Fortify => action switch
            {
                ActionType.Fortify => ValidationResult.Valid(),

                ActionType.SkipPhase => ValidationResult.Valid(),

                ActionType.EndTurn => ValidationResult.Valid(),

                _ => ValidationResult.Invalid(EngineError.ActionNotAllowedInPhase)
            },
            
            _ => ValidationResult.Invalid(EngineError.ActionNotAllowedInPhase)
        };
    }
}