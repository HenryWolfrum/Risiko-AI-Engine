using System;
using System.Runtime.CompilerServices;
using RiskEngine.Mutation;
using RiskEngine.Rules;
using RiskEngine.Validation;

namespace RiskEngine;

public static class GameRunner
{
    public static GameState PlayGame(GameLayout layout, IRiskPlayer[] players, int seed)
    {
        // Initialen Random-State und GameState erzeugen
        EngineRandom rng = new EngineRandom(seed);
        GameState state = GameInitializer.CreateInitialState(layout, seed);

        while (state.CurrentRound <= layout.Config.MaxRounds)
        {
            byte player = state.PlayerTurn;

            // Check ob Spieler noch im Spiel ist
            if (!GameStateHelper.IsPlayerAlive(in state, player))
            {
                AdvanceToNextTurn(ref state, layout.Config.PlayerCount);
                continue;
            }

            IRiskPlayer currentPlayer = players[player];

            // --- GAME LOOP FOR CURRENT PLAYER ---

            // 1. CARD TURN IN PHASE
            ExecuteCardTurnInPhase(ref state, currentPlayer, layout);
            
            // 2. REINFORCEMENT PHASE
            ExecuteReinforcePhase(ref state, currentPlayer, layout);

            // 3. ATTACK PHASE
            ExecuteAttackPhase(ref state, players, layout, ref rng);

            // 4. FORTIFY PHASE
            ExecuteFortifyPhase(ref state, currentPlayer, layout);

            // Prüfen, ob der aktuelle Spieler gewonnen hat
            if (HasPlayerWon(in state, player))
            {
                return state;
            }

            // Nächster Spieler ist an der Reihe
            AdvanceToNextTurn(ref state, layout.Config.PlayerCount);
        }

        return state;
    }

    private static void ExecuteCardTurnInPhase(ref GameState state, IRiskPlayer player, GameLayout layout)
    {
        state.CurrentPhase = GamePhase.CardTurnIn;

        // 1. Mandatory Trade Phase (Forced when holding >= 5 cards)
        while (GameStateHelper.GetPlayerCardCount(in state, state.PlayerTurn) >= EngineConstants.FORCE_TRADE_CARD_COUNT)
        {
            GameAction action = player.DecideAction(in state, GamePhase.CardTurnIn, layout);
            ValidationResult validation = TurnInCardsRules.Validate(in state, in action, layout.Deck);

            if (validation.IsValid)
            {
                CardTurnInMutator.Apply(ref state, in action);
            }
            else
            {
                // Invalid action on forced trade results in auto-selecting the first valid set
                GameAction fallbackAction = CardHelper.FindFirstValidSet(in state, state.PlayerTurn, layout.Deck);
                CardTurnInMutator.Apply(ref state, in fallbackAction);
            }
        }
    
        // 2. Optional Trade Phase (Player has 3 or 4 cards remaining and CAN trade if desired)
        if (CardHelper.HasValidSet(in state, state.PlayerTurn, layout.Deck))
        {
            GameAction action = player.DecideAction(in state, GamePhase.CardTurnIn, layout);

            if (action.Type == ActionType.TurnInCards)
            {
                ValidationResult validation = TurnInCardsRules.Validate(in state, in action, layout.Deck);
            
                if (validation.IsValid)
                {
                    CardTurnInMutator.Apply(ref state, in action);
                }
            }
        }
    }
    
    private static void ExecuteReinforcePhase(ref GameState state, IRiskPlayer player, GameLayout layout)
    {
        state.CurrentPhase = GamePhase.Reinforce;

        // 1. Calculate and set initial reinforcement troops
        byte totalTroops = ReinforcementCalculator.CalculateTroops(in state, layout.Map);
        GameStateHelper.SetPlayerTroopsToPlace(ref state, state.PlayerTurn, totalTroops);

        // 2. Determine max allowed placement attempts based on owned territories count
        int maxAttempts = GameStateHelper.GetOwnedTerritoryCount(in state, state.PlayerTurn);

        for (int i = 0; i < maxAttempts; i++)
        {
            // Early exit: Stop if player has already placed all available troops
            if (GameStateHelper.GetPlayerTroopsToPlace(in state, state.PlayerTurn) == 0)
                break;

            GameAction action = player.DecideAction(in state, GamePhase.Reinforce, layout);
            ValidationResult validation = ReinforceRules.Validate(in state, in action);

            if (validation.IsValid)
            {
                ReinforceMutator.Apply(ref state, in action);
            }
            else
            {
                // Invalid action -> Fail-fast fallback: Dump remaining troops and exit loop
                ApplyReinforceFallback(ref state);
                return;
            }
        }

        // 3. Fallback cleanup: If loop finishes but player still has unplaced troops left
        if (GameStateHelper.GetPlayerTroopsToPlace(in state, state.PlayerTurn) > 0)
        {
            ApplyReinforceFallback(ref state);
        }
    }

    /// <summary>
    /// Dumps all remaining troops onto the player's first owned territory.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ApplyReinforceFallback(ref GameState state)
    {
        byte fallbackTerritory = GameStateHelper.GetFirstTerritoryOwnedBy(in state, state.PlayerTurn);
        byte remainingTroops = GameStateHelper.GetPlayerTroopsToPlace(in state, state.PlayerTurn);

        GameAction fallbackAction = new GameAction
        {
            Type = ActionType.Reinforce,
            TargetTerritory = fallbackTerritory,
            TroopCount = remainingTroops
        };  

        ReinforceMutator.Apply(ref state, in fallbackAction);
    }
    
