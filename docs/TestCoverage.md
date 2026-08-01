# Detailed Test Coverage


## GameInitializer

| Test ID | Description | Status |
|---|---|---|
| INIT-001 | Creates valid initial state with valid territory owners and starting troops | Covered |
| INIT-002 | Assigns territories to every player | Covered |
| INIT-003 | Creates deterministic initial states with identical seeds | Covered |
| INIT-004 | Marks all initialized players as alive | Covered |
| INIT-005 | Assigns every territory exactly once | Covered |
| INIT-006 | Initializes the correct total troop count | Covered |


## GameStateHelper

| Test ID | Description | Status |
|---|---|---|
| STATE-001 | Stores and retrieves territory owners | Covered |
| STATE-002 | Stores and retrieves territory troop counts | Covered |
| STATE-004 | Counts owned territories correctly | Covered |
| STATE-005 | Stores player reinforcement troops | Covered |
| STATE-007 | Creates an empty state without territory owners | Covered |
| STATE-008 | Creates correct player territory bitboards | Covered |
| STATE-009 | Returns zero owned territories for players without territories | Covered |
| STATE-010 | Returns the first owned territory | Covered |
| STATE-011 | Returns NO_VALUE for players without territories | Covered |
| STATE-012 | Tracks player elimination correctly | Covered |
| STATE-013 | Restores eliminated players | Covered |
| STATE-014 | Creates an empty state with zero troops on every territory | Covered |
| STATE-015 | Initializes game progression correctly | Covered |
| STATE-016 | Overwrites territory ownership correctly | Covered |
| STATE-017 | Overwrites territory troop counts correctly | Covered |
| STATE-018 | Overwrites player reinforcement troops correctly | Covered |


## CombatResolver

| Test ID | Description | Status |
|---|---|---|
| COMBAT-001 | Resolves a single dice combat with exactly one troop loss | Covered |
| COMBAT-003 | Resolves multiple dice comparisons correctly | Covered |
| COMBAT-004 | Produces deterministic combat results for identical seeds | Covered |
| COMBAT-005 | Respects the selected attacker and defender dice counts | Covered |
| COMBAT-006 | Resolves the correct number of comparisons for every valid dice combination | Covered |
| COMBAT-007 | Keeps combat losses within valid comparison bounds | Covered |


## AttackRules

| Test ID | Description | Status |
|---|---|---|
| ATTACK-001 | Accepts a valid attack between adjacent enemy territories | Covered |
| ATTACK-002 | Rejects attacks from territories not owned by the active player | Covered |
| ATTACK-003 | Rejects attacks against territories owned by the active player | Covered |
| ATTACK-004 | Rejects attacks between non-adjacent territories | Covered |
| ATTACK-005 | Rejects attacks from territories with fewer than two troops | Covered |
| ATTACK-006 | Rejects attacker dice counts exceeding available troops | Covered |
| ATTACK-007 | Calculates the correct maximum defender dice based on troop count | Covered |
| ATTACK-009 | Rejects attacks where source and target territory are identical | Covered |
| ATTACK-010 | Accepts attacks using the maximum legal attacker dice count | Covered |


## AttackMutator

| Test ID | Description | Status |
|---|---|---|
| MUTATE-ATTACK-001 | Applies attacker troop losses to the source territory | Covered |
| MUTATE-ATTACK-002 | Applies defender troop losses to the target territory | Covered |
| MUTATE-ATTACK-003 | Sets defender troop count to zero when losses equal the remaining troops | Covered |
| MUTATE-ATTACK-004 | Prevents defender troop count underflow when losses exceed the remaining troops | Covered |
| MUTATE-ATTACK-005 | Modifies only the participating territories during combat mutation | Covered |
| MUTATE-ATTACK-006 | Does not modify territory ownership during combat mutation | Covered |
| MUTATE-ATTACK-007 | Leaves troop counts unchanged when no combat losses occur | Covered |
| MUTATE-ATTACK-008 | Applies attacker and defender troop losses in a single combat mutation | Covered |


## ReinforceRules

| Test ID | Description | Status |
|---|---|---|
| REINFORCE-001 | Allows reinforcement on an owned territory with sufficient available troops | Covered |
| REINFORCE-002 | Rejects reinforcement on an enemy territory | Covered |
| REINFORCE-003 | Rejects reinforcement actions with zero troops | Covered |
| REINFORCE-004 | Rejects reinforcement requests exceeding available troops | Covered |
| REINFORCE-005 | Rejects reinforcement on invalid territory ids | Covered |
| REINFORCE-006 | Accepts spending all remaining reinforcement troops | Covered |
| REINFORCE-007 | Produces deterministic validation results for identical inputs | Covered |


