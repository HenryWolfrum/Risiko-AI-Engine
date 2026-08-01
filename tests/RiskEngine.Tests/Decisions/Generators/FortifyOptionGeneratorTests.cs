using RiskEngine.State;
using RiskEngine.State.Generation;
using RiskEngine.State.Tests.Helpers;

namespace RiskEngine.Tests.Decisions.Generators;

public class FortifyOptionGeneratorTests
{
    /*
     * FORTIFYGEN-001
     *
     * Two connected owned territories
     * should generate one fortify
     * decision and one end turn decision.
     *
     * Guarantees:
     * - connected territories can fortify
     * - end turn decision is generated
     */
    [Fact]
    public void FORTIFYGEN_001_ShouldGenerateSingleFortifyOption()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .Create()
            .WithTerritories(2)
            .Connect(0, 1)
            .Build();

        var state = GameStateHelper.CreateEmpty(2, 2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryTroops(ref state, 0, 3);

        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = FortifyOptionGenerator.Generate(in state, layout, decisions);

        // Assert
        Assert.Equal(2, count);

        bool foundFortify = false;
        bool foundEndTurn = false;

        for (int i = 0; i < count; i++)
        {
            switch (decisions[i].Kind)
            {
                case DecisionKind.Fortify:
                {
                    var fortify = decisions[i].GetFortifyData();

                    if (fortify.SourceTerritory == 0 &&
                        fortify.TargetTerritory == 1)
                    {
                        foundFortify = true;

                        Assert.Equal(1, decisions[i].Parameter.Min);
                        Assert.Equal(2, decisions[i].Parameter.Max);
                    }

                    break;
                }

                case DecisionKind.EndTurn:
                    foundEndTurn = true;
                    break;
            }
        }

        Assert.True(foundFortify);
        Assert.True(foundEndTurn);
    }
    
    
 /*
 * FORTIFYGEN-002
 *
 * Enemy territories should split
 * owned territory clusters and
 * prevent fortification between them.
 *
 * Guarantees:
 * - fortify is only possible through owned paths
 * - enemy territories break connectivity
 */
[Fact]
public void FORTIFYGEN_002_ShouldNotFortifyAcrossEnemyTerritory()
{
    // Arrange
    var layout = TestLayoutBuilder
        .Create()
        .WithTerritories(4)
        .Connect(0, 1)
        .Connect(1, 2)
        .Connect(2, 3)
        .Build();

    var state = GameStateHelper.CreateEmpty(2, 4);

    state.PlayerTurn = 0;

    // Player 0
    GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 0, 3);

    GameStateHelper.SetTerritoryOwner(ref state, 2, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 2, 3);

    GameStateHelper.SetTerritoryOwner(ref state, 3, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 3, 1);

    // Enemy territory splits the graph
    GameStateHelper.SetTerritoryOwner(ref state, 1, 1);
    GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

    Span<DecisionOption> decisions = stackalloc DecisionOption[16];

    // Act
    int count = FortifyOptionGenerator.Generate(in state, layout, decisions);

    // Assert
    Assert.Equal(2, count);

    bool found23 = false;
    bool foundInvalid = false;
    bool foundEndTurn = false;

    for (int i = 0; i < count; i++)
    {
        switch (decisions[i].Kind)
        {
            case DecisionKind.Fortify:
            {
                var fortify = decisions[i].GetFortifyData();

                if (fortify.SourceTerritory == 2 &&
                    fortify.TargetTerritory == 3)
                {
                    found23 = true;
                }

                if (fortify.SourceTerritory == 0 ||
                    fortify.TargetTerritory == 0)
                {
                    foundInvalid = true;
                }

                break;
            }

            case DecisionKind.EndTurn:
                foundEndTurn = true;
                break;
        }
    }

    Assert.True(found23);
    Assert.False(foundInvalid);
    Assert.True(foundEndTurn);
}


/*
 * FORTIFYGEN-003
 *
 * A source territory should
 * generate fortify decisions
 * to every reachable territory
 * in its connected cluster.
 *
 * Guarantees:
 * - all reachable targets are generated
 * - no reachable target is missing
 */
[Fact]
public void FORTIFYGEN_003_ShouldGenerateAllReachableTargets()
{
    // Arrange
    var layout = TestLayoutBuilder
        .Create()
        .WithTerritories(3)
        .Connect(0, 1)
        .Connect(1, 2)
        .Build();

    var state = GameStateHelper.CreateEmpty(2, 3);

    state.PlayerTurn = 0;

    GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 0, 3);

    GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 1, 1);

    GameStateHelper.SetTerritoryOwner(ref state, 2, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 2, 1);

    Span<DecisionOption> decisions = stackalloc DecisionOption[8];

    // Act
    int count = FortifyOptionGenerator.Generate(in state, layout, decisions);

    // Assert
    Assert.Equal(3, count);

    bool found01 = false;
    bool found02 = false;
    bool foundEndTurn = false;

    for (int i = 0; i < count; i++)
    {
        switch (decisions[i].Kind)
        {
            case DecisionKind.Fortify:
            {
                var fortify = decisions[i].GetFortifyData();

                if (fortify.SourceTerritory == 0 &&
                    fortify.TargetTerritory == 1)
                {
                    found01 = true;
                }

                if (fortify.SourceTerritory == 0 &&
                    fortify.TargetTerritory == 2)
                {
                    found02 = true;
                }

                break;
            }

            case DecisionKind.EndTurn:
                foundEndTurn = true;
                break;
        }
    }

    Assert.True(found01);
    Assert.True(found02);
    Assert.True(foundEndTurn);
}

/*
 * FORTIFYGEN-004
 *
 * Territories with only one troop
 * should not generate fortify
 * source decisions.
 *
 * Guarantees:
 * - fortify requires at least two troops
 * - territories with one troop are never sources
 */
[Fact]
public void FORTIFYGEN_004_ShouldIgnoreSourcesWithSingleTroop()
{
    // Arrange
    var layout = TestLayoutBuilder
        .Create()
        .WithTerritories(2)
        .Connect(0, 1)
        .Build();

    var state = GameStateHelper.CreateEmpty(2, 2);

    state.PlayerTurn = 0;

    GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 0, 1);

    GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
    GameStateHelper.SetTerritoryTroops(ref state, 1, 5);

    Span<DecisionOption> decisions = stackalloc DecisionOption[8];

    // Act
    int count = FortifyOptionGenerator.Generate(in state, layout, decisions);

    // Assert
    Assert.Equal(2, count);

    bool foundInvalid = false;
    bool foundValid = false;
    bool foundEndTurn = false;

    for (int i = 0; i < count; i++)
    {
        switch (decisions[i].Kind)
        {
            case DecisionKind.Fortify:
            {
                var fortify = decisions[i].GetFortifyData();

                if (fortify.SourceTerritory == 0)
                    foundInvalid = true;

                if (fortify.SourceTerritory == 1 &&
                    fortify.TargetTerritory == 0)
                    foundValid = true;

                break;
            }

            case DecisionKind.EndTurn:
                foundEndTurn = true;
                break;
        }
    }

    Assert.False(foundInvalid);
    Assert.True(foundValid);
    Assert.True(foundEndTurn);
}
}