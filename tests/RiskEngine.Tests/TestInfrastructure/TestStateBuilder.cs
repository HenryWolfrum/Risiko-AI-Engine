namespace RiskEngine.State.Tests.TestInfrastructure;

using RiskEngine.State;

/// <summary>
/// Fluent builder for creating deterministic GameState instances in unit tests.
///
/// The builder intentionally does not enforce game rules.
/// It exists to create arbitrary test scenarios, including invalid states.
/// </summary>
public sealed class TestStateBuilder
{
    private readonly GameLayout _layout;
    private GameState _state;


    private TestStateBuilder(GameLayout layout)
    {
        _layout = layout;
        _state = GameStateHelper.CreateEmpty(
            layout.Config.PlayerCount,
            layout.Map.TerritoryCount);
    }


    /// <summary>
    /// Creates a new builder with an empty initialized game state.
    /// </summary>
    public static TestStateBuilder Create(GameLayout layout)
    {
        return new TestStateBuilder(layout);
    }


    /// <summary>
    /// Sets the player whose turn it currently is.
    /// </summary>
    public TestStateBuilder WithPlayerTurn(byte player)
    {
        _state.PlayerTurn = player;
        return this;
    }


    /// <summary>
    /// Sets the current game phase.
    /// </summary>
    public TestStateBuilder WithPhase(GamePhase phase)
    {
        _state.CurrentPhase = phase;
        return this;
    }


    /// <summary>
    /// Sets ownership and troop count for a territory.
    /// </summary>
    public TestStateBuilder WithTerritory(int territoryId, byte owner, byte troops)
    {
        ValidateTerritoryId(territoryId);

        GameStateHelper.SetTerritoryOwner(ref _state, territoryId, owner);
        GameStateHelper.SetTerritoryTroops(ref _state, territoryId, troops);

        return this;
    }


    /// <summary>
    /// Assigns a mission (by catalog index) to a player.
    /// </summary>
    public TestStateBuilder WithPlayerMission(byte player, byte missionId)
    {
        GameStateHelper.SetPlayerMission(ref _state, player, missionId);

        return this;
    }


    /// <summary>
    /// Sets the number of reinforcement troops waiting to be placed.
    /// </summary>
    public TestStateBuilder WithTroopsToPlace(byte player, byte troops)
    {
        GameStateHelper.SetPlayerTroopsToPlace(ref _state, player, troops);

        return this;
    }


    /// <summary>
    /// Adds a card to a player's hand.
    /// </summary>
    public TestStateBuilder WithCard(byte player, byte cardId)
    {
        CardHelper.AddCardToPlayer(
            ref _state,
            player,
            cardId,
            _layout.Deck);

        return this;
    }


    /// <summary>
    /// Adds multiple cards to a player's hand.
    /// </summary>
    public TestStateBuilder WithCards(byte player, params byte[] cardIds)
    {
        foreach (var cardId in cardIds)
        {
            CardHelper.AddCardToPlayer(
                ref _state,
                player,
                cardId,
                _layout.Deck);
        }

        return this;
    }


    /// <summary>
    /// Removes a player from the active player bitboard.
    /// Useful for elimination and GameRunner tests.
    /// </summary>
    public TestStateBuilder WithPlayerEliminated(byte player)
    {
        GameStateHelper.EliminatePlayer(ref _state, player);

        return this;
    }


    /// <summary>
    /// Marks a player as alive.
    /// Useful when constructing custom states.
    /// </summary>
    public TestStateBuilder WithPlayerAlive(byte player)
    {
        GameStateHelper.SetPlayerAlive(ref _state, player);

        return this;
    }


    /// <summary>
    /// Returns the configured GameState instance.
    /// </summary>
    public GameState Build()
    {
        return _state;
    }


    /// <summary>
    /// Ensures the territory index is valid before accessing the fixed arrays.
    /// </summary>
    private void ValidateTerritoryId(int territoryId)
    {
        if (territoryId < 0 || territoryId >= _layout.Map.TerritoryCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(territoryId),
                $"Territory ID must be between 0 and {_layout.Map.TerritoryCount - 1}.");
        }
    }
}