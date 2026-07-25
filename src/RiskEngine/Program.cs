using RiskEngine;
using RiskEngine.Validation;

Console.WriteLine("=== VALIDATOR TEST ===");


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
        game.Map);


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
    game.Map);


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
    game.Map);


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
    game.Map);


Console.WriteLine(
    $"One troop attack: {result.IsValid}, Error: {result.Error}");
    