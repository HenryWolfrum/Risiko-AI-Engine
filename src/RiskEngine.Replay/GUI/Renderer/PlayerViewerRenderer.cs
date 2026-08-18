using System;
using System.Numerics;
using Raylib_cs;
using RiskEngine.AI.Configuration;
using RiskEngine.Mission;
using RiskEngine.Replay.GUI.Controls;
using RiskEngine.State;

namespace RiskEngine.Replay.GUI;

public class PlayerViewerRenderer : ISectionRenderer
{
    private static readonly Color BackgroundColor = new(30, 33, 42, 255);
    private static readonly Color CardBgColor = new(42, 47, 60, 255);
    private static readonly Color SubPanelBgColor = new(33, 37, 48, 255);
    private static readonly Color BorderColor = new(65, 70, 85, 255);
    private static readonly Color TextMutedColor = new(160, 165, 180, 255);

    private readonly Button _prevPlayerBtn;
    private readonly Button _nextPlayerBtn;

    private int _selectedPlayerId = 0;
    private int _lastTrackedTurn = -1;

    public PlayerViewerRenderer()
    {
        Color btnBg = new(55, 62, 78, 255);
        Color btnHover = new(75, 85, 108, 255);
        Color btnPress = new(38, 42, 54, 255);
        Color btnBorder = new(90, 98, 118, 255);

        _prevPlayerBtn = new Button(default, "<")
        {
            FontSize = 16,
            NormalColor = btnBg,
            HoverColor = btnHover,
            PressedColor = btnPress,
            TextColor = Color.White,
            HoverTextColor = Color.Gold,
            BorderColor = btnBorder
        };

        _nextPlayerBtn = new Button(default, ">")
        {
            FontSize = 16,
            NormalColor = btnBg,
            HoverColor = btnHover,
            PressedColor = btnPress,
            TextColor = Color.White,
            HoverTextColor = Color.Gold,
            BorderColor = btnBorder
        };
    }

    public void Render(Rectangle bounds, ReplayUIContext context)
    {
        var player = context.Player;
        var frame = player.CurrentFrame;
        var state = frame.State;
        ulong seed = player._replay.Header.Seed;
        Vector2 mousePos = Raylib.GetMousePosition();

        // 1. Automatisch dem aktiven Spieler folgen bei neuem Zug
        if (state.PlayerTurn != _lastTrackedTurn)
        {
            _selectedPlayerId = state.PlayerTurn;
            _lastTrackedTurn = state.PlayerTurn;
        }

        // 2. Äußerer Panel-Hintergrund
        Raylib.DrawRectangleRec(bounds, BackgroundColor);
        Raylib.DrawRectangleLinesEx(bounds, 1, BorderColor);

        float padding = 12f;
        float headerHeight = 30f;

        Raylib.DrawText("PLAYER VIEWER", (int)(bounds.X + padding), (int)(bounds.Y + 8), 15, Color.LightGray);

        // 3. Karten-Hauptbereich
        Rectangle cardBounds = new(
            bounds.X + padding, 
            bounds.Y + headerHeight + 4f, 
            bounds.Width - (padding * 2f),
            bounds.Height - headerHeight - (padding * 2f)
        );

        bool isActiveTurn = (state.PlayerTurn == _selectedPlayerId);

        // Dossier-Inhalt zeichnen (Schicht 1)
        RenderSinglePlayerDossier(cardBounds,player, _selectedPlayerId, in state, seed, isActiveTurn);

        // 4. Navigation-Buttons im Dossier-Header platzieren (Schicht 2)
        float btnWidth = 34f;
        float btnHeight = 30f;
        float navY = cardBounds.Y + 10f;

        _prevPlayerBtn.Bounds = new Rectangle(cardBounds.X + 12f, navY, btnWidth, btnHeight);
        _nextPlayerBtn.Bounds = new Rectangle(cardBounds.X + cardBounds.Width - 12f - btnWidth, navY, btnWidth, btnHeight);

        var playerCount = player._replay.Header.PlayerConfigs.Length;
        if (_prevPlayerBtn.DrawAndCheck(mousePos))
        {
            _selectedPlayerId = (_selectedPlayerId + 3) % playerCount ;
        }

        if (_nextPlayerBtn.DrawAndCheck(mousePos))
        {
            _selectedPlayerId = (_selectedPlayerId + 1) % playerCount;
        }
    }

