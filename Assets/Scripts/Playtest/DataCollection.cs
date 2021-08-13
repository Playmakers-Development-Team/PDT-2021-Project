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
        private readonly CommandManager commandManager;
        private readonly TurnManager turnManager;
        private readonly UnitManager unitManager;
        
        public PlaytestData Data { get; set; }

        public DataCollection(PlaytestData data)
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            Data = data;
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
            Data.CurrentRound.Time = roundDuration;
            Data.CurrentRound.RoundUnits = DataProcessing.RoundUnits();
            Data.CurrentRound.CurrentInsight = turnManager.Insight.Value;
            
            Data.Rounds.Add(new PlaytestRoundData());
        }

        private void TurnManipulated(IUnit unit, IUnit targetUnit)
        {
            Data.CurrentRound.RoundActions.Add(new PlaytestRoundActionData(DataProcessing.TurnManipulatedRoundAction(unit, targetUnit)));

            var unitData = GetUnitData(unit);

            unitData.TimesTurnManipulated++;
        }

        private void UpdateMoveUsage(IUnit unit, Vector2Int startCoord, Vector2Int targetCoord)
        {
            Data.CurrentRound.RoundActions.Add(new PlaytestRoundActionData(DataProcessing.MovementRoundAction(unit, startCoord, targetCoord)));
            
            int distance = ManhattanDistance.GetManhattanDistance(startCoord, targetCoord);

            var unitData = GetUnitData(unit);

            unitData.TimesMoved++;
            unitData.DistanceMoved += distance;
        }

        private PlaytestUnitData GetUnitData(IUnit unit)
        {
            var unitData = Data.Units.Find(u => u.Unit == unit);

            if (unitData != null)
                return unitData;
            
            unitData = new PlaytestUnitData(unit, true);
            Data.Units.Add(unitData);

            return unitData;
        }

        private void OnAbility(AbilityCommand cmd)
        {
            Data.CurrentRound.AbilitiesUsed.Add(cmd.Ability);

            if (Data.Abilities.ContainsKey(cmd.Ability))
                Data.Abilities[cmd.Ability]++;
            else
                Data.Abilities.Add(cmd.Ability, 1);
            
            Data.CurrentRound.RoundActions.Add(new PlaytestRoundActionData(
                DataProcessing.AbilityRoundAction(cmd)
            ));
        }

        public void OnEndTurn(EndTurnCommand cmd, float turnDuration)
        {
            Data.TurnCount++;
            
            Data.TimeForTurns.Add(Data.TurnCount, turnDuration);
        }
        
        private void OnStartMove(StartMoveCommand cmd) => UpdateMoveUsage(cmd.Unit, cmd.StartCoords, cmd.TargetCoords);

        private void OnTurnManipulated(TurnManipulatedCommand cmd)
        {
            Data.AmountOfTurnsManipulated++;
            TurnManipulated(cmd.Unit, cmd.TargetUnit);
        }

        private void OnMeditated(MeditatedCommand cmd)
        {
            Data.MeditateAmount++;
            
            Data.CurrentRound.RoundActions.Add(new PlaytestRoundActionData(DataProcessing.MeditateRoundAction(cmd)));
        }

        public void EndGame(bool playerWin, float encounterDuration)
        {
            Data.EncounterDuration = encounterDuration;
            
            Data.EndStateUnits = DataProcessing.EndStateUnits();

            Data.PlayerWin = playerWin;
        }

        public void InitialiseStats()
        {
            Data.ActiveScene = SceneManager.GetActiveScene().name;
        
            foreach (var unit in unitManager.AllUnits)
            {
                switch (unit)
                {
                    case PlayerUnit pUnit:
                        Data.PlayerHealthPool += pUnit.HealthStat.Value;
                        break;
                    case EnemyUnit eUnit:
                        Data.EnemyHealthPool += eUnit.HealthStat.Value;
                        break;
                }

                Data.Units.Add(new PlaytestUnitData(unit, true));
            }
            
            foreach (var unit in turnManager.CurrentTurnQueue)
                Data.InitialUnitOrder += unit.Name + Environment.NewLine;

            Data.InitialUnits = DataProcessing.InitialUnits(Data);

            Data.Rounds.Add(new PlaytestRoundData());
        }
    }
}
