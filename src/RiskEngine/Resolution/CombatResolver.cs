namespace RiskEngine.Resolution;

public static class CombatResolver
{
    // Resolves one combat round
    public static CombatResult Resolve(in GameState state, in GameAction action, ref EngineRandom rng)
    {
        // Get selected dice count
        byte attackerDiceCount = action.ChosenAttackerDiceCount;
        byte defenderDiceCount = action.ChosenDefenderDiceCount;

        // Allocate dice on the stack
        Span<byte> attackerDice = stackalloc byte[EngineConstants.ATTACKER_DICE_COUNT];
        Span<byte> defenderDice = stackalloc byte[EngineConstants.DEFENDER_DICE_COUNT];

        // Roll dice
        RollDice(attackerDice, attackerDiceCount, ref rng);
        RollDice(defenderDice, defenderDiceCount, ref rng);

        // Highest dice first
        SortDescending(attackerDice, attackerDiceCount);
        SortDescending(defenderDice, defenderDiceCount);

        // Compare dice
        return ResolveCombat(attackerDice, attackerDiceCount, defenderDice, defenderDiceCount);
    }

    // Rolls the requested number of dice
    private static void RollDice(Span<byte> dice, byte count, ref EngineRandom rng)
    {
        for (byte i = 0; i < count; i++)
        {
            dice[i] = rng.RollDice();
        }
    }

    // Compares dice and returns troop losses
    private static CombatResult ResolveCombat(Span<byte> attackerDice, byte attackerCount, Span<byte> defenderDice, byte defenderCount)
    {
        CombatResult result = default;

        byte comparisons = attackerCount;

        //Do exactly defenderCount comparsions
        if (defenderCount < comparisons)
        {
            comparisons = defenderCount;
        }

        for (byte i = 0; i < comparisons; i++)
        {
            //Defender looses if STRICTLY greater
            if (attackerDice[i] > defenderDice[i])
            {
                result.DefenderLosses++;
            }
            //Attacker looses
            else
            {
                result.AttackerLosses++;
            }
        }

        return result;
    }

    // Sorts up to three dice descending by foot
    private static void SortDescending(Span<byte> dice, byte count)
    {
        //Sorting two dices by foot
        if (count >= 2 && dice[0] < dice[1])
        {
            Swap(ref dice[0], ref dice[1]);
        }

        //Sorting for Three Dices by foot
       if (count == 3)
        {
            if (dice[1] < dice[2])
            {
                Swap(ref dice[1], ref dice[2]);
            }

            if (dice[0] < dice[1])
            {
                Swap(ref dice[0], ref dice[1]);
            }
        }
    }

    // Swaps two values
    private static void Swap(ref byte a, ref byte b)
    {
        byte temp = a;
        a = b;
        b = temp;
    }
}