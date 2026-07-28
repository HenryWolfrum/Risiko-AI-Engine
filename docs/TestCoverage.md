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

### Reinforcement Mutation

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

### ConquerRules

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

