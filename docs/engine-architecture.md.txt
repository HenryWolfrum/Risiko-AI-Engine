# Risiko-Simulations-Engine

Eine performante, speichereffiziente Spiel-Engine für das Strategiespiel Risiko, optimiert für schnelle (KI-)Simulationen.

## Architektur und Design

* **`Program.cs`:** Bisher Testumgebung für Einzelkomponenten. Wird später durch den Haupteinstiegspunkt/Simulation-Loop ersetzt.
* **`MapLayout` (`class`):** Unveränderliches Layout einer Risiko-Karte. Enthält die allgemeine Topologie: Namen der Territorien, Kontinente sowie Nachbarschaften (als flache `byte[]`-Arrays).
* **`RiskMapFactory` (`class`):** Fabrikklasse mit hardcodierten, vorgefertigten Risiko-Karten (z. B. Standard-Weltkarte). Gibt fertig validierte `MapLayout`-Instanzen zurück.
* **`Territory` (`struct`):** Beinhaltet lediglich `OwnerId` und `TroopCount`. Da in Simulationen Millionen Zustände verarbeitet werden, wird auf schwere Daten (wie Namen/Strings) verzichtet – diese liegen im `MapLayout`. Die ID entspricht strikt dem Array-Index.
* **`Continent` (`struct`):** Besitzt ID, Name, Bonus und Länderanzahl. Da es pro Karte nur sehr wenige Kontinente gibt und diese statisch sind, verursacht die Datenstruktur keinen Performance-Flaschenhals.
* **`GamePhase` (`enum`):** Modelliert sauber und speichereffizient (`: byte`) die einzelnen Spielphasen als Zustandsautomat.