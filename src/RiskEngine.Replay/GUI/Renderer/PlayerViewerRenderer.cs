using System;
using System.Numerics;
using Raylib_cs;
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
        int seed = player._replay.Header.Seed;
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

        Raylib.DrawText("PLAYER DOSSIER", (int)(bounds.X + padding), (int)(bounds.Y + 8), 15, Color.LightGray);

        // 3. Karten-Hauptbereich
        Rectangle cardBounds = new(
            bounds.X + padding,
            bounds.Y + headerHeight + 4f,
            bounds.Width - (padding * 2f),
            bounds.Height - headerHeight - (padding * 2f)
        );

        bool isActiveTurn = (state.PlayerTurn == _selectedPlayerId);

        // Dossier-Inhalt zeichnen (Schicht 1)
        RenderSinglePlayerDossier(cardBounds, _selectedPlayerId, in state, seed, isActiveTurn);

        // 4. Navigation-Buttons im Dossier-Header platzieren (Schicht 2)
        float btnWidth = 34f;
        float btnHeight = 30f;
        float navY = cardBounds.Y + 10f;

        _prevPlayerBtn.Bounds = new Rectangle(cardBounds.X + 12f, navY, btnWidth, btnHeight);
        _nextPlayerBtn.Bounds = new Rectangle(cardBounds.X + cardBounds.Width - 12f - btnWidth, navY, btnWidth, btnHeight);

        if (_prevPlayerBtn.DrawAndCheck(mousePos))
        {
            _selectedPlayerId = (_selectedPlayerId + 3) % 4;
        }

        if (_nextPlayerBtn.DrawAndCheck(mousePos))
        {
            _selectedPlayerId = (_selectedPlayerId + 1) % 4;
        }
    }

    private static void RenderSinglePlayerDossier(Rectangle bounds, int playerId, in GameState state, int seed, bool isActiveTurn)
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

        // --- B) Sub-Panel: Meta-Info (Type & Seed) ---
        Rectangle metaBox = new(innerX, currentY, contentWidth, 34f);
        Raylib.DrawRectangleRounded(metaBox, 0.12f, 4, SubPanelBgColor);
        Raylib.DrawRectangleRoundedLinesEx(metaBox, 0.12f, 4, 1.0f, BorderColor);

        string botType = "RandomBot";
        Raylib.DrawText($"Type: {botType}", (int)(metaBox.X + 12f), (int)(metaBox.Y + 9f), 14, TextMutedColor);
        Raylib.DrawText($"Seed: {seed}", (int)(metaBox.X + metaBox.Width - 95f), (int)(metaBox.Y + 9f), 14, TextMutedColor);

        currentY += 44f;

        if (!isAlive)
        {
            Rectangle elimBox = new(innerX, currentY, contentWidth, 60f);
            Raylib.DrawRectangleRounded(elimBox, 0.1f, 4, new Color(50, 25, 30, 255));
            Raylib.DrawRectangleRoundedLinesEx(elimBox, 0.1f, 4, 1.0f, new Color(200, 60, 60, 255));
            Raylib.DrawText("STATUS: ELIMINIERT", (int)(elimBox.X + 16f), (int)(elimBox.Y + 20f), 18, new Color(240, 80, 80, 255));
            return;
        }

        // --- C) Sub-Panel: Territorien & Kontinente ---
        Rectangle territoryBox = new(innerX, currentY, contentWidth, 68f);
        Raylib.DrawRectangleRounded(territoryBox, 0.08f, 4, SubPanelBgColor);
        Raylib.DrawRectangleRoundedLinesEx(territoryBox, 0.08f, 4, 1.0f, BorderColor);

        string continents = GetPlayerContinentsText(in state, playerId);
        Raylib.DrawText($"Territories: {territoryCount}", (int)(territoryBox.X + 14f), (int)(territoryBox.Y + 12f), 15, Color.White);
        Raylib.DrawText($"Continents: {continents}", (int)(territoryBox.X + 14f), (int)(territoryBox.Y + 37f), 14, Color.SkyBlue);

        currentY += 78f;

        // --- D) Sub-Panel: Verstärkungsbonus ---
        Rectangle bonusBox = new(innerX, currentY, contentWidth, 80f);
        Raylib.DrawRectangleRounded(bonusBox, 0.08f, 4, SubPanelBgColor);
        Raylib.DrawRectangleRoundedLinesEx(bonusBox, 0.08f, 4, 1.0f, BorderColor);

        int baseBonus = Math.Max(territoryCount / 3, 3);
        Raylib.DrawText("Verstärkungsbonus:", (int)(bonusBox.X + 14f), (int)(bonusBox.Y + 10f), 14, Color.LightGray);
        
        string bonusFormula = $"Max({territoryCount}/3, 3) + Kontinente";
        string bonusTotal = $"= {baseBonus} Truppen";
        Raylib.DrawText(bonusFormula, (int)(bonusBox.X + 14f), (int)(bonusBox.Y + 32f), 13, Color.Lime);
        Raylib.DrawText(bonusTotal, (int)(bonusBox.X + 14f), (int)(bonusBox.Y + 52f), 14, Color.Lime);

        currentY += 90f;

        // --- E) Sub-Panel: Karten auf der Hand ---
        Rectangle cardsBox = new(innerX, currentY, contentWidth, 122f);
        Raylib.DrawRectangleRounded(cardsBox, 0.08f, 4, SubPanelBgColor);
        Raylib.DrawRectangleRoundedLinesEx(cardsBox, 0.08f, 4, 1.0f, BorderColor);

        Raylib.DrawText("Karten in Hand:", (int)(cardsBox.X + 14f), (int)(cardsBox.Y + 10f), 15, Color.Gold);

        float cardTextY = cardsBox.Y + 34f;
        Raylib.DrawText("Infanterie: Alaska", (int)(cardsBox.X + 18f), (int)cardTextY, 13, TextMutedColor); cardTextY += 19f;
        Raylib.DrawText("Kavallerie: -", (int)(cardsBox.X + 18f), (int)cardTextY, 13, TextMutedColor); cardTextY += 19f;
        Raylib.DrawText("Artillerie: China", (int)(cardsBox.X + 18f), (int)cardTextY, 13, TextMutedColor); cardTextY += 19f;
        Raylib.DrawText("Joker: Joker (1)", (int)(cardsBox.X + 18f), (int)cardTextY, 13, TextMutedColor);

        currentY += 132f;

        // --- F) Sub-Panel: Mission ---
        Rectangle missionBox = new(innerX, currentY, contentWidth, 46f);
        Raylib.DrawRectangleRounded(missionBox, 0.08f, 4, SubPanelBgColor);
        Raylib.DrawRectangleRoundedLinesEx(missionBox, 0.08f, 4, 1.0f, new Color(180, 100, 30, 255));

        int targetPlayer = (playerId + 1) % 4;
        Raylib.DrawText($"Mission: Eliminieren Sie Spieler {targetPlayer}!", (int)(missionBox.X + 14f), (int)(missionBox.Y + 14f), 14, Color.Orange);
    }

    private static string GetPlayerContinentsText(in GameState state, int playerId)
    {
        return "Nordamerika, Afrika";
    }
}