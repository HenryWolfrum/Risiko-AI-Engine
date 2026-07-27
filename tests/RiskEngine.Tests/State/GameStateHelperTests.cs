namespace RiskEngine.Tests.State;

using RiskEngine;
using Xunit;

public class GameStateHelperTests
{
    /*
     * STATE-001
     *
     * Territory ownership should be stored and retrieved correctly.
     *
     * Guarantees:
     * - owner assignment is persistent
     * - getter returns assigned owner
     */
    [Fact]
    public void STATE_001_ShouldStoreAndRetrieveTerritoryOwner()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);


        // Act
        GameStateHelper.SetTerritoryOwner(ref state, 3, 1);


        // Assert
        var owner = GameStateHelper.GetTerritoryOwner(in state, 3);

        Assert.Equal(1, owner);
    }


    /*
     * STATE-002
     *
     * Territory troop values should be stored and retrieved correctly.
     *
     * Guarantees:
     * - troop mutations affect only selected territory
     */
    [Fact]
    public void STATE_002_ShouldStoreAndRetrieveTerritoryTroops()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);


        // Act
        GameStateHelper.SetTerritoryTroops(ref state, 5, 10);


        // Assert
        var troops = GameStateHelper.GetTerritoryTroops(in state, 5);

        Assert.Equal(10, troops);
    }


    /*
     * STATE-003
     *
     * Player alive bitboard should correctly track active players.
     *
     * Guarantees:
     * - players can be marked alive
     * - eliminated players are removed
     */
    [Fact]
    public void STATE_003_PlayerAliveBitboard_ShouldTrackPlayerStatus()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(4);


        // Assert initial state
        Assert.True(GameStateHelper.IsPlayerAlive(in state, 0));


        // Act
        GameStateHelper.EliminatePlayer(ref state, 0);


        // Assert
        Assert.False(GameStateHelper.IsPlayerAlive(in state, 0));
    }


    /*
     * STATE-004
     *
     * Owned territory count should match actual ownership.
     *
     * Guarantees:
     * - territory counting works correctly
     * - bitboard calculation is correct
     */
    [Fact]
    public void STATE_004_ShouldCountOwnedTerritories()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 2, 1);


        // Act
        var count = GameStateHelper.GetOwnedTerritoryCount(in state, 0);


        // Assert
        Assert.Equal(2, count);
    }


    /*
     * STATE-005
     *
     * Player reinforcement troops should be stored correctly.
     */
    [Fact]
    public void STATE_005_ShouldStorePlayerReinforcements()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);


        // Act
        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 15);


        // Assert
        Assert.Equal(
            15,
            GameStateHelper.GetPlayerTroopsToPlace(in state, 0));
    }


    /*
     * STATE-006
     *
     * Player card bitboard should correctly add and query cards.
     *
     * Guarantees:
     * - cards can be assigned
     * - card lookup works
     */
    [Fact]
    public void STATE_006_PlayerCards_ShouldBeStoredInBitboard()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);


        // Act
        GameStateHelper.AddCardToPlayer(ref state, 0, 5);


        // Assert
        Assert.True(
            GameStateHelper.PlayerHasCard(in state, 0, 5));
    }
    
    /*
     * STATE-007
     *
     * Empty GameState should not assign territories.
     *
     * Guarantees:
     * - empty state uses NO_VALUE for territory ownership
     */
    [Fact]
    public void STATE_007_EmptyState_ShouldHaveNoTerritoryOwners()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);


        // Assert
        for (byte territory = 0; territory < EngineConstants.DEFAULT_TERRITORY_COUNT; territory++)
        {
            Assert.Equal(EngineConstants.NO_VALUE, GameStateHelper.GetTerritoryOwner(in state, territory));
        }
    }
    
                /*
     * STATE-008
     *
     * Player territory bitboard must represent
     * exactly the territories owned by a player.
     *
     * Guarantees:
     * - owned territories are included
     * - foreign territories are excluded
     */
    [Fact]
    public void STATE_008_ShouldCreateCorrectPlayerTerritoryBitboard()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);
    
        GameStateHelper.SetTerritoryOwner(ref state, 0, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);
        GameStateHelper.SetTerritoryOwner(ref state, 2, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 3, 1);
    
    
        // Act
        var player0Territories =
            GameStateHelper.GetPlayerTerritoriesBitboard(in state, 0);
    
        var player1Territories =
            GameStateHelper.GetPlayerTerritoriesBitboard(in state, 1);
    
    
        // Assert
        Assert.Equal(0b0101UL, player0Territories);
        Assert.Equal(0b1010UL, player1Territories);
    }
    
    
    /*
     * STATE-009
     *
     * A player without territories must have
     * an owned territory count of zero.
     *
     * Guarantees:
     * - eliminated players are handled correctly
     */
    [Fact]
    public void STATE_009_PlayerWithoutTerritories_ShouldReturnZeroOwnedTerritories()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);
    
        GameStateHelper.SetTerritoryOwner(ref state, 0, 1);
        GameStateHelper.SetTerritoryOwner(ref state, 1, 1);
    
    
        // Act
        var count =
            GameStateHelper.GetOwnedTerritoryCount(in state, 0);
    
    
        // Assert
        Assert.Equal(0, count);
    }
    
    
    /*
     * STATE-010
     *
     * GetFirstTerritoryOwnedBy should return
     * the lowest owned territory id.
     *
     * Guarantees:
     * - first owned territory can be found
     * - missing ownership returns NO_VALUE
     */
    [Fact]
    public void STATE_010_ShouldReturnFirstOwnedTerritory()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);
    
        GameStateHelper.SetTerritoryOwner(ref state, 5, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 2, 0);
        GameStateHelper.SetTerritoryOwner(ref state, 8, 1);
    
    
        // Act
        var firstTerritory = GameStateHelper.GetFirstTerritoryOwnedBy(in state, 0);
    
    
        // Assert
        Assert.Equal(2, firstTerritory);
    }
    
    
    /*
     * STATE-011
     *
     * A player without territories should return NO_VALUE.
     *
     * Guarantees:
     * - missing territories are not confused with territory 0
     */
    [Fact]
    public void STATE_011_PlayerWithoutTerritories_ShouldReturnNoValue()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);
    
    
        // Act
        var territory =
            GameStateHelper.GetFirstTerritoryOwnedBy(in state, 0);
    
    
        // Assert
        Assert.Equal(EngineConstants.NO_VALUE, territory);
    }
    
    
    /*
     * STATE-012
     *
     * Player alive bitboard operations must correctly
     * track active and eliminated players.
     *
     * Guarantees:
     * - alive players are detected
     * - eliminated players are removed
     */
    [Fact]
    public void STATE_012_PlayerAliveBitboard_ShouldTrackElimination()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(3);
    
    
        // Act
        GameStateHelper.EliminatePlayer(ref state, 1);
    
    
        // Assert
        Assert.True(GameStateHelper.IsPlayerAlive(in state, 0));
        Assert.False(GameStateHelper.IsPlayerAlive(in state, 1));
        Assert.True(GameStateHelper.IsPlayerAlive(in state, 2));
    
        Assert.Equal(2, GameStateHelper.GetActivePlayerCount(in state));
    }
    
    
    /*
     * STATE-013
     *
     * Card helper operations must correctly
     * add, remove and count cards.
     *
     * Guarantees:
     * - cards can be assigned
     * - card ownership can be queried
     * - card removal works
     */
    [Fact]
    public void STATE_013_CardOperations_ShouldMaintainCorrectCardState()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);
    
    
        // Act
        GameStateHelper.AddCardToPlayer(ref state, 0, 5);
        GameStateHelper.AddCardToPlayer(ref state, 0, 10);
    
    
        // Assert
        Assert.True(GameStateHelper.PlayerHasCard(in state, 0, 5));
        Assert.True(GameStateHelper.PlayerHasCard(in state, 0, 10));
    
        Assert.Equal(2, GameStateHelper.GetPlayerCardCount(in state, 0));
    
    
        // Act
        GameStateHelper.RemoveCardFromPlayer(ref state, 0, 5);
    
    
        // Assert
        Assert.False(GameStateHelper.PlayerHasCard(in state, 0, 5));
        Assert.Equal(1, GameStateHelper.GetPlayerCardCount(in state, 0));
    }
}