## ReinforceMutator

| Test ID | Description | Status |
|---|---|---|
| MUTATE-REINFORCE-001 | Adds reinforcement troops to the selected territory | Covered |
| MUTATE-REINFORCE-002 | Reduces the active player's remaining reinforcement troop pool | Covered |
| MUTATE-REINFORCE-003 | Modifies only the selected territory during reinforcement | Covered |
| MUTATE-REINFORCE-004 | Accumulates multiple reinforcement actions correctly | Covered |
| MUTATE-REINFORCE-005 | Reinforces a territory that initially contains no troops | Covered |
| MUTATE-REINFORCE-006 | Leaves other players' reinforcement troop pools unchanged | Covered |


### FortifyRules

| Test ID | Description | Status |
|---|---|---|
| FORTIFY-001 | Accepts valid fortification between connected owned territories | Covered |
| FORTIFY-002 | Rejects invalid source territory ids | Covered |
| FORTIFY-003 | Rejects invalid target territory ids | Covered |
| FORTIFY-004 | Rejects fortification where source and target are identical | Covered |
| FORTIFY-006 | Rejects fortification when no connected path through owned territories exists | Covered |
| FORTIFY-007 | Rejects moving all troops from the source territory | Covered |
| FORTIFY-008 | Rejects fortification when the source territory is not owned by the active player | Covered |
| FORTIFY-009 | Rejects fortification when the target territory is not owned by the active player | Covered |

## FortifyMutator

| Test ID | Description | Status |
|---|---|---|
| MUTATE-FORTIFY-001 | Removes troops from the source territory during fortification | Covered |
| MUTATE-FORTIFY-002 | Adds troops to the target territory during fortification | Covered |
| MUTATE-FORTIFY-003 | Moves troops between the source and target territories | Covered |
| MUTATE-FORTIFY-004 | Preserves the total troop count during fortification | Covered |
| MUTATE-FORTIFY-005 | Modifies only the participating territories during fortification | Covered |
| MUTATE-FORTIFY-006 | Moves exactly one troop during fortification | Covered |

## ConquerRules

| Test ID | Description | Status |
|---|---|---|
| CONQUER-001 | Accepts valid troop movement after conquering a territory | Covered |
| CONQUER-002 | Rejects invalid source territory ids | Covered |
| CONQUER-003 | Rejects invalid target territory ids | Covered |
| CONQUER-004 | Rejects troop movement when the source territory is not owned by the active player | Covered |
| CONQUER-005 | Rejects troop movement when the target territory has not been conquered yet | Covered |
| CONQUER-006 | Rejects troop movement when only one troop remains in the source territory | Covered |
| CONQUER-007 | Rejects troop movement of zero troops | Covered |
| CONQUER-008 | Rejects troop movement exceeding the maximum movable troops | Covered |
| CONQUER-009 | Accepts moving the maximum legal number of troops after conquering | Covered |

## ConquerMutator

| Test ID | Description | Status |
|---|---|---|
| MUTATE-CONQUER-001 | Removes moved troops from the source territory during conquest | Covered |
| MUTATE-CONQUER-002 | Transfers ownership of the conquered territory to the active player | Covered |
| MUTATE-CONQUER-003 | Places the moved troops onto the conquered territory | Covered |
| MUTATE-CONQUER-004 | Updates both the source and target territories during conquest | Covered |
| MUTATE-CONQUER-005 | Modifies only the participating territories during conquest | Covered |


## TurnInCardsRules

| Test ID | Description | Status |
|---|---|---|
| TURNIN-001 | Accepts a valid card set consisting of one Infantry, one Cavalry and one Artillery card | Covered |
| TURNIN-002 | Accepts any valid card set containing a Joker | Covered |
| TURNIN-003 | Rejects card ids outside the deck | Covered |
| TURNIN-004 | Rejects card sets containing cards not owned by the active player | Covered |
| TURNIN-005 | Rejects invalid card combinations consisting of two equal card types and one different card type without a Joker | Covered |

## CardTurnInMutator

