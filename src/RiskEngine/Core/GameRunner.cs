using RiskEngine.Actions;
using RiskEngine.Constants;
using RiskEngine.Execution;
using RiskEngine.Layout;
using RiskEngine.Mission;
using RiskEngine.Observer;
using RiskEngine.Player;
using RiskEngine.Random;
using RiskEngine.State;

namespace RiskEngine.Core;

/// <summary>
/// Main game loop controller.
/// Responsible only for global game flow.
/// </summary>
public static class GameRunner
{
    public static GameState PlayGame(GameLayout layout, IRiskPlayer[] players, ulong seed, IGameObserver? observer = null)
    {
        // Create deterministic random generator.
        var rng = new EngineRandom(seed);

        // Create initial game state.
        var state = GameInitializer.CreateInitialState(layout, ref rng);

        observer?.OnGameStarted(in state);

        // Main game loop.
        while (state.CurrentRound < layout.Config.MaxRounds && state.CurrentPhase != GamePhase.Terminated)
        {
            var currentPlayer = state.PlayerTurn;

            // Skip eliminated players.
            if (!GameStateHelper.IsPlayerAlive(in state, currentPlayer))
            {
                // Only one player remains.
                if (GameStateHelper.GetActivePlayerCount(in state) <= 1)
                {
                    var winner = EngineConstants.NO_VALUE;

                    for (byte p = 0; p < layout.Config.PlayerCount; p++)
                    {
                        if (!GameStateHelper.IsPlayerAlive(in state, p))
                            continue;

                        winner = p;
                        break;
                    }

                    return TerminateGame(ref state, observer, winner);
                }

                AdvanceToNextTurn(ref state, layout.Config.PlayerCount);

                continue;
            }

            // Execute the current player's turn.
            TurnExecutor.ExecuteTurn(ref state, players, layout, ref rng, observer);
            
            // Check victory condition.
            if (HasPlayerWon(in state, in layout, currentPlayer))
                return TerminateGame(ref state, observer, currentPlayer);

            // Move to the next player.
            AdvanceToNextTurn(ref state, layout.Config.PlayerCount);
        }

        // Game ended because the round limit was reached
        // or the state was already marked as terminated.
        return TerminateGame(ref state, observer);
    }

    /// <summary>
    /// Advances the turn counter and updates the round number.
    /// </summary>
    private static void AdvanceToNextTurn(ref GameState state, byte playerCount)
    {
        state.PlayerTurn = (byte)((state.PlayerTurn + 1) % playerCount);

        // A new round starts after the last player.
        if (state.PlayerTurn == 0)
            state.CurrentRound++;
    }

    /// <summary>
    /// Checks whether the current player's mission is fulfilled.
    /// </summary>
    private static bool HasPlayerWon(in GameState state, in GameLayout layout, byte player)
    {
        return MissionEvaluator.IsFulfilled(state, layout, player);
    }

    /// <summary>
    /// Terminates the game and notifies observers once.
    /// </summary>
    private static GameState TerminateGame(ref GameState state, IGameObserver? observer = null, byte winnerId = EngineConstants.NO_VALUE)
    {
        GameStateHelper.Terminate(ref state, winnerId);
        observer?.OnGameFinished(in state);
        

        return state;
    }
}
