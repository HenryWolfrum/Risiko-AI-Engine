namespace RiskEngine.State.Tests.State;

using RiskEngine.State;
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
        for (byte territory = 0; territory < EngineConstants.MAX_TERRITORIES; territory++)
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
     * Eliminated players can be restored.
     *
     * Guarantees:
     * - eliminated players can be marked alive again
     * - alive bitboard is updated correctly
     */
    [Fact]
    public void STATE_013_ShouldRestoreEliminatedPlayer()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(3);

        GameStateHelper.EliminatePlayer(ref state, 1);

        // Act
        GameStateHelper.SetPlayerAlive(ref state, 1);

        // Assert
        Assert.True(GameStateHelper.IsPlayerAlive(in state, 1));
        Assert.Equal(3, GameStateHelper.GetActivePlayerCount(in state));
    }
   
    /*
     * STATE-014
     *
     * Empty GameState should initialize all territories
     * with zero troops.
     *
     * Guarantees:
     * - every territory starts with zero troops
     */
    [Fact]
    public void STATE_014_EmptyState_ShouldInitializeZeroTroops()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        // Assert
        for (byte territory = 0; territory < EngineConstants.MAX_TERRITORIES; territory++)
        {
            Assert.Equal(
                0,
                GameStateHelper.GetTerritoryTroops(in state, territory));
        }
    }
    
    
    /*
     * STATE-015
     *
     * Empty GameState should initialize
     * game progression correctly.
     *
     * Guarantees:
     * - game starts in round one
     * - game starts in default phase
     */
    [Fact]
    public void STATE_015_EmptyState_ShouldInitializeGameProgress()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        // Assert
        Assert.Equal(1, state.CurrentRound);
        Assert.Equal(GamePhase.Default, state.CurrentPhase);
    }
    
    /*
     * STATE-016
     *
     * Territory ownership can be updated.
     *
     * Guarantees:
     * - owner changes overwrite previous values
     * - getter always returns the latest owner
     */
    [Fact]
    public void STATE_016_ShouldOverwriteTerritoryOwner()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryOwner(ref state, 3, 0);

        // Act
        GameStateHelper.SetTerritoryOwner(ref state, 3, 1);

        // Assert
        Assert.Equal(1, GameStateHelper.GetTerritoryOwner(in state, 3));
    }
    
    
    /*
     * STATE-017
     *
     * Territory troop counts can be updated.
     *
     * Guarantees:
     * - troop changes overwrite previous values
     * - getter always returns the latest troop count
     */
    [Fact]
    public void STATE_017_ShouldOverwriteTerritoryTroops()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetTerritoryTroops(ref state, 5, 3);

        // Act
        GameStateHelper.SetTerritoryTroops(ref state, 5, 10);

        // Assert
        Assert.Equal(
            10,
            GameStateHelper.GetTerritoryTroops(in state, 5));
    }
    
    /*
     * STATE-018
     *
     * Player reinforcement troops can be updated.
     *
     * Guarantees:
     * - reinforcement updates overwrite previous values
     * - getter always returns the latest value
     */
    [Fact]
    public void STATE_018_ShouldOverwritePlayerReinforcements()
    {
        // Arrange
        var state = GameStateHelper.CreateEmpty(2);

        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 5);

        // Act
        GameStateHelper.SetPlayerTroopsToPlace(ref state, 0, 12);

        // Assert
        Assert.Equal(12, GameStateHelper.GetPlayerTroopsToPlace(in state, 0));
    }
}