| Test ID | Description | Status |
|---|---|---|
| MUTATE-TURNIN-001 | Removes all traded cards from the active player's hand | Covered |
| MUTATE-TURNIN-002 | Increases the traded card set counter | Covered |
| MUTATE-TURNIN-003 | Awards the correct reinforcement bonus for the first traded set | Covered |
| MUTATE-TURNIN-004 | Adds the reinforcement bonus to the player's existing reinforcement troop pool | Covered |
| MUTATE-TURNIN-005 | Awards the territory ownership bonus for owned territory cards | Covered |
| MUTATE-TURNIN-006 | Does not award the territory ownership bonus for territories not owned by the active player | Covered |


## RuleValidator

| Test ID | Description | Status |
|---|---|---|
| VALIDATOR-001 | Accepts reinforcement actions during the Reinforce phase and forwards them to `ReinforceRules` | Covered |
| VALIDATOR-002 | Rejects reinforcement actions outside the Reinforce phase before rule validation | Covered |
| VALIDATOR-003 | Accepts `SkipPhase` during the Attack phase without additional rule validation | Covered |
| VALIDATOR-004 | Accepts `EndTurn` during the Attack phase without additional rule validation | Covered |
| VALIDATOR-005 | Forwards valid attack actions to `AttackRules` and returns the underlying validation result unchanged | Covered |


## MapTraverser

| Test ID | Description | Status |
|---|---|---|
| PATH-001 | Finds a direct path between two adjacent territories owned by the same player | Covered |
| PATH-002 | Finds an indirect path through multiple connected territories owned by the same player | Covered |
| PATH-003 | Returns no path when no owned connection exists between source and target | Covered |
| PATH-004 | Rejects path search when the source territory is not owned by the requested player | Covered |
| PATH-005 | Rejects path search when the target territory is not owned by the requested player | Covered |
| PATH-006 | Returns a valid path when source and target territory are identical | Covered |
| CONN-001 | Confirms connectivity for a map where all territories are transitively connected | Covered |
| CONN-002 | Rejects maps split into disconnected components (islands) | Covered |
| CONN-003 | Confirms connectivity for a single-territory map | Covered |
| CONN-004 | Rejects empty maps with zero territories | Covered |
| CONN-005 | Rejects maps containing an isolated territory with no connections | Covered |
| CONN-006 | Rejects directed graphs where territories are unreachable from starting territory 0 | Covered |
| UNDIR-001 | Confirms an undirected graph when all adjacent territory links are symmetric | Covered |
| UNDIR-002 | Rejects a graph containing a single one-way (directed) connection | Covered |
| UNDIR-003 | Confirms an isolated single-territory map as undirected | Covered |
| UNDIR-004 | Confirms an empty map with zero territories as undirected | Covered |
| UNDIR-005 | Rejects a complex graph when a single reverse connection is missing | Covered |


## AttackOptionGenerator

| Test ID | Description | Status |
|---|---|---|
| ATTACKGEN-001 | Generates exactly one attack decision and one skip decision for a single attack opportunity | Covered |
| ATTACKGEN-002 | Generates all possible attacks from a territory with multiple attackable neighbours | Covered |
| ATTACKGEN-003 | Generates attacks from multiple eligible source territories | Covered |
| ATTACKGEN-004 | Generates no attack options when none are possible, but keeps phase control decisions | Covered |
| ATTACKGEN-005 | Ignores friendly (own) neighbouring territories | Covered |
| ATTACKGEN-006 | Does not generate an attack from a source territory with only a single troop | Covered |
| ATTACKGEN-007 | Never generates duplicate attack decisions | Covered |

## CardTurnInOptionGenerator

| Test ID | Description | Status |
|---|---|---|
| CARDTURNINGEN-001 | Generates a single valid card set plus SkipPhase when trade is optional | Covered |
| CARDTURNINGEN-002 | Generates all valid card sets held by the player | Covered |
| CARDTURNINGEN-003 | Ignores invalid card combinations, still generates SkipPhase | Covered |
| CARDTURNINGEN-004 | Omits SkipPhase when a card trade is mandatory | Covered |
| CARDTURNINGEN-005 | Never generates duplicate card turn-in decisions | Covered |

## FortifyOptionGenerator

| Test ID | Description | Status |
|---|---|---|
| FORTIFYGEN-001 | Generates a single fortify option between connected owned territories | Covered |
| FORTIFYGEN-002 | Does not fortify across territory owned by an enemy (breaks connectivity) | Covered |
| FORTIFYGEN-003 | Generates all reachable fortify targets | Covered |
| FORTIFYGEN-004 | Ignores source territories with only a single troop | Covered |

