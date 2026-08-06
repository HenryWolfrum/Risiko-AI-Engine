using RiskEngine.Mission;
using RiskEngine.State;
using RiskEngine.State.Tests.Helpers;
using RiskEngine.State.Tests.TestInfrastructure;
using Xunit;

namespace RiskEngine.State.Tests.Mission;

public class MissionEvaluatorTests
{
    // =====================================================================
    // WorldDomination
    // =====================================================================

    /*
     * MISSION-001
     * WorldDomination is fulfilled only once the player owns every
     * territory on the map - not just "most" of them.
     */
    [Fact]
    public void MISSION_001_WorldDomination_ShouldRequireEveryTerritory()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(playerCount: 2).Build();

        var missionId = FindMissionIndex(layout, MissionType.WorldDomination);

        // 5 of 6 territories owned -> not fulfilled yet
        var almostState = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithTerritory(0, owner: 0, troops: 1)
            .WithTerritory(1, owner: 0, troops: 1)
            .WithTerritory(2, owner: 0, troops: 1)
            .WithTerritory(3, owner: 0, troops: 1)
            .WithTerritory(4, owner: 0, troops: 1)
            .WithTerritory(5, owner: 1, troops: 1)
            .Build();

        Assert.False(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in almostState, in layout, 0));

        // All 6 territories owned -> fulfilled
        var fullState = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithTerritory(0, owner: 0, troops: 1)
            .WithTerritory(1, owner: 0, troops: 1)
            .WithTerritory(2, owner: 0, troops: 1)
            .WithTerritory(3, owner: 0, troops: 1)
            .WithTerritory(4, owner: 0, troops: 1)
            .WithTerritory(5, owner: 0, troops: 1)
            .Build();

        Assert.True(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in fullState, in layout, 0));
    }

    // =====================================================================
    // ConquerTerritories
    // =====================================================================

    /*
     * MISSION-002
     * ConquerTerritories without a minimum troop requirement only cares
     * about the owned territory count.
     */
    [Fact]
    public void MISSION_002_ConquerTerritories_ShouldFulfillOnceCountReached()
    {
        var (layout, missionId) = BuildLayoutWithMission(new MissionDefinition
        {
            Id = 0,
            Type = MissionType.ConquerTerritories,
            RequiredTerritories = 4,
            MinTroopsPerTerritory = 0
        });

        var belowThreshold = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithTerritory(0, owner: 0, troops: 1)
            .WithTerritory(1, owner: 0, troops: 1)
            .WithTerritory(2, owner: 0, troops: 1)
            .WithTerritory(3, owner: 1, troops: 1)
            .WithTerritory(4, owner: 1, troops: 1)
            .WithTerritory(5, owner: 1, troops: 1)
            .Build();

        Assert.False(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in belowThreshold, in layout, 0));

        var atThreshold = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithTerritory(0, owner: 0, troops: 1)
            .WithTerritory(1, owner: 0, troops: 1)
            .WithTerritory(2, owner: 0, troops: 1)
            .WithTerritory(3, owner: 0, troops: 1)
            .WithTerritory(4, owner: 1, troops: 1)
            .WithTerritory(5, owner: 1, troops: 1)
            .Build();

        Assert.True(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in atThreshold, in layout, 0));
    }

    /*
     * MISSION-003
     * With a minimum-troop requirement, ALL territories currently owned by
     * the player must meet it - not just the required subset. A player who
     * owns enough territories but has a single weak one anywhere should NOT
     * have the mission fulfilled yet.
     *
     * NOTE: this is a real behavioural quirk worth double-checking against
     * your intended game design - the classic Risk rule only requires the
     * minimum on the territories counted toward the mission, not on every
     * territory the player happens to additionally own.
     */
    [Fact]
    public void MISSION_003_ConquerTerritories_WithMinTroops_ShouldFailIfAnyOwnedTerritoryIsBelowMinimum()
    {
        var (layout, missionId) = BuildLayoutWithMission(new MissionDefinition
        {
            Id = 0,
            Type = MissionType.ConquerTerritories,
            RequiredTerritories = 3,
            MinTroopsPerTerritory = 2
        });

        // Player owns 4 territories (>= required 3), but territory 3 only has 1 troop.
        var oneWeakTerritory = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithTerritory(0, owner: 0, troops: 2)
            .WithTerritory(1, owner: 0, troops: 2)
            .WithTerritory(2, owner: 0, troops: 2)
            .WithTerritory(3, owner: 0, troops: 1)
            .WithTerritory(4, owner: 1, troops: 1)
            .WithTerritory(5, owner: 1, troops: 1)
            .Build();

        Assert.False(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in oneWeakTerritory, in layout, 0));

        // Same ownership, but every owned territory now meets the minimum.
        var allMeetMinimum = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithTerritory(0, owner: 0, troops: 2)
            .WithTerritory(1, owner: 0, troops: 2)
            .WithTerritory(2, owner: 0, troops: 2)
            .WithTerritory(3, owner: 0, troops: 2)
            .WithTerritory(4, owner: 1, troops: 1)
            .WithTerritory(5, owner: 1, troops: 1)
            .Build();

        Assert.True(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in allMeetMinimum, in layout, 0));
    }

    // =====================================================================
    // ConquerContinents
    // =====================================================================

    /*
     * MISSION-004
     * ConquerContinents must require ALL continents in the target mask -
     * controlling only one of two required continents must not fulfill it.
     * Uses a dedicated two-continent map, since TestLayoutBuilder's default
     * small map only has a single continent.
     */
    [Fact]
    public void MISSION_004_ConquerContinents_ShouldRequireAllTargetContinents()
    {
        var layout = BuildTwoContinentLayout(out byte continentAMask, out byte continentBMask);

        var missionId = FindMissionIndex(layout, MissionType.ConquerContinents);

        // Player 0 owns all of continent A, none of continent B.
        var onlyOneContinent = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithTerritory(0, owner: 0, troops: 1) // Continent A
            .WithTerritory(1, owner: 0, troops: 1) // Continent A
            .WithTerritory(2, owner: 1, troops: 1) // Continent B
            .WithTerritory(3, owner: 1, troops: 1) // Continent B
            .Build();

        Assert.False(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in onlyOneContinent, in layout, 0));

        // Player 0 now owns both continents.
        var bothContinents = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithTerritory(0, owner: 0, troops: 1)
            .WithTerritory(1, owner: 0, troops: 1)
            .WithTerritory(2, owner: 0, troops: 1)
            .WithTerritory(3, owner: 0, troops: 1)
            .Build();

        Assert.True(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in bothContinents, in layout, 0));
    }

    // =====================================================================
    // EliminatePlayer
    // =====================================================================

    /*
     * MISSION-005
     * EliminatePlayer is fulfilled exactly when the target player is no
     * longer alive - independent of the evaluating player's own territory
     * count.
     */
    [Fact]
    public void MISSION_005_EliminatePlayer_ShouldDependOnlyOnTargetsAliveStatus()
    {
        var (layout, missionId) = BuildLayoutWithMission(new MissionDefinition
        {
            Id = 0,
            Type = MissionType.EliminatePlayer,
            TargetPlayerId = 1
        });

        var targetAlive = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithPlayerAlive(0)
            .WithPlayerAlive(1)
            .Build();

        Assert.False(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in targetAlive, in layout, 0));

        var targetEliminated = TestStateBuilder.Create(layout)
            .WithPlayerMission(0, missionId)
            .WithPlayerAlive(0)
            .WithPlayerEliminated(1)
            .Build();

        Assert.True(RiskEngine.Mission.MissionEvaluator.IsFulfilled(in targetEliminated, in layout, 0));
    }

    /*
     * MISSION-006
     * CheckEliminationWin must only trigger for the player whose mission
     * actually targets the eliminated player - a bystander with an
     * unrelated elimination mission must not accidentally "win" just
     * because someone else was eliminated.
     */
    [Fact]
    public void MISSION_006_CheckEliminationWin_ShouldOnlyMatchCorrectTarget()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout(playerCount: 3).Build();
        var map = layout.Map;
        var deck = layout.Deck;
        var config = layout.Config;

        var missions = new MissionCatalog(new[]
        {
            new MissionDefinition { Id = 0, Type = MissionType.EliminatePlayer, TargetPlayerId = 2 }, // targets player 2
            new MissionDefinition { Id = 1, Type = MissionType.EliminatePlayer, TargetPlayerId = 0 }, // targets player 0
            new MissionDefinition { Id = 2, Type = MissionType.WorldDomination }
        }, fallbackMissionId: 2);

        var customLayout = new GameLayout(map, deck, config, missions);

        var state = TestStateBuilder.Create(customLayout)
            .WithPlayerMission(0, 0)
            .WithPlayerMission(1, 1)
            .WithPlayerMission(2, 2)
            .WithPlayerAlive(0)
            .WithPlayerAlive(1)
            .WithPlayerEliminated(2)
            .Build();

        // Player 1's mission does NOT target player 2 -> must not falsely win.
        bool eliminatedIsPlayer2 = RiskEngine.Mission.MissionEvaluator.CheckEliminationWin(
            in state, in customLayout, eliminatedPlayerId: 2, out byte winnerId);

        Assert.True(eliminatedIsPlayer2);
        Assert.Equal(0, winnerId); // only player 0's mission targets player 2
    }

    // =====================================================================
    // Helpers
    // =====================================================================

    private static (GameLayout layout, byte missionId) BuildLayoutWithMission(MissionDefinition mission)
    {
        var builder = TestLayoutBuilder.CreateSmallRiskLayout(playerCount: 2);

        var missions = new MissionCatalog(new[] { mission }, fallbackMissionId: 0);

        var layout = new GameLayout(builder.BuildMap(), builder.BuildDeck(), builder.BuildConfig(), missions);

        return (layout, mission.Id);
    }

    private static byte FindMissionIndex(GameLayout layout, MissionType type)
    {
        for (int i = 0; i < layout.Missions.Count; i++)
        {
            if (layout.Missions[i].Type == type)
            {
                return (byte)i;
            }
        }

        throw new InvalidOperationException($"No mission of type {type} found in catalog.");
    }

    /// <summary>
    /// Builds a minimal 4-territory map split into two 2-territory continents,
    /// with a single ConquerContinents mission targeting both.
    /// </summary>
    private static GameLayout BuildTwoContinentLayout(out byte continentAMask, out byte continentBMask)
    {
        var territoryNames = new[] { "A0", "A1", "B0", "B1" };

        var adjacencies = new byte[][]
        {
            new byte[] { 1, 2 },
            new byte[] { 0, 3 },
            new byte[] { 0, 3 },
            new byte[] { 1, 2 }
        };

        var territoryToContinent = new byte[] { 0, 0, 1, 1 };

        var continents = new[]
        {
            new Continent(0, "Continent A", bonusTroops: 1, territoryCount: 2),
            new Continent(1, "Continent B", bonusTroops: 1, territoryCount: 2)
        };

        var map = new MapLayout(territoryNames, adjacencies, territoryToContinent, continents);

        var cardTypes = new[] { CardType.Infantry, CardType.Cavalry, CardType.Artillery, CardType.Infantry, CardType.Joker, CardType.Joker };
        var deck = new DeckLayout(cardTypes);

        var config = new EngineConfig(playerCount: 2,100);

        continentAMask = 0b0011; // territories 0,1
        continentBMask = 0b1100; // territories 2,3

        var missions = new MissionCatalog(new[]
        {
            new MissionDefinition
            {
                Id = 0,
                Type = MissionType.ConquerContinents,
                TargetContinentMask = (byte)((1 << 0) | (1 << 1))
            }
        }, fallbackMissionId: 0);

        return new GameLayout(map, deck, config, missions);
    }
}