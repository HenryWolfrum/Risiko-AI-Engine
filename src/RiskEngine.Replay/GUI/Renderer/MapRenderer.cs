using System;
using System.IO;
using System.Numerics;
using Raylib_cs;
using RiskEngine.State;

namespace RiskEngine.Replay.GUI;

public class MapRenderer : ISectionRenderer
{
    private Texture2D _mapTexture;
    private bool _isLoaded;
    private bool _loadAttempted;

    // Desaturated, soft light blue for ocean background
    private static readonly Color MapBackgroundColor = new(150, 185, 210, 255);

    public void Render(Rectangle bounds, ReplayUIContext context)
    {
        if (!_loadAttempted)
        {
            LoadMapTexture();
        }

        // --- LAYER 1: BACKGROUND & MAP ---
        Raylib.DrawRectangleRec(bounds, MapBackgroundColor);

        if (!_isLoaded)
        {
            Raylib.DrawText("Map image not found", (int)bounds.X + 20, (int)bounds.Y + 20, 20, Color.DarkBlue);
            return;
        }

        float scale = Math.Min(bounds.Width / _mapTexture.Width, bounds.Height / _mapTexture.Height);
        float renderWidth = _mapTexture.Width * scale;
        float renderHeight = _mapTexture.Height * scale;

        float offsetX = bounds.X + (bounds.Width - renderWidth) / 2f;
        float offsetY = bounds.Y + (bounds.Height - renderHeight) / 2f;
        Vector2 renderOffset = new(offsetX, offsetY);

        Raylib.DrawTextureEx(_mapTexture, renderOffset, 0.0f, scale, Color.White);

        // Dynamic radius and font size based on map scale
        float radius = Math.Clamp(renderWidth * 0.0135f, 14f, 28f);
        int fontSize = (int)(radius * 1.2f);

        var currentFrame = context.Player.CurrentFrame;
        var action = currentFrame.Action; // Assuming this holds your action data or is null/empty

        // --- LAYER 2: ACTION HIGHLIGHTS & ARROWS (Underneath circles) ---
        if (action != null)
        {
            switch (action.Value.Type)
            {
                case ActionType.Reinforce:
                    Vector2 reinforceTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    DrawTerritoryHighlight(reinforceTarget, radius, Color.Lime);
                    break;

                case ActionType.Attack:
                    Vector2 attackSource = GetScreenPos(action.Value.SourceTerritory, renderOffset, renderWidth, renderHeight);
                    Vector2 attackTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    
                    DrawTerritoryHighlight(attackSource, radius, Color.Red);
                    DrawTerritoryHighlight(attackTarget, radius, Color.Orange);
                    DrawActionArrow(attackSource, attackTarget, radius, Color.Red, 4f);
                    break;

                case ActionType.Conquer:
                    Vector2 conquerTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    DrawTerritoryHighlight(conquerTarget, radius, Color.Yellow);
                    break;

                case ActionType.Fortify:
                    Vector2 fortifySource = GetScreenPos(action.Value.SourceTerritory, renderOffset, renderWidth, renderHeight);
                    Vector2 fortifyTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    
                    DrawTerritoryHighlight(fortifySource, radius, Color.SkyBlue);
                    DrawTerritoryHighlight(fortifyTarget, radius, Color.SkyBlue);
                    DrawActionArrow(fortifySource, fortifyTarget, radius, Color.SkyBlue, 3f);
                    break;

                case ActionType.TurnInCards:
                    break;
            }
        }

        // --- LAYER 3: TERRITORY CIRCLES & TROOP COUNTS ---
        for (int i = 0; i < TerritoryLayout.Entries.Length; i++)
        {
            Vector2 center = GetScreenPos(i, renderOffset, renderWidth, renderHeight);

            int troops = GameStateHelper.GetTerritoryTroops(currentFrame.State, i);
            Color playerColor = ReplayViewer.GetPlayerColor(GameStateHelper.GetTerritoryOwner(currentFrame.State, i));

            Raylib.DrawCircleV(center, radius + 2f, Color.Black);
            Raylib.DrawCircleV(center, radius, playerColor);

            string text = troops.ToString();
            int textWidth = Raylib.MeasureText(text, fontSize);
            Raylib.DrawText(text, (int)(center.X - textWidth / 2f), (int)(center.Y - fontSize / 2f), fontSize, Color.White);
        }

        // --- LAYER 4: ACTION BADGES ---
        if (action != null)
        {
            switch (action.Value.Type)
            {
                case ActionType.Reinforce:
                    Vector2 reinforceTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    DrawBadge(reinforceTarget + new Vector2(0, -radius - 12f), $"+{action.Value.TroopCount}", Color.Lime, Color.Black);
                    break;

                case ActionType.Attack:
                    Vector2 atkSource = GetScreenPos(action.Value.SourceTerritory, renderOffset, renderWidth, renderHeight);
                    Vector2 atkTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    Vector2 midAtk = (atkSource + atkTarget) / 2f;
                    
                    DrawBadge(midAtk, $"{action.Value.ChosenAttackerDiceCount} v {action.Value.ChosenDefenderDiceCount}", Color.Red, Color.White);
                    break;

                case ActionType.Conquer:
                    Vector2 conquerTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    DrawBadge(conquerTarget + new Vector2(0, -radius - 12f), $"+{action.Value.TroopCount}", Color.Yellow, Color.Black);
                    break;

                case ActionType.Fortify:
                    Vector2 fortSource = GetScreenPos(action.Value.SourceTerritory, renderOffset, renderWidth, renderHeight);
                    Vector2 fortTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    Vector2 midFort = (fortSource + fortTarget) / 2f;
                    
                    DrawBadge(midFort, $"-> {action.Value.TroopCount}", Color.SkyBlue, Color.Black);
                    break;
            }
        }
        
        // --- LAYER 5: ACTION BANNER (Unten Links) ---
        if (action != null)
        {
            var layout = context.Player._replay.Header.Layout;
            byte activePlayerId = (byte)currentFrame.State.PlayerTurn;

            string actionText = ActionTranslator.TranslateAction(action.Value, layout, activePlayerId);
            DrawActionBanner(bounds, actionText, activePlayerId);
        }

        // --- LAYER 6: GAME STATS / HUD (Oben Rechts) ---
        DrawTradedCardSetsHUD(bounds, currentFrame.State.CardSetsTradedCount);
    }