    private static void RenderSinglePlayerDossier(Rectangle bounds,ReplayPlayer player, int playerId, in GameState state, ulong seed, bool isActiveTurn)
    {
     
        
        int territoryCount = GameStateHelper.GetOwnedTerritoryCount(in state, (byte)playerId);
        bool isAlive = territoryCount > 0;
        Color playerColor = ReplayViewer.GetPlayerColor(playerId);

        // Hauptkarte Rahmen & Hintergrund
        Raylib.DrawRectangleRounded(bounds, 0.03f, 4, CardBgColor);
        Raylib.DrawRectangleRoundedLinesEx(bounds, 0.03f, 4, isActiveTurn ? 2.0f : 1.0f, isActiveTurn ? playerColor : BorderColor);

        float innerX = bounds.X + 14f;
        float contentWidth = bounds.Width - 28f;
        float currentY = bounds.Y + 12f;

        // --- A) Header: Zentrierter Name & Status ---
        string titleText = $"Player {playerId}";
        int titleFontSize = 20;
        int titleWidth = Raylib.MeasureText(titleText, titleFontSize);
        float titleX = bounds.X + (bounds.Width - titleWidth) / 2f;
        
        Raylib.DrawText(titleText, (int)titleX, (int)currentY + 4, titleFontSize, playerColor);

        if (isActiveTurn)
        {
            Raylib.DrawText("AM ZUG", (int)(titleX + titleWidth + 10), (int)currentY + 9, 11, Color.Lime);
        }

        currentY += 46f;

        // --- B) Sub-Panel: Meta-Info (Dynamisch nach Konfigurationstyp) ---
        PlayerConfiguration playerConfig = player._replay.Header.PlayerConfigs[playerId];

        // Dynamische Höhe basierend auf der Anzahl der Parameter
        float metaPanelHeight = GetConfigPanelHeight(playerConfig);

        Rectangle metaBox = new(innerX, currentY, contentWidth, metaPanelHeight);
        Raylib.DrawRectangleRounded(metaBox, 0.12f, 4, SubPanelBgColor);
        Raylib.DrawRectangleRoundedLinesEx(metaBox, 0.12f, 4, 1.0f, BorderColor);

        // Render-Funktion aufrufen, die nach Typ unterscheidet
        RenderPlayerConfigDetails(metaBox, playerConfig);

        // currentY dynamisch nach Paneel-Höhe anpassen
        currentY += metaPanelHeight + 10f;

        if (!isAlive)
        {
            Rectangle elimBox = new(innerX, currentY, contentWidth, 60f);
            Raylib.DrawRectangleRounded(elimBox, 0.1f, 4, new Color(50, 25, 30, 255));
            Raylib.DrawRectangleRoundedLinesEx(elimBox, 0.1f, 4, 1.0f, new Color(200, 60, 60, 255));
            Raylib.DrawText("STATUS: ELIMINIERT", (int)(elimBox.X + 16f), (int)(elimBox.Y + 20f), 18, new Color(240, 80, 80, 255));
            return;
        }

        // --- C) Sub-Panel: Territorien & Kontinente ---
        var layout = player._replay.Header.Layout;

        // Auslesen der Bitmaske für den ausgewählten Spieler
        var continentMask = MissionHelper.GetControlledContinentMask(state, layout, (byte)playerId);

        Rectangle territoryBox = new(innerX, currentY, contentWidth, 68f);
        Raylib.DrawRectangleRounded(territoryBox, 0.08f, 4, SubPanelBgColor);
        Raylib.DrawRectangleRoundedLinesEx(territoryBox, 0.08f, 4, 1.0f, BorderColor);

        // Dynamische Ermittlung der Kontinent-Namen via Bitmaske
        string continents = GetPlayerContinentsText(continentMask, layout.Map);

        Raylib.DrawText($"Territories: {territoryCount}", (int)(territoryBox.X + 14f), (int)(territoryBox.Y + 12f), 15, Color.White);
        Raylib.DrawText($"Continents: {continents}", (int)(territoryBox.X + 14f), (int)(territoryBox.Y + 37f), 14, Color.SkyBlue);

        currentY += 78f;
        
        // --- D) Sub-Panel: Verstärkungsbonus ---

        // 1. Basis-Verstärkung berechnen
        byte baseBonus = ReinforcementCalculator.CalculateBaseTroops(in state, (byte)playerId);

        // 3. Formel-Bestandteile dynamisch zusammenbauen
        List<string> formulaParts = new() { baseBonus.ToString() };
        int totalTroops = baseBonus;

        for (int i = 0; i < layout.Map.Continents.Length; i++)
        {
            // Ist Kontinent i unter Kontrolle?
            if ((continentMask & (1UL << i)) != 0)
            {
                var continent = layout.Map.Continents[i];
                int bonus = continent.BonusTroops; // Hinweis: Falls deine Eigenschaft anders heißt (z. B. TroopBonus), hier anpassen

                // Variante A (Mit Namen): "Nordamerika(5)"
                formulaParts.Add($"{continent.Name}({bonus})");
        
                // Variante B (Nur Zahl): Falls du lieber "4 + 5 + 2 = 11" willst, nutze stattdessen:
                // formulaParts.Add(bonus.ToString());

                totalTroops += bonus;
            }
        }

        // Ergibt z. B.: "4 + Nordamerika(5) + Afrika(2) = 11"
        string bonusFormula = string.Join(" + ", formulaParts) + $" = {totalTroops} Truppen";

        // 4. Panel zeichnen
        Rectangle bonusBox = new(innerX, currentY, contentWidth, 68f);
        Raylib.DrawRectangleRounded(bonusBox, 0.08f, 4, SubPanelBgColor);
        Raylib.DrawRectangleRoundedLinesEx(bonusBox, 0.08f, 4, 1.0f, BorderColor);

        Raylib.DrawText("Verstärkungsbonus:", (int)(bonusBox.X + 14f), (int)(bonusBox.Y + 10f), 14, Color.LightGray);
        Raylib.DrawText(bonusFormula, (int)(bonusBox.X + 14f), (int)(bonusBox.Y + 35f), 15, Color.Lime);

        currentY += 78f;
        
        // --- E) Sub-Panel: Karten auf der Hand ---
Rectangle cardsBox = new(innerX, currentY, contentWidth, 122f);
Raylib.DrawRectangleRounded(cardsBox, 0.08f, 4, SubPanelBgColor);
Raylib.DrawRectangleRoundedLinesEx(cardsBox, 0.08f, 4, 1.0f, BorderColor);

Raylib.DrawText("Cards:", (int)(cardsBox.X + 14f), (int)(cardsBox.Y + 10f), 15, Color.Gold);

var cardsBitboard = CardHelper.GetPlayerCardsBitboard(state, (byte)playerId);
var cardsMask = layout.Deck.AllCardsMask;
var territoryToType = layout.Deck.TerritoryToType;

List<string> infantry = new();
List<string> cavalry = new();
List<string> artillery = new();
int jokerCount = 0;

// Nur die Karten des Spielers filtern
ulong playerHand = cardsBitboard & cardsMask;

for (int i = 0; i < layout.Deck.CardCount; i++)
{
    // 1. ZUERST prüfen, ob der Spieler diese Karte überhaupt hat!
    if ((playerHand & (1UL << i)) == 0)
        continue;

    var cardType = territoryToType[i]; 

    // 2. Je nach Typ verarbeiten (Joker greifen nicht auf TerritoryNames zu)
    switch (cardType)
    {
        case CardType.Infantry:
            infantry.Add(layout.Map.TerritoryNames[i]);
            break;
        case CardType.Cavalry:
            cavalry.Add(layout.Map.TerritoryNames[i]);
            break;
        case CardType.Artillery:
            artillery.Add(layout.Map.TerritoryNames[i]);
            break;
        case CardType.Joker:
            jokerCount++;
            break;
    }
}

// Formatierte Strings erzeugen
string infantryText  = infantry.Count > 0  ? string.Join(", ", infantry)  : "-";
string cavalryText   = cavalry.Count > 0   ? string.Join(", ", cavalry)   : "-";
string artilleryText = artillery.Count > 0 ? string.Join(", ", artillery) : "-";
string jokerText     = jokerCount > 0      ? $"Joker ({jokerCount})"        : "-";

// Visualisierung
float cardTextY = cardsBox.Y + 34f;
float lineSpacing = 19f;

Raylib.DrawText($"Infanterie: {infantryText}", (int)(cardsBox.X + 18f), (int)cardTextY, 13, TextMutedColor); 
cardTextY += lineSpacing;

Raylib.DrawText($"Kavallerie: {cavalryText}", (int)(cardsBox.X + 18f), (int)cardTextY, 13, TextMutedColor); 
cardTextY += lineSpacing;

Raylib.DrawText($"Artillerie: {artilleryText}", (int)(cardsBox.X + 18f), (int)cardTextY, 13, TextMutedColor); 
cardTextY += lineSpacing;

Raylib.DrawText($"Joker: {jokerText}", (int)(cardsBox.X + 18f), (int)cardTextY, 13, TextMutedColor);

currentY += 132f;
        
        // --- F) Sub-Panel: Mission ---
        Rectangle missionBox = new(innerX, currentY, contentWidth, 46f);
        Raylib.DrawRectangleRounded(missionBox, 0.08f, 4, SubPanelBgColor);
        Raylib.DrawRectangleRoundedLinesEx(missionBox, 0.08f, 4, 1.0f, new Color(180, 100, 30, 255));

        byte playerMissionId = MissionHelper.GetPlayerMission(state, (byte)playerId);
        MissionDefinition mission = layout.Missions[playerMissionId];
        string missionText = MissionTranslator.TranslateMission(mission,layout);
        Raylib.DrawText(missionText, (int)(missionBox.X + 14f), (int)(missionBox.Y + 14f), 14, Color.Orange);
    }

