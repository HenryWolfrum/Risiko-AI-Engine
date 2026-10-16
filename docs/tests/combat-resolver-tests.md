# Combat Resolver Tests

Based on the purpose, rules, preconditions, and implementation constraints defined in `CombatResolver.md`, the following testing requirements are derived.

| ID | Type | Test | Expected Result |
|---|---|---|---|
| COMBAT_001 | Rule | Dice Comparison | Higher attacker rolls cause defender losses; ties and lower rolls cause attacker losses. |
| COMBAT_002 | Calculation | Comparison Count | The number of comparisons equals the smaller dice count. |
| COMBAT_003 | Rule | Dice Sorting | Dice are sorted in descending order for supported dice counts (1, 2, 3). |
| COMBAT_004 | Boundary | Maximum Dice Counts | Dice counts up to the respective configured maximums are handled correctly. |
| COMBAT_005 | Preconditions | Invalid Dice Counts | Invalid dice counts are handled according to the defined input contract. |
| COMBAT_006 | Determinism | Identical Inputs | Identical inputs and initial RNG states produce identical results. |
| COMBAT_007 | Responsibility | State Immutability | Combat resolution does not modify the game state. |