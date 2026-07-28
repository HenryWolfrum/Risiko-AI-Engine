using RiskEngine.Rules;

namespace RiskEngine.Tests.Validation;

using RiskEngine;
using RiskEngine.Tests.Helpers;
using Xunit;

public class TurnInCardsRulesTests
{
    /*
     * TURNIN-001
     *
     * A valid set containing one Infantry,
     * one Cavalry and one Artillery card
     * should be accepted.
     *
     * Guarantees:
     * - all different card types are valid
     */
    [Fact]
    public void TURNIN_001_AllDifferentCards_ShouldBeAccepted()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck); // Infantry
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck); // Cavalry
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck); // Artillery

        var action = new GameAction
        {
            Card1 = 0,
            Card2 = 1,
            Card3 = 2
        };

        // Act
        var result = TurnInCardsRules.Validate(in state, in action, layout.Deck);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }


    /*
     * TURNIN-002
     *
     * Any set containing a Joker
     * should be accepted.
     *
     * Guarantees:
     * - Joker immediately forms a valid set
     */
    [Fact]
    public void TURNIN_002_JokerSet_ShouldBeAccepted()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 6, layout.Deck); // Joker
        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);

        var action = new GameAction
        {
            Card1 = 6,
            Card2 = 0,
            Card3 = 1
        };

        // Act
        var result = TurnInCardsRules.Validate(in state, in action, layout.Deck);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(EngineError.None, result.Error);
    }


    /*
     * TURNIN-003
     *
     * Card ids outside the deck
     * should be rejected.
     *
     * Guarantees:
     * - invalid card ids are detected
     */
    [Fact]
    public void TURNIN_003_InvalidCardId_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        var action = new GameAction
        {
            Card1 = layout.Deck.CardCount,
            Card2 = 1,
            Card3 = 2
        };

        // Act
        var result = TurnInCardsRules.Validate(in state, in action, layout.Deck);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.UnknownCard, result.Error);
    }


    /*
     * TURNIN-004
     *
     * Every submitted card
     * must belong to the player.
     *
     * Guarantees:
     * - missing card ownership is rejected
     */
    [Fact]
    public void TURNIN_004_CardNotOwned_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);

        var action = new GameAction
        {
            Card1 = 0,
            Card2 = 1,
            Card3 = 2
        };

        // Act
        var result = TurnInCardsRules.Validate(in state, in action, layout.Deck);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.CardNotOwned, result.Error);
    }


    /*
     * TURNIN-005
     *
     * Two equal card types and one different type
     * without a Joker should be rejected.
     *
     * Guarantees:
     * - invalid card combinations are detected
     */
    [Fact]
    public void TURNIN_005_InvalidCardSet_ShouldBeRejected()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck); // Infantry
        CardHelper.AddCardToPlayer(ref state, 0, 3, layout.Deck); // Infantry
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck); // Cavalry

        var action = new GameAction
        {
            Card1 = 0,
            Card2 = 3,
            Card3 = 1
        };

        // Act
        var result = TurnInCardsRules.Validate(in state, in action, layout.Deck);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(EngineError.InvalidCardSet, result.Error);
    }
}