    /// <summary>
    /// Wandelt die Kontinent-Bitmaske eines Spielers in einen lesbaren String um.
    /// </summary>
    private static string GetPlayerContinentsText(ulong continentMask, MapLayout layout)
    {
        // Bitmaske 0 = Spieler besitzt keinen einzigen Kontinent komplett
        if (continentMask == 0)
            return "None";

        List<string> controlled = new();

        // Iteriere über alle definierten Kontinente
        for (int i = 0; i < layout.Continents.Length; i++)
        {
            // Prüfe, ob das Bit für Kontinent i gesetzt ist (Option 1)
            bool isControlled = (continentMask & (1UL << i)) != 0;

            if (isControlled)
            {
                // Direkt über den Index auf den Namen des Kontinents zugreifen
                controlled.Add(layout.Continents[i].Name);
            }
        }

        return controlled.Count > 0 
            ? string.Join(", ", controlled) 
            : "None";
    }
    
    /// <summary>
    /// Berechnet die Höhe des Panels abhängig davon, wie viele Zeilen die Config benötigt.
    /// </summary>
    private static float GetConfigPanelHeight(PlayerConfiguration config)
    {
        return config switch
        {
            RandomBotConfiguration => 34f,      // Type + Seed in 1 Zeile
            _ => 34f                            // Fallback
        };
    }

    /// <summary>
    /// Rendert die spezifischen Eigenschaften je nach Config-Klasse.
    /// </summary>
    private static void RenderPlayerConfigDetails(Rectangle box, PlayerConfiguration config)
    {
        float startX = box.X + 12f;
        float startY = box.Y + 9f;

        switch (config)
        {
            case RandomBotConfiguration randomBot:
                // Bot Typ links, Seed rechts
                Raylib.DrawText("Type: RandomBot", (int)startX, (int)startY, 14, TextMutedColor);
                Raylib.DrawText($"Seed: {randomBot.Seed}", (int)(box.X + box.Width - 110f), (int)startY, 14, TextMutedColor);
                break;

      

            default:
                // Fallback für unbekannte oder Basis-Player
                string typeName = config.GetType().Name.Replace("Configuration", "");
                Raylib.DrawText($"Type: {typeName}", (int)startX, (int)startY, 14, TextMutedColor);
                break;
        }
    }
}