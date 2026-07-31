# AI Interface Architecture

## Motivation

The communication layer between the engine and the outside world (in our case the `IRiskPlayer` interface) represents the second architectural pillar of the engine.

While the first pillar is responsible for deterministic and efficient game execution, the second pillar focuses on exposing legal decisions to external agents in a clean and implementation-independent manner.

To achieve this, every executable engine action should satisfy the following design principles:

- **Semantic uniqueness** – every action represents exactly one concrete game operation.
- **Implementation independence** – external systems must not depend on internal engine details.
- **Responsibility separation** – the engine is responsible for execution, while external agents are responsible only for decision making.

---

## Action Spaces

Artificial intelligence does not reason about a single action in isolation. Instead, it selects one action from a **set of legal decisions**, referred to as an **Action Space**.

An Action Space should satisfy the following properties:

- **Semantic completeness** – every legal decision must be representable.
- **Semantic correctness** – every represented decision must be legal.
- **Engine independence** – the representation should describe *what can be chosen*, not *how the engine executes it*.

This representation differs fundamentally from the internal `GameAction` representation.

---

## Design Challenge

A straightforward implementation would expose every executable `GameAction` to the AI.

However, this directly conflicts with the goals of the first architectural pillar.

Many game phases contain a parameterized integer choice.

Examples include:

- Number of reinforcement troops
- Number of attacking dice
- Number of fortification troops
- Number of conquer troops

Expanding every possible parameter value into a separate executable action causes the number of generated actions to grow unnecessarily, although the underlying decision remains identical.

---

## Proposed Solution

The Risk environment is naturally divided into six game phases:

- Card Turn-In
- Reinforcement
- Attack
- Defend
- Conquer
- Fortify

When analysing the decision process rather than the executable actions, only four atomic decision types exist.

### Parameter Space

A bounded integer interval.

Examples:

- Number of troops
- Number of attacking dice
- Number of defending dice

### Territory Space

Selection of a single territory.

### Territory Pair Space

Selection of an ordered `(Source, Target)` territory pair.

### Card Triple Space

Selection of one legal card combination.

---

## Phase Composition

Each game phase can be expressed as a combination of these atomic decision spaces.

| Phase | Decision Space |
|--------|----------------|
| Card Turn-In | Card Triple Space |
| Reinforcement | Territory Space + Parameter Space |
| Attack | Territory Pair Space + Parameter Space |
| Defend | Parameter Space |
| Conquer | Parameter Space |
| Fortify | Territory Pair Space + Parameter Space |

This decomposition provides a complete and semantically minimal description of every legal decision within the Risk environment.

---

## Architectural Separation

The engine and the AI operate on different abstraction levels.

```
Engine
    │
    ▼
Generate Action Spaces
    │
    ▼
IRiskPlayer
    │
    ▼
Select Decision
    │
    ▼
Materialize one GameAction
    │
    ▼
Validator → Executor → Mutator
```

The engine exclusively executes concrete `GameAction` instances.

The AI never receives executable actions directly. Instead, it receives only the corresponding decision spaces and returns a single selected decision, from which exactly one executable `GameAction` is materialized.

---

## Benefits

### Semantic Benefits

- Clear separation between executable actions and decision spaces.
- Every type has exactly one responsibility.
- No redundant or phase-dependent fields.
- The AI interface becomes independent from internal engine implementation.

### Performance Benefits

Parameterized action spaces avoid generating large numbers of equivalent executable actions.

Instead of materializing every possible `GameAction`, only the semantic decision space is generated.

After the AI has selected a decision, exactly one concrete `GameAction` is created and executed.

This reduces temporary object generation and keeps the communication layer compact while leaving the internal execution pipeline unchanged.
