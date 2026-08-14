using System.Collections.Generic;
using RiskEngine.Mission;
using RiskEngine.State;

namespace RiskEngine.Replay.GUI;

public static class MissionTranslator
{
    public static string TranslateMission(MissionDefinition mission, GameLayout layout)
    {
        switch (mission.Type)
        {
            case MissionType.WorldDomination:
                return "Erlangen Sie die Weltherrschaft!";

            case MissionType.EliminatePlayer:
                var target = mission.TargetPlayerId;
                return $"Eliminieren Sie Spieler {target}!";

            case MissionType.ConquerTerritories:
                var totalCount = mission.RequiredTerritories;
                var minTroops = mission.MinTroopsPerTerritory;
                
                if (minTroops <= 1)
                {
                    return $"Erobern Sie {totalCount} Gebiete Ihrer Wahl!";
                }
                
                return $"Kontrollieren Sie {totalCount} Gebiete mit je {minTroops} Truppen!";

            case MissionType.ConquerContinents:
                var targetContinentMask = mission.TargetContinentMask;
                var continents = layout.Map.Continents;
                List<string> targetNames = new();

                // Bitmaske iterieren und geforderte Kontinente ermitteln
                for (int i = 0; i < continents.Length; i++)
                {
                    if ((targetContinentMask & (1UL << i)) != 0)
                    {
                        targetNames.Add(continents[i].Name);
                    }
                }

                if (targetNames.Count == 0)
                {
                    return "Erobern Sie die geforderten Kontinente!";
                }
                
                string continentsJoined = targetNames.Count > 1
                    ? string.Join(", ", targetNames.GetRange(0, targetNames.Count - 1)) + " und " + targetNames[^1]
                    : targetNames[0];

                return $"Erobern Sie {continentsJoined}!";

            default:
                return "Unbekannte Missionsart";
        }
    }
}