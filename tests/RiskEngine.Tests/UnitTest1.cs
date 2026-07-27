namespace RiskEngine.Tests.Validation;

using Xunit;
using RiskEngine;
using RiskEngine.Validation;
using RiskEngine.Tests.Helpers;

public class AttackRulesTests
{
    [Fact]
    public void Map_Should_Create_Bidirectional_Connections()
    {
        var layout = TestLayoutBuilder.Create()
            .WithTerritories(3)
            .Connect(0,1)
            .Build();


        Assert.True(
            layout.Map.AreNeighbors(0,1));

        Assert.True(
            layout.Map.AreNeighbors(1,0));
    }
}