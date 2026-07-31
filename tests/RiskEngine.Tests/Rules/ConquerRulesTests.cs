using RiskEngine.State.Rules;
using RiskEngine.State.Tests.Helpers;

namespace RiskEngine.State.Tests.Validation;

public class ConquerRulesTests
{
    /*
     * CONQUER-001
     *
     * A valid troop movement into a conquered
     * territory should be accepted.
     *
     * Guarantees:
     * - source territory is owned
     * - target territory has been conquered
     * - troop movement is within legal bounds
     */
    [Fact]
    public void CONQUER_001_ValidConquerMove_ShouldBeAccepted()
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

        // Territory has already been conquered.
        GameStateHelper.SetTerritoryTroops(ref state, 1, 0);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 3
        };

        // Act
        var result = ConquerRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }
    
    
    /*
     * CONQUER-002
     *
     * Source territory must exist on the map.
     *
     * Guarantees:
     * - invalid source territory ids are rejected
     */
    [Fact]
    public void CONQUER_002_InvalidSourceTerritory_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        var action = new GameAction
        {
            SourceTerritory = layout.Map.TerritoryCount,
            TargetTerritory = 0,
            TroopCount = 1
        };

        // Act
        var result = ConquerRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidTerritory, result.Error);
    }
    
    /*
     * CONQUER-003
     *
     * Target territory must exist on the map.
     *
     * Guarantees:
     * - invalid target territory ids are rejected
     */
    [Fact]
    public void CONQUER_003_InvalidTargetTerritory_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = layout.Map.TerritoryCount,
            TroopCount = 1
        };

        // Act
        var result = ConquerRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidTerritory, result.Error);
    }
    
    /*
     * CONQUER-004
     *
     * Source territory must belong
     * to the active player.
     *
     * Guarantees:
     * - enemy source territories are rejected
     */
    [Fact]
    public void CONQUER_004_SourceNotOwned_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 1);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 0);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 2
        };

        // Act
        var result = ConquerRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.TerritoryNotOwned, result.Error);
    }
    
    /*
     * CONQUER-005
     *
     * Target territory must already
     * be conquered.
     *
     * Guarantees:
     * - target territories with remaining troops
     *   are rejected
     */
    [Fact]
    public void CONQUER_005_TargetNotYetConquered_ShouldBeRejected()
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

        // Target still contains defending troops.
        GameStateHelper.SetTerritoryTroops(ref state, 1, 2);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 2
        };

        // Act
        var result = ConquerRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidTarget, result.Error);
    }
    
    
    /*
     * CONQUER-006
     *
     * Source territory must retain
     * at least one troop.
     *
     * Guarantees:
     * - conquering is rejected when only
     *   one troop remains in the source territory
     */
    [Fact]
    public void CONQUER_006_SourceWithOneTroop_ShouldBeRejected()
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
        GameStateHelper.SetTerritoryTroops(ref state, 1, 0);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 1
        };

        // Act
        var result = ConquerRules.Validate(
            in state,
            in action,
            layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.NotEnoughTroops, result.Error);
    }
    
    /*
     * CONQUER-007
     *
     * At least one troop must move
     * into the conquered territory.
     *
     * Guarantees:
     * - zero troop movements are rejected
     */
    [Fact]
    public void CONQUER_007_ZeroTroopsToMove_ShouldBeRejected()
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
        GameStateHelper.SetTerritoryTroops(ref state, 1, 0);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 0
        };

        // Act
        var result = ConquerRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidTroopCount, result.Error);
    }
    
    /*
     * CONQUER-008
     *
     * Cannot move more troops than
     * available from the source territory.
     *
     * Guarantees:
     * - troop movement exceeding the legal maximum
     *   is rejected
     */
    [Fact]
    public void CONQUER_008_MovingTooManyTroops_ShouldBeRejected()
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
        GameStateHelper.SetTerritoryTroops(ref state, 1, 0);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 5
        };

        // Act
        var result = ConquerRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidTroopCount, result.Error);
    }
    
    /*
     * CONQUER-009
     *
     * Maximum legal troop movement
     * should be accepted.
     *
     * Guarantees:
     * - moving the maximum allowed troops succeeds
     * - one troop remains in the source territory
     */
    [Fact]
    public void CONQUER_009_MaximumLegalTroopMove_ShouldBeAccepted()
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
        GameStateHelper.SetTerritoryTroops(ref state, 1, 0);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1,
            TroopCount = 4
        };

        // Act
        var result = ConquerRules.Validate(in state, in action, layout.Map);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }
    
}