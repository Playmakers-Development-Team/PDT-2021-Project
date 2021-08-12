using System;
using Abilities.Commands;
using Commands;
using Managers;
using Turn;
using Turn.Commands;
using Units;
using Units.Commands;
using Units.Enemies;
using Units.Players;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Playtest
{
    public class DataCollection
    {
        private CommandManager commandManager;
        private PlaytestData data;
        private TurnManager turnManager;
        private UnitManager unitManager;

        public DataCollection(PlaytestData data)
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            this.data = data;
        }
        
        public void OnEnable()
        {
            commandManager.ListenCommand<TurnManipulatedCommand>(OnTurnManipulated);
            commandManager.ListenCommand<MeditatedCommand>(OnMeditated);
            commandManager.ListenCommand<AbilityCommand>(OnAbility);
            commandManager.ListenCommand<StartMoveCommand>(OnStartMove);
        }

        public void OnDisable()
        {
            commandManager.UnlistenCommand<TurnManipulatedCommand>(OnTurnManipulated);
            commandManager.UnlistenCommand<MeditatedCommand>(OnMeditated);
            commandManager.UnlistenCommand<AbilityCommand>(OnAbility);
            commandManager.UnlistenCommand<StartMoveCommand>(OnStartMove);
        }

        public void OnPrepareRound(PrepareRoundCommand cmd, float roundDuration)
        {
            data.CurrentRound.Time = roundDuration;
            
            data.Rounds.Add(new PlaytestRoundData(
                turnManager.Insight.Value,
                DataProcessing.RoundUnits()
            ));
        }

        private void TurnManipulated(IUnit unit, IUnit targetUnit)
        {
            data.CurrentRound.RoundActions.Add(new PlaytestRoundActionData(DataProcessing.TurnManipulatedRoundAction(unit, targetUnit)));

            // TODO: Repeated code. See UpdateMoveUsage.
            var playtestUnitData = data.Units.Find(u => u.Unit == unit);

            if (playtestUnitData != null)
            {
                playtestUnitData.TimesTurnManipulated++;
            }
            else
            {
                // TODO: Make this more verbose.
                Debug.LogWarning("Turn manipulating unit not found in playtest data.");
            }
        }

        private void UpdateMoveUsage(IUnit unit, Vector2Int startCoord, Vector2Int targetCoord)
        {
            data.CurrentRound.RoundActions.Add(new PlaytestRoundActionData(DataProcessing.MovementRoundAction(unit, startCoord, targetCoord)));
            
            int distance = ManhattanDistance.GetManhattanDistance(startCoord, targetCoord);

            // TODO: Repeated code. See TurnManipulated.
            var playtestUnitData = data.Units.Find(u => u.Unit == unit);

            if (playtestUnitData != null)
            {
                playtestUnitData.TimesMoved++;
                playtestUnitData.DistanceMoved += distance;
            }
            else
            {
                // TODO: Make this more verbose.
                Debug.LogWarning("Moving unit not found in playtest data.");
            }
        }

        private void OnAbility(AbilityCommand cmd)
        {
            data.CurrentRound.AbilitiesUsed.Add(cmd.Ability);

            if (data.Abilities.ContainsKey(cmd.Ability))
                data.Abilities[cmd.Ability]++;
            else
                data.Abilities.Add(cmd.Ability,1);
            
            data.CurrentRound.RoundActions.Add(new PlaytestRoundActionData(DataProcessing.AbilityRoundAction(cmd.Ability, cmd.OriginCoordinate, cmd.TargetVector, cmd.AbilityUser)));
        }

        public void OnEndTurn(EndTurnCommand cmd, float turnDuration)
        {
            // TODO: Pretty sure this is being stored in TurnManager already.
            data.TurnCount++;
            
            data.TimeForTurns.Add(data.TurnCount, turnDuration);
        }
        
        private void OnStartMove(StartMoveCommand cmd) => UpdateMoveUsage(cmd.Unit, cmd.StartCoords, cmd.TargetCoords);

        private void OnTurnManipulated(TurnManipulatedCommand cmd)
        {
            data.AmountOfTurnsManipulated++;
            TurnManipulated(cmd.Unit, cmd.TargetUnit);
        }

        private void OnMeditated(MeditatedCommand cmd)
        {
            data.MeditateAmount++;
            
            data.CurrentRound.RoundActions.Add(new PlaytestRoundActionData(DataProcessing.MeditateRoundAction(cmd)));
        }

        public void EndGame(bool playerWin, float encounterDuration)
        {
            data.EncounterDuration = encounterDuration;
            
            data.EndStateUnits = DataProcessing.EndStateUnits();

            data.PlayerWin = playerWin;
        }

        public void InitialiseStats()
        {
            data.ActiveScene = SceneManager.GetActiveScene().name;
        
            foreach (var unit in unitManager.AllUnits)
            {
                switch (unit)
                {
                    case PlayerUnit pUnit:
                        data.PlayerHealthPool += pUnit.HealthStat.Value;
                        break;
                    case EnemyUnit eUnit:
                        data.EnemyHealthPool += eUnit.HealthStat.Value;
                        break;
                }

                data.Units.Add(new PlaytestUnitData(unit, true));
            }
            
            foreach (var unit in turnManager.CurrentTurnQueue)
                data.InitialUnitOrder += unit.Name + Environment.NewLine;

            data.InitialUnits = DataProcessing.InitialUnits(data);
        }
    }
}
