using RiskEngine.Actions;
using RiskEngine.Layout;
using RiskEngine.Observer;
using RiskEngine.Player;
using RiskEngine.Random;
using RiskEngine.State;

namespace RiskEngine.Execution;

/// <summary>
///     Executes the complete turn sequence for a single player.
///     This class only coordinates phases and does not contain rule logic.
/// </summary>
public static class TurnExecutor
{
    /// <summary>
    ///     Executes all phases of the current player's turn.
    /// </summary>
    public static void ExecuteTurn(ref GameState state, IRiskPlayer[] players, GameLayout layout, ref EngineRandom rng, IGameObserver? observer = null)
    {
        // Get the player instance for the current turn.
        byte playerOnTurn = GameStateHelper.GetPlayerOnTurn(in state);
        var currentPlayer = players[playerOnTurn];


        // 1. Card trade-in phase.
        CardTurnInExecutor.Execute(ref state, currentPlayer, layout, ref rng, observer);


        // 2. Reinforcement phase.
        ReinforceExecutor.Execute(ref state, currentPlayer, layout, ref rng, observer);


        // 3. Attack phase.
        AttackExecutor.Execute(ref state, players, layout, ref rng, observer);


        // Game could already be decided during attack/conquest.
        if (state.CurrentPhase == GamePhase.Terminated) return;


        // 4. Fortification phase.
        FortifyExecutor.Execute(ref state, currentPlayer, layout, ref rng, observer);
    }
}