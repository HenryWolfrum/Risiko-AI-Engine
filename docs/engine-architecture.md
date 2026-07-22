# Risiko-Simulations-Engine

Eine performante, speichereffiziente Spiel-Engine für das Strategiespiel Risiko, optimiert für schnelle (KI-)Simulationen, Determinismus und komfortables Replay-Debugging.

---

## 1. Statische Daten & Konfiguration (Layout-Ebene)

Diese Komponenten sind **unveränderlich (immutable)** und existieren während einer Partie nur ein einziges Mal im Speicher.

* **`EngineConstants` (`struct` / `class`):**  
  Zentrale Obergrenzen und Standardwerte (z. B. Min/Max Spieler, max. Runden, Kartenanzahl).
* **`EngineConfig` (`readonly struct`):**  
  Laufzeit-Metadaten einer konkreten Partie (z. B. gewählte `PlayerCount`, `MaxRounds`).
* **`Continent` (`struct`):**  
  Beinhaltet ID, Name, Bonus-Truppen und Länderanzahl.
* **`MapLayout` (`sealed class`):**  
  Unveränderliches Topologie-Layout der Karte: Territorien-Namen, Kontinent-Zuordnungen und Adjazenzlisten (`byte[][]`).
* **`DeckLayout` (`sealed class`):**  
  Statisches Mapping aller Kartentypen (`CardType`) zu den Territorien-IDs inklusive Jokern.
* **`GameLayout` (`sealed class`):**  
  Bündelt `MapLayout`, `DeckLayout` und `EngineConfig`. Bildet das unveränderliche „Regelheft“ für eine laufende Partie.
* **`RiskMapFactory` (`static class`):**  
  Erzeugt vorgefertigte `GameLayout`-Instanzen (z. B. die Standard-Weltkarte) für eine definierte Spieleranzahl.

---

## 2. Dynamischer Spielzustand (State-Ebene)

* **`GameState` (`struct`):**  
  * Schlanker, hochperformanter Daten-Container für den **aktuellen Zustand auf dem Brett**.
  * Enthält flache Arrays für Besitztümer (`TerritoryOwners`), Truppenstärken (`TerritoryTroops`), Spielerzustände (Kartenhand-Anzahl, Reservetruppen, Eliminierungs-Status) sowie Runden- und Phasen-Informationen.
  * **Wichtig:** Keine Objektreferenzen oder Strings! Nur Indizes und Zahlenwerte für maximale Kopiergeschwindigkeit (z. B. bei KI-Baumsuchen).

---

## 3. Logik & Steuerung (Controller-Ebene)

* **`TurnPhase` (`enum : byte`):**  
  Modelliert speichereffizient die einzelnen Phasen eines Zugs (Draft / Place, Attack, Fortify, Card Trade) als Zustandsautomat.
* **`GameAction` (`struct` / `readonly record struct`):**  
  Beschreibt eine konkrete, vom Spieler oder der KI gewählte Aktion (z. B. *Angriff von A nach B mit N Truppen*).
* **`GameController` (`class`):**  
  * Verwaltet das `GameLayout` und hält den **aktuellen `GameState`**.
  * Verarbeitet Züge im Schema `State -> Action -> State`.
  * Validiert Regeln und führt Zustands-Übergänge durch.

---

## 4. Deterministische Simulation & Replay-System

* **`DeterministicRandom` (`class`):**  
  Zentraler Zufallsgenerator, der strikt über einen **Seed** gesteuert wird. Stellt sicher, dass Partien bei gleichem Seed zu 100 % identisch ablaufen.
* **`GameCrashReport` (`class`):**  
  Wird bei Regelverletzungen oder Abstürzen während Massen-Simulationen (z. B. 10.000 Spiele) erzeugt. Speichert lediglich `Seed`, `EngineConfig` und die `ActionHistory`.
* **`GameReplayer` (`static class`):**  
  Lädt ein `GameCrashReport`-Artefakt, baut das Spiel mit demselben Seed nach und spult die Aktionen exakt bis zum Fehlerpunkt vor – die Basis für das spätere Analysepanel im Frontend.