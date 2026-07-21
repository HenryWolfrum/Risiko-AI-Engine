using RiskEngine;

Console.WriteLine("=== RISIKO ENGINE - MAP LAYOUT TEST ===");

// 1. Die Karte wird über unsere statische Fabrik einmalig erzeugt
MapLayout map = RiskMapFactory.CreateStandardRiskMap();

Console.WriteLine($"\n[Karte Geladen]:");
Console.WriteLine($"- Anzahl Kontinente: {map.Continents.Length}");
Console.WriteLine($"- Geladene Territorien: {map.TerritoryNames.Length}");

// 2. Kontinente-Übersicht testen
Console.WriteLine("\n--- Kontinente ---");
foreach (var continent in map.Continents)
{
    Console.WriteLine($"ID {continent.Id}: {continent.Name,-12} | Bonus: +{continent.BonusTroops} Truppen | Länder: {continent.TerritoryCount}");
}

// 3. Graph- & Nachbarschaftstest (Alaska ID 0)
byte alaskaId = 0;
byte northwestId = 1;
byte kamchatkaId = 29;

Console.WriteLine($"\n--- Graph / Nachbarschafts-Test ---");
Console.WriteLine($"Land [0]: {map.TerritoryNames[alaskaId]}");

// Abfrage über das MapLayout
bool grenztAnNorthwest = map.AreNeighbors(alaskaId, northwestId);
bool grenztAnKamchatka = map.AreNeighbors(alaskaId, kamchatkaId);

Console.WriteLine($"Grenzt Alaska an Nordwest-Territorium (ID 1)? -> {grenztAnNorthwest}");
Console.WriteLine($"Grenzt Alaska an Kamtschatka (ID 29)?            -> {grenztAnKamchatka}");
Console.WriteLine($"Grenzt Alaska an Japan (ID 37)?                   -> {map.AreNeighbors(alaskaId, 37)}");

Console.WriteLine("\n=== METADATEN-TEST ERFOLGREICH BEENDET ===");