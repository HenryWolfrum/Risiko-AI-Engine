using Xunit;
using RiskEngine.Core;
using RiskEngine.Mission;
using RiskEngine.Random;
using RiskEngine.State;
using RiskEngine.Tests.TestInfrastructure;

namespace RiskEngine.Tests.Core;

public class GameInitializerTests
{
    //Checks whether all Territories start with  1 Troops and a valid owner
    [Fact]
    public void INIT_001_GameInitializer_ShouldCreateValidInitialState()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(2).Build();
        var rng = new EngineRandom(seed: 123);

        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);

        // Assert
        for (byte territory = 0; territory < layout.Map.TerritoryCount; territory++)
        {
            var owner = GameStateHelper.GetTerritoryOwner(in state, territory);
            var troops = GameStateHelper.GetTerritoryTroops(in state, territory);

            Assert.True(owner < layout.Config.PlayerCount, $"Territory {territory} has invalid owner {owner}");
            Assert.Equal(1, troops);
        }
    }

    [Fact]
    public void INIT_002_GameInitializer_ShouldAssignTerritoriesToEveryPlayer()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(4).Build();
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

    [Fact]
    public void INIT_003_GameInitializer_ShouldBeDeterministicForSameSeed()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(3).Build();
        var rng1 = new EngineRandom(seed: 999);
        var rng2 = new EngineRandom(seed: 999);

        // Act
        var state1 = GameInitializer.CreateInitialState(layout, ref rng1);
        var state2 = GameInitializer.CreateInitialState(layout, ref rng2);

        // Assert
        for (byte territory = 0; territory < layout.Map.TerritoryCount; territory++)
        {
            Assert.Equal(
                GameStateHelper.GetTerritoryOwner(in state1, territory),
                GameStateHelper.GetTerritoryOwner(in state2, territory));

            Assert.Equal(
                GameStateHelper.GetTerritoryTroops(in state1, territory),
                GameStateHelper.GetTerritoryTroops(in state2, territory));
        }

        Assert.Equal(GameStateHelper.GetPlayerOnTurn(state1), GameStateHelper.GetPlayerOnTurn(state2));
    }

    [Fact]
    public void INIT_004_GameInitializer_ShouldMarkAllPlayersAlive()
    {
        // Arrange
        const byte playerCount = 5;
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(playerCount).Build();
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

    [Fact]
    public void INIT_005_GameInitializer_ShouldAssignEveryTerritory()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(4).Build();
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

    [Fact]
    public void INIT_006_GameInitializer_ShouldAssignCorrectTotalTroops()
    {
        // Arrange
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(3).Build();
        var rng = new EngineRandom(seed: 7);

        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);

        // Assert
        int troopCount = 0;
        for (byte territory = 0; territory < layout.Map.TerritoryCount; territory++)
        {
            troopCount += GameStateHelper.GetTerritoryTroops(in state, territory);
        }

        Assert.Equal(layout.Map.TerritoryCount, troopCount);
    }

    [Fact]
    public void INIT_007_GameInitializer_ShouldHandleRemainderTerritoriesFairly()
    {
        
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(4).Build();
        var rng = new EngineRandom(seed: 123);

        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);

     
        int totalAssigned = 0;
        for (byte p = 0; p < 4; p++)
        {
            totalAssigned += GameStateHelper.GetOwnedTerritoryCount(in state, p);
        }

        Assert.Equal(6, totalAssigned);
    }
    
    [Fact]
    public void INIT_008_GameInitializer_ShouldFallbackToDefaultMissionIfCatalogIsTooSmall()
    {
        // Arrange
        var customMissions = new MissionDefinition[]
        {
            new() { Id = 0, Type = MissionType.WorldDomination },
            new() { Id = 1, Type = MissionType.WorldDomination },
            new() { Id = 99, Type = MissionType.WorldDomination }
        };

        var layout = TestLayoutBuilder.CreateCircularLayout(42,4)
            .WithMissionCatalog(customMissions, fallbackMissionId: 99)
            .Build();
    
        var rng = new EngineRandom(seed: 42);

        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);
        
        for (byte p = 0; p < 4; p++)
        {
            Assert.Equal(99, MissionHelper.GetPlayerMission(in state,p));
        }
    }

    [Fact]
    public void INIT_009_GameInitializer_ShouldFallbackOnlyForSelfEliminationMission()
    {
        // Arrange
        var customMissions = new MissionDefinition[]
        {
            new() { Id = 0, Type = MissionType.EliminatePlayer, TargetPlayerId = 0 }, // Guaranteed self elimination case for playerId 0
            new() { Id = 1, Type = MissionType.EliminatePlayer, TargetPlayerId = 0 },
            new() { Id = 2, Type = MissionType.EliminatePlayer, TargetPlayerId = 0},
            new() { Id = 99, Type = MissionType.EliminatePlayer, TargetPlayerId = 0 }
        };

        var layout = TestLayoutBuilder.CreateCircularLayout(42,3)
            .WithMissionCatalog(customMissions, 99) // Fallback-ID = 99
            .Build();

        var rng = new EngineRandom(seed: 42);

        // Act
        var state = GameInitializer.CreateInitialState(layout, ref rng);

        // Assert
        bool foundFallback = false;
        bool foundNormalMission = false;

        for (byte p = 0; p < 3; p++)
        {
            var assignedMission = MissionHelper.GetPlayerMission(in state,p);
            if (assignedMission == 99)
            {
                foundFallback = true;
            }
            else
            {
                foundNormalMission = true;
            }
        }

        Assert.True(foundFallback, "Die Self-Elimination-Mission wurde nicht erkannt; Es wurde kein Fallback ausgelöst.");
        Assert.True(foundNormalMission, "Es wurden fälschlicherweise alle Missionen überschrieben (Kollateralschaden).");
    }
}