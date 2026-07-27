namespace RiskEngine.Tests.Helpers;

using RiskEngine;

/// <summary>
/// Fluent builder for creating deterministic GameLayout instances for unit tests.
/// Creates small artificial Risk maps instead of relying on the full production map.
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
    /// Creates a new test layout builder instance.
    /// </summary>
    public static TestLayoutBuilder Create()
    {
        return new TestLayoutBuilder();
    }


    /// <summary>
    /// Defines the number of territories in the test map.
    /// </summary>
    public TestLayoutBuilder WithTerritories(int count)
    {
        if (count <= 0 || count > EngineConstants.DEFAULT_TERRITORY_COUNT) 
            throw new ArgumentOutOfRangeException(nameof(count));

        _territoryCount = count;

        InitializeAdjacencyList();

        return this;
    }


    /// <summary>
    /// Defines the amount of players for the test configuration.
    /// </summary>
    public TestLayoutBuilder WithPlayerCount(byte playerCount)
    {
        _playerCount = playerCount;

        return this;
    }


    /// <summary>
    /// Adds an undirected connection between two territories.
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
    /// Builds the final GameLayout instance.
    /// </summary>
    public GameLayout Build()
    {
        EnsureAdjacencyInitialized();


        var territoryNames = new string[_territoryCount];

        for (byte i = 0; i < _territoryCount; i++)
        {
            territoryNames[i] = $"Territory{i}";
        }


        var adjacencyArray = new byte[_territoryCount][];

        for (var i = 0; i < _territoryCount; i++)
        {
            adjacencyArray[i] = _adjacencies[i].ToArray();
        }


        // All territories belong to one test continent
        var territoryToContinent = new byte[_territoryCount];


        var continents = new[]
        {
            new Continent(
                0,
                "Test Continent",
                0,
                (byte)_territoryCount)
        };


        // One card per territory + two jokers
        var cardTypes = new CardType[_territoryCount + 2];

        for (var i = 0; i < _territoryCount; i++)
        {
            cardTypes[i] = CardType.Infantry;
        }

        cardTypes[_territoryCount] = CardType.Joker;
        cardTypes[_territoryCount + 1] = CardType.Joker;


        var map = new MapLayout(territoryNames, adjacencyArray, territoryToContinent, continents);


        var deck = new DeckLayout(cardTypes);


        var config = new EngineConfig(_playerCount, (byte)_territoryCount);


        return new GameLayout(map, deck, config);
    }


    private void InitializeAdjacencyList()
    {
        _adjacencies.Clear();

        for (var i = 0; i < _territoryCount; i++)
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
}