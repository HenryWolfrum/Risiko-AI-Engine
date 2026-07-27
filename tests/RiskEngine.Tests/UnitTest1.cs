namespace RiskEngine.Tests.Validation;

using Xunit;
using RiskEngine;
using RiskEngine.Validation;
using RiskEngine.Tests.Helpers;

public class AttackRulesTests
{
    private readonly GameLayout _layout;

    public AttackRulesTests()
    {
        // Lädt deine echte Risk-Map mit allen 42 Territorien und Adjazenzen!
        _layout = RiskMapFactory.CreateStandardRiskMap();
    }

    [Fact]
    public void Validate_ShouldFail_WhenTerritoriesAreNotAdjacent()
    {
        // ARRANGE: Alaska (0) und Argentinien (12) sind definitiv nicht benachbart
        var state = TestStateBuilder.Create()
            .WithPlayerTurn(0)
            .WithTerritory(territoryId: 0, owner: 0, troops: 5)  // Alaska (Spieler 0)
            .WithTerritory(territoryId: 12, owner: 1, troops: 2) // Argentinien (Spieler 1)
            .Build();

        var action = new GameAction
        {
            Type = ActionType.Attack,
            SourceTerritory = 0,  // Alaska
            TargetTerritory = 12, // Argentinien
            ChosenAttackerDiceCount = 3
        };

        // ACT
        var result = AttackRules.Validate(in state, in action, _layout.Map);

        // ASSERT
        Assert.False(result.IsValid);
        Assert.Equal(GameError.TerritoriesNotAdjacent, result.Error);
    }

    [Fact]
    public void Validate_ShouldSucceed_WhenAttackingAcrossPacificBorder()
    {
        // ARRANGE: Alaska (0) greift Kamtschatka (29) an – das ist eine valide Seeverbindung!
        var state = TestStateBuilder.Create()
            .WithPlayerTurn(0)
            .WithTerritory(territoryId: 0, owner: 0, troops: 4)  // Alaska
            .WithTerritory(territoryId: 29, owner: 1, troops: 2) // Kamtschatka
            .Build();

        var action = new GameAction
        {
            Type = ActionType.Attack,
            SourceTerritory = 0,
            TargetTerritory = 29,
            ChosenAttackerDiceCount = 3
        };

        // ACT
        var result = AttackRules.Validate(in state, in action, _layout.Map);

        // ASSERT
        Assert.True(result.IsValid);
        Assert.Equal(GameError.None, result.Error);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAttackerHasNotEnoughTroops()
    {
        // ARRANGE: Alaska (0) greift Nordwest-Territorium (1) an, hat aber nur 1 Truppe
        var state = TestStateBuilder.Create()
            .WithPlayerTurn(0)
            .WithTerritory(territoryId: 0, owner: 0, troops: 1)  // Alaska
            .WithTerritory(territoryId: 1, owner: 1, troops: 2)  // Nordwest-Territorium
            .Build();

        var action = new GameAction
        {
            Type = ActionType.Attack,
            SourceTerritory = 0,
            TargetTerritory = 1,
            ChosenAttackerDiceCount = 1
        };

        // ACT
        var result = AttackRules.Validate(in state, in action, _layout.Map);

        // ASSERT
        Assert.False(result.IsValid);
        Assert.Equal(GameError.NotEnoughTroops, result.Error);
    }
}