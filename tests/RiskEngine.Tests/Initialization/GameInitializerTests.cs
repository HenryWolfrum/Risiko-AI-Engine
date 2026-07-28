namespace RiskEngine.Tests.Initialization;

using RiskEngine;
using RiskEngine.Tests.Helpers;
using Xunit;

public class GameInitializerTests
{
    /*
     * INIT-001
     *
     * The initializer must assign every territory
     * to a valid player with the correct starting troops.
     *
     * Guarantees:
     * - every territory has a valid owner
     * - every territory starts with exactly one troop
     */
    [Fact]
    public void INIT_001_GameInitializer_ShouldCreateValidInitialState()
    {
        // Arrange
        var layout =
            TestLayoutBuilder
                .CreateSmallRiskLayout(playerCount: 2)
                .Build();

        var rng = new EngineRandom(seed: 123);


        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);


        // Assert
        for (byte territory = 0; territory < layout.Map.TerritoryCount; territory++)
        {
            var owner = GameStateHelper.GetTerritoryOwner(in state, territory);
            var troops = GameStateHelper.GetTerritoryTroops(in state, territory);


            Assert.True(
                owner < layout.Config.PlayerCount,
                $"Territory {territory} has invalid owner {owner}");


            Assert.True(troops == 1, $"Territory {territory} should start with exactly one troop but has {troops}");
        }
    }

    /*
     * INIT-002
     *
     * Every player must receive territories.
     */
    [Fact]
    public void INIT_002_GameInitializer_ShouldAssignTerritoriesToEveryPlayer()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(playerCount: 4)
                    .Build();
        var rng = new EngineRandom(seed: 42);


        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);


        // Assert
        for (byte player = 0; player < layout.Config.PlayerCount; player++)
        {
            var territoryCount = GameStateHelper.GetOwnedTerritoryCount(in state, player);


            Assert.True(territoryCount > 0, $"Player {player} owns no territories");
        }
    }


    /*
     * INIT-003
     *
     * Initialization must be deterministic.
     *
     * Same seed + same layout = same state.
     */
    [Fact]
    public void INIT_003_GameInitializer_ShouldBeDeterministicForSameSeed()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(playerCount: 3)
                .Build();


        var rng1 = new EngineRandom(seed: 999);
        var rng2 = new EngineRandom(seed: 999);


        // Act
        var state1 = GameInitializer.CreateInitialState(layout, ref rng1);

        var state2 = GameInitializer.CreateInitialState(layout, ref rng2);


        // Assert
        for (byte territory = 0; territory < layout.Map.TerritoryCount; territory++)
        {
            Assert.Equal(GameStateHelper.GetTerritoryOwner(in state1, territory), GameStateHelper.GetTerritoryOwner(in state2, territory));


            Assert.Equal(GameStateHelper.GetTerritoryTroops(in state1, territory), GameStateHelper.GetTerritoryTroops(in state2, territory));
        }


        Assert.Equal(state1.PlayerTurn, state2.PlayerTurn);
    }


    /*
     * INIT-004
     *
     * All created players must be marked alive.
     */
    [Fact]
    public void INIT_004_GameInitializer_ShouldMarkAllPlayersAlive()
    {
        // Arrange
        const byte playerCount = 5;

        var layout = TestLayoutBuilder.CreateSmallRiskLayout(playerCount)
                     .Build();

        var rng = new EngineRandom(seed: 1);


        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);


        // Assert
        for (byte player = 0; player < playerCount; player++)
        {
            Assert.True(GameStateHelper.IsPlayerAlive(in state, player), $"Player {player} should be alive");
        }
        
        Assert.Equal(playerCount, GameStateHelper.GetActivePlayerCount(in state));
    }
    
    /*
     * INIT-005
     *
     * Every territory must be assigned exactly once.
     *
     * Guarantees:
     * - no territory is left unassigned
     * - territory ownership is complete
     */
    [Fact]
    public void INIT_005_GameInitializer_ShouldAssignEveryTerritory()
    {
        // Arrange
        var layout = TestLayoutBuilder
                .CreateSmallRiskLayout(playerCount: 4)
                .Build();

        var rng = new EngineRandom(seed: 42);

        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);

        // Assert
        int ownedTerritories = 0;

        for (byte player = 0; player < layout.Config.PlayerCount; player++)
        {
            ownedTerritories += GameStateHelper.GetOwnedTerritoryCount(in state, player);
        }

        Assert.Equal(layout.Map.TerritoryCount, ownedTerritories);
    }
    
    /*
     * INIT-006
     *
     * The total number of starting troops must equal
     * the number of territories.
     *
     * Guarantees:
     * - every territory starts with one troop
     * - total troop count is correct
     */
    [Fact]
    public void INIT_006_GameInitializer_ShouldAssignCorrectTotalTroops()
    {
        // Arrange
        var layout =
            TestLayoutBuilder
                .CreateSmallRiskLayout(playerCount: 3)
                .Build();

        var rng = new EngineRandom(seed: 7);

        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);

        // Assert
        int troopCount = 0;

        for (byte territory = 0; territory < layout.Map.TerritoryCount; territory++)
        {
            troopCount +=
                GameStateHelper.GetTerritoryTroops(in state, territory);
        }

        Assert.Equal(layout.Map.TerritoryCount, troopCount);
    }
}