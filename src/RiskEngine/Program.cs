using RiskEngine;
using RiskEngine.Validation;
using RiskEngine.Resolution;

Console.WriteLine("=== VALIDATOR TEST ===");

unsafe
{
    Console.WriteLine(sizeof(GameState));

}

GameLayout game = RiskMapFactory.CreateStandardRiskMap();

GameState state = GameInitializer.CreateInitialState(game, 123);


// Eigenes Gebiet suchen
byte ownTerritory = 255;

for (byte i = 0; i < game.Map.TerritoryNames.Length; i++)
{
    if (state.GetTerritoryOwner(i) == state.PlayerTurn)
    {
        ownTerritory = i;
        break;
    }
}



Console.WriteLine("\n=== ATTACK RULES TEST ===");

GameState attackState =
    GameInitializer.CreateInitialState(game, 123);

attackState.CurrentPhase = GamePhase.Attack;


// Suche gültiges Angriffspaar
byte attackerTerritory = 255;
byte defenderTerritory = 255;

for (byte i = 0; i < game.Map.TerritoryNames.Length; i++)
{
    if (attackState.GetTerritoryOwner(i) != attackState.PlayerTurn)
        continue;

    for (byte j = 0; j < game.Map.TerritoryNames.Length; j++)
    {
        if (attackState.GetTerritoryOwner(j) == attackState.PlayerTurn)
            continue;

        if (game.Map.AreNeighbors(i, j))
        {
            attackerTerritory = i;
            defenderTerritory = j;
            break;
        }
    }

    if (attackerTerritory != 255)
        break;
}


// Truppen für gültigen Angriff vorbereiten
attackState.SetTerritoryTroops(
    attackerTerritory,
    5);


GameAction attackAction = new GameAction
{
    Type = ActionType.Attack,
    SourceTerritory = attackerTerritory,
    TargetTerritory = defenderTerritory,
    ChosenAttackerDiceCount = 3,
    ChosenDefenderDiceCount = 2
};


Console.WriteLine(
    attackState.GetTerritoryTroops(attackerTerritory));

ValidationResult result =
    RuleValidator.Validate(
        attackState,
        attackAction,
        game);


Console.WriteLine(
    $"Valid attack: {result.IsValid}, Error: {result.Error}");

Console.WriteLine(
    $"From {attackerTerritory} -> {defenderTerritory}");


Console.WriteLine("\n=== ATTACK INVALID TESTS ===");


// 1. Angriff auf eigenes Gebiet
GameAction ownTerritoryAttack = new GameAction
{
    Type = ActionType.Attack,
    SourceTerritory = attackerTerritory,
    TargetTerritory = attackerTerritory,
    ChosenAttackerDiceCount = 3,
    ChosenDefenderDiceCount = 2
};


result = RuleValidator.Validate(
    attackState,
    ownTerritoryAttack,
    game);


Console.WriteLine(
    $"Own territory attack: {result.IsValid}, Error: {result.Error}");



// 2. Nicht benachbartes gegnerisches Gebiet suchen
byte nonAdjacentEnemy = 255;

for (byte i = 0; i < game.Map.TerritoryNames.Length; i++)
{
    if (attackState.GetTerritoryOwner(i) != attackState.PlayerTurn &&
        !game.Map.AreNeighbors(attackerTerritory, i))
    {
        nonAdjacentEnemy = i;
        break;
    }
}


GameAction nonAdjacentAttack = new GameAction
{
    Type = ActionType.Attack,
    SourceTerritory = attackerTerritory,
    TargetTerritory = nonAdjacentEnemy,
    ChosenAttackerDiceCount = 3,
    ChosenDefenderDiceCount = 2
};


result = RuleValidator.Validate(
    attackState,
    nonAdjacentAttack,
    game);


Console.WriteLine(
    $"Non adjacent attack: {result.IsValid}, Error: {result.Error}");



// 3. Zu wenig Truppen
attackState.SetTerritoryTroops(
    attackerTerritory,
    1);


GameAction weakAttack = new GameAction
{
    Type = ActionType.Attack,
    SourceTerritory = attackerTerritory,
    TargetTerritory = defenderTerritory,
    ChosenAttackerDiceCount = 1,
    ChosenDefenderDiceCount = 1
};


result = RuleValidator.Validate(
    attackState,
    weakAttack,
    game);


Console.WriteLine(
    $"One troop attack: {result.IsValid}, Error: {result.Error}");



Console.WriteLine("\n=== DICE RULE TESTS ===");


// Angreifer mit 2 Truppen
attackState.SetTerritoryTroops(
    attackerTerritory,
    2);


// Verteidiger 1 Würfel -> Angreifer darf 2 Würfel
GameAction twoDiceAttack = new GameAction
{
    Type = ActionType.Attack,
    SourceTerritory = attackerTerritory,
    TargetTerritory = defenderTerritory,
    ChosenAttackerDiceCount = 2,
    ChosenDefenderDiceCount = 1
};


result = RuleValidator.Validate(
    attackState,
    twoDiceAttack,
    game);


Console.WriteLine(
    $"2 attacker dice vs 1 defender die: {result.IsValid}, Error: {result.Error}");



// Verteidiger 2 Würfel -> Angreifer darf nur 1 Würfel
GameAction invalidTwoDiceAttack = new GameAction
{
    Type = ActionType.Attack,
    SourceTerritory = attackerTerritory,
    TargetTerritory = defenderTerritory,
    ChosenAttackerDiceCount = 2,
    ChosenDefenderDiceCount = 2
};


result = RuleValidator.Validate(
    attackState,
    invalidTwoDiceAttack,
    game);


Console.WriteLine(
    $"2 attacker dice vs 2 defender dice: {result.IsValid}, Error: {result.Error}");



// 5 Angreifertruppen -> 3 Würfel erlaubt
attackState.SetTerritoryTroops(
    attackerTerritory,
    5);

attackState.SetTerritoryTroops(
    defenderTerritory,
    2);


GameAction threeDiceAttack = new GameAction
{
    Type = ActionType.Attack,
    SourceTerritory = attackerTerritory,
    TargetTerritory = defenderTerritory,
    ChosenAttackerDiceCount = 3,
    ChosenDefenderDiceCount = 2
};



result = RuleValidator.Validate(
    attackState,
    threeDiceAttack,
    game);


Console.WriteLine(
    $"3 attacker dice vs 2 defender dice: {result.IsValid}, Error: {result.Error}");


Console.WriteLine("\n=== COMBAT RESOLVER TEST ===");

GameState combatState =
    GameInitializer.CreateInitialState(game, 123);

combatState.SetTerritoryTroops(
    attackerTerritory,
    5);

combatState.SetTerritoryTroops(
    defenderTerritory,
    2);


GameAction combatAction = new GameAction
{
    Type = ActionType.Attack,
    SourceTerritory = attackerTerritory,
    TargetTerritory = defenderTerritory,
    ChosenAttackerDiceCount = 3,
    ChosenDefenderDiceCount = 2
};


EngineRandom rng = new EngineRandom(42);


CombatResult combatResult = CombatResolver.Resolve( combatState, combatAction, ref rng);


Console.WriteLine(
    $"Attacker losses: {combatResult.AttackerLosses}");

Console.WriteLine(
    $"Defender losses: {combatResult.DefenderLosses}");
    
    