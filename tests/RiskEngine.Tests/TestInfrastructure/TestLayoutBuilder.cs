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
    /// Builds only the MapLayout without validating the complete GameLayout.
    /// Useful for validator and traverser tests that intentionally use invalid maps.
    /// </summary>
    public MapLayout BuildMap()
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

        var continents = new[]
        {
            new Continent(
                id: 0,
                name: "Test Continent",
                bonusTroops: 0,
                territoryCount: (byte)_territoryCount)
        };

        return new MapLayout(
            territoryNames,
            adjacencyArray,
            territoryToContinent,
            continents);
    }
    /// <summary>
    /// Builds only the DeckLayout.
    /// </summary>
    public DeckLayout BuildDeck()
    {
        var cardTypes = new CardType[_territoryCount + 2];

        for (int i = 0; i < _territoryCount; i++)
        {
            cardTypes[i] = (CardType)(i % 3);
        }

        cardTypes[^2] = CardType.Joker;
        cardTypes[^1] = CardType.Joker;

        return new DeckLayout(cardTypes);
    }
    /// <summary>
    /// Builds only the EngineConfig.
    /// </summary>
    public EngineConfig BuildConfig()
    {
        return new EngineConfig(playerCount: _playerCount);
    }
    
    /// <summary>
    /// Builds the final GameLayout.
    /// </summary>
    public GameLayout Build()
    {
        return new GameLayout(
            BuildMap(),
            BuildDeck(),
            BuildConfig());
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
    /// Adds a one-way connection.
    /// Useful for testing invalid directed maps.
    /// </summary>
    public TestLayoutBuilder ConnectOneWay(byte source, byte target)
    {
        EnsureAdjacencyInitialized();

        _adjacencies[source].Add(target);

        return this;
    }
    
    /// <summary>
    /// Removes an existing undirected connection.
    /// </summary>
    public TestLayoutBuilder Disconnect(byte territoryA, byte territoryB)
    {
        EnsureAdjacencyInitialized();

        _adjacencies[territoryA].Remove(territoryB);
        _adjacencies[territoryB].Remove(territoryA);

        return this;
    }
    
    /// <summary>
    /// Removes every connection from a territory.
    /// Useful for creating disconnected maps.
    /// </summary>
    public TestLayoutBuilder Isolate(byte territory)
    {
        EnsureAdjacencyInitialized();

        foreach (var neighbour in _adjacencies[territory].ToArray())
        {
            _adjacencies[neighbour].Remove(territory);
        }

        _adjacencies[territory].Clear();

        return this;
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