using RiskEngine.AI.Configuration;
using RiskEngine.Constants;
using RiskEngine.Core;
using RiskEngine.Layout;

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
        var playerConfigs = ConfigurePlayerConfigs(config.PlayerCount);

        //Select a game seed
        var seed = ConfigureGameSeed();


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
            Console.WriteLine("Custom layouts are not yet supported. Using the standard Risk layout.");

        return RiskMapFactory.CreateStandardRiskMap(config);
    }

    private static EngineConfig ConfigureEngineConfig()
    {
        if (SetupHelper.AskYesNo("Use default configuration?")) return EngineConfig.Default;

        //1. Get player count
        var playerCount = (byte)SetupHelper.AskRange("Number of players", EngineConstants.MIN_PLAYERS,
            EngineConstants.MAX_PLAYERS);

        //2. Get max Rounds
        var maxRounds =
            (ushort)SetupHelper.AskRange("Maximum Rounds", EngineConstants.MIN_ROUNDS, EngineConstants.MAX_ROUNDS);

        return new EngineConfig(playerCount, maxRounds);
    }


    private static ulong ConfigureGameSeed()
    {
        return SetupHelper.AskRange("Seed of Game", 0, int.MaxValue);
    }


    private static PlayerConfiguration[] ConfigurePlayerConfigs(int playerCount)
    {
        var configs = new PlayerConfiguration[playerCount];

        for (var i = 0; i < playerCount; i++)
        {
            Console.WriteLine();
            Console.WriteLine($"=== Configure Player {i + 1} ===");

            configs[i] = ConfigurePlayer();
        }

        return configs;
    }

    private static PlayerConfiguration ConfigurePlayer()
    {
        var agentTypes = Enum.GetValues<AgentType>();

        Console.WriteLine("Select player type:");
        for (var i = 0; i < agentTypes.Length; i++) Console.WriteLine($"{i + 1}. {agentTypes[i]}Bot");

        var selection = SetupHelper.AskRange("Selection", 1, (ulong)agentTypes.Length);
        var selectedType = agentTypes[selection - 1];

        return ConfigureByAgentType(selectedType);
    }

    private static PlayerConfiguration ConfigureByAgentType(AgentType type)
    {
        return type switch
        {
            AgentType.Random => ConfigureRandomBot(),
            AgentType.Aggro => ConfigureAggroBot(),
            _ => throw new NotSupportedException($"Configuration for {type} is not implemented.")
        };
    }

    private static RandomBotConfiguration ConfigureRandomBot()
    {
        Console.WriteLine("\n--- RandomBot Configuration ---");
        var seed = SetupHelper.AskRange("Random seed", ulong.MinValue, ulong.MaxValue);

        return new RandomBotConfiguration { Seed = seed };
    }

    private static AggroBotConfiguration ConfigureAggroBot()
    {
        Console.WriteLine("\n--- AggroBot Selected ---");
        return new AggroBotConfiguration();
    }
}