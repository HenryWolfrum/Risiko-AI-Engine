using RiskEngine.AI.Configuration;
using RiskEngine.State;

namespace RiskEngine.Replay.Setup;

public static class ReplaySetup
{
    public static ReplayHeader Create()
    {
        //Configure the Engine
       EngineConfig config = ConfigureEngineConfig();
       
       //Select the layout simulated on
       GameLayout layout = ConfigureLayout(config);

       //Create the participating players
       PlayerConfiguration[] playerConfigs = ConfigurePlayerConfigs(config.PlayerCount);
       
       //Select a game seed
       int seed = ConfigureGameSeed();
       
       
       return new ReplayHeader
       {
           Seed = seed,
           Layout = layout,
           PlayerConfigs = playerConfigs
       };


    }

    private static GameLayout ConfigureLayout(EngineConfig config)
    {
        if (!SetupHelper.AskYesNo("Use standard Risk layout?"))
        {
            Console.WriteLine("Custom layouts are not yet supported. Using the standard Risk layout.");
        }

        return RiskMapFactory.CreateStandardRiskMap(config);
    }

    private static EngineConfig ConfigureEngineConfig()
    {
        if (SetupHelper.AskYesNo("Use default configuration?"))
        {
            return new EngineConfig();

        }
        
        //1. Get player count
        byte playerCount =(byte) SetupHelper.AskInt("Number of players", EngineConstants.MIN_PLAYERS,EngineConstants.MAX_PLAYERS);
        
        //2. Get max Rounds
        ushort maxRounds = (ushort)SetupHelper.AskInt("Maximum Rounds",EngineConstants.MIN_ROUNDS,EngineConstants.MAX_ROUNDS);
        
        return new EngineConfig(playerCount, maxRounds);
    }

    
    private static int ConfigureGameSeed()
    {
        return SetupHelper.AskInt("Seed of Game", 0, int.MaxValue);
      
    }
    
    
    
    private static PlayerConfiguration[] ConfigurePlayerConfigs(int playerCount)
    {
        PlayerConfiguration[] configs = new PlayerConfiguration[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            Console.WriteLine();
            Console.WriteLine($"=== Configure Player {i + 1} ===");

            configs[i] = ConfigurePlayer();
        }

        return configs;
    }

    private static PlayerConfiguration ConfigurePlayer()
    {
        while (true)
        {
            Console.WriteLine("Select player type:");
            Console.WriteLine("1. RandomBot");

            int selection = SetupHelper.AskInt("Selection", 1, 1);

            switch (selection)
            {
                case 1:
                    return ConfigureRandomBot();
            }

            Console.WriteLine("Unsupported player type.");
        }
    }

    private static RandomBotConfiguration ConfigureRandomBot()
    {
        Console.WriteLine();
        Console.WriteLine("RandomBot Configuration");

        int seed = SetupHelper.AskInt(
            "Random seed",
            int.MinValue,
            int.MaxValue);

        return new RandomBotConfiguration
        {
            Seed = seed
        };
    }
}