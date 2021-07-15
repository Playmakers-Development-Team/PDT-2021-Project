using System.Collections;
using System.Threading.Tasks;
using Audio;
using Commands;
using E7.Minefield;
using Grid;
using Managers;
using NUnit.Framework;
using Tests.Beacons;
using Tests.Beacons.Base;
using Tests.Constraints;
using Turn;
using Turn.Commands;
using UI;
using Units;
using Units.Commands;
using Units.Enemies;
using Units.Players;
using UnityEngine;

namespace Tests
{
    public abstract class BaseTest : SceneTest
    {
        protected InputBeacon InputBeacon { get; } = new InputBeacon();
        
        // The Default testing scene is MainTest
        protected override string Scene => "MainTest";

        protected virtual float TimeScale => 10f;
        
        protected GridManager GridManager => ManagerLocator.Get<GridManager>();
        protected TurnManager TurnManager => ManagerLocator.Get<TurnManager>();
        protected UnitManager UnitManager => ManagerLocator.Get<UnitManager>();
        protected AudioManager AudioManager => ManagerLocator.Get<AudioManager>();
        protected UIManager UIManager => ManagerLocator.Get<UIManager>();
        protected PlayerManager PlayerManager => ManagerLocator.Get<PlayerManager>();
        protected EnemyManager EnemyManager => ManagerLocator.Get<EnemyManager>();
        protected CommandManager CommandManager => ManagerLocator.Get<CommandManager>();

        protected IEnumerator PrepareAndActivateScene()
        {
            ManagerLocator.Initialize();
            Task task = CommandManager.WaitForCommand<TurnQueueCreatedCommand>();
            yield return ActivateScene();
            yield return new WaitUntil(() => task.IsCompleted);
            Time.timeScale = TimeScale;
        }

        [TearDown] 
        protected void TestCleanup()
        {
            Time.timeScale = 1f;
        }
    }
}
