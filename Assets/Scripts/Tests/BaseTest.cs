using Audio;
using E7.Minefield;
using Grid;
using Managers;
using Tests.Beacons;
using Turn;
using UI;
using Units;
using Units.Enemies;
using Units.Players;

namespace Tests
{
    public abstract class BaseTest : SceneTest
    {
        protected InputBeacon InputBeacon { get; } = new InputBeacon();
        
        // The Default testing scene is MainTest
        protected override string Scene => "MainTest";
        
        protected GridManager GridManager => ManagerLocator.Get<GridManager>();
        protected TurnManager TurnManager => ManagerLocator.Get<TurnManager>();
        protected UnitManager UnitManager => ManagerLocator.Get<UnitManager>();
        protected AudioManager AudioManager => ManagerLocator.Get<AudioManager>();
        protected UIManager UIManager => ManagerLocator.Get<UIManager>();
        protected PlayerManager PlayerManager => ManagerLocator.Get<PlayerManager>();
        protected EnemyManager EnemyManager => ManagerLocator.Get<EnemyManager>();
    }
}
