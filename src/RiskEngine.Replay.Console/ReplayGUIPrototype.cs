using System.Numerics;
using Raylib_cs;
using RiskEngine.Replay;
using RiskEngine.State;

namespace RiskEngine.Replay.ConsoleView;

public class ReplayGuiPrototype
{
    private readonly Dictionary<byte, Vector2> _territoryPositions;

    private readonly Dictionary<byte, Color> _playerColors = new()
    {
        { 0, Color.Red },
        { 1, Color.Blue },
        { 2, Color.Green },
        { 3, Color.Yellow },
        { 4, Color.Magenta },
        { 5, Color.Orange }
    };

    public ReplayGuiPrototype()
    {
        _territoryPositions = GenerateImageBasedCoordinates();
    }

    public void Run(ReplayPlayer player, GameLayout layout)
    {
        Raylib.InitWindow(1280, 720, "Risiko Engine - Replay Viewer");
        Raylib.SetTargetFPS(60);

        Texture2D mapTexture = Raylib.LoadTexture("image_2449a9.jpg");

        while (!Raylib.WindowShouldClose())
        {
            HandleWindowShortcuts(player);

            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(20, 20, 25, 255));

            // --- LAYER 0: Hintergrundbild (stark abgedunkelt für hohen Kontrast) ---
            if (mapTexture.Id != 0)
            {
                Rectangle sourceRec = new Rectangle(0, 0, mapTexture.Width, mapTexture.Height);
                Rectangle destRec = new Rectangle(0, 0, 1280, 720);
                Raylib.DrawTexturePro(mapTexture, sourceRec, destRec, Vector2.Zero, 0f, new Color(255, 255, 255, 120));
            }

            var currentFrame = player.CurrentFrame;
            var state = currentFrame.State;
            int totalTerritories = layout.Map.TerritoryCount;

            // Ermittle relevante Territorien, die sich im aktuellen Frame verändert haben
            HashSet<byte> activeTerritories = GetActiveTerritories(player, totalTerritories);

            // --- LAYER 1: Standard-Verbindungsnetz (Sehr dezent im Hintergrund) ---
            for (byte tId = 0; tId < totalTerritories; tId++)
            {
                Vector2 startPos = GetOrCalculatePosition(tId);
                IEnumerable<byte> neighbors = layout.Map.Adjacencies[tId];

                foreach (byte nId in neighbors)
                {
                    if (tId < nId)
                    {
                        Vector2 endPos = GetOrCalculatePosition(nId);

                        if ((tId == 0 && nId == 29) || (tId == 29 && nId == 0))
                        {
                            Raylib.DrawLineEx(startPos, new Vector2(-50, startPos.Y), 1.5f, new Color(80, 80, 90, 30));
                            Raylib.DrawLineEx(endPos, new Vector2(1330, endPos.Y), 1.5f, new Color(80, 80, 90, 30));
                        }
                        else
                        {
                            Raylib.DrawLineEx(startPos, endPos, 1.5f, new Color(80, 80, 90, 30));
                        }
                    }
                }
            }

            // --- LAYER 2: Aktionslinie zwischen beteiligten Territorien (z. B. Angriff / Verschiebung) ---
            if (activeTerritories.Count == 2)
            {
                var activeArray = activeTerritories.ToArray();
                Vector2 posA = GetOrCalculatePosition(activeArray[0]);
                Vector2 posB = GetOrCalculatePosition(activeArray[1]);

                // Sonderbehandlung Wrap-Around Alaska (0) <-> Kamtschatka (29)
                if ((activeArray[0] == 0 && activeArray[1] == 29) || (activeArray[0] == 29 && activeArray[1] == 0))
                {
                    Raylib.DrawLineEx(posA, new Vector2(-50, posA.Y), 4f, Color.Gold);
                    Raylib.DrawLineEx(posB, new Vector2(1330, posB.Y), 4f, Color.Gold);
                }
                else
                {
                    // Dicke leuchtende Aktionslinie
                    Raylib.DrawLineEx(posA, posB, 4f, Color.Gold);
                }
            }

            // --- LAYER 3: Territorien (Nodes) zeichnen ---
            for (byte tId = 0; tId < totalTerritories; tId++)
            {
                Vector2 pos = GetOrCalculatePosition(tId);
                byte owner = GameStateHelper.GetTerritoryOwner(state, tId);
                ushort troops = GameStateHelper.GetTerritoryTroops(state, tId);

                bool isActive = activeTerritories.Contains(tId);
                Color baseColor = _playerColors.GetValueOrDefault(owner, Color.Gray);

                if (isActive)
                {
                    // === AKTIVES TERRITORIUM (Fokus) ===
                    // Goldene Aura / Glow-Ring
                    Raylib.DrawCircleV(pos, 26, new Color(255, 215, 0, 180));
                    // Vollsatter Spieler-Kreis (vergrößert)
                    Raylib.DrawCircleV(pos, 21, baseColor);
                    // Konturschichten
                    Raylib.DrawCircleLines((int)pos.X, (int)pos.Y, 21, Color.White);
                    Raylib.DrawCircleLines((int)pos.X, (int)pos.Y, 22, Color.Black);

                    // Truppenanzahl (groß & fett)
                    string text = troops.ToString();
                    int textWidth = Raylib.MeasureText(text, 16);
                    Raylib.DrawText(text, (int)pos.X - textWidth / 2, (int)pos.Y - 8, 16, Color.White);
                }
                else
                {
                    // === INAKTIVES TERRITORIUM (Stark gedimmt) ===
                    byte alpha = 40; // Sehr dezent
                    Color nodeColor = new Color(baseColor.R, baseColor.G, baseColor.B, alpha);
                    Color borderColor = new Color((byte)0, (byte)0, (byte)0, (byte)50);
                    Color textColor = new Color((byte)200, (byte)200, (byte)200, (byte)70);

                    Raylib.DrawCircleV(pos, 15, nodeColor);
                    Raylib.DrawCircleLines((int)pos.X, (int)pos.Y, 15, borderColor);

                    string text = troops.ToString();
                    int textWidth = Raylib.MeasureText(text, 12);
                    Raylib.DrawText(text, (int)pos.X - textWidth / 2, (int)pos.Y - 6, 12, textColor);
                }
            }

            // --- LAYER 4: Statusleiste oben ---
            DrawHeaderStatus(player, state);

            Raylib.EndDrawing();
        }

