using RiskEngine.State;
using RiskEngine.State.Generation;
using RiskEngine.State.Tests.Helpers;

namespace RiskEngine.Tests.Decisions.Generators;

public class CardTurnInOptionGeneratorTests
{
    /*
     * CARDTURNINGEN-001
     *
     * A single valid card set should
     * generate one card turn-in option
     * and one skip phase option.
     *
     * Guarantees:
     * - valid card set is generated
     * - skip phase is generated when trade is optional
     */
    [Fact]
    public void CARDTURNINGEN_001_ShouldGenerateSingleValidSetAndSkipPhase()
    {
        // Arrange
        var layout = TestLayoutBuilder
            .Create()
            .WithTerritories(3)
            .Connect(0,1)
            .Connect(1,2)
            .Build();

        var state = GameStateHelper.CreateEmpty(2, 3);

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = CardTurnInOptionGenerator.Generate(in state, layout, decisions);

        // Assert
        Assert.Equal(2, count);

        bool foundSet = false;
        bool foundSkip = false;

        for (int i = 0; i < count; i++)
        {
            switch (decisions[i].Kind)
            {
                case DecisionKind.CardTurnIn:
                {
                    var cards = decisions[i].GetCardTriple();

                    if (cards.Card1 == 0 &&
                        cards.Card2 == 1 &&
                        cards.Card3 == 2)
                    {
                        foundSet = true;
                    }

                    break;
                }

                case DecisionKind.SkipPhase:
                    foundSkip = true;
                    break;
            }
        }

        Assert.True(foundSet);
        Assert.True(foundSkip);
    }
    
    /*
     * CARDTURNINGEN-002
     *
     * Multiple valid card sets should
     * generate one decision for each
     * valid combination.
     *
     * Guarantees:
     * - every valid card set is generated
     * - no valid card set is missing
     */
    [Fact]
    public void CARDTURNINGEN_002_ShouldGenerateAllValidCardSets()
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

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 3, layout.Deck);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = CardTurnInOptionGenerator.Generate(in state, layout, decisions);

        // Assert
        Assert.Equal(3, count);

        bool found012 = false;
        bool found123 = false;
        bool foundSkip = false;

        for (int i = 0; i < count; i++)
        {
            switch (decisions[i].Kind)
            {
                case DecisionKind.CardTurnIn:
                {
                    var cards = decisions[i].GetCardTriple();

                    if (cards.Card1 == 0 &&
                        cards.Card2 == 1 &&
                        cards.Card3 == 2)
                    {
                        found012 = true;
                    }

                    if (cards.Card1 == 1 &&
                        cards.Card2 == 2 &&
                        cards.Card3 == 3)
                    {
                        found123 = true;
                    }

                    break;
                }

                case DecisionKind.SkipPhase:
                    foundSkip = true;
                    break;
            }
        }

        Assert.True(found012);
        Assert.True(found123);
        Assert.True(foundSkip);
    }
    
    
    /*
     * CARDTURNINGEN-003
     *
     * Invalid card combinations should
     * not generate a card turn-in
     * decision.
     *
     * Guarantees:
     * - invalid card sets are ignored
     * - skip phase is still generated
     */
    [Fact]
    public void CARDTURNINGEN_003_ShouldIgnoreInvalidCardSets()
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

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 3, layout.Deck);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = CardTurnInOptionGenerator.Generate(in state, layout, decisions);

        // Assert
        Assert.Equal(1, count);

        Assert.Equal(DecisionKind.SkipPhase, decisions[0].Kind);
    }
    
    /*
     * CARDTURNINGEN-004
     *
     * Players with a mandatory
     * card trade should not
     * receive a skip option.
     *
     * Guarantees:
     * - skip phase is omitted when trade is mandatory
     */
    [Fact]
    public void CARDTURNINGEN_004_ShouldNotGenerateSkipWhenTradeIsMandatory()
    {
        // Arrange
        var builder = TestLayoutBuilder
            .Create()
            .WithTerritories(EngineConstants.FORCE_TRADE_CARD_COUNT);

        for (byte i = 0; i < EngineConstants.FORCE_TRADE_CARD_COUNT - 1; i++)
        {
            builder.Connect(i, (byte)(i + 1));
        }

        var layout = builder.Build();

        var state = GameStateHelper.CreateEmpty(2, EngineConstants.FORCE_TRADE_CARD_COUNT);

        state.PlayerTurn = 0;

        for (byte card = 0; card < EngineConstants.FORCE_TRADE_CARD_COUNT; card++)
        {
            CardHelper.AddCardToPlayer(ref state, 0, card, layout.Deck);
        }

        Span<DecisionOption> decisions = stackalloc DecisionOption[32];

        // Act
        int count = CardTurnInOptionGenerator.Generate(in state, layout, decisions);

        // Assert
        Assert.True(count > 0);

        for (int i = 0; i < count; i++)
        {
            Assert.NotEqual(DecisionKind.SkipPhase, decisions[i].Kind);
        }
    }
    
    /*
     * CARDTURNINGEN-005
     *
     * Every valid card set should
     * be generated exactly once.
     *
     * Guarantees:
     * - no duplicate card turn-in decisions exist
     */
    [Fact]
    public void CARDTURNINGEN_005_ShouldNotGenerateDuplicateCardSets()
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

        state.PlayerTurn = 0;

        CardHelper.AddCardToPlayer(ref state, 0, 0, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 1, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 2, layout.Deck);
        CardHelper.AddCardToPlayer(ref state, 0, 3, layout.Deck);

        Span<DecisionOption> decisions = stackalloc DecisionOption[8];

        // Act
        int count = CardTurnInOptionGenerator.Generate(in state, layout, decisions);

        // Assert
        int occurrences012 = 0;
        int occurrences123 = 0;

        for (int i = 0; i < count; i++)
        {
            if (decisions[i].Kind != DecisionKind.CardTurnIn)
                continue;

            var cards = decisions[i].GetCardTriple();

            if (cards.Card1 == 0 &&
                cards.Card2 == 1 &&
                cards.Card3 == 2)
            {
                occurrences012++;
            }

            if (cards.Card1 == 1 &&
                cards.Card2 == 2 &&
                cards.Card3 == 3)
            {
                occurrences123++;
            }
        }

        Assert.Equal(1, occurrences012);
        Assert.Equal(1, occurrences123);
    }
}