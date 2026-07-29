using RiskEngine.Mission;

namespace RiskEngine.State;

/// <summary>
/// Main game loop controller.
/// Responsible only for global game flow.
/// </summary>
public static class GameRunner
{
    public static GameState PlayGame(GameLayout layout, IRiskPlayer[] players, int seed)
    {
        // Create deterministic random generator
        var rng = new EngineRandom(seed);


        // Create initial game state
        var state = GameInitializer.CreateInitialState(layout, ref rng);


        // Main game loop
        while (state.CurrentRound <= layout.Config.MaxRounds && state.CurrentPhase!=GamePhase.Terminated)
        {
            var currentPlayer = state.PlayerTurn;


            // Skip eliminated players
            if (!GameStateHelper.IsPlayerAlive(in state, currentPlayer))
            {
                AdvanceToNextTurn(ref state, layout.Config.PlayerCount);

                continue;
            }


            // Execute complete player turn
            Execution.TurnExecutor.ExecuteTurn(ref state, players, layout, ref rng);


            // Check victory condition
            if (HasPlayerWon(in state, in layout, currentPlayer))
            {
                GameStateHelper.Terminate(ref state, currentPlayer);
                return state;
            }


            // Move to next player
            AdvanceToNextTurn(ref state, layout.Config.PlayerCount);
        }


        // Return final state if maximum rounds are reached
        GameStateHelper.Terminate(ref state, EngineConstants.NO_VALUE);
        return state;
    }


    /// <summary>
    /// Advances the turn counter and updates round number.
    /// </summary>
    private static void AdvanceToNextTurn(ref GameState state, byte playerCount)
    {
        // Move to next player
        state.PlayerTurn = (byte)((state.PlayerTurn + 1) % playerCount);


        // New round starts after the last player
        if (state.PlayerTurn == 0)
        {
            state.CurrentRound++;
        }
    }


    /// <summary>
    /// Checks whether only one active player remains.
    /// </summary>
    private static bool HasPlayerWon(in GameState state,in GameLayout layout, byte player)
    {
        return MissionEvaluator.IsFulfilled(state,layout,player);
    }
}