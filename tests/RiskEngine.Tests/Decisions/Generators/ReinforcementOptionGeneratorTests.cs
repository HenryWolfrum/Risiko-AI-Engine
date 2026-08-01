using RiskEngine.State;
using RiskEngine.State.Generation;

namespace RiskEngine.Tests.Decisions.Generators;

public class ReinforcementOptionGeneratorTests
{
    /*
     * REINFORCEGEN-001
     *
     * Every owned territory should
     * generate one reinforcement
     * decision.
     *
     * Guarantees:
     * - every owned territory is generated
     * - reinforcement range is correct
     */
    [Fact]
    public void REINFORCEGEN_001_ShouldGenerateOptionForEveryOwnedTerritory()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2, 4);

        state.PlayerTurn = 0;

        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 5);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 2, 1);
        GameStateHelper.SetTerritoryOwner(ref state, 3, 0);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = ReinforcementOptionGenerator.Generate(in state, decisions);

        // Assert
        Assert.Equal(3, count);

        bool found0 = false;
        bool found1 = false;
        bool found3 = false;

        for (int i = 0; i < count; i++)
        {
            Assert.Equal(DecisionKind.Reinforce, decisions[i].Kind);

            var reinforce = decisions[i].GetReinforceData();

            if (reinforce.TargetTerritory == 0)
                found0 = true;

            if (reinforce.TargetTerritory == 1)
                found1 = true;

            if (reinforce.TargetTerritory == 3)
                found3 = true;

            Assert.Equal(1, decisions[i].Parameter.Min);
            Assert.Equal(5, decisions[i].Parameter.Max);
        }

        Assert.True(found0);
        Assert.True(found1);
        Assert.True(found3);
    }
    
    
    /*
     * REINFORCEGEN-002
     *
     * A player without remaining
     * reinforcement troops should
     * receive no decisions.
     *
     * Guarantees:
     * - no reinforcement decisions are generated
     */
    [Fact]
    public void REINFORCEGEN_002_ShouldReturnNoOptionsWhenNoTroopsRemain()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2, 2);

        state.PlayerTurn = 0;

        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 0);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = ReinforcementOptionGenerator.Generate(in state, decisions);

        // Assert
        Assert.Equal(0, count);
    }
    
    
    /*
     * REINFORCEGEN-003
     *
     * Reinforcement decisions should
     * expose the complete valid troop
     * placement range.
     *
     * Guarantees:
     * - minimum reinforcement is one troop
     * - maximum reinforcement equals available troops
     */
    [Fact]
    public void REINFORCEGEN_003_ShouldGenerateCorrectParameterRange()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2, 1);

        state.PlayerTurn = 0;

        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 7);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);

        Span<DecisionOption> decisions = stackalloc DecisionOption[4];

        // Act
        int count = ReinforcementOptionGenerator.Generate(in state, decisions);

        // Assert
        Assert.Equal(1, count);

        Assert.Equal(DecisionKind.Reinforce, decisions[0].Kind);

        var reinforce = decisions[0].GetReinforceData();

        Assert.Equal(0, reinforce.TargetTerritory);

        Assert.Equal(1, decisions[0].Parameter.Min);
        Assert.Equal(7, decisions[0].Parameter.Max);
    }
    
    /*
     * REINFORCEGEN-004
     *
     * Every owned territory should
     * generate exactly one reinforcement
     * decision.
     *
     * Guarantees:
     * - no duplicate reinforcement decisions exist
     */
    [Fact]
    public void REINFORCEGEN_004_ShouldNotGenerateDuplicateOptions()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2, 3);

        state.PlayerTurn = 0;

        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 5);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 2, 0);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = ReinforcementOptionGenerator.Generate(in state, decisions);

        // Assert
        int territory0 = 0;
        int territory1 = 0;
        int territory2 = 0;

        for (int i = 0; i < count; i++)
        {
            var reinforce = decisions[i].GetReinforceData();

            switch (reinforce.TargetTerritory)
            {
                case 0:
                    territory0++;
                    break;

                case 1:
                    territory1++;
                    break;

                case 2:
                    territory2++;
                    break;
            }
        }

        Assert.Equal(1, territory0);
        Assert.Equal(1, territory1);
        Assert.Equal(1, territory2);
    }
}