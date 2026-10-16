# Combat Resolution

## Purpose

Resolves a single combat round and returns the resulting troop losses without modifying the game state.

## Rules

1. The maximum number of dice per side is defined by `MAX_ATTACKER_DICE` and `MAX_DEFENDER_DICE`.
2. Both sets of dice are sorted in descending order before comparison.
3. The number of comparisons equals the smaller dice count.
4. The attacker wins a comparison only if their roll is strictly higher. Ties go to the defender.
5. Each comparison results in exactly one troop loss.

## Preconditions

- Dice counts must be valid and within the supported buffer limits.

## Implementation Notes

- Dice buffers are allocated using `stackalloc`.
- Sorting is precalculated for the common dice counts (1, 2, 3).
- Randomness is provided by the caller through `EngineRandom`.

## Responsibilities

- Roll and compare dice.
- Calculate and return troop losses.
- Leave state updates and conquest handling to other systems.