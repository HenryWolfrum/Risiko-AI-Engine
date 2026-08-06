using RiskEngine.AI.Bots;
using RiskEngine.AI.Configuration;
using RiskEngine.State;

namespace RiskEngine.AI.Factory;

public static class PlayerFactory
{
    public static IRiskPlayer Create(PlayerConfiguration configuration)
    {
        return configuration switch
        {
            RandomBotConfiguration random => new RandomBot(new EngineRandom(random.Seed)),

            _ => throw new NotSupportedException($"Unsupported configuration type: {configuration.GetType().Name}")
        };
    }
    
    //Create multiple players
    public static IRiskPlayer[] Create(PlayerConfiguration[] configurations)
    {
        IRiskPlayer[] players = new IRiskPlayer[configurations.Length];

        for (int i = 0; i < configurations.Length; i++)
        {
            players[i] = Create(configurations[i]);
        }

        return players;
    }
}