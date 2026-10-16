using System.Numerics;
using Raylib_cs;
using RiskEngine.Actions;
using RiskEngine.State;

namespace RiskEngine.Replay.GUI.Renderer;

public class MapRenderer : ISectionRenderer
{
    // Desaturated, soft light blue for ocean background
    private static readonly Color MapBackgroundColor = new(150, 185, 210, 255);
    private bool _isLoaded;
    private bool _loadAttempted;
    private Texture2D _mapTexture;

    public void Render(Rectangle bounds, ReplayUIContext context)
    {
        if (!_loadAttempted) LoadMapTexture();

        // --- LAYER 1: BACKGROUND & MAP ---
        Raylib.DrawRectangleRec(bounds, MapBackgroundColor);

        if (!_isLoaded)
        {
            Raylib.DrawText("Map image not found", (int)bounds.X + 20, (int)bounds.Y + 20, 20, Color.DarkBlue);
            return;
        }

        var scale = Math.Min(bounds.Width / _mapTexture.Width, bounds.Height / _mapTexture.Height);
        var renderWidth = _mapTexture.Width * scale;
        var renderHeight = _mapTexture.Height * scale;

        var offsetX = bounds.X + (bounds.Width - renderWidth) / 2f;
        var offsetY = bounds.Y + (bounds.Height - renderHeight) / 2f;
        Vector2 renderOffset = new(offsetX, offsetY);

        Raylib.DrawTextureEx(_mapTexture, renderOffset, 0.0f, scale, Color.White);

        // Dynamic radius and font size based on map scale
        var radius = Math.Clamp(renderWidth * 0.0135f, 14f, 28f);
        var fontSize = (int)(radius * 1.2f);

        var currentFrame = context.Player.CurrentFrame;
        var action = currentFrame.Action; // Assuming this holds your action data or is null/empty

        // --- LAYER 2: ACTION HIGHLIGHTS & ARROWS (Underneath circles) ---
        if (action != null)
            switch (action.Value.Type)
            {
                case ActionType.Reinforce:
                    var reinforceTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth,
                        renderHeight);
                    DrawTerritoryHighlight(reinforceTarget, radius, Color.Lime);
                    break;

                case ActionType.Attack:
                    var attackSource = GetScreenPos(action.Value.SourceTerritory, renderOffset, renderWidth,
                        renderHeight);
                    var attackTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth,
                        renderHeight);

                    DrawTerritoryHighlight(attackSource, radius, Color.Red);
                    DrawTerritoryHighlight(attackTarget, radius, Color.Orange);
                    DrawActionArrow(attackSource, attackTarget, radius, Color.Red, 4f);
                    break;

