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
                CombatResult result = CombatResolver.Resolve(ref state, in action, ref rng);
                
                AttackMutator.Apply(ref state, in action, ref result);
                break;
            }
            
            //Conquer
            case ActionType.Conquer:
            {
                ConquerMutator.Apply(ref state, in action);
                break;
            }

            //Reinforce
            case ActionType.Reinforce:
            {
                ReinforceMutator.Apply(ref state, in action);
                break;
            }

            //Fortify
            case ActionType.Fortify:
            {
                FortifyMutator.Apply(ref state, in action);
                break;
            }

            //Turn In Cards
            case ActionType.TurnInCards:
            {
                CardTurnInMutator.Apply(ref state, in action);
                break;
            }
        }
    }
}