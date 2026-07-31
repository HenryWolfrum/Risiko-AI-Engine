using RiskEngine.State;

namespace RiskEngine.Decisions;

public static class ActionFactoryHelper
{
    public static GameAction TradeCards(byte card1, byte card2, byte card3)
    {
        return new GameAction()
        {
            Type = ActionType.TurnInCards,
            Card1 = card1,
            Card2 = card2,
            Card3 = card3
        };
    }

    public static GameAction Reinforce(byte target, byte amount)
    {
        return new GameAction()
        {
            Type = ActionType.Reinforce,
            TargetTerritory = target,
            TroopCount = amount
        };
    }

    public static GameAction Attack(byte source, byte target, byte diceCount)
    {
        return new GameAction()
        {
            Type = ActionType.Attack,
            SourceTerritory = source,
            TargetTerritory = target,
            ChosenAttackerDiceCount = diceCount
        };
    }

    public static GameAction Defend(byte diceCount)
    {
        return new GameAction()
        {
            Type = ActionType.Defend,
            ChosenDefenderDiceCount = diceCount
        };
    }

    public static GameAction Conquer(byte amount)
    {
        return new GameAction()
        {
            Type = ActionType.Conquer,
            TroopCount = amount
        };
    }
    
    public static GameAction Fortify(byte source, byte target, byte amount)
    {
        return new GameAction()
        {
            Type = ActionType.Fortify,
            SourceTerritory = source,
            TargetTerritory = target,
            TroopCount = amount
        };
    }

    public static GameAction Skip()
    {
        return new GameAction()
        {
            Type = ActionType.SkipPhase
        };
    }
    
    public static GameAction EndTurn()
    {
        return new GameAction()
        {
            Type = ActionType.EndTurn
        };
    }
}