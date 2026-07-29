using RiskEngine.State.Mutation;
using RiskEngine.State.Tests.Helpers;

namespace RiskEngine.State.Tests.Mutation;

public class CardTurnInMutatorTests
{
    /*
     * MUTATE-TURNIN-001
     *
     * Turning in cards should remove
     * all traded cards from the active player's hand.
     *
     * Guarantees:
     * - first card is removed
     * - second card is removed
     * - third card is removed
     */
    [Fact]
    public void MUTATE_TURNIN_001_ShouldRemoveAllTradedCardsFromPlayerHand()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        var action = new GameAction
        {
            Card1 = 0,
            Card2 = 1,
            Card3 = 2
        };

        // Act
        CardTurnInMutator.Apply(ref state, in action, layout.Deck);

        // Assert
        Assert.False(CardHelper.PlayerHasCard(in state, 0, 0, layout.Deck));
        Assert.False(CardHelper.PlayerHasCard(in state, 0, 1, layout.Deck));
        Assert.False(CardHelper.PlayerHasCard(in state, 0, 2, layout.Deck));
    }
    
    /*
     * MUTATE-TURNIN-002
     *
     * Turning in cards should increase
     * the traded card set counter.
     *
     * Guarantees:
     * - traded card set count increases by one
     */
    [Fact]
    public void MUTATE_TURNIN_002_ShouldIncreaseCardSetTradeCount()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        var action = new GameAction
        {
            Card1 = 0,
            Card2 = 1,
            Card3 = 2
        };

        // Act
        CardTurnInMutator.Apply(ref state, in action, layout.Deck);

        // Assert
        Assert.Equal(1, state.CardSetsTradedCount);
    }
    
    /*
     * MUTATE-TURNIN-003
     *
     * Turning in cards should award
     * the correct reinforcement bonus
     * for the first traded set.
     *
     * Guarantees:
     * - player receives four reinforcement troops
     */
    [Fact]
    public void MUTATE_TURNIN_003_ShouldAwardFirstTradeReinforcementBonus()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        var action = new GameAction
        {
            Card1 = 0,
            Card2 = 1,
            Card3 = 2
        };

        // Act
        CardTurnInMutator.Apply(ref state, in action, layout.Deck);

        // Assert
        Assert.Equal(4, GameStateHelper.GetPlayerTroopsToPlace(in state, 0));
    }
    
    /*
     * MUTATE-TURNIN-004
     *
     * Turning in cards should add
     * reinforcement bonus to the player's
     * existing troop pool.
     *
     * Guarantees:
     * - existing reinforcement troops are preserved
     * - bonus troops are added
     */
    [Fact]
    public void MUTATE_TURNIN_004_ShouldAddBonusToExistingReinforcementPool()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 3);

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        var action = new GameAction
        {
            Card1 = 0,
            Card2 = 1,
            Card3 = 2
        };

        // Act
        CardTurnInMutator.Apply(ref state, in action, layout.Deck);

        // Assert
        Assert.Equal(7, GameStateHelper.GetPlayerTroopsToPlace(in state, 0));
    }
    
    /*
     * MUTATE-TURNIN-005
     *
     * Turning in cards should award
     * the territory ownership bonus
     * for owned territory cards.
     *
     * Guarantees:
     * - owned territory receives two bonus troops
     */
    [Fact]
    public void MUTATE_TURNIN_005_ShouldAwardOwnedTerritoryBonus()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        var action = new GameAction
        {
            Card1 = 0,
            Card2 = 1,
            Card3 = 2
        };

        // Act
        CardTurnInMutator.Apply(ref state, in action, layout.Deck);

        // Assert
        Assert.Equal(7, GameStateHelper.GetTerritoryTroops(in state, 0));
    }
    
    /*
     * MUTATE-TURNIN-006
     *
     * Turning in cards should not award
     * the territory ownership bonus
     * for territories not owned by the active player.
     *
     * Guarantees:
     * - non-owned territory troop count remains unchanged
     */
    [Fact]
    public void MUTATE_TURNIN_006_ShouldNotAwardTerritoryBonusForUnownedTerritory()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .CreateSmallRiskLayout()
            .Build();

        var state = GameStateHelper.CreateEmpty(2);

        state.PlayerTurn = 0;

        GameStateHelper.SetTerritoryOwner(ref state, 0, 1);
        GameStateHelper.SetTerritoryTroops(ref state, 0, 5);

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        var action = new GameAction
        {
            Card1 = 0,
            Card2 = 1,
            Card3 = 2
        };

        // Act
        CardTurnInMutator.Apply(ref state, in action, layout.Deck);

        // Assert
        Assert.Equal(5, GameStateHelper.GetTerritoryTroops(in state, 0));
    }
}