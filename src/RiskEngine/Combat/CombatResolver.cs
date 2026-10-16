using RiskEngine.Layout;

namespace RiskEngine.Layout.Combat;

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

     
        RollDice(attackerDice, attackerDiceCount, ref rng);
        RollDice(defenderDice, defenderDiceCount, ref rng);

    
        SortDescending(attackerDice, attackerDiceCount);
        SortDescending(defenderDice, defenderDiceCount);

   
        return ResolveCombat(attackerDice, attackerDiceCount, defenderDice, defenderDiceCount);
    }

   
    private static void RollDice(Span<byte> dice, byte count, ref EngineRandom rng)
    {
        for (byte i = 0; i < count; i++) dice[i] = rng.RollDice();
    }

    
    private static CombatResult ResolveCombat(Span<byte> attackerDice, byte attackerCount, Span<byte> defenderDice, byte defenderCount)
    {
        CombatResult result = default;

        var comparisons = Math.Min(attackerCount, defenderCount);

        for (byte i = 0; i < comparisons; i++)
         
            //The defender wins ties
            if (attackerDice[i] > defenderDice[i])
                result.DefenderLosses++;
         
            else
                result.AttackerLosses++;

        return result;
    }

    /// <summary>
    /// Sorts dice in descending order so the highest dice are compared first.
    /// </summary>
    /// <remarks>
    /// The implementation is specialized for the maximum of three dice per side.
    /// </remarks>
    private static void SortDescending(Span<byte> dice, int count)
    {
        if (count <= 1)
            return;

        if (count == 2)
        {
            if (dice[0] < dice[1])
                Swap(ref dice[0], ref dice[1]);

            return;
        }

        if (count == 3)
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
        for (int i = 1; i < count; i++)
        {
            int j = i;

            while (j > 0 && dice[j - 1] < dice[j])
            {
                Swap(ref dice[j - 1], ref dice[j]);
                j--;
            }
        }
    }
    
    private static void Swap(ref byte a, ref byte b)
    {
        var temp = a;
        a = b;
        b = temp;
    }
}