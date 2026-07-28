namespace RiskEngine.Tests.Helpers;

using RiskEngine;

/// <summary>
/// Fluent builder for creating deterministic GameLayout instances for unit tests.
/// Creates small artificial Risk maps instead of relying on the production map.
/// </summary>
public sealed class TestLayoutBuilder
{
    private int _territoryCount = 2;
    private byte _playerCount = 2;

    private readonly List<HashSet<byte>> _adjacencies = new();

    private TestLayoutBuilder()
    {
    }

    /// <summary>
    /// Creates a new builder.
    /// </summary>
    public static TestLayoutBuilder Create()
    {
        return new TestLayoutBuilder();
    }

    /// <summary>
    /// Defines the number of territories.
    /// </summary>
    public TestLayoutBuilder WithTerritories(int count)
    {
        if (count <= 0 || count > EngineConstants.MAX_TERRITORIES)
            throw new ArgumentOutOfRangeException(nameof(count));

        _territoryCount = count;
        InitializeAdjacencyList();

        return this;
    }

    /// <summary>
    /// Defines the number of players.
    /// </summary>
    public TestLayoutBuilder WithPlayerCount(byte playerCount)
    {
        _playerCount = playerCount;
        return this;
    }

    /// <summary>
    /// Adds an undirected connection.
    /// </summary>
    public TestLayoutBuilder Connect(byte territoryA, byte territoryB)
    {
        EnsureAdjacencyInitialized();

        if (territoryA >= _territoryCount || territoryB >= _territoryCount)
            throw new ArgumentOutOfRangeException();

        _adjacencies[territoryA].Add(territoryB);
        _adjacencies[territoryB].Add(territoryA);

        return this;
    }

    /// <summary>
    /// Builds the final GameLayout.
    /// </summary>
    public GameLayout Build()
    {
        EnsureAdjacencyInitialized();

        // -----------------------
        // Territory names
        // -----------------------

        var territoryNames = new string[_territoryCount];

        for (int i = 0; i < _territoryCount; i++)
        {
            territoryNames[i] = $"Territory{i}";
        }

        // -----------------------
        // Adjacency
        // -----------------------

        var adjacencyArray = new byte[_territoryCount][];

        for (int i = 0; i < _territoryCount; i++)
        {
            adjacencyArray[i] = _adjacencies[i].ToArray();
        }

        // -----------------------
        // Continents
        // -----------------------

        var territoryToContinent = new byte[_territoryCount];

        var continents = new Continent[]
        {
            new Continent(
                id: 0,
                name: "Test Continent",
                bonusTroops: 0,
                territoryCount: (byte)_territoryCount)
        };

        // -----------------------
        // Deck
        // -----------------------

        // One territory card per territory plus two jokers.
        var cardTypes = new CardType[_territoryCount + 2];

        for (int i = 0; i < _territoryCount; i++)
        {
            cardTypes[i] = (CardType)(i % 3);
        }

        cardTypes[^2] = CardType.Joker;
        cardTypes[^1] = CardType.Joker;

        // -----------------------
        // Layout
        // -----------------------

        var map = new MapLayout(territoryNames, adjacencyArray, territoryToContinent, continents);

        var deck = new DeckLayout(cardTypes);

        var config = new EngineConfig(playerCount: _playerCount);

        return new GameLayout(map, deck, config);
    }

    private void InitializeAdjacencyList()
    {
        _adjacencies.Clear();

        for (int i = 0; i < _territoryCount; i++)
        {
            _adjacencies.Add(new HashSet<byte>());
        }
    }

    private void EnsureAdjacencyInitialized()
    {
        if (_adjacencies.Count != _territoryCount)
        {
            InitializeAdjacencyList();
        }
    }

    /// <summary>
    /// Creates a deterministic six-territory Risk-like test map.
    /// </summary>
    public static TestLayoutBuilder CreateSmallRiskLayout(byte playerCount = 2)
    {
        return Create()
            .WithTerritories(6)
            .WithPlayerCount(playerCount)
            .Connect(0, 1)
            .Connect(0, 2)
            .Connect(1, 2)
            .Connect(1, 3)
            .Connect(2, 4)
            .Connect(3, 4)
            .Connect(3, 5)
            .Connect(4, 5);
    }
}