        Raylib.UnloadTexture(mapTexture);
        Raylib.CloseWindow();
    }

    /// <summary>
    /// Vergleicht den aktuellen Frame mit dem vorherigen Frame, um exakt die Territorien zu ermitteln,
    /// auf denen sich Truppen oder Besitzer verändert haben.
    /// </summary>
    private static HashSet<byte> GetActiveTerritories(ReplayPlayer player, int totalTerritories)
    {
        var active = new HashSet<byte>();
        
        if (player.CurrentFrameIndex == 0) 
            return active;

        var currState = player.CurrentFrame.State;
        
        // Hole den vorherigen Frame über den Index
        var prevState = player.GetFrame(player.CurrentFrameIndex - 1).State;

        for (byte tId = 0; tId < totalTerritories; tId++)
        {
            byte prevOwner = GameStateHelper.GetTerritoryOwner(prevState, tId);
            byte currOwner = GameStateHelper.GetTerritoryOwner(currState, tId);
            ushort prevTroops = GameStateHelper.GetTerritoryTroops(prevState, tId);
            ushort currTroops = GameStateHelper.GetTerritoryTroops(currState, tId);

            // Falls sich Besitzer oder Truppenstärke geändert haben -> Territorium ist aktiv
            if (prevOwner != currOwner || prevTroops != currTroops)
            {
                active.Add(tId);
            }
        }

        return active;
    }

    private void DrawHeaderStatus(ReplayPlayer player, GameState state)
    {
        Raylib.DrawRectangle(0, 0, 1280, 40, new Color(20, 20, 25, 230));

        string statusPart1 = $"Frame: {player.CurrentFrameIndex}/{player.FrameCount - 1}  |  Runde: {state.CurrentRound}  |  Am Zug: ";
        string statusPlayer = $"Player {state.PlayerTurn}";
        string statusPart3 = $"  |  Phase: {state.CurrentPhase}";

        int xOffset = 15;

        Raylib.DrawText(statusPart1, xOffset, 11, 18, Color.RayWhite);
        xOffset += Raylib.MeasureText(statusPart1, 18);

        Color currentPlayerColor = _playerColors.GetValueOrDefault((byte)state.PlayerTurn, Color.RayWhite);
        Raylib.DrawText(statusPlayer, xOffset, 11, 18, currentPlayerColor);
        xOffset += Raylib.MeasureText(statusPlayer, 18);

        Raylib.DrawText(statusPart3, xOffset, 11, 18, Color.RayWhite);
    }

    private static void HandleWindowShortcuts(ReplayPlayer player)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.Space) || Raylib.IsKeyPressed(KeyboardKey.Enter)) 
            player.NextEvent();
            
        if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.Backspace)) 
            player.PreviousEvent();
            
        if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S)) 
            player.NextPlayer();
            
        if (Raylib.IsKeyPressed(KeyboardKey.Up)) 
            player.PreviousPlayer();
            
        if (Raylib.IsKeyPressed(KeyboardKey.PageDown) || Raylib.IsKeyPressed(KeyboardKey.R)) 
            player.NextRound();
            
        if (Raylib.IsKeyPressed(KeyboardKey.PageUp)) 
            player.PreviousRound();
    }

    private Vector2 GetOrCalculatePosition(byte territoryId)
    {
        if (_territoryPositions.TryGetValue(territoryId, out var pos))
            return pos;

        int col = territoryId % 10;
        int row = territoryId / 10;
        return new Vector2(80 + col * 110, 500 + row * 60);
    }

    private static Dictionary<byte, Vector2> GenerateImageBasedCoordinates()
    {
        return new Dictionary<byte, Vector2>
        {
            // --- Nordamerika (IDs 0 - 8) ---
            { 0,  new Vector2(100, 160) }, // Alaska
            { 1,  new Vector2(220, 160) }, // Nordwest-Territorium
            { 2,  new Vector2(200, 240) }, // Alberta
            { 3,  new Vector2(290, 250) }, // Ontario
            { 4,  new Vector2(380, 260) }, // Ostkanada
            { 5,  new Vector2(210, 330) }, // Weststaaten
            { 6,  new Vector2(300, 350) }, // Oststaaten
            { 7,  new Vector2(440, 100) }, // Grönland
            { 8,  new Vector2(230, 440) }, // Mittelamerika

            // --- Südamerika (IDs 9 - 12) ---
            { 9,  new Vector2(310, 520) }, // Venezuela
            { 10, new Vector2(310, 610) }, // Peru
            { 11, new Vector2(410, 580) }, // Brasilien
            { 12, new Vector2(330, 690) }, // Argentinien

            // --- Afrika (IDs 13 - 18) ---
            { 13, new Vector2(560, 510) }, // Nordafrika
            { 14, new Vector2(660, 490) }, // Ägypten
            { 15, new Vector2(640, 600) }, // Zentralafrika
            { 16, new Vector2(720, 580) }, // Ostafrika
            { 17, new Vector2(760, 690) }, // Madagaskar
            { 18, new Vector2(650, 690) }, // Südafrika

            // --- Europa (IDs 19 - 25) ---
            { 19, new Vector2(530, 400) }, // Westeuropa
            { 20, new Vector2(630, 390) }, // Südeuropa
            { 21, new Vector2(620, 310) }, // Nordeuropa
            { 22, new Vector2(620, 180) }, // Skandinavien
            { 23, new Vector2(520, 300) }, // Großbritannien
            { 24, new Vector2(530, 210) }, // Island
            { 25, new Vector2(740, 250) }, // Russland

            // --- Asien (IDs 26 - 37) ---
            { 26, new Vector2(840, 230) }, // Ural
            { 27, new Vector2(940, 210) }, // Sibirien
            { 28, new Vector2(1060, 190) },// Jakutien
            { 29, new Vector2(1160, 200) },// Kamtschatka
            { 30, new Vector2(1010, 260) },// Irkutsk
            { 31, new Vector2(1020, 330) },// Mongolei
            { 32, new Vector2(960, 390) }, // China
            { 33, new Vector2(1000, 490) },// Südostasien
            { 34, new Vector2(880, 460) }, // Indien
            { 35, new Vector2(830, 350) }, // Afghanistan
            { 36, new Vector2(750, 440) }, // Naher Osten
            { 37, new Vector2(1160, 350) },// Japan

            // --- Australien (IDs 38 - 41) ---
            { 38, new Vector2(1010, 600) },// Indonesien
            { 39, new Vector2(1120, 570) },// Neu Guinea
            { 40, new Vector2(1040, 680) },// Westaustralien
            { 41, new Vector2(1140, 670) } // Ostaustralien
        };
    }
}