using RiskEngine.State;
using RiskEngine.State.Generation;
using RiskEngine.State.Tests.Helpers;

namespace RiskEngine.Tests.Decisions.Generators;

public class AttackOptionGeneratorTests
{
    /*
     * ATTACKGEN-001
     *
     * A single attack opportunity should
     * generate exactly one attack decision
     * and one skip decision.
     *
     * Guarantees:
     * - attack decision is generated
     * - skip decision is generated
     * - no additional decisions exist
     */
    [Fact]
    public void ATTACKGEN_001_ShouldGenerateSingleAttackAndSkip()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .Create()
            .WithTerritories(2)
            .Connect(0, 1)
            .Build();

        var state = GameStateHelper.CreateEmpty(2, 2);

        state.CurrentPhase = GamePhase.Attack;
        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryTroops(ref state, 0, 3);

        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = AttackOptionGenerator.Generate(in state, layout, decisions);

        // Assert
        Assert.Equal(3, count);

        bool foundAttack = false;
        bool foundSkip = false;
        bool foundEndTurn = false;

        for (int i = 0; i < count; i++)
        {
            switch (decisions[i].Kind)
            {
                case DecisionKind.Attack:
                {
                    var attack = decisions[i].GetAttackData();

                    if (attack.SourceTerritory == 0 &&
                        attack.TargetTerritory == 1)
                    {
                        foundAttack = true;
                    }

                    break;
                }

                case DecisionKind.SkipPhase:
                    foundSkip = true;
                    break;
                case DecisionKind.EndTurn:
                    foundEndTurn = true;
                    break;
            }
        }

        Assert.True(foundAttack);
        Assert.True(foundSkip);
        Assert.True(foundEndTurn);
    }
    
    
    /*
     * ATTACKGEN-002
     *
     * A territory with two attackable
     * neighbouring enemies should generate
     * both attack decisions and the end turn decision.
     *
     * Guarantees:
     * - every legal attack is generated
     * - no legal attack is missing
     * - end turn decision is generated
     */
    [Fact]
    public void ATTACKGEN_002_ShouldGenerateAllPossibleAttacks()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .Create()
            .WithTerritories(3)
            .Connect(0, 1)
            .Connect(0, 2)
            .Build();

        var state = GameStateHelper.CreateEmpty(2, 3);

        state.CurrentPhase = GamePhase.Attack;
        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryTroops(ref state, 0, 3);

        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

        GameStateHelper.SetTerritoryOwner(ref state, 2, 1);
        GameStateHelper.SetTerritoryTroops(ref state, 2, 1);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = AttackOptionGenerator.Generate(in state, layout, decisions);

        // Assert
        Assert.Equal(4, count);

        bool foundAttack01 = false;
        bool foundAttack02 = false;
        bool foundSkip = false;
        bool foundEndTurn = false;

        for (int i = 0; i < count; i++)
        {
            switch (decisions[i].Kind)
            {
                case DecisionKind.Attack:
                {
                    var attack = decisions[i].GetAttackData();

                    if (attack.SourceTerritory == 0 && attack.TargetTerritory == 1)
                        foundAttack01 = true;

                    if (attack.SourceTerritory == 0 && attack.TargetTerritory == 2)
                        foundAttack02 = true;

                    break;
                }

                case DecisionKind.SkipPhase:
                    foundSkip = true;
                    break;
                
                case DecisionKind.EndTurn:
                    foundEndTurn = true;
                    break;
            }
        }

        Assert.True(foundAttack01);
        Assert.True(foundAttack02);
        Assert.True(foundSkip);
        Assert.True(foundEndTurn);
        
    }
    
    
    /*
 * ATTACKGEN-003
 *
 * Multiple attacking territories should
 * each contribute their legal attack
 * decisions.
 *
 * Guarantees:
 * - attacks from every owned territory are generated
 * - no attack source is ignored
 * - skip and end turn decisions are generated
 */
