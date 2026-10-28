using RiskEngine.Cards;
using RiskEngine.Constants;
using RiskEngine.Core;
using RiskEngine.Layout;
using RiskEngine.Mission;

namespace RiskEngine.Tests.TestInfrastructure;

/// <summary>
///     Fluent builder for creating deterministic GameLayout instances for unit tests.
///     Creates small artificial Risk maps instead of relying on the production map.
/// </summary>
public sealed class TestLayoutBuilder
{
    private readonly List<HashSet<byte>> _adjacencies = new();
    private byte _playerCount = 2;
    private int _territoryCount = 2;

    private MissionCatalog _catalog;
    private bool _hasCustomCatalog; // Flag, ob ein eigener Katalog gesetzt wurde
    private byte _fallbackMissionId = 0;

    private TestLayoutBuilder()
    {
    }

    /// <summary>
    ///     Creates a new builder.
    /// </summary>
    public static TestLayoutBuilder Create()
    {
        return new TestLayoutBuilder();
    }

    /// <summary>
    ///     Defines the number of territories.
    /// </summary>
    public TestLayoutBuilder WithTerritories(int count)
    {
        _territoryCount = count;
        InitializeAdjacencyList();

        return this;
    }

    /// <summary>
    ///     Defines the number of players.
    /// </summary>
    public TestLayoutBuilder WithPlayerCount(byte playerCount)
    {
        _playerCount = playerCount;
        return this;
    }

    /// <summary>
    ///     Sets a custom mission catalog with optional fallback ID.
    /// </summary>
    public TestLayoutBuilder WithMissionCatalog(MissionDefinition[] missions, byte fallbackMissionId = 0)
    {
        if (missions == null || missions.Length < 1)
        {
            throw new ArgumentException("Catalogue array must contain at least one MissionDefinition", nameof(missions));
        }
        
        _fallbackMissionId = fallbackMissionId;
        _catalog = new MissionCatalog(missions, _fallbackMissionId);
        _hasCustomCatalog = true; // Aktivieren
        return this;
    }

    /// <summary>
    ///     Shorthand to define just a custom fallback mission ID for default catalogs.
    /// </summary>
    public TestLayoutBuilder WithFallbackMissionId(byte fallbackMissionId)
    {
        _fallbackMissionId = fallbackMissionId;
        return this;
    }

    /// <summary>
    ///     Adds an undirected connection.
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
    ///     Builds only the MapLayout without validating the complete GameLayout.
    /// </summary>
    public MapLayout BuildMap()
    {
        EnsureAdjacencyInitialized();

        var territoryNames = new string[_territoryCount];
        for (var i = 0; i < _territoryCount; i++) territoryNames[i] = $"Territory{i}";

        var adjacencyArray = new byte[_territoryCount][];
        for (var i = 0; i < _territoryCount; i++) adjacencyArray[i] = _adjacencies[i].ToArray();

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
    ///     Builds only the DeckLayout.
    /// </summary>
    public DeckLayout BuildDeck()
    {
        var cardTypes = new CardType[_territoryCount + 2];

        for (var i = 0; i < _territoryCount; i++) cardTypes[i] = (CardType)(i % 3);

        cardTypes[^2] = CardType.Joker;
        cardTypes[^1] = CardType.Joker;

        return new DeckLayout(cardTypes);
    }

    /// <summary>
    ///     Builds only the EngineConfig.
    /// </summary>
    public EngineConfig BuildConfig()
    {
        return new EngineConfig(playerCount: _playerCount, EngineConstants.MAX_ROUNDS);
    }

    /// <summary>
    ///     Builds the MissionCatalog (returns the custom one if provided, otherwise builds a default one).
    /// </summary>
    public MissionCatalog BuildMissionCatalog()
    {
        if (_hasCustomCatalog)
        {
            return _catalog;
        }

        var missions = new MissionDefinition[_playerCount];

        for (byte i = 0; i < _playerCount; i++)
            missions[i] = new MissionDefinition
            {
                Id = i,
                Type = MissionType.WorldDomination
            };

        return new MissionCatalog(missions, _fallbackMissionId);
    }

    /// <summary>
    ///     Builds the final GameLayout.
    /// </summary>
    public GameLayout Build()
    {
        return new GameLayout(BuildMap(), BuildDeck(), BuildConfig(), BuildMissionCatalog());
    }

    private void InitializeAdjacencyList()
    {
        _adjacencies.Clear();

        for (var i = 0; i < _territoryCount; i++) _adjacencies.Add(new HashSet<byte>());
    }

    private void EnsureAdjacencyInitialized()
    {
        if (_adjacencies.Count != _territoryCount) InitializeAdjacencyList();
    }

    /// <summary>
    ///     Adds a one-way connection.
    /// </summary>
    public TestLayoutBuilder ConnectOneWay(byte source, byte target)
    {
        EnsureAdjacencyInitialized();

        _adjacencies[source].Add(target);

        return this;
    }
 
    /// <summary>
    ///     Erstellt automatisch eine valide, zirkulär verknüpfte Map mit beliebiger Territorien-Anzahl.
    ///     Jedes Territorium i ist mit (i + 1) % count verbunden.
    /// </summary>
    public static TestLayoutBuilder CreateCircularLayout(int territoryCount, byte playerCount = 2)
    {
        var builder = Create()
            .WithTerritories(territoryCount)
            .WithPlayerCount(playerCount);

        for (int i = 0; i < territoryCount; i++)
        {
            byte current = (byte)i;
            byte next = (byte)((i + 1) % territoryCount);
            builder.Connect(current, next);
        }

        return builder;
    }

    /// <summary>
    ///     Creates a deterministic six-territory Risk-like test map.
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