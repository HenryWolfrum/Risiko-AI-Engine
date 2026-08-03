using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using RiskEngine.State;
using RiskEngine.State.Generation;

/// <summary>
/// Represents a lightweight, semantically complete legal decision option.
/// Internally packs option data into 3 raw bytes to eliminate heap allocations.
/// </summary>
public readonly struct DecisionOption
{
    public DecisionKind Kind { get; }

    private readonly byte _value1;
    private readonly byte _value2;
    private readonly byte _value3;
    
    public ParameterSpace Parameter { get; }

    private DecisionOption(DecisionKind kind, byte value1, byte value2, byte value3, ParameterSpace parameter)
    {
        Kind = kind;
        _value1 = value1;
        _value2 = value2;
        _value3 = value3;
        Parameter = parameter;
    }

    #region Factory Methods

    public static DecisionOption CardTurnIn(byte card1, byte card2, byte card3)
    {
        return new DecisionOption(DecisionKind.CardTurnIn, card1, card2, card3, ParameterSpace.None);
    }

    public static DecisionOption Reinforce(byte target, byte minTroops, byte maxTroops)
    {
        return new DecisionOption(DecisionKind.Reinforce, target, EngineConstants.NO_VALUE, EngineConstants.NO_VALUE, new ParameterSpace(minTroops, maxTroops));
    }

    public static DecisionOption Attack(byte source, byte target, byte minDice, byte maxDice)
    {
        return new DecisionOption(DecisionKind.Attack, source, target, EngineConstants.NO_VALUE, new ParameterSpace(minDice, maxDice));
    }

    public static DecisionOption Defend(byte minDice, byte maxDice)
    {
        return new DecisionOption(DecisionKind.Defend, EngineConstants.NO_VALUE, EngineConstants.NO_VALUE, EngineConstants.NO_VALUE, new ParameterSpace(minDice, maxDice));
    }
    
    public static DecisionOption Conquer(byte source, byte target, byte minTroops, byte maxTroops)
    {
        return new DecisionOption(DecisionKind.Conquer, source, target, EngineConstants.NO_VALUE, new ParameterSpace(minTroops, maxTroops));
    }

    public static DecisionOption Fortify(byte source, byte target, byte minTroops, byte maxTroops)
    {
        return new DecisionOption(DecisionKind.Fortify, source, target, EngineConstants.NO_VALUE, new ParameterSpace(minTroops, maxTroops));
    }

    public static DecisionOption SkipPhase()
    {
        return new DecisionOption(
            DecisionKind.SkipPhase,
            value1: EngineConstants.NO_VALUE,
            value2: EngineConstants.NO_VALUE,
            value3: EngineConstants.NO_VALUE,
            parameter: ParameterSpace.None);
    }

    public static DecisionOption EndTurn()
    {
        return new DecisionOption(
            DecisionKind.EndTurn,
            value1: EngineConstants.NO_VALUE,
            value2: EngineConstants.NO_VALUE,
            value3: EngineConstants.NO_VALUE,
            parameter: ParameterSpace.None);
    }

    #endregion

    #region Typed Data Accessors (Zero Allocation Views)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AttackData GetAttackData()
    {
        Debug.Assert(Kind == DecisionKind.Attack, "Invalid DecisionKind for AttackData.");
        return new AttackData(_value1, _value2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConquerData GetConquerData()
    {
        Debug.Assert(Kind == DecisionKind.Conquer, "Invalid DecisionKind for ConquerData.");
        return new ConquerData(_value1, _value2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CardTripleData GetCardTriple()
    {
        Debug.Assert(Kind == DecisionKind.CardTurnIn, "Invalid DecisionKind for CardTriple.");
        return new CardTripleData(_value1, _value2, _value3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReinforceData GetReinforceData()
    {
        Debug.Assert(Kind == DecisionKind.Reinforce, "Invalid DecisionKind for ReinforceData.");
        return new ReinforceData(_value1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FortifyData GetFortifyData()
    {
        Debug.Assert(Kind == DecisionKind.Fortify, "Invalid DecisionKind for FortifyData.");
        return new FortifyData(_value1, _value2);
    }

    #endregion
}