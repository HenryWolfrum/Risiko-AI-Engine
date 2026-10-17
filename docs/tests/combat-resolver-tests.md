# CombatResolver – Test Specification

| ID         | Type | Test | Expected Result |
|------------|---|---|---|
| COMBAT_001 | Rule | Dice Comparison | Higher attacker rolls cause defender losses; ties and lower rolls cause attacker losses. |
| COMBAT_002 | Calculation | Comparison Count | The number of comparisons equals the smaller dice count. |
| COMBAT_003 | Rule | Dice Sorting | Dice are sorted in descending order for supported dice counts (1, 2, 3). |
| COMBAT_004 | Boundary | Maximum Dice Counts | Maximum valid attacker and defender dice counts are handled correctly. |
| COMBAT_005 | Determinism | Identical Inputs | Identical actions and initial RNG states produce identical results. |
| COMBAT_006 | Invariant | Loss Consistency | Losses remain within valid bounds, and their sum equals the number of comparisons. |