    private static void ExecuteAttackPhase(ref GameState state, IRiskPlayer[] players, GameLayout layout, ref EngineRandom rng)
    {
        state.CurrentPhase = GamePhase.Attack;

        // As long as player can possibly attack
        while (GameStateHelper.CanPlayerAttack(in state, state.PlayerTurn, layout.Map))
        {
            // 1. Attacker decides about target and attack dice count
            IRiskPlayer attackerPlayer = players[state.PlayerTurn];
            GameAction attackAction = attackerPlayer.DecideAction(in state, GamePhase.Attack, layout);

            // Skip/End Attack Phase
            if (attackAction.Type == ActionType.SkipPhase || attackAction.Type == ActionType.EndTurn)
            {
                break;
            }

            // Validate Attack
            ValidationResult validation = AttackRules.Validate(in state, in attackAction, layout.Map);

            if (!validation.IsValid)
            {
                // Invalid Action -> Fail-Fast: Phase End
                break;
            }

            // 2. Get Defender and get defender Dice Count
            byte defenderPlayerId = GameStateHelper.GetTerritoryOwner(in state, attackAction.TargetTerritory);
            IRiskPlayer defenderPlayer = players[defenderPlayerId];

            byte defenderDice = defenderPlayer.DecideDefenderDice(in state, in attackAction);

            // Validate/Fallback for Defender Dice Count
            if (!AttackRules.IsValidDefenderDice(in state, attackAction.TargetTerritory, defenderDice))
            {
                defenderDice = AttackRules.GetMaxDefenderDice(in state, attackAction.TargetTerritory);
            }
    
            attackAction.ChosenDefenderDiceCount = defenderDice;

            // 3. Resolve Battle (Applies losses via GameStateMutator)
            GameStateMutator.Apply(ref state, in attackAction, ref rng);

            // 4. Check for Conquest: Defender territory wiped out
            if (GameStateHelper.GetTerritoryTroops(in state, attackAction.TargetTerritory) == 0)
            {
                ExecuteConquer(ref state, layout, attackerPlayer, defenderPlayerId, in attackAction, ref rng);

                // Win condition: Only 1 player remaining
                if (GameStateHelper.GetActivePlayerCount(in state) <= 1)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Handles troop transfer into the conquered territory and checks for defender elimination.
    /// </summary>
    private static void ExecuteConquer(ref GameState state, GameLayout layout, IRiskPlayer attacker, byte defenderId, in GameAction attackAction, ref EngineRandom rng)
    {
        // Ask attacker how many troops to move into the newly captured territory
        GameAction conquerAction = attacker.DecideAction(in state, GamePhase.Conquer, layout);
    
        // Ensure action setup is correct
        conquerAction.Type = ActionType.Conquer;
        conquerAction.SourceTerritory = attackAction.SourceTerritory;
        conquerAction.TargetTerritory = attackAction.TargetTerritory;

        // Validate/Clamp troop count (Min = AttackerDice, Max = SourceTroops - 1)
        byte sourceTroops = GameStateHelper.GetTerritoryTroops(in state, conquerAction.SourceTerritory);
        byte maxMoveable = (byte)(sourceTroops - 1);
        byte minRequired = attackAction.ChosenAttackerDiceCount;

        if (conquerAction.ConquerTroopCount < minRequired || conquerAction.ConquerTroopCount > maxMoveable)
        {
            // Fail-safe Fallback: Move minimum required troops
            conquerAction.ConquerTroopCount = minRequired;
        }

        // Mutate state (Transfer ownership and move troops)
        GameStateMutator.Apply(ref state, in conquerAction, ref rng);

        // Check Player Elimination: Did the defender lose their last territory?
        if (GameStateHelper.GetOwnedTerritoryCount(in state, defenderId) == 0)
        {
            byte attackerId = state.PlayerTurn;
            GameStateHelper.EliminateAndTransferCards(ref state, attackerId, defenderId);
            
            // Check if attacker now needs to trade in captured cards
            ExecuteCardTurnInPhase(ref state, attacker, layout);
        }
    }

    private static void ExecuteFortifyPhase(ref GameState state, IRiskPlayer player, GameLayout layout)
    {
        state.CurrentPhase = GamePhase.Fortify;

        byte activePlayer = state.PlayerTurn;
        byte ownedTerritoriesCount = (byte)GameStateHelper.GetOwnedTerritoryCount(in state, activePlayer);

        //Limit n choose 2 (move actions)
        int maxFortifyMoves = (ownedTerritoriesCount * (ownedTerritoriesCount - 1)) >> 1;
        int moveCounter = 0;

        while (moveCounter < maxFortifyMoves)
        {
            GameAction action = player.DecideAction(in state, GamePhase.Fortify, layout);

            if (action.Type == ActionType.SkipPhase || action.Type == ActionType.EndTurn)
            {
                //Player ends Phase/Turn
                break;
            }

            if (action.Type == ActionType.Fortify)
            {
                byte sourceTroops = GameStateHelper.GetTerritoryTroops(in state, action.SourceTerritory);
                
                byte maxMoveable = (byte)(sourceTroops - 1);
                

                ValidationResult validation = FortifyRules.Validate(in state, in action, layout.Map);
                if (validation.IsValid)
                {
                    FortifyMutator.Apply(ref state, in action);
                    moveCounter++;
                }
                else
                {
                    //Invalid Action break
                    break; 
                }
            }
            else
            {
                break;
            }
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AdvanceToNextTurn(ref GameState state, byte playerCount)
    {
        state.PlayerTurn = (byte)((state.PlayerTurn + 1) % playerCount);
        if (state.PlayerTurn == 0)
        {
            state.CurrentRound++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasPlayerWon(in GameState state, byte player)
    {
        return GameStateHelper.GetActivePlayerCount(in state) == 1 && GameStateHelper.IsPlayerAlive(in state, player);
    }
}