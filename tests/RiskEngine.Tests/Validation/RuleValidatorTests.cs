using RiskEngine.Tests.Helpers;

namespace RiskEngine.Tests.Validation;

public class RuleValidatorTests
{
    /*
     * VALIDATOR-001
     *
     * Reinforcement actions should be accepted
     * during the reinforcement phase.
     *
     * Guarantees:
     * - phase validation allows reinforcement
     * - action is forwarded to ReinforceRules
     */
    [Fact]
    public void VALIDATOR_001_ReinforceInReinforcePhase_ShouldBeAccepted()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;
        state.CurrentPhase = GamePhase.Reinforce;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 5);

        var action = new GameAction
        {
            Type = ActionType.Reinforce,
            SourceTerritory = 0,
            TroopCount = 3
        };

        // Act
        var result = RuleValidator.Validate(in state, in action, layout);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }
    
    /*
     * VALIDATOR-002
     *
     * Reinforcement actions should be rejected
     * outside the reinforcement phase.
     *
     * Guarantees:
     * - phase validation is enforced
     * - action is rejected before rule validation
     */
    [Fact]
    public void VALIDATOR_002_ReinforceOutsideReinforcePhase_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;
        state.CurrentPhase = GamePhase.Attack;

        var action = new GameAction
        {
            Type = ActionType.Reinforce,
            SourceTerritory = 0,
            TroopCount = 3
        };

        // Act
        var result = RuleValidator.Validate(in state, in action, layout);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.ActionNotAllowedInPhase, result.Error);
    }
    
    /*
     * VALIDATOR-003
     *
     * Skipping the attack phase
     * should always be allowed.
     *
     * Guarantees:
     * - SkipPhase is accepted during Attack phase
     * - no additional rule validation is required
     */
    [Fact]
    public void VALIDATOR_003_SkipPhaseDuringAttack_ShouldBeAccepted()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;
        state.CurrentPhase = GamePhase.Attack;

        var action = new GameAction
        {
            Type = ActionType.SkipPhase
        };

        // Act
        var result = RuleValidator.Validate(
            in state,
            in action,
            layout);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }
    
    /*
     * VALIDATOR-004
     *
     * Ending the turn during the attack phase
     * should always be allowed.
     *
     * Guarantees:
     * - EndTurn is accepted during Attack phase
     * - no additional rule validation is required
     */
    [Fact]
    public void VALIDATOR_004_EndTurnDuringAttack_ShouldBeAccepted()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;
        state.CurrentPhase = GamePhase.Attack;

        var action = new GameAction
        {
            Type = ActionType.EndTurn
        };

        // Act
        var result = RuleValidator.Validate(in state, in action, layout);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }
    
    /*
     * VALIDATOR-005
     *
     * Attack actions should be forwarded
     * to the attack rule validator.
     *
     * Guarantees:
     * - RuleValidator dispatches Attack actions correctly
     * - successful attack validation is returned unchanged
     */
    [Fact]
    public void VALIDATOR_005_ValidAttack_ShouldBeForwarded()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;
        state.CurrentPhase = GamePhase.Attack;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);
        GameStateHelper.SetTerritoryTroops(ref state, 1, 2);

        var action = new GameAction
        {
            Type = ActionType.Attack,
            SourceTerritory = 0,
            TargetTerritory = 1,
            ChosenAttackerDiceCount = 3
        };

        // Act
        var result = RuleValidator.Validate( in state, in action, layout);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }
}