namespace RiskEngine;

public static unsafe class AttackMutator
{
    // Applies combat result to the game state
    public static void Apply(ref GameState state, in GameAction action, in CombatResult result)
    {
        byte attackerTerritory = action.SourceTerritory;
        byte defenderTerritory = action.TargetTerritory;

        ApplyLosses(ref state, attackerTerritory, defenderTerritory, result);

        // Check if defender lost the territory
        if (state.TerritoryTroops[defenderTerritory] == 0)
        {
            ConquerTerritory(ref state, action);
        }
    }


    // Removes lost troops from both territories
    private static void ApplyLosses(ref GameState state, byte attackerTerritory, byte defenderTerritory, in CombatResult result)
    {
        state.TerritoryTroops[attackerTerritory] -= result.AttackerLosses;

        byte defenderTroops = state.TerritoryTroops[defenderTerritory];

        //Defensive extra check to stop byte underflow
        if (result.DefenderLosses >= defenderTroops)
        {
            state.TerritoryTroops[defenderTerritory] = 0;
        }
        else
        {
            state.TerritoryTroops[defenderTerritory] = (byte)(defenderTroops - result.DefenderLosses);
        }
    }


    // Transfers territory ownership after conquest
    private static void ConquerTerritory(ref GameState state, in GameAction action)
    {
        byte attacker = state.PlayerTurn;
        byte target = action.TargetTerritory;

        state.TerritoryOwners[target] = attacker;
        state.TerritoryTroops[target] = action.ConquerTroopCount;
    }
}