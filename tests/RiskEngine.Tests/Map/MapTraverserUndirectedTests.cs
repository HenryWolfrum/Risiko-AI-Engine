using RiskEngine.State.Tests.Helpers;

namespace RiskEngine.State.Tests.Map;

public class MapTraverserUndirectedTests
{
    /*
     * UNDIR-001
     *
     * A map where all connections are symmetric (bidirectional)
     * should be confirmed as undirected.
     *
     * Guarantees:
     * - fully symmetric map returns true
     */
    [Fact]
    public void UNDIR_001_ShouldReturnTrueForSymmetricMap()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(3)
            .Connect(0, 1)
            .Connect(1, 2)
            .BuildMap();

        // Act
        var result = MapTraverser.IsUndirected(map);

        // Assert
        Assert.True(result);
    }

    /*
     * UNDIR-002
     *
     * A map containing a one-way connection (directed edge)
     * should fail the undirected check.
     *
     * Guarantees:
     * - directed connection returns false
     */
    [Fact]
    public void UNDIR_002_ShouldReturnFalseForOneWayConnection()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(2)
            .ConnectOneWay(0, 1) // 0 -> 1, but not 1 -> 0
            .BuildMap();

        // Act
        var result = MapTraverser.IsUndirected(map);

        // Assert
        Assert.False(result);
    }

    /*
     * UNDIR-003
     *
     * A single territory map with no adjacencies
     * is trivially undirected.
     *
     * Guarantees:
     * - single isolated territory returns true
     */
    [Fact]
    public void UNDIR_003_ShouldReturnTrueForSingleTerritory()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(1)
            .BuildMap();

        // Act
        var result = MapTraverser.IsUndirected(map);

        // Assert
        Assert.True(result);
    }

    /*
     * UNDIR-004
     *
     * An empty map contains no edges to validate
     * and is trivially undirected.
     *
     * Guarantees:
     * - empty map returns true
     */
    [Fact]
    public void UNDIR_004_ShouldReturnTrueForEmptyMap()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(0)
            .BuildMap();

        // Act
        var result = MapTraverser.IsUndirected(map);

        // Assert
        Assert.True(result);
    }

    /*
     * UNDIR-005
     *
     * A complex map where almost all edges are bidirectional,
     * but a single link is missing its reverse connection, must fail.
     *
     * Guarantees:
     * - partially asymmetric graph returns false
     */
    [Fact]
    public void UNDIR_005_ShouldReturnFalseWhenSingleReverseEdgeIsMissing()
    {
        // Arrange
        var map = TestLayoutBuilder
            .Create()
            .WithTerritories(3)
            .Connect(0, 1)        // 0 <-> 1 (valid)
            .ConnectOneWay(1, 2) // 1 -> 2 (missing 2 -> 1)
            .BuildMap();

        // Act
        var result = MapTraverser.IsUndirected(map);

        // Assert
        Assert.False(result);
    }
}