[Fact]
public void ATTACKGEN_003_ShouldGenerateAttacksFromMultipleSources()
{
    // Arrange
    var layout = TestLayoutBuilder
        .Create()
        .WithTerritories(4)
        .Connect(0, 1)
        .Connect(2, 3)
        .Connect(0,2)
        .Build();

    var state = GameStateHelper.CreateEmpty(2, 4);

    state.CurrentPhase = GamePhase.Attack;
    state.PlayerTurn = 0;

    GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 0, 3);

    GameStateHelper.SetTerritoryOwner(ref state, 1, 1);
    GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

    GameStateHelper.SetTerritoryOwner(ref state, 2, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 2, 3);

    GameStateHelper.SetTerritoryOwner(ref state, 3, 1);
    GameStateHelper.SetTerritoryTroops(ref state, 3, 1);

    Span<DecisionOption> decisions = stackalloc DecisionOption[8];

    // Act
    int count = AttackOptionGenerator.Generate(in state, layout, decisions);

    // Assert
    Assert.Equal(4, count);

    bool foundAttack01 = false;
    bool foundAttack23 = false;
    bool foundSkip = false;
    bool foundEndTurn = false;

    for (int i = 0; i < count; i++)
    {
        switch (decisions[i].Kind)
        {
            case DecisionKind.Attack:
            {
                var attack = decisions[i].GetAttackData();

                if (attack.SourceTerritory == 0 &&
                    attack.TargetTerritory == 1)
                {
                    foundAttack01 = true;
                }

                if (attack.SourceTerritory == 2 &&
                    attack.TargetTerritory == 3)
                {
                    foundAttack23 = true;
                }

                break;
            }

            case DecisionKind.SkipPhase:
                foundSkip = true;
                break;

            case DecisionKind.EndTurn:
                foundEndTurn = true;
                break;
        }
    }

    Assert.True(foundAttack01);
    Assert.True(foundAttack23);
    Assert.True(foundSkip);
    Assert.True(foundEndTurn);
}


/*
 * ATTACKGEN-004
 *
 * A player without any legal attack
 * should only receive phase control
 * decisions.
 *
 * Guarantees:
 * - no attack decisions are generated
 * - skip decision is generated
 * - end turn decision is generated
 */
[Fact]
public void ATTACKGEN_004_ShouldGenerateNoAttacksWhenNoneArePossible()
{
    // Arrange
    var layout = TestLayoutBuilder
        .Create()
        .WithTerritories(2)
        .Connect(0, 1)
        .Build();

    var state = GameStateHelper.CreateEmpty(2, 2);

    state.CurrentPhase = GamePhase.Attack;
    state.PlayerTurn = 0;

    GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

    GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 1, 3);

    Span<DecisionOption> decisions = stackalloc DecisionOption[8];

    // Act
    int count = AttackOptionGenerator.Generate(in state, layout, decisions);

    // Assert
    Assert.Equal(2, count);

    bool foundAttack = false;
    bool foundSkip = false;
    bool foundEndTurn = false;

    for (int i = 0; i < count; i++)
    {
        switch (decisions[i].Kind)
        {
            case DecisionKind.Attack:
                foundAttack = true;
                break;

            case DecisionKind.SkipPhase:
                foundSkip = true;
                break;

            case DecisionKind.EndTurn:
                foundEndTurn = true;
                break;
        }
    }

    Assert.False(foundAttack);
    Assert.True(foundSkip);
    Assert.True(foundEndTurn);
}

/*
 * ATTACKGEN-005
 *
 * Friendly neighbouring territories
 * should never generate attack
 * decisions.
 *
 * Guarantees:
 * - only enemy territories are attack targets
 * - friendly territories are ignored
 */
