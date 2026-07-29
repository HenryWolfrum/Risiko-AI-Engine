using RiskEngine.State.Tests.Helpers;
using RiskEngine.State.Validation;

namespace RiskEngine.State.Tests.Validation;

using RiskEngine.State;
using RiskEngine.State.Tests.Helpers;
using Xunit;

public class AttackRulesTests
{
    /*
     * ATTACK-001
     *
     * A valid attack between adjacent enemy territories
     * should be accepted.
     *
     * Guarantees:
     * - source ownership is correct
     * - target is enemy
     * - topology allows attack
     */
    [Fact]
    public void ATTACK_001_ValidAttack_ShouldBeAccepted()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 3);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ChosenAttackerDiceCount = 2
        };


        // Act
        var result = AttackRules.Validate(
            in state,
            in action,
            layout.Map);


        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }


    /*
     * ATTACK-002
     *
     * Source territory must belong to active player.
     */
    [Fact]
    public void ATTACK_002_SourceNotOwned_ShouldBeInvalid()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 1);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ChosenAttackerDiceCount = 1
        };


        // Act
        var result = AttackRules.Validate(in state, in action, layout.Map);


        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.TerritoryNotOwned, result.Error);
    }


    /*
     * ATTACK-003
     *
     * Player cannot attack a territory they already own.
     */
    [Fact]
    public void ATTACK_003_TargetOwnedBySamePlayer_ShouldBeInvalid()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ChosenAttackerDiceCount = 1
        };


        // Act
        var result = AttackRules.Validate(
            in state,
            in action,
            layout.Map);


        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidTarget, result.Error);
    }


    /*
     * ATTACK-004
     *
     * Territories must be adjacent.
     */
    [Fact]
    public void ATTACK_004_NonAdjacentTerritories_ShouldBeInvalid()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 5, 1);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 5,
            ChosenAttackerDiceCount = 1
        };


        // Act
        var result = AttackRules.Validate(
            in state,
            in action,
            layout.Map);


        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.TerritoriesNotAdjacent, result.Error);
    }


    /*
     * ATTACK-005
     *
     * Territory needs at least two troops to attack.
     */
    [Fact]
    public void ATTACK_005_NotEnoughTroops_ShouldBeInvalid()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 1);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ChosenAttackerDiceCount = 1
        };


        // Act
        var result = AttackRules.Validate(
            in state,
            in action,
            layout.Map);


        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.NotEnoughTroops, result.Error);
    }


    /*
     * ATTACK-006
     *
     * Attacker dice count must respect available troops.
     */
    [Fact]
    public void ATTACK_006_InvalidDiceCount_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 2);


        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ChosenAttackerDiceCount = 3
        };


        // Act
        var result = AttackRules.Validate(
            in state,
            in action,
            layout.Map);


        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidDiceCount, result.Error);
    }


    /*
     * ATTACK-007
     *
     * Defender dice amount depends on troop count.
     */
    [Fact]
    public void ATTACK_007_DefenderDiceLimit_ShouldDependOnTroops()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 1);


        // Act
        var oneTroopMax =
            AttackRules.GetMaxDefenderDice(in state, 0);


        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);


        var manyTroopsMax =
            AttackRules.GetMaxDefenderDice(in state, 0);


        // Assert
        Assert.Equal(1, oneTroopMax);
        Assert.Equal(2, manyTroopsMax);
    }
    
    
    /*
     * ATTACK-009
     *
     * Source and target territory
     * must not be identical.
     *
     * Guarantees:
     * - self attacks are rejected
     */
    [Fact]
    public void ATTACK_009_SameSourceAndTarget_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 0,
            ChosenAttackerDiceCount = 1
        };

        // Act
        var result = AttackRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
    }
    
    /*
     * ATTACK-010
     *
     * Maximum legal attacker dice
     * should be accepted.
     *
     * Guarantees:
     * - highest valid attacker dice count
     *   passes validation
     */
    [Fact]
    public void ATTACK_010_MaximumLegalDice_ShouldBeAccepted()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 4);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 2);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            ChosenAttackerDiceCount = 3
        };

        // Act
        var result = AttackRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.True(result.IsValid);
    }
}