    private void LoadMapTexture()
    {
        _loadAttempted = true;
        string imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "risk_map.png");

        if (File.Exists(imagePath))
        {
            _mapTexture = Raylib.LoadTexture(imagePath);
            _isLoaded = _mapTexture.Id != 0;
        }
    }

    // --- HELPER METHODS ---

    private static Vector2 GetScreenPos(int territoryId, Vector2 offset, float width, float height)
    {
        var entry = TerritoryLayout.Entries[territoryId];
        return offset + new Vector2(entry.ScaleX * width, entry.ScaleY * height);
    }

    private static void DrawTerritoryHighlight(Vector2 position, float baseRadius, Color color)
    {
        Raylib.DrawCircleLinesV(position, baseRadius + 4f, color);
        Raylib.DrawCircleLinesV(position, baseRadius + 5f, color);
        Raylib.DrawCircleLinesV(position, baseRadius + 6f, color);
    }

    private static void DrawActionArrow(Vector2 start, Vector2 end, float circleRadius, Color color, float thickness)
    {
        Vector2 direction = Vector2.Normalize(end - start);
        float distance = Vector2.Distance(start, end);

        if (distance < circleRadius * 2.5f) return;

        Vector2 p1 = start + direction * (circleRadius + 7f);
        Vector2 p2 = end - direction * (circleRadius + 9f);

        Raylib.DrawLineEx(p1, p2, thickness, color);

        float arrowSize = 10f;
        Vector2 right = new(-direction.Y, direction.X);

        Vector2 headLeft = p2 - direction * arrowSize + right * (arrowSize * 0.6f);
        Vector2 headRight = p2 - direction * arrowSize - right * (arrowSize * 0.6f);

        Raylib.DrawTriangle(p2, headRight, headLeft, color);
    }

    private static void DrawBadge(Vector2 position, string text, Color bgColor, Color textColor)
    {
        int fontSize = 14;
        int paddingHorizontal = 6;
        int paddingVertical = 3;

        int textWidth = Raylib.MeasureText(text, fontSize);
        float rectWidth = textWidth + (paddingHorizontal * 2);
        float rectHeight = fontSize + (paddingVertical * 2);

        Rectangle rect = new(
            position.X - (rectWidth / 2f),
            position.Y - (rectHeight / 2f),
            rectWidth,
            rectHeight
        );

        Raylib.DrawRectangleRounded(rect, 0.4f, 4, Color.Black); 
        
        Rectangle innerRect = new(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
        Raylib.DrawRectangleRounded(innerRect, 0.4f, 4, bgColor);

        Raylib.DrawText(text, (int)(position.X - textWidth / 2f), (int)(position.Y - fontSize / 2f), fontSize, textColor);
    }
    
    private static void DrawActionBanner(Rectangle bounds, string text, byte playerId)
    {
        int fontSize = 15;
        int textWidth = Raylib.MeasureText(text, fontSize);

        float paddingX = 14f;
        float paddingY = 10f;
        float boxWidth = textWidth + (paddingX * 2);
        float boxHeight = fontSize + (paddingY * 2);

        float margin = 15f;
        
        Rectangle box = new(
            bounds.X + margin,
            bounds.Y + bounds.Height - boxHeight - margin,
            boxWidth,
            boxHeight
        );

        Color playerColor = ReplayViewer.GetPlayerColor(playerId);
        Color boxBg = new(20, 24, 32, 220);

        Raylib.DrawRectangleRounded(box, 0.25f, 4, boxBg);
        Raylib.DrawRectangleRoundedLinesEx(box, 0.25f, 4, 1.5f, playerColor);

        Raylib.DrawText(text, (int)(box.X + paddingX), (int)(box.Y + paddingY), fontSize, Color.White);
    }

    /// <summary>
    /// Zeichnet eine kleine HUD-Anzeige oben rechts auf der Karte für eingelöste Kartensets.
    /// </summary>
    private static void DrawTradedCardSetsHUD(Rectangle bounds, byte cardSetsTradedCount)
    {
        string text = $"Traded Card Sets: {cardSetsTradedCount}";
        int fontSize = 14;
        int textWidth = Raylib.MeasureText(text, fontSize);

        float paddingX = 12f;
        float paddingY = 8f;
        float boxWidth = textWidth + (paddingX * 2);
        float boxHeight = fontSize + (paddingY * 2);

        float margin = 15f;

        // Oben rechts positionieren
        Rectangle box = new(
            bounds.X + bounds.Width - boxWidth - margin,
            bounds.Y + margin,
            boxWidth,
            boxHeight
        );

        Color boxBg = new(20, 24, 32, 220); // Semi-transparentes Dunkelgrau
        Color accentColor = new(240, 200, 40, 255); // Gold/Gelb als Akzentfarbe für Karten

        Raylib.DrawRectangleRounded(box, 0.25f, 4, boxBg);
        Raylib.DrawRectangleRoundedLinesEx(box, 0.25f, 4, 1.5f, accentColor);

        Raylib.DrawText(text, (int)(box.X + paddingX), (int)(box.Y + paddingY), fontSize, Color.White);
    }
}