                case ActionType.Conquer:
                    var conquerTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth,
                        renderHeight);
                    DrawTerritoryHighlight(conquerTarget, radius, Color.Yellow);
                    break;

                case ActionType.Fortify:
                    var fortifySource = GetScreenPos(action.Value.SourceTerritory, renderOffset, renderWidth,
                        renderHeight);
                    var fortifyTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth,
                        renderHeight);

                    DrawTerritoryHighlight(fortifySource, radius, Color.SkyBlue);
                    DrawTerritoryHighlight(fortifyTarget, radius, Color.SkyBlue);
                    DrawActionArrow(fortifySource, fortifyTarget, radius, Color.SkyBlue, 3f);
                    break;

                case ActionType.TurnInCards:
                    break;
            }

        // --- LAYER 3: TERRITORY CIRCLES & TROOP COUNTS ---
        for (var i = 0; i < TerritoryLayout.Entries.Length; i++)
        {
            var center = GetScreenPos(i, renderOffset, renderWidth, renderHeight);

            int troops = GameStateHelper.GetTerritoryTroops(currentFrame.State, i);
            var playerColor = ReplayViewer.GetPlayerColor(GameStateHelper.GetTerritoryOwner(currentFrame.State, i));

            Raylib.DrawCircleV(center, radius + 2f, Color.Black);
            Raylib.DrawCircleV(center, radius, playerColor);

            var text = troops.ToString();
            var textWidth = Raylib.MeasureText(text, fontSize);
            Raylib.DrawText(text, (int)(center.X - textWidth / 2f), (int)(center.Y - fontSize / 2f), fontSize,
                Color.White);
        }

        // --- LAYER 4: ACTION BADGES ---
        if (action != null)
            switch (action.Value.Type)
            {
                case ActionType.Reinforce:
                    var reinforceTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth,
                        renderHeight);
                    DrawBadge(reinforceTarget + new Vector2(0, -radius - 12f), $"+{action.Value.TroopCount}",
                        Color.Lime, Color.Black);
                    break;

                case ActionType.Attack:
                    var atkSource = GetScreenPos(action.Value.SourceTerritory, renderOffset, renderWidth, renderHeight);
                    var atkTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    var midAtk = (atkSource + atkTarget) / 2f;

                    DrawBadge(midAtk,
                        $"{action.Value.ChosenAttackerDiceCount} v {action.Value.ChosenDefenderDiceCount}", Color.Red,
                        Color.White);
                    break;

                case ActionType.Conquer:
                    var conquerTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth,
                        renderHeight);
                    DrawBadge(conquerTarget + new Vector2(0, -radius - 12f), $"+{action.Value.TroopCount}",
                        Color.Yellow, Color.Black);
                    break;

                case ActionType.Fortify:
                    var fortSource = GetScreenPos(action.Value.SourceTerritory, renderOffset, renderWidth,
                        renderHeight);
                    var fortTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth,
                        renderHeight);
                    var midFort = (fortSource + fortTarget) / 2f;

                    DrawBadge(midFort, $"-> {action.Value.TroopCount}", Color.SkyBlue, Color.Black);
                    break;
            }

        // --- LAYER 5: ACTION BANNER (Unten Links) ---
        if (action != null)
        {
            var layout = context.Player._replay.Header.Layout;
            var activePlayerId = (byte)currentFrame.State.PlayerTurn;

            var actionText = ActionTranslator.TranslateAction(action.Value, layout, activePlayerId);
            DrawActionBanner(bounds, actionText, activePlayerId);
        }

        // --- LAYER 6: GAME STATS / HUD (Oben Rechts) ---
        DrawTradedCardSetsHUD(bounds, currentFrame.State.CardSetsTradedCount);
    }

    private void LoadMapTexture()
    {
        _loadAttempted = true;
        var imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "risk_map.png");

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
        var direction = Vector2.Normalize(end - start);
        var distance = Vector2.Distance(start, end);

        if (distance < circleRadius * 2.5f) return;

        var p1 = start + direction * (circleRadius + 7f);
        var p2 = end - direction * (circleRadius + 9f);

        Raylib.DrawLineEx(p1, p2, thickness, color);

        var arrowSize = 10f;
        Vector2 right = new(-direction.Y, direction.X);

        var headLeft = p2 - direction * arrowSize + right * (arrowSize * 0.6f);
        var headRight = p2 - direction * arrowSize - right * (arrowSize * 0.6f);

        Raylib.DrawTriangle(p2, headRight, headLeft, color);
    }

    private static void DrawBadge(Vector2 position, string text, Color bgColor, Color textColor)
    {
        var fontSize = 14;
        var paddingHorizontal = 6;
        var paddingVertical = 3;

        var textWidth = Raylib.MeasureText(text, fontSize);
        float rectWidth = textWidth + paddingHorizontal * 2;
        float rectHeight = fontSize + paddingVertical * 2;

        Rectangle rect = new(
            position.X - rectWidth / 2f,
            position.Y - rectHeight / 2f,
            rectWidth,
            rectHeight
        );

        Raylib.DrawRectangleRounded(rect, 0.4f, 4, Color.Black);

        Rectangle innerRect = new(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
        Raylib.DrawRectangleRounded(innerRect, 0.4f, 4, bgColor);

        Raylib.DrawText(text, (int)(position.X - textWidth / 2f), (int)(position.Y - fontSize / 2f), fontSize,
            textColor);
    }

    private static void DrawActionBanner(Rectangle bounds, string text, byte playerId)
    {
        var fontSize = 15;
        var textWidth = Raylib.MeasureText(text, fontSize);

        var paddingX = 14f;
        var paddingY = 10f;
        var boxWidth = textWidth + paddingX * 2;
        var boxHeight = fontSize + paddingY * 2;

        var margin = 15f;

        Rectangle box = new(
            bounds.X + margin,
            bounds.Y + bounds.Height - boxHeight - margin,
            boxWidth,
            boxHeight
        );

        var playerColor = ReplayViewer.GetPlayerColor(playerId);
        Color boxBg = new(20, 24, 32, 220);

        Raylib.DrawRectangleRounded(box, 0.25f, 4, boxBg);
        Raylib.DrawRectangleRoundedLinesEx(box, 0.25f, 4, 1.5f, playerColor);

        Raylib.DrawText(text, (int)(box.X + paddingX), (int)(box.Y + paddingY), fontSize, Color.White);
    }

    /// <summary>
    ///     Zeichnet eine kleine HUD-Anzeige oben rechts auf der Karte für eingelöste Kartensets.
    /// </summary>
    private static void DrawTradedCardSetsHUD(Rectangle bounds, byte cardSetsTradedCount)
    {
        var text = $"Traded Card Sets: {cardSetsTradedCount}";
        var fontSize = 14;
        var textWidth = Raylib.MeasureText(text, fontSize);

        var paddingX = 12f;
        var paddingY = 8f;
        var boxWidth = textWidth + paddingX * 2;
        var boxHeight = fontSize + paddingY * 2;

        var margin = 15f;

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