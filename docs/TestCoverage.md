## Detailed Test Coverage


### GameInitializer

| Test ID | Description | Status |
|---|---|---|
| INIT-001 | Creates valid initial state with valid territory owners and starting troops | Covered |
| INIT-002 | Assigns territories to every player | Covered |
| INIT-003 | Creates deterministic initial states with identical seeds | Covered |
| INIT-004 | Marks all initialized players as alive | Covered |


### GameStateHelper

The GameStateHelper layer provides the central access and mutation interface
for the GameState structure.

These tests guarantee that state handling remains deterministic and consistent.
This layer is critical for:

- game simulations
- replay systems
- AI search algorithms
- large scale bot evaluation


| Test ID | Description | Status |
|---|---|---|
| STATE-001 | Stores and retrieves territory ownership correctly | Covered |
| STATE-002 | Stores and retrieves territory troop values correctly | Covered |
| STATE-003 | Tracks player alive state through bitboard operations | Covered |
| STATE-004 | Counts owned territories correctly | Covered |
| STATE-005 | Stores player reinforcement troops correctly | Covered |
| STATE-006 | Stores and queries player cards through bitboards | Covered |
| STATE-007 | Empty GameState initializes territories with NO_VALUE ownership | Covered |
| STATE-008 | Creates correct player territory bitboards | Covered |
| STATE-009 | Returns zero owned territories for players without territories | Covered |
| STATE-010 | Returns the first owned territory correctly | Covered |
| STATE-011 | Returns NO_VALUE when no territory exists | Covered |
| STATE-012 | Tracks player elimination and active player count correctly | Covered |
| STATE-013 | Adds, removes and counts player cards correctly | Covered |


## CombatResolver Coverage

| Test ID | Description | Status |
|---|---|---|
| COMBAT-001 | Single dice combat resolves correctly | Covered |
| COMBAT-002 | Equal dice rolls cause attacker losses according to Risk rules | Covered |
| COMBAT-003 | Multiple dice comparisons resolve correctly | Covered |
| COMBAT-004 | Same seed produces identical combat results | Covered |
| COMBAT-005 | Resolver respects selected dice counts | Covered |


## AttackRules Coverage

| Test ID    | Description |
|------------|---|
| ATTACK-001 | Valid adjacent attack is accepted |
| ATTACK-002 | Source territory must belong to active player |
| ATTACK-003 | Cannot attack own territory |
| ATTACK-004 | Non adjacent territories are rejected |
| ATTACK-005 | Territory needs minimum troops to attack |
| ATTACK-006 | Invalid attacker dice count is rejected |
| ATTACK-007 | Defender dice limit depends on troops |

### Conquer

| Test ID | Description | Status |
|---|---|---|
| CONQUER-001 | Planned | Planned |


### Reinforcement Rules

| Test ID | Description |
|---|---|
| REINFORCE-001 | Player can reinforce owned territory |
| REINFORCE-002 | Enemy territories cannot be reinforced |
| REINFORCE-003 | Zero troop placement is rejected |
| REINFORCE-004 | Reinforcement pool limits placement |
| REINFORCE-005 | Invalid territory ids are rejected |

### Reinforcement Mutation

| Test ID | Description |
|---|---|
| MUTATE-REINFORCE-001 | Reinforcement adds troops to target territory |
| MUTATE-REINFORCE-002 | Reinforcement decreases player troop pool |
| MUTATE-REINFORCE-003 | Other territories remain unchanged |
| MUTATE-REINFORCE-004 | Multiple reinforcements accumulate correctly |


### Fortify

| Test ID | Description | Status |
|---|---|---|
| FORTIFY-001 | Planned | Planned |


### Cards

| Test ID | Description | Status |
|---|---|---|
| CARD-001 | Planned | Planned |


### GameRunner

| Test ID | Description | Status |
|---|---|---|
| GAME-001 | Planned | Planned |


### Layout Validation

| Test ID | Description | Status |
|---|---|---|
| LAYOUT-001 | Planned | Planned |