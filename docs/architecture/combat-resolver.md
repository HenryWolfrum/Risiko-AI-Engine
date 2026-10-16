# Combat Resolution

## Purpose

The `CombatResolver` resolves a single combat round between an attacker and a defender. It determines troop losses based on the selected dice counts and the resulting dice rolls.

The resolver calculates the outcome of the combat round but does not apply the resulting troop losses to the game state.

## Combat Rules

### Dice Counts

The attacker and defender select their respective dice counts according to the combat rules. The implementation supports a maximum of three dice per side.

The resolver expects valid dice counts. These must be guaranteed by the calling code or validated before the resolver is invoked.

### Dice Comparison

Both sets of dice are sorted in descending order, ensuring that the highest rolls are compared first.

The number of comparisons is determined by the smaller of the two dice counts. Each comparison results in one troop loss.

The attacker wins a comparison only if the attacker's die is strictly higher than the defender's die. In case of a tie, the defender wins.

## Implementation Decisions

### Specialized Sorting

The sorting implementation is specialized for a maximum of three dice. This avoids the need for a general-purpose sorting algorithm for such a small, fixed input size.

### Stack Allocation

The dice buffers are allocated using `stackalloc`. This avoids allocating separate managed arrays for the dice rolls during combat resolution.

### Randomness

Dice rolls use the engine's `EngineRandom` instance, passed by reference. This allows the resolver to advance the random number generator maintained by the caller.

## Responsibilities and Boundaries

The `CombatResolver` is responsible for:

- Rolling the selected number of dice.
- Sorting dice in descending order.
- Comparing dice according to the combat rules.
- Returning the resulting troop losses.

It does not:

- Validate the entire game action.
- Apply troop losses to the game state.
- Handle territory conquest or troop movement after a conquest.

These responsibilities belong to other parts of the engine.