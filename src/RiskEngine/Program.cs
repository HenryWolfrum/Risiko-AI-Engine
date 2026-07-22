using RiskEngine;

Console.WriteLine("=== RISIKO ENGINE - MAP LAYOUT TEST ===");

// 1. Die Karte wird über unsere statische Fabrik einmalig erzeugt
GameLayout game = RiskMapFactory.CreateStandardRiskMap();
MapLayout map = game.Map;
DeckLayout deck = game.Deck;

// Quick Check
Console.WriteLine($"Territorien: {map.TerritoryNames.Length}");
Console.WriteLine($"Kontinente:  {map.Continents.Length}");
Console.WriteLine($"Karten:      {deck.TerritoryToType.Length}");

// Beispiel: Alaska prüfen
Console.WriteLine($"\n[0] {map.TerritoryNames[0]} ({map.Continents[map.TerritoryToContinent[0]].Name})");
Console.WriteLine($"Kartentyp: {deck.TerritoryToType[0]}");
Console.WriteLine($"Nachbar-IDs: {string.Join(", ", map.Adjacencies[0])}");

Console.WriteLine("\nOK!");