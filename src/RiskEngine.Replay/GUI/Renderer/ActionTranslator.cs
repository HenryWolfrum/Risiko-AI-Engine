using System;
using RiskEngine.State;

namespace RiskEngine.Replay.GUI;

public static class ActionTranslator
{
    public static string TranslateAction(GameAction action, GameLayout layout, byte playerId)
    {
        var names = layout.Map.TerritoryNames;
        string playerTag = $"P{playerId}";

        switch (action.Type)
        {
            case ActionType.TurnInCards:
                string desc1 = FormatCard(action.Card1, layout);
                string desc2 = FormatCard(action.Card2, layout);
                string desc3 = FormatCard(action.Card3, layout);

                return $"({playerTag}) traded Cards: {desc1}, {desc2}, {desc3}";

            case ActionType.Reinforce:
                string targetTerritory = GetTerritoryName(names, action.TargetTerritory);
                return $"({playerTag}) reinforces {targetTerritory} +{action.TroopCount}";

            case ActionType.Attack:
                string attacker = GetTerritoryName(names, action.SourceTerritory);
                string defender = GetTerritoryName(names, action.TargetTerritory);
                return $"({playerTag}) attacks: {attacker} -> {defender}";

            case ActionType.Conquer:
                string conquered = GetTerritoryName(names, action.TargetTerritory);
                return $"({playerTag}) conquered {conquered}! Moved in {action.TroopCount} troops";

            case ActionType.Fortify:
                string from = GetTerritoryName(names, action.SourceTerritory);
                string to = GetTerritoryName(names, action.TargetTerritory);
                return $"({playerTag}) moves {action.TroopCount} troops: {from} -> {to}";

            default:
                return $"({playerTag}) executed {action.Type}";
        }
    }

    /// <summary>
    /// Liest den Territoriumsnamen sicher aus, um IndexOutOfRangeException bei Platzhaltern (z.B. 255) zu verhindern.
    /// </summary>
    private static string GetTerritoryName(string[] names, byte territoryId)
    {
        if (territoryId >= names.Length)
            return $"Territory #{territoryId}";

        return names[territoryId];
    }

    /// <summary>
    /// Formatiert eine Karte sicher und fängt Joker/Wildcards sowie ungültige Indizes ab.
    /// </summary>
    private static string FormatCard(byte cardId, GameLayout layout)
    {
        var territoryNames = layout.Map.TerritoryNames;

        // Prüfen, ob die ID außerhalb der Territorien liegt (z.B. Joker)
        if (cardId >= territoryNames.Length || cardId >= layout.Deck.TerritoryToType.Length)
        {
            return "Joker";
        }

        string name = territoryNames[cardId];
        var type = layout.Deck.TerritoryToType[cardId];

        return $"{name} ({type})";
    }
}