## ReinforcementOptionGenerator

| Test ID | Description | Status |
|---|---|---|
| REINFORCEGEN-001 | Generates a reinforcement option for every owned territory | Covered |
| REINFORCEGEN-002 | Returns no reinforcement options when no troops remain | Covered |
| REINFORCEGEN-003 | Generates the correct parameter range for available troops | Covered |
| REINFORCEGEN-004 | Never generates duplicate reinforcement options | Covered |

## ConquerOptionGenerator

| Test ID | Description | Status |
|---|---|---|
| — | No tests. Reads `state.AttackerTerritory`, which is currently set incorrectly and reset too early in `AttackExecutor` (see gap analysis above) | **Missing** |

## DefendOptionGenerator

| Test ID | Description | Status |
|---|---|---|
| — | No tests. Reads `state.DefenderTerritory`, affected by the same premature-reset issue in `AttackExecutor` | **Missing** |


## MissionEvaluator

| Test ID | Description | Status |
|---|---|---|
| MISSION-001 | WorldDomination mission requires ownership of every territory on the map | Covered |
| MISSION-002 | ConquerTerritories mission is fulfilled once the required territory count is reached | Covered |
| MISSION-003 | ConquerTerritories with minimum troops fails if any owned territory is below the minimum | Covered |
| MISSION-004 | ConquerContinents mission requires all target continents to be owned | Covered |
| MISSION-005 | EliminatePlayer mission depends only on the target's alive status | Covered |
| MISSION-006 | CheckEliminationWin only matches the correct mission target | Covered |

## CardHelper

| Test ID | Description | Status |
|---|---|---|
| CARD-001 | Adds a card to a player's hand | Covered |
| CARD-002 | Removes a card from a player's hand | Covered |
| CARD-003 | Counts a player's cards correctly | Covered |
| CARD-004 | Returns the correct player card bitboard | Covered |
| CARD-005 | Transfers cards on player elimination | Covered |
| CARD-006 | Returns the currently available (undealt) cards | Covered |
| CARD-007 | Gives a bonus card to a player | Covered |
| CARD-008 | Detects a set of three Infantry cards as valid | Covered |


## ActionFactory / ActionFactoryHelper

| Test ID | Description | Status |
|---|---|---|
| — | No tests. Boundary-checking logic (`OptionIndex` out of range, `ChosenParameter` outside `[Min..Max]`) and the mapping from every `DecisionKind` to a concrete `GameAction` are currently unverified | **Missing** |

## ReinforcementCalculator

| Test ID | Description | Status |
|---|---|---|
| — | No tests. Base income (`floor(owned/3)`, minimum threshold) and continent-bonus aggregation via bitboard masks are currently unverified | **Missing** |

## Execution (TurnExecutor, AttackExecutor, ConquerExecutor, ReinforceExecutor, FortifyExecutor, CardTurnInExecutor)

| Test ID | Description | Status |
|---|---|---|
| — | No tests anywhere in this layer. This is where the confirmed `AttackerTerritory`/`DefenderTerritory` cache bug lives (see gap analysis above); it also owns turn/phase sequencing, conquest, elimination, and bonus-card awarding | **Missing** |

## GameRunner

| Test ID | Description | Status |
|---|---|---|
| — | No tests. Main game loop (turn/round advancement, termination on max rounds, win detection, last-player-standing handling) is currently only exercised manually via `RiskEngine.Simulations` | **Missing** |

## Niedrige Priorität (nicht dringend)

| Component | Notes |
|---|---|
| GameLayoutValidator | Validiert statische Map-Daten einmalig beim Start; geringes Änderungsrisiko |
| RiskMapFactory | Erzeugt die feste Standardkarte; am ehesten indirekt über GameLayoutValidator abgedeckt |
| EngineRandom | Dünner RNG-Wrapper; Determinismus wird bereits indirekt über INIT-003/COMBAT-004 mitgeprüft |
| AttackHelper | `CanPlayerAttack`-Logik, wird von `AttackExecutor` verwendet; sollte mitziehen, sobald Execution getestet wird |
| MissionCatalog / MissionHelper | Datencontainer bzw. Hilfsfunktionen um `MissionEvaluator`, das selbst gut getestet ist |
