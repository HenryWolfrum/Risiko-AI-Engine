using RiskEngine.AI.Configuration;
using RiskEngine.AI.Factory;
using RiskEngine.Core;
using RiskEngine.Layout;
using RiskEngine.Observer;
using RiskEngine.Player;
using RiskEngine.Setup.Configuration;
using RiskEngine.State;

namespace RiskEngine.Setup;

public static class GameLauncher
{
    public static GameState Play(GameConfiguration config, IGameObserver? observer = null)
    {
        IRiskPlayer[] players = PlayerFactory.Create(config.PlayerConfigurations.ToArray());
        return GameRunner.PlayGame(config.Layout, players, config.Seed, observer);
    }
}