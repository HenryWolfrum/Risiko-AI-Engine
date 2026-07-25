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


Console.WriteLine($"Player {state.PlayerTurn} owns territory {ownTerritory}");


// 1. Gültige Aktion
GameAction validAction = new GameAction
{
    Type = ActionType.PlaceTroops,
    SourceTerritory = ownTerritory,
    TroopCount = 1
};

state.CurrentPhase = GamePhase.Reinforce;
state.SetPlayerTroopsToPlace(state.PlayerTurn, 5);

ValidationResult result = RuleValidator.Validate(state, validAction);


Console.WriteLine($"Valid action: {result.IsValid}, Error: {result.Error}");


// 2. Ungültig: fremdes Gebiet
byte enemyTerritory = 255;

for (byte i = 0; i < game.Map.TerritoryNames.Length; i++)
{
    if (state.GetTerritoryOwner(i) != state.PlayerTurn)
    {
        enemyTerritory = i;
        break;
    }
}


GameAction invalidOwnerAction = new GameAction
{
    Type = ActionType.PlaceTroops,
    SourceTerritory = enemyTerritory,
    TroopCount = 1
};


result = RuleValidator.Validate(state, invalidOwnerAction);


Console.WriteLine(
    $"Enemy territory: {result.IsValid}, Error: {result.Error}");


// 3. Ungültig: falsche Phase
state.CurrentPhase = GamePhase.Attack;


result = RuleValidator.Validate(state, validAction);


Console.WriteLine(
    $"Wrong phase: {result.IsValid}, Error: {result.Error}");