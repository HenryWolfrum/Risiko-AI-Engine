# Detailed Test Coverage

## GameInitializer

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


