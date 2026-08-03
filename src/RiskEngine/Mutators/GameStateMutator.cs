using RiskEngine.State.Resolution;

namespace RiskEngine.State.Mutation;

/// <summary>
/// Central mutation dispatcher.
/// Applies validated game actions to the game state.
/// </summary>
public static class GameStateMutator
{
    /// <summary>
    /// Applies an action and updates the game state.
    /// Validation must happen before calling this method.
    /// </summary>
    public static void Apply(ref GameState state, in GameAction action, ref EngineRandom rng,GameLayout layout)
    {
        switch (action.Type)
        {
            case ActionType.Attack:
            {
                // Resolve combat first, then apply troop losses.
                var result = CombatResolver.Resolve(in state, in action, ref rng);

                AttackMutator.Apply(ref state, in action, in result);
                break;
            }


            case ActionType.Conquer:
            {
                // Transfer ownership and move troops.
                ConquerMutator.Apply(ref state, in action);
                break;
            }


            case ActionType.Reinforce:
            {
                // Place reinforcement troops.
                ReinforceMutator.Apply(ref state, in action);
                break;
            }


            case ActionType.Fortify:
            {
                // Move troops between connected friendly territories.
                FortifyMutator.Apply(ref state, in action);
                break;
            }


            case ActionType.TurnInCards:
            {
                // Exchange a valid card set for reinforcements.
                CardTurnInMutator.Apply(ref state, in action,layout.Deck); 
                break;
            }


            default:
            {
                throw new InvalidOperationException($"Unsupported action type: {action.Type}");
            }
        }
    }
    
    
}

