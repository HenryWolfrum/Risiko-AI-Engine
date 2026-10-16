using RiskEngine.Layout;

namespace RiskEngine.State;

public static class AttackHelper
{
    public static bool CanPlayerAttack(in GameState state, byte player, MapLayout map)
    {
        //Foreach territory
        for (var territory = 0; territory < map.TerritoryCount; territory++)
        {
            //Territory doesnt belong to attacker
            if (GameStateHelper.GetTerritoryOwner(state, territory) != player)
                continue;

            //Territory has at most one Troop
            if (GameStateHelper.GetTerritoryTroops(state, territory) <= 1)
                continue;

            //Get Neighbor for attack Territroy
            var neighbours = map.Adjacencies[territory];

            //Check each neighbor
            for (var i = 0; i < neighbours.Length; i++)
                //Is neighbor enemy territory?
                if (GameStateHelper.GetTerritoryOwner(state, neighbours[i]) != player)
                    return true;
        }

        return false;
    }
}