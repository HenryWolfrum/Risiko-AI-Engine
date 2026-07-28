using RiskEngine.Tests.Helpers;

namespace RiskEngine.Tests.State;

public class CardHelperTests
{
    
    [Fact]
    public void CARD_001_ShouldAddCardToPlayer()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout().Build();
        var state = GameStateHelper.CreateEmpty(2);

        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        Assert.True(CardHelper.PlayerHasCard(in state, 0, 2, layout.Deck));
    }
    
    [Fact]
    public void CARD_002_ShouldRemoveCardFromPlayer()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout().Build();
        var state = GameStateHelper.CreateEmpty(2);

        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        CardHelper.RemoveCardFromPlayer(ref state, 0, 2, layout.Deck);

        Assert.False(CardHelper.PlayerHasCard(in state, 0, 2, layout.Deck));
    }
    
    [Fact]
    public void CARD_003_ShouldCountPlayerCards()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout().Build();
        var state = GameStateHelper.CreateEmpty(2);

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        Assert.Equal(3, CardHelper.GetPlayerCardCount(in state, 0));
    }
    
    [Fact]
    public void CARD_004_ShouldReturnPlayerCardBitboard()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout().Build();
        var state = GameStateHelper.CreateEmpty(2);

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 3, layout.Deck);

        ulong expected = (1UL << 0) | (1UL << 3);

        Assert.Equal(expected, CardHelper.GetPlayerCardsBitboard(in state, 0));
    }
    
    [Fact]
    public void CARD_005_ShouldTransferCardsOnPlayerElimination()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout().Build();
        var state = GameStateHelper.CreateEmpty(2);

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 1, 3, layout.Deck);

        CardHelper.EliminateAndTransferCards(ref state, 0, 1);

        Assert.Equal(2, CardHelper.GetPlayerCardCount(in state, 0));

        Assert.Equal(0, CardHelper.GetPlayerCardCount(in state, 1));
    }
    
    [Fact]
    public void CARD_006_ShouldReturnAvailableCards()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout().Build();
        var state = GameStateHelper.CreateEmpty(2);

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 1, 1, layout.Deck);

        ulong available = CardHelper.GetAvailableCards(in state, layout.Deck);

        Assert.False((available & (1UL << 0)) != 0);
        Assert.False((available & (1UL << 1)) != 0);

        Assert.True((available & (1UL << 2)) != 0);
    }
    
    [Fact]
    public void CARD_007_ShouldGiveBonusCard()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout().Build();
        var state = GameStateHelper.CreateEmpty(2);

        var rng = new EngineRandom(12345);

        CardHelper.GiveBonusCard(ref state, 0, ref rng, layout.Deck);

        Assert.Equal(1, CardHelper.GetPlayerCardCount(in state, 0));
    }
    
    [Fact]
    public void CARD_008_ShouldDetectThreeInfantryCards()
    {
        var layout = TestLayoutBuilder.CreateSmallRiskLayout().Build();
        var state = GameStateHelper.CreateEmpty(2);

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        Assert.True(CardHelper.HasValidSet(in state, 0, layout.Deck));
    }
}