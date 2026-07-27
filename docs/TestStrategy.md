# Test Strategy

## Purpose

This document defines the testing philosophy and structure
of the RiskEngine.

The goal is to guarantee:

- correctness of game rules
- deterministic simulations
- stable state transitions
- reliable AI training environments

The test suite acts as a specification of engine behavior.


---

# Testing Philosophy

The engine is tested on multiple abstraction levels.

Small artificial maps are used for isolated rule testing.
They allow precise control over game situations.

The official Risk map is used for:

- integration tests
- complete simulations
- bot evaluation

Tests should prefer simple deterministic scenarios
over realistic but uncontrolled scenarios.


---

# Test Layers


## Unit Tests

Purpose:

Verify isolated components.

Examples:

- CombatResolver
- Individual rule classes
- GameStateHelper


## State Mutation Tests

Purpose:

Verify that valid actions correctly transform GameState.

Examples:

- AttackMutator
- FortifyMutator
- ConquerMutator


## Execution Tests

Purpose:

Verify coordination and game flow.

Examples:

- TurnExecutor
- GameRunner


## Integration Tests

Purpose:

Verify complete simulations.

Examples:

- Full games
- Bot vs Bot simulations
- Standard Risk map


---

# Test Environments


## Synthetic Test Environment

Used for:

- rule validation
- edge cases
- deterministic scenarios


Created with:

- TestStateBuilder
- TestLayoutBuilder


Advantages:

- fast
- predictable
- focused


## Standard Risk Environment

Created with:

RiskMapFactory.CreateStandardRiskMap()


Used for:

- integration testing
- simulation
- AI evaluation


---

# Determinism

All simulations must be reproducible.

A fixed seed must produce:

- identical initial states
- identical random outcomes
- identical game progression


This allows:

- debugging
- replay systems
- AI evaluation
- regression testing


---

# Naming Convention

Tests should describe behavior.

Preferred:

```
Attack_ShouldFail_WhenTargetIsNotAdjacent()
```

Avoid:

```
AttackTest1()
```


---

# Test Identification

Important tests receive a unique identifier.

Format:

```
AREA-NUMBER
```

Examples:

```
INIT-001
STATE-001
RULE-001
MUTATION-001
EXEC-001
```

The identifier is used for:

- test comments
- documentation
- debugging discussions


---

## Coverage Status

| Component | Status |
|---|---|
| GameInitializer | Completed |
| GameStateHelper | Completed |
| CombatResolver | Completed |
| Attack Rules | Completed |
| Reinforce Rules | Completed |
| Reinforce Mutator | Completed |
| Conquer | Planned |
| Fortify | Planned |
| Cards | Planned |
| GameRunner | Planned |
| Layout Validation | Planned |
---

# Detailed Test Documentation

Detailed test cases are documented separately.

This document only describes:

- testing principles
- architecture
- environments
- coverage state

Individual test specifications include:

- test identifier
- tested behavior
- guarantees
- covered edge cases


---

# AI Environment Requirements

The engine must provide:

- deterministic simulations
- valid state transitions
- reproducible matches
- measurable outcomes


These properties are required for:

- RandomBot evaluation
- Elo ranking
- MCTS experiments
- reinforcement learning environments
## State Invariants

The GameState layer follows explicit invariants.

Important rules:

- Unassigned territories use `EngineConstants.NO_VALUE`
- Missing territory references must never resolve to valid territory IDs
- Player IDs and missing values must remain distinguishable
- Empty states must not accidentally assign ownership

These invariants prevent ambiguous states and are required for reliable AI simulations.