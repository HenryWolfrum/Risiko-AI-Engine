using RiskEngine.Tests.Helpers;

namespace RiskEngine.Tests.Map;

public class MapTraverserConnectedTests
{
    /*
     * CONNECTED-001
     *
     * A fully connected map
     * should be recognized
     * as connected.
     *
     * Guarantees:
     * - connected map returns true
     */
    [Fact]
    public void CONNECTED_001_ShouldRecognizeConnectedMap()
    {
        // Arrange
        var map = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .BuildMap();

        // Act
        var result = MapTraverser.IsConnected(map);

        // Assert
        Assert.True(result);
    }
    
    
    
}