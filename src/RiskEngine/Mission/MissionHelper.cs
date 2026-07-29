namespace RiskEngine.Mission;

using System.Runtime.CompilerServices;
using RiskEngine.State;
public static unsafe class MissionHelper
{
    /// <summary>
    /// Gets the assigned mission ID for a specific player from the game state.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetPlayerMission(in GameState state, byte playerIndex)
    {
        return state.PlayerMissions[playerIndex];
    }

    /// <summary>
    /// Checks if all territories owned by a player meet or exceed the required minimum troop count.
    /// </summary>
    public static bool HasMinTroopsOnAllTerritories(in GameState state, byte playerIndex, byte minTroops)
    {
        for (int i = 0; i < EngineConstants.MAX_TERRITORIES; i++)
        {
            if (GameStateHelper.GetTerritoryOwner(in state, i) == playerIndex &&
                GameStateHelper.GetTerritoryTroops(in state, i) < minTroops)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Generates a bitmask representing all continents fully controlled by the player.
    /// Bit n is set to 1 if the player owns every territory in continent n.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetControlledContinentMask(in GameState state, in GameLayout layout, byte playerIndex)
    {
        ulong playerTerritories = GameStateHelper.GetPlayerTerritoriesBitboard(in state, playerIndex);
        byte controlledContinentsMask = 0;

        for (int c = 0; c < layout.Map.ContinentMasks.Length; c++)
        {
            ulong continentMask = layout.Map.ContinentMasks[c];

            // Player controls the continent if their territories fully overlap the precalculated continent mask
            if ((playerTerritories & continentMask) == continentMask)
            {
                controlledContinentsMask |= (byte)(1 << c);
            }
        }

        return controlledContinentsMask;
    }
}