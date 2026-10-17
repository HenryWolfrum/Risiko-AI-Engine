
using RiskEngine.Actions;
using RiskEngine.Constants;
using RiskEngine.Random;

namespace RiskEngine.Combat;

/// <summary>
/// Resolves a combat round by rolling and comparing attacker and defender dice.
/// </summary>
/// <remarks>
/// Expects validated dice counts within the limits defined by the combat rules.
/// </remarks>
public static class CombatResolver
{
    public static CombatResult Resolve(in GameAction action, ref EngineRandom rng)
    {
        var attackerDiceCount = action.ChosenAttackerDiceCount;
        var defenderDiceCount = action.ChosenDefenderDiceCount;

        Span<byte> attackerDice = stackalloc byte[EngineConstants.MAX_ATTACKER_DICE];
        Span<byte> defenderDice = stackalloc byte[EngineConstants.MAX_DEFENDER_DICE];

        RollDice(attackerDice[..attackerDiceCount], ref rng);
        RollDice(defenderDice[..defenderDiceCount], ref rng);

        return ResolveRolls(attackerDice[..attackerDiceCount], defenderDice[..defenderDiceCount]);
    }

    /// <summary>
    /// Rolls the specified number of dice into the provided buffer.
    /// </summary>
    private static void RollDice(Span<byte> dice, ref EngineRandom rng)
    {
        for (var i = 0; i < dice.Length; i++)
            dice[i] = rng.RollDice();
    }

    /// <summary>
    /// Sorts the dice in descending order and resolves the combat.
    /// The attacker wins a comparison only when their die is strictly higher.
    /// The defender wins ties.
    /// </summary>
    internal static CombatResult ResolveRolls(Span<byte> attackerDice, Span<byte> defenderDice)
    {
        SortDescending(attackerDice);
        SortDescending(defenderDice);

        CombatResult result = default;
        var comparisons = Math.Min(attackerDice.Length, defenderDice.Length);

        for (var i = 0; i < comparisons; i++)
        {
            if (attackerDice[i] > defenderDice[i])
                result.DefenderLosses++;
            else
                result.AttackerLosses++;
        }

        return result;
    }

    /// <summary>
    /// Sorts dice in descending order.
    /// Specialized for the usual maximum of three dice per side.
    /// </summary>
    private static void SortDescending(Span<byte> dice)
    {
        if (dice.Length <= 1)
            return;

        if (dice.Length == 2)
        {
            if (dice[0] < dice[1])
                Swap(ref dice[0], ref dice[1]);

            return;
        }

        if (dice.Length == 3)
        {
            if (dice[0] < dice[1])
                Swap(ref dice[0], ref dice[1]);

            if (dice[1] < dice[2])
                Swap(ref dice[1], ref dice[2]);

            if (dice[0] < dice[1])
                Swap(ref dice[0], ref dice[1]);

            return;
        }

        // Fallback for larger dice counts.
        for (var i = 1; i < dice.Length; i++)
        {
            var j = i;

            while (j > 0 && dice[j - 1] < dice[j])
            {
                Swap(ref dice[j - 1], ref dice[j]);
                j--;
            }
        }
    }

    private static void Swap(ref byte a, ref byte b)
    {
        (a, b) = (b, a);
    }
}
