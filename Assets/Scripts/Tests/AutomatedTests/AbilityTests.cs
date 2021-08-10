using System.Collections;
using E7.Minefield;
using NUnit.Framework;
using TenetStatuses;
using Tests.Beacons;
using Tests.Constraints;
using Tests.Utilities;
using Turn.Commands;
using Units.Commands;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.AutomatedTests
{
    [Category("Automated Testing")]
    public class AbilityTests : BaseTest
    {
        protected override string Scene => "AbilityTest";

        [UnityTest, Order(0)]
        [Timeout(8000)]
        public IEnumerator ProvidingTenets()
        {
            TenetStatus expectedEnemyTenets = new TenetStatus(TenetType.Sorrow, 3);
            TenetStatus expectedEstelleTenets = new TenetStatus(TenetType.Pride, 1);
            
            yield return PrepareAndActivateScene();
            yield return TurnTester.WaitUnitTurn(UnitBeacons.Estelle);
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityA, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);

            Assert.Beacon(UnitBeacons.Estelle, Any.EqualsTenets(expectedEstelleTenets));
            Assert.Beacon(UnitBeacons.EnemyA, Any.EqualsTenets(expectedEnemyTenets));
        }

        [UnityTest, Order(1)] 
        [Timeout(12000)]
        public IEnumerator AbilityCosts()
        {
            yield return PrepareAndActivateScene();
            
            yield return TurnTester.WaitUnitTurn(UnitBeacons.Estelle);
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityB, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);
            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(10));
            yield return TurnTester.NextTurnUntilUnit(UnitBeacons.Estelle);
            
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityA, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);
            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(10));
            Assert.Beacon(UnitBeacons.Estelle, Any.EqualsTenets(new TenetStatus(TenetType.Pride, 1)));
            yield return TurnTester.NextTurnUntilUnit(UnitBeacons.Estelle);
            
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityB, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);

            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(7));
            Assert.Beacon(UnitBeacons.Estelle, Any.EqualsTenets(new TenetStatus(TenetType.Pride, 0)));
            
            yield return DelayForViewing();
        }
        
        [UnityTest, Order(2)] 
        [Timeout(12000)]
        public IEnumerator AbilityBonus()
        {
            yield return PrepareAndActivateScene();
            
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityA, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);
            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(10));
            Assert.Beacon(UnitBeacons.EnemyA, Any.EqualsTenets(new TenetStatus(TenetType.Sorrow, 3)));
            yield return TurnTester.NextTurnUntilUnit(UnitBeacons.Estelle);
            
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityC, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);
            Assert.Beacon(UnitBeacons.EnemyA, Any.EqualsTenets(new TenetStatus(TenetType.Sorrow, 0)));
            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(6));
            
            yield return DelayForViewing();
        }

        [UnityTest, Order(10)] 
        [Timeout(12000)]
        public IEnumerator IndependentMechanic()
        {
            yield return PrepareAndActivateScene();
            
            // Move niles into position for later
            yield return InputTester.MoveUnitTo(UnitBeacons.Niles, GridBeacons.A);
            yield return InputTester.UnitUseAbility(UnitBeacons.Helena, 
                UIBeacons.AbilityA, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(7));
            
            // Move near niles
            yield return InputTester.MoveUnitTo(UnitBeacons.Helena, GridBeacons.B);
            yield return InputTester.UnitUseAbility(UnitBeacons.Helena, 
                UIBeacons.AbilityA, UnitBeacons.EnemyB);
            // Should do no damage
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(7));
        }
        
        [UnityTest, Order(10)] 
        [Timeout(12000)]
        public IEnumerator AffinityMechanic()
        {
            yield return PrepareAndActivateScene();
            
            // Should not gain anything
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityA, UnitBeacons.Niles);
            Assert.Beacon(UnitBeacons.EnemyB, Any.EqualsTenets(new TenetStatus(TenetType.Humility, 0)));
            
            // Move Niles next to Helena
            yield return InputTester.MoveUnitTo(UnitBeacons.Helena, GridBeacons.B);
            yield return InputTester.MoveUnitTo(UnitBeacons.Niles, GridBeacons.A);
            
            // Should gain humility
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityA, UnitBeacons.Niles);
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Humility, 3)));
        }
        
        [UnityTest, Order(10)] 
        [Timeout(24000)]
        public IEnumerator KindledMechanic()
        {
            yield return PrepareAndActivateScene();
            
            // Gain a Passion tenet, should do no damage since we only have 1 tenet in total
            yield return InputTester.UnitUseAbility(UnitBeacons.Helena, 
                UIBeacons.AbilityB, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(10));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Passion, 1)));
            
            // Only deal 3 damage when it has more than 1 tenets in total
            yield return InputTester.UnitUseAbility(UnitBeacons.Helena, 
                UIBeacons.AbilityB, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(7));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Passion, 2)));
            
            // Remove all tenets of a unit
            TurnTester.DoUnit(UnitBeacons.Helena, u => u.ClearAllTenetStatus());
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Passion, 0)));
            
            // Gain a Joy tenet, the ability also deals 3 damage
            yield return InputTester.UnitUseAbility(UnitBeacons.Helena, 
                UIBeacons.AbilityC, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(4));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Joy, 1)));
            
            // Only deal 3 damage when it has more than 1 tenets in total
            yield return InputTester.UnitUseAbility(UnitBeacons.Helena, 
                UIBeacons.AbilityB, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(1));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Joy, 1)));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Passion, 1)));
        }
        
        [UnityTest, Order(10)] 
        [Timeout(24000)]
        public IEnumerator EnnuiMechanic()
        {
            yield return PrepareAndActivateScene();
            
            // Move closer to enemy
            yield return InputTester.MoveUnitTo(UnitBeacons.Niles, GridBeacons.A);

            // Gain 2 Apathy
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityB, GridBeacons.B);
            
            // Move closer to enemy
            yield return InputTester.MoveUnitTo(UnitBeacons.Niles, GridBeacons.B);
            
            // Cannot do damage, because the latest tenet is only Apathy
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityB, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(10));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Apathy, 2)));
            
            // Gain 2 Sorrow Tenet
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityC, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(10));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Apathy, 2)));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Sorrow, 2)));
            
            // Finally do 6 damage, consuming all of Sorrow
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityB, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(4));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Apathy, 2)));
            Assert.Beacon(UnitBeacons.Niles, Any.EqualsTenets(new TenetStatus(TenetType.Sorrow, 0)));
        }
        
        [UnityTest, Order(10)] 
        [Timeout(24000)]
        public IEnumerator RepressionMechanic()
        {
            yield return PrepareAndActivateScene();
            
            // Deal damage and gain joy tenet
            yield return InputTester.UnitUseAbility(UnitBeacons.Helena, 
                UIBeacons.AbilityC, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(7));
            Assert.Beacon(UnitBeacons.Helena, Any.UnitEqualsHealth(20));
            Assert.Beacon(UnitBeacons.Helena, Any.EqualsTenets(new TenetStatus(TenetType.Joy, 1)));
            
            // After gaining 3 Joy now, take 4 damage
            yield return InputTester.UnitUseAbility(UnitBeacons.Helena, 
                UIBeacons.AbilityC, UnitBeacons.EnemyB);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(4));
            Assert.Beacon(UnitBeacons.Helena, Any.EqualsTenets(new TenetStatus(TenetType.Joy, 0)));
            Assert.Beacon(UnitBeacons.Helena, Any.UnitEqualsHealth(13));
        }
        
        [UnityTest, Order(10)] 
        [Timeout(24000)]
        public IEnumerator CatharsisMechanic()
        {
            yield return PrepareAndActivateScene();
            
            // Move to be Niles closer to Helena
            yield return InputTester.MoveUnitTo(UnitBeacons.Niles, GridBeacons.A);
            
            // Gain Sorrow tenet
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityC, GridBeacons.B);
            Assert.Beacon(UnitBeacons.EnemyB, Any.UnitEqualsHealth(10));
            
            // Move to Helena to be adjacent to Niles
            yield return InputTester.MoveUnitTo(UnitBeacons.Helena, GridBeacons.B);

            // Finally has enough Sorrow to give 2 Attack that will last for the encounter
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityC, UnitBeacons.Helena);
            
            // Wait 1 whole round
            yield return TurnTester.WaitUnitTurn(UnitBeacons.Niles);
            // Should start with 2 Attack
            Assert.Beacon(UnitBeacons.Helena, Any.UnitEqualsAttack(2));
        }
    }
}
