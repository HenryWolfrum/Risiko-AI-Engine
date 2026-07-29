using RiskEngine.Tests.Helpers;

namespace RiskEngine.Tests.Map;

public class MapTraverserConnectedTests
{
    /*
     * CONN-001
     *
     * A map where all territories are transitively
     * connected should be marked as connected.
     *
     * Guarantees:
     * - fully connected map returns true
     */
    [Fact]
    public void CONN_001_ShouldReturnTrueForFullyConnectedMap()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(4)
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .BuildMap();

        // Act
        var result = MapTraverser.IsConnected(map);

        // Assert
        Assert.True(result);
    }

    /*
     * CONN-002
     *
     * A map divided into disconnected components (islands)
     * should fail the connectivity check.
     *
     * Guarantees:
     * - disconnected islands return false
     */
    [Fact]
    public void CONN_002_ShouldReturnFalseForDisconnectedMap()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(4)
            .Connect(0, 1)
            // 2 and 3 are connected to each other, but isolated from 0 and 1
            .Connect(2, 3)
            .BuildMap();

        // Act
        var result = MapTraverser.IsConnected(map);

        // Assert
        Assert.False(result);
    }

    /*
     * CONN-003
     *
     * A map with a single territory is trivially connected.
     *
     * Guarantees:
     * - single territory map returns true
     */
    [Fact]
    public void CONN_003_ShouldReturnTrueForSingleTerritoryMap()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(1)
            .BuildMap();

        // Act
        var result = MapTraverser.IsConnected(map);

        // Assert
        Assert.True(result);
    }

    /*
     * CONN-004
     *
     * An empty map contains no territories and
     * is considered invalid / disconnected.
     *
     * Guarantees:
     * - empty map returns false
     */
    [Fact]
    public void CONN_004_ShouldReturnFalseForEmptyMap()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(0)
            .BuildMap();

        // Act
        var result = MapTraverser.IsConnected(map);

        // Assert
        Assert.False(result);
    }

    /*
     * CONN-005
     *
     * A map where a single territory has no connections
     * to the rest of the map should return false.
     *
     * Guarantees:
     * - isolated territory causes check to fail
     */
    [Fact]
    public void CONN_005_ShouldReturnFalseWhenTerritoryIsIsolated()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(3)
            .Connect(0, 1)
            // Territory 2 is completely disconnected
            .BuildMap();

        // Act
        var result = MapTraverser.IsConnected(map);

        // Assert
        Assert.False(result);
    }

    /*
     * CONN-006
     *
     * In a directed graph scenario where node 0 cannot reach
     * other nodes, connectivity should return false.
     *
     * Guarantees:
     * - unreachable nodes from node 0 return false
     */
    [Fact]
    public void CONN_006_ShouldReturnFalseWhenUnreachableFromStartTerritory()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(2)
            .ConnectOneWay(1, 0) // 1 can reach 0, but 0 cannot reach 1
            .BuildMap();

        // Act
        var result = MapTraverser.IsConnected(map);

        // Assert
        Assert.False(result);
    }
}