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
        // We draw arrows and highlights first so they sit cleanly under the territory circles.
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
                    
                    // If your action also provides a SourceTerritory for Conquer, you can uncomment this:
                    // Vector2 conquerSource = GetScreenPos(action.SourceTerritory, renderOffset, renderWidth, renderHeight);
                    // DrawActionArrow(conquerSource, conquerTarget, radius, Color.Yellow, 3f);
                    break;

                case ActionType.Fortify:
                    Vector2 fortifySource = GetScreenPos(action.Value.SourceTerritory, renderOffset, renderWidth, renderHeight);
                    Vector2 fortifyTarget = GetScreenPos(action.Value.TargetTerritory, renderOffset, renderWidth, renderHeight);
                    
                    DrawTerritoryHighlight(fortifySource, radius, Color.SkyBlue);
                    DrawTerritoryHighlight(fortifyTarget, radius, Color.SkyBlue);
                    DrawActionArrow(fortifySource, fortifyTarget, radius, Color.SkyBlue, 3f);
                    break;
                    
                case ActionType.TurnInCards:
                    // Card turn-in usually doesn't affect a specific territory visually.
                    // Could trigger a UI effect elsewhere.
                    break;
            }
        }

        // --- LAYER 3: TERRITORY CIRCLES & TROOP COUNTS ---
        for (int i = 0; i < TerritoryLayout.Entries.Length; i++)
        {
            var entry = TerritoryLayout.Entries[i];

            // Calculate relative coordinates to absolute screen positions
            Vector2 center = GetScreenPos(i, renderOffset, renderWidth, renderHeight);

            // Fetch state data for the current territory
            int troops = GameStateHelper.GetTerritoryTroops(currentFrame.State, i);
            Color playerColor = ReplayViewer.GetPlayerColor(GameStateHelper.GetTerritoryOwner(currentFrame.State, i));

            // A) Black outline for maximum contrast
            Raylib.DrawCircleV(center, radius + 2f, Color.Black);

            // B) Player colored fill
            Raylib.DrawCircleV(center, radius, playerColor);

            // C) Centered troop count
            string text = troops.ToString();
            int textWidth = Raylib.MeasureText(text, fontSize);
            Raylib.DrawText(text, (int)(center.X - textWidth / 2f), (int)(center.Y - fontSize / 2f), fontSize, Color.White);
        }

        // --- LAYER 4: ACTION BADGES (On top of everything) ---
        // Drawn last to ensure numbers/dice are never covered by adjacent territory circles.
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
                    
                    // Displays the dice count in the middle of the arrow (e.g. "3 v 2")
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

    /// <summary>
    /// Calculates the screen position for a given territory ID based on map scaling.
    /// </summary>
    private static Vector2 GetScreenPos(int territoryId, Vector2 offset, float width, float height)
    {
        var entry = TerritoryLayout.Entries[territoryId];
        return offset + new Vector2(entry.ScaleX * width, entry.ScaleY * height);
    }

    /// <summary>
    /// Draws concentric circles around a territory to highlight it during an action.
    /// </summary>
    private static void DrawTerritoryHighlight(Vector2 position, float baseRadius, Color color)
    {
        Raylib.DrawCircleLinesV(position, baseRadius + 4f, color);
        Raylib.DrawCircleLinesV(position, baseRadius + 5f, color);
        Raylib.DrawCircleLinesV(position, baseRadius + 6f, color);
    }

    /// <summary>
    /// Draws an arrow between two territories, automatically calculating gaps so it doesn't overlap the circles.
    /// </summary>
    private static void DrawActionArrow(Vector2 start, Vector2 end, float circleRadius, Color color, float thickness)
    {
        Vector2 direction = Vector2.Normalize(end - start);
        float distance = Vector2.Distance(start, end);

        // Do not draw if territories are too close to each other
        if (distance < circleRadius * 2.5f) return;

        // Offset start and end positions to perfectly touch the outer edge of the territory circles
        Vector2 p1 = start + direction * (circleRadius + 7f);
        Vector2 p2 = end - direction * (circleRadius + 9f);

        Raylib.DrawLineEx(p1, p2, thickness, color);

        // Draw arrow head (triangle)
        float arrowSize = 10f;
        Vector2 right = new(-direction.Y, direction.X);

        Vector2 headLeft = p2 - direction * arrowSize + right * (arrowSize * 0.6f);
        Vector2 headRight = p2 - direction * arrowSize - right * (arrowSize * 0.6f);

        Raylib.DrawTriangle(p2, headRight, headLeft, color);
    }

    /// <summary>
    /// Draws a small rounded rectangle badge with text (used for troop counts or dice results).
    /// </summary>
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

        // Black border/shadow
        Raylib.DrawRectangleRounded(rect, 0.4f, 4, Color.Black); 
        
        // Inner colored background
        Rectangle innerRect = new(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
        Raylib.DrawRectangleRounded(innerRect, 0.4f, 4, bgColor);

        // Centered Text
        Raylib.DrawText(text, (int)(position.X - textWidth / 2f), (int)(position.Y - fontSize / 2f), fontSize, textColor);
    }
}