[Fact]
public void ATTACKGEN_005_ShouldIgnoreFriendlyNeighbours()
{
    // Arrange
    var layout = TestLayoutBuilder
        .Create()
        .WithTerritories(3)
        .Connect(0, 1)
        .Connect(0, 2)
        .Build();

    var state = GameStateHelper.CreateEmpty(2, 3);

    state.CurrentPhase = GamePhase.Attack;
    state.PlayerTurn = 0;

    GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 0, 3);

    // Friendly neighbour
    GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

    // Enemy neighbour
    GameStateHelper.SetTerritoryOwner(ref state, 2, 1);
    GameStateHelper.SetTerritoryTroops(ref state, 2, 1);

    Span<DecisionOption> decisions = stackalloc DecisionOption[8];

    // Act
    int count = AttackOptionGenerator.Generate(in state, layout, decisions);

    // Assert
    Assert.Equal(3, count);

    bool foundFriendlyAttack = false;
    bool foundEnemyAttack = false;

    for (int i = 0; i < count; i++)
    {
        if (decisions[i].Kind != DecisionKind.Attack)
            continue;

        var attack = decisions[i].GetAttackData();

        if (attack.SourceTerritory == 0 &&
            attack.TargetTerritory == 1)
        {
            foundFriendlyAttack = true;
        }

        if (attack.SourceTerritory == 0 &&
            attack.TargetTerritory == 2)
        {
            foundEnemyAttack = true;
        }
    }

    Assert.False(foundFriendlyAttack);
    Assert.True(foundEnemyAttack);
}
    
/*
 * ATTACKGEN-006
 *
 * Territories with only one troop
 * should not generate attack
 * decisions.
 *
 * Guarantees:
 * - attacks require at least two troops
 * - phase control decisions remain available
 */
[Fact]
public void ATTACKGEN_006_ShouldNotGenerateAttackWithSingleTroop()
{
    // Arrange
    var layout = TestLayoutBuilder
        .Create()
        .WithTerritories(2)
        .Connect(0, 1)
        .Build();

    var state = GameStateHelper.CreateEmpty(2, 2);

    state.CurrentPhase = GamePhase.Attack;
    state.PlayerTurn = 0;

    GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 0, 1);

    GameStateHelper.SetTerritoryOwner(ref state, 1, 1);
    GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

    Span<DecisionOption> decisions = stackalloc DecisionOption[8];

    // Act
    int count = AttackOptionGenerator.Generate(in state, layout, decisions);

    // Assert
    Assert.Equal(2, count);

    bool foundAttack = false;
    bool foundSkip = false;
    bool foundEndTurn = false;

    for (int i = 0; i < count; i++)
    {
        switch (decisions[i].Kind)
        {
            case DecisionKind.Attack:
                foundAttack = true;
                break;

            case DecisionKind.SkipPhase:
                foundSkip = true;
                break;

            case DecisionKind.EndTurn:
                foundEndTurn = true;
                break;
        }
    }

    Assert.False(foundAttack);
    Assert.True(foundSkip);
    Assert.True(foundEndTurn);
}

/*
 * ATTACKGEN-007
 *
 * Every legal attack should be
 * generated exactly once.
 *
 * Guarantees:
 * - no duplicate attack decisions exist
 */
[Fact]
public void ATTACKGEN_007_ShouldNotGenerateDuplicateAttacks()
{
    // Arrange
    var layout = TestLayoutBuilder
        .Create()
        .WithTerritories(3)
        .Connect(0, 1)
        .Connect(0, 2)
        .Build();

    var state = GameStateHelper.CreateEmpty(2, 3);

    state.CurrentPhase = GamePhase.Attack;
    state.PlayerTurn = 0;

    GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 0, 5);

    GameStateHelper.SetTerritoryOwner(ref state, 1, 1);
    GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

    GameStateHelper.SetTerritoryOwner(ref state, 2, 1);
    GameStateHelper.SetTerritoryTroops(ref state, 2, 1);

    Span<DecisionOption> decisions = stackalloc DecisionOption[8];

    // Act
    int count = AttackOptionGenerator.Generate(in state, layout, decisions);

    // Assert
    int attack01 = 0;
    int attack02 = 0;

    for (int i = 0; i < count; i++)
    {
        if (decisions[i].Kind != DecisionKind.Attack)
            continue;

        var attack = decisions[i].GetAttackData();

        if (attack.SourceTerritory == 0 &&
            attack.TargetTerritory == 1)
        {
            attack01++;
        }

        if (attack.SourceTerritory == 0 &&
            attack.TargetTerritory == 2)
        {
            attack02++;
        }
    }

    Assert.Equal(1, attack01);
    Assert.Equal(1, attack02);
}

}