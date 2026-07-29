namespace RiskEngine.State.Tests.Mutation;

public class AttackMutatorTests
{
    /*
     * MUTATE-ATTACK-001
     *
     * Combat casualties should reduce
     * attacker troops on the source territory.
     *
     * Guarantees:
     * - attacker troop losses are applied
     */
    [Fact]
    public void MUTATE_ATTACK_001_ShouldApplyAttackerLosses()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 10);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1
        };

        var result = new CombatResult
        {
            AttackerLosses = 2,
            DefenderLosses = 0
        };

        // Act
        AttackMutator.Apply(ref state, in action, in result);

        // Assert
        Assert.Equal(8, GameStateHelper.GetTerritoryTroops(in state, 0));
    }
    
    
    /*
     * MUTATE-ATTACK-002
     *
     * Combat casualties should reduce
     * defender troops on the target territory.
     *
     * Guarantees:
     * - defender troop losses are applied
     */
    [Fact]
    public void MUTATE_ATTACK_002_ShouldApplyDefenderLosses()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 1, 7);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1
        };

        var result = new CombatResult
        {
            AttackerLosses = 0,
            DefenderLosses = 3
        };

        // Act
        AttackMutator.Apply(ref state, in action, in result);

        // Assert
        Assert.Equal(
            4,
            GameStateHelper.GetTerritoryTroops(in state, 1));
    }
    
    /*
     * MUTATE-ATTACK-003
     *
     * Defender troops should never
     * become negative.
     *
     * Guarantees:
     * - defender troops become zero
     *   when losses equal troop count
     */
    [Fact]
    public void MUTATE_ATTACK_003_ShouldSetDefenderTroopsToZero_WhenLossesEqualTroops()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 1, 3);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1
        };

        var result = new CombatResult
        {
            AttackerLosses = 0,
            DefenderLosses = 3
        };

        // Act
        AttackMutator.Apply(ref state, in action, in result);

        // Assert
        Assert.Equal(0, GameStateHelper.GetTerritoryTroops(in state, 1));
    }
    
    /*
     * MUTATE-ATTACK-004
     *
     * Defender troops should never
     * underflow.
     *
     * Guarantees:
     * - defender troops become zero
     *   when losses exceed troop count
     */
    [Fact]
    public void MUTATE_ATTACK_004_ShouldPreventDefenderTroopUnderflow()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 1, 2);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1
        };

        var result = new CombatResult
        {
            AttackerLosses = 0,
            DefenderLosses = 5
        };

        // Act
        AttackMutator.Apply(ref state, in action, in result);

        // Assert
        Assert.Equal(0, GameStateHelper.GetTerritoryTroops(in state, 1));
    }
    
    /*
     * MUTATE-ATTACK-005
     *
     * Combat should only modify
     * the participating territories.
     *
     * Guarantees:
     * - unrelated territories remain unchanged
     */
    [Fact]
    public void MUTATE_ATTACK_005_ShouldNotModifyUnrelatedTerritories()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(3);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 10);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 5);
        GameStateHelper.SetTerritoryTroops(ref state, 2, 8);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1
        };

        var result = new CombatResult
        {
            AttackerLosses = 2,
            DefenderLosses = 1
        };

        // Act
        AttackMutator.Apply(ref state, in action, in result);

        // Assert
        Assert.Equal(8, GameStateHelper.GetTerritoryTroops(in state, 2));
    }
    
    /*
     * MUTATE-ATTACK-006
     *
     * Combat casualties should not
     * change territory ownership.
     *
     * Guarantees:
     * - territory owners remain unchanged
     */
    [Fact]
    public void MUTATE_ATTACK_006_ShouldNotModifyTerritoryOwnership()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1
        };

        var result = new CombatResult
        {
            AttackerLosses = 1,
            DefenderLosses = 2
        };

        // Act
        AttackMutator.Apply(ref state, in action, in result);

        // Assert
        Assert.Equal(0, GameStateHelper.GetTerritoryOwner(in state, 0));
        Assert.Equal(1, GameStateHelper.GetTerritoryOwner(in state, 1));
    }
    
    /*
     * MUTATE-ATTACK-007
     *
     * Zero combat losses should
     * leave troop counts unchanged.
     *
     * Guarantees:
     * - attacker troops remain unchanged
     * - defender troops remain unchanged
     */
    [Fact]
    public void MUTATE_ATTACK_007_ShouldLeaveStateUnchanged_WhenNoLossesOccur()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 8);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 5);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1
        };

        var result = new CombatResult
        {
            AttackerLosses = 0,
            DefenderLosses = 0
        };

        // Act
        AttackMutator.Apply(ref state, in action, in result);

        // Assert
        Assert.Equal(
            8,
            GameStateHelper.GetTerritoryTroops(in state, 0));

        Assert.Equal(
            5,
            GameStateHelper.GetTerritoryTroops(in state, 1));
    }
    
    
    /*
     * MUTATE-ATTACK-008
     *
     * Combat should apply
     * attacker and defender losses
     * in a single mutation.
     *
     * Guarantees:
     * - attacker losses are applied
     * - defender losses are applied
     */
    [Fact]
    public void MUTATE_ATTACK_008_ShouldApplyLossesToBothTerritories()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 9);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 6);

        var action = new GameAction
        {
            SourceTerritory = 0,
            TargetTerritory = 1
        };

        var result = new CombatResult
        {
            AttackerLosses = 2,
            DefenderLosses = 3
        };

        // Act
        AttackMutator.Apply(ref state, in action, in result);

        // Assert
        Assert.Equal(
            7,
            GameStateHelper.GetTerritoryTroops(in state, 0));

        Assert.Equal(
            3,
            GameStateHelper.GetTerritoryTroops(in state, 1));
    }
}