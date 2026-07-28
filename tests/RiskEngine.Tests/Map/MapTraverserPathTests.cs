using RiskEngine.Tests.Helpers;

namespace RiskEngine.Tests.Map;

public class MapTraverserPathTests
{
    /*
     * PATH-001
     *
     * A direct connection between two
     * owned territories should be found.
     *
     * Guarantees:
     * - direct owned neighbour is reachable
     */
    [Fact]
    public void PATH_001_ShouldFindDirectConnection()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .Create()
            .WithTerritories(2)
            .Connect(0, 1)
            .Build();

        var state = GameStateHelper.CreateEmpty(2, 2);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);

        // Act
        var result = MapTraverser.HasPath(state, layout.Map, 0, 1, 0);

        // Assert
        Assert.True(result);
    }
    
    /*
     * PATH-002
     *
     * An indirect connection through
     * owned territories should be found.
     *
     * Guarantees:
     * - multi-step owned path is reachable
     */
    [Fact]
    public void PATH_002_ShouldFindIndirectConnection()
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

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 2, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 3, 0);

        // Act
        var result = MapTraverser.HasPath(state, layout.Map, 0, 3, 0);

        // Assert
        Assert.True(result);
    }
    
    /*
     * PATH-003
     *
     * Territories without a connecting
     * owned path should not be reachable.
     *
     * Guarantees:
     * - disconnected territories are unreachable
     */
    [Fact]
    public void PATH_003_ShouldReturnFalseWhenNoPathExists()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(3)
            .ConnectOneWay(0,1)
            .BuildMap();

        Assert.False(MapTraverser.IsUndirected(map));

        var state = GameStateHelper.CreateEmpty(2, 4);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 2, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 3, 0);

        // Act
        var result = MapTraverser.HasPath(state, map, 0, 3, 0);

        // Assert
        Assert.False(result);
    }
    
    /*
     * PATH-004
     *
     * Source territory must belong
     * to the requested player.
     *
     * Guarantees:
     * - foreign source is rejected
     */
    [Fact]
    public void PATH_004_ShouldReturnFalseWhenSourceIsNotOwned()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .Create()
            .WithTerritories(2)
            .Connect(0, 1)
            .Build();

        var state = GameStateHelper.CreateEmpty(2, 2);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 1);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);

        // Act
        var result = MapTraverser.HasPath(state, layout.Map, 0, 1, 0);

        // Assert
        Assert.False(result);
    }
    
    /*
     * PATH-005
     *
     * Target territory must belong
     * to the requested player.
     *
     * Guarantees:
     * - foreign target is rejected
     */
    [Fact]
    public void PATH_005_ShouldReturnFalseWhenTargetIsNotOwned()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .Create()
            .WithTerritories(2)
            .Connect(0, 1)
            .Build();

        var state = GameStateHelper.CreateEmpty(2, 2);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);

        // Act
        var result = MapTraverser.HasPath(state, layout.Map, 0, 1, 0);

        // Assert
        Assert.False(result);
    }
    
    
    /*
     * PATH-006
     *
     * A territory should always
     * have a path to itself.
     *
     * Guarantees:
     * - identical source and target return true
     */
    [Fact]
    public void PATH_006_ShouldReturnTrueWhenSourceEqualsTarget()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .Create()
            .WithTerritories(1)
            .Build();

        var state = GameStateHelper.CreateEmpty(2, 1);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);

        // Act
        var result = MapTraverser.HasPath(state, layout.Map, 0, 0, 0);

        // Assert
        Assert.True(result);
    }
}