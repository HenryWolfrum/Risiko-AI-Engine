using RiskEngine;
using RiskEngine.Mutation;
using RiskEngine.Resolution;

// Mutates the game state based on an action
public static class GameStateMutator
{
    public static void Apply(ref GameState state, in GameAction action, ref EngineRandom rng)
    {
        switch (action.Type)
        {
            //Attack
            case ActionType.Attack:
            {
                CombatResult result = CombatResolver.Resolve(state, action, ref rng);
                
                AttackMutator.Apply(ref state, action, result);
                break;
            }

            //Reinforce
            case ActionType.PlaceTroops:
            {
                ReinforceMutator.Apply(ref state, action);
                break;
            }

            //Fortify
            case ActionType.Fortify:
            {
                FortifyMutator.Apply(ref state, action);
                break;
            }

            //Turn In Cards
            case ActionType.TurnInCards:
            {
                CardTurnInMutator.Apply(ref state, action);
                break;
            }
        }
    }
}