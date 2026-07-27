namespace RiskEngine.Tests.Helpers;

using RiskEngine;

public class TestStateBuilder
{
    private GameState _state;

    private TestStateBuilder(byte playerCount)
    {
        _state = GameStateHelper.CreateEmpty(playerCount);
    }

    public static TestStateBuilder Create(byte playerCount = 2)
    {
        return new TestStateBuilder(playerCount);
    }

    public TestStateBuilder WithPlayerTurn(byte player)
    {
        _state.PlayerTurn = player;
        return this;
    }

    public TestStateBuilder WithPhase(GamePhase phase)
    {
        _state.CurrentPhase = phase;
        return this;
    }

    public TestStateBuilder WithTerritory(int territoryId, byte owner, byte troops)
    {
        GameStateHelper.SetTerritoryOwner(ref _state, territoryId, owner);
        GameStateHelper.SetTerritoryTroops(ref _state, territoryId, troops);
        return this;
    }

    public TestStateBuilder WithTroopsToPlace(byte player, byte troops)
    {
        GameStateHelper.SetPlayerTroopsToPlace(ref _state, player, troops);
        return this;
    }

    public TestStateBuilder WithCard(byte player, byte cardId)
    {
        GameStateHelper.AddCardToPlayer(ref _state, player, cardId);
        return this;
    }

    public GameState Build()
    {
        return _state;
    }
}