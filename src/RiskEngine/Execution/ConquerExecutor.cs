using RiskEngine.Mutation;

namespace RiskEngine.Execution;

/// <summary>
/// Executes the conquest phase after a successful attack.
/// Handles troop movement into conquered territory and player elimination.
/// </summary>
public static class ConquerExecutor
{
    /// <summary>
    /// Executes the conquest phase.
    /// The attacker moves troops from the attacking territory
    /// into the newly conquered territory.
    /// </summary>
    public static void Execute(ref GameState state, IRiskPlayer attacker, byte defenderId, in GameAction attackAction, GameLayout layout, ref EngineRandom rng)
    {
        state.CurrentPhase = GamePhase.Conquer;


        // Ask attacker how many troops should move
        // into the conquered territory.
        var conquerAction = attacker.DecideAction(in state, GamePhase.Conquer, layout);


        // Ensure source and target are always the conquered
        // territory pair from the previous attack.
        conquerAction.Type = ActionType.Conquer;
        conquerAction.SourceTerritory = attackAction.SourceTerritory;

        conquerAction.TargetTerritory = attackAction.TargetTerritory;


        // Validate conquest action through central rule system.
        var validation = RuleValidator.Validate(in state, in conquerAction, layout);


        if (!validation.IsValid)
        {
            // Safe fallback:
            // Move one troop into the conquered territory.
            conquerAction = CreateFallbackConquerAction(in attackAction);
        }
        
        // Apply troop movement and ownership transfer.
        GameStateMutator.Apply(ref state, in conquerAction, ref rng);


        // Check whether defender lost their final territory.
        CheckPlayerElimination(ref state, defenderId);
    }


    /// <summary>
    /// Creates a safe fallback conquest action.
    /// Moves only one troop into the conquered territory.
    /// </summary>
    private static GameAction CreateFallbackConquerAction(in GameAction attackAction)
    {
        return new GameAction
        {
            Type = ActionType.Conquer,

            SourceTerritory = attackAction.SourceTerritory,

            TargetTerritory = attackAction.TargetTerritory,

            ConquerTroopCount = 1
        };
    }
    
    /// <summary>
    /// Removes eliminated players and transfers their cards.
    /// </summary>
    private static void CheckPlayerElimination(ref GameState state, byte defenderId)
    {
        // Defender still owns territories.
        if (GameStateHelper.GetOwnedTerritoryCount(in state, defenderId) > 0)
        {
            return;
        }
        
        var attackerId = state.PlayerTurn;
        
        // Transfer cards and remove defender from active players.
        GameStateHelper.EliminateAndTransferCards(ref state, attackerId, defenderId);
    }
}