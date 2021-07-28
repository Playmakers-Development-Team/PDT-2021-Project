using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Abilities;
using Abilities.Commands;
using Commands;
using Cysharp.Threading.Tasks;
using Grid.GridObjects;
using ICSharpCode.NRefactory.Ast;
using Managers;
using Turn;
using Turn.Commands;
using Units;
using Units.Commands;
using Units.Enemies;
using Units.Players;
using Units.Stats;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Utilities;
using static System.Int32;

namespace Playtest
{
    [ExecuteAlways]
    public class Playtest : MonoBehaviour
    {

        [Tooltip("Determines whether or not it will record the data from the game")]
        [SerializeField] private bool canRecordPlaytestData;
        
        [SerializeField] private PlaytestData data;

        
        private struct TemplateUnit
        {
            public int amount { get; set; }

            public List<IUnit> Units;
            
            public override string ToString()
            {
                string temp = "";
                
                foreach (IUnit unit in Units)
                    temp += unit.Name + " ";

                return temp + $"| {amount}";
            }
        }
        
        #region Managers

        private CommandManager commandManager;
        private TurnManager turnManager;
        private UnitManager unitManager;
        private PlayerManager playerManager;

        #endregion

        #region UnitIntialise
        
        private TemplateUnit mostDamageDealtUnits = new TemplateUnit();
        private TemplateUnit leastDamageDealtUnits = new TemplateUnit();
        private TemplateUnit mostDamageTakenUnits = new TemplateUnit();
        private TemplateUnit leastDamageTakenUnits = new TemplateUnit();
        private TemplateUnit mostTurnManipulatedUnits = new TemplateUnit();
        private TemplateUnit mostTimesMovedUnits = new TemplateUnit();
        private TemplateUnit leastTimesMovedUnits = new TemplateUnit();
        private TemplateUnit farthestMovedUnits = new TemplateUnit();

        #endregion

        #region EntryFields

        private const string levelPlayedField = "entry.295363220";
        private const string initialUnitStatField = "entry.512851580";
        private const string initialTimelineField = "entry.887732368";
        private const string endUnitStatField = "entry.682653089";
        private const string endAbilityUsagefield = "entry.225796238";
        private const string amountOfTimesTurnManipulatedField = "entry.1916883487";
        private const string totalTimesMeditatedField = "entry.1342463486";
        private const string whichTeamWonField = "entry.1541678721";
        private const string unitsThatTookTheMostDamageField = "entry.1698034145";
        private const string unitsThatTookTheLeastDamageField = "entry.987243088";
        private const string unitsThatDealtTheMostDamageField = "entry.2144379506";
        private const string unitsThatDealtTheLeastDamageField = "entry.2086514507";
        private const string unitsThatTurnManipulatedTheMostField = "entry.1814874951";
        private const string unitsThatMovedTheMostField = "entry.892339909";
        private const string unitsThatMovedTheLeastField = "entry.92297117";
        private const string unitsThatMovedTheFarthestField = "entry.601566501";
        private const string averageTimeTakenPerRoundField = "entry.216162399";
        private const string averageTimeTakenPerTurnField = "entry.102012017";
        private const string totalPlaytimeField = "entry.13297372";
        private const string totalInsightGainedField = "entry.521626075";

        private static readonly string[] roundFields =
        {
            "N/A",
            "entry.1697534877","entry.1217979400","entry.1049741033","entry.957301180","entry.225039077",
            "entry.225796238","entry.1733593626","entry.1822572691","entry.1416126283","entry.1255273599",
            "entry.1343072204","entry.1950527946","entry.11679908","entry.1792358652","entry.1020408910",
            "entry.375944825", "entry.772404417", "entry.139966911", "entry.205423091", "entry.599673676",
            "entry.2108334703","entry.1887166720","entry.952498432","entry.827097642","entry.1867678434",
            "entry.675949053","entry.1519987537", "entry.2005715946", "entry.535465027", "entry.386028226"
        };

        #endregion

        private bool canTimeRounds = false;
        private bool canTimeTurns = false;
        private bool canTimeOverall = false;
        
        private const string url = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdnv_MhoRAG5l7yFEVhFOvBLpIgKGynzoiHUhjP7f19L-99Fw/formResponse";
        
        private void InitialiseStats()
        {
            #region DataInitialise

            data.RoundEntry = "";
            data.InitialUnits = "";
            data.InitialUnitOrder = "";
            data.EndStateUnits = "";
            data.TimesMoved = 0;
            data.RoundCount = 0;
            data.AmountOfTurnsManipulated = 0;
            data.PlayerHealthPool = 0;
            data.EnemyHealthPool = 0;
            data.MeditateAmount = 0;
            data.AbilityUsage = "";
            data.BattleOutcome = "";
            data.TurnTimer = 0;
            data.RoundTimer = 0;
            data.OverallTime = 0;
            data.TurnCount = 0;

            data.TimeForRounds.Clear();
            data.TimeForTurns.Clear();
            data.AbilitiesUsedInARound.Clear();
            

            data.TurnManipulationData = "";
            data.UnitsMoved.Clear();
            data.Abilities.Clear();
            data.UnitsTurnManipulated.Clear();
            data.UnitsMovedLeast.Clear();
            
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

                data.InitialUnits = data.InitialUnits +  unit.Name + " HP: " +unit.HealthStat
                                        .Value + " ATK: " + unit.AttackStat.Value + " DEF: " + unit.DefenceStat.Value + " MP: " +
                                    unit.MovementPoints.Value + " SPD: " + unit.SpeedStat.Value 
                                    + " CORD: " + unit.Coordinate +
                                    Environment.NewLine;
            }
            
            foreach (var unit in turnManager.CurrentTurnQueue)
                data.InitialUnitOrder = data.InitialUnitOrder +  unit.Name + Environment.NewLine;
            
            #endregion

            canTimeOverall = true;
            canTimeRounds = true;
            canTimeTurns = true;

            #region EntriesInitialisation
            
            data.Entries.Clear();
            
            data.Entries.Add(new Tuple<string,string>(data.ActiveScene,levelPlayedField));
            data.Entries.Add(new Tuple<string,string>(data.InitialUnits,initialUnitStatField));
            data.Entries.Add(new Tuple<string,string>(data.InitialUnitOrder,initialTimelineField));

            #endregion
            
        }

        /// <summary>
        /// All data that is processed during the game will be calculated in this function
        /// </summary>
        private void EndGame(bool playerWin)
        {
            List<IUnit> tempTurnManipulatedUnits = new List<IUnit>();
            int curMaxTurnManipulated = MinValue;
            
            List<IUnit> tempUnitsMoved = new List<IUnit>();
            int curMaxUnitMoved = MinValue;
            
            List<IUnit> tempUnitsMovedFarthest = new List<IUnit>();
            int curFarthestUnit = MinValue;
            
            List<IUnit> tempUnitsMovedLeast = new List<IUnit>();
            int curLeastUnitMoved = MaxValue;


            int EndHealthPool = 0;
            
            canTimeOverall = false;
            canTimeTurns = false;
            canTimeRounds = false;

            foreach (IUnit unit in unitManager.AllUnits)
            {

                EndHealthPool += unit.HealthStat.Value;
                
                data.EndStateUnits = data.EndStateUnits +  unit.Name + " HP: " +unit.HealthStat
                                        .Value + " ATK: " + unit.AttackStat.Value + " DEF: " + unit.DefenceStat.Value + " MP: " +
                                    unit.MovementPoints.Value + " SPD: " + unit.SpeedStat.Value +
                                    Environment.NewLine;
            }

            #region TurnManipulationEndGame

            foreach (KeyValuePair<IUnit,int> unit in data.UnitsTurnManipulated.OrderByDescending
            (key => key.Value))
            {
                if (unit.Value > curMaxTurnManipulated)
                {
                    curMaxTurnManipulated = unit.Value;
                    tempTurnManipulatedUnits.Clear();
                    tempTurnManipulatedUnits.Add(unit.Key);
                }
                else if (unit.Value == curMaxTurnManipulated)
                {
                    tempTurnManipulatedUnits.Add(unit.Key);
                }
            }

            mostTurnManipulatedUnits.Units = tempTurnManipulatedUnits;
            mostTurnManipulatedUnits.amount = curMaxTurnManipulated;

            if (mostTurnManipulatedUnits.amount == MinValue)
                mostTurnManipulatedUnits.amount = 0;

            #endregion
            
            #region UnitMovementEndGame

            foreach (KeyValuePair<IUnit,int> unit in data.UnitsMoved.OrderByDescending
                (key => key.Value))
            {
                if (unit.Value > curMaxUnitMoved)
                {
                    curMaxUnitMoved = unit.Value;
                    tempUnitsMoved.Clear();
                    tempUnitsMoved.Add(unit.Key);
                }
                else if (unit.Value == curMaxUnitMoved)
                {
                    tempUnitsMoved.Add(unit.Key);
                }
            }

            mostTimesMovedUnits.Units = tempUnitsMoved;
            mostTimesMovedUnits.amount = curMaxUnitMoved;
            
            foreach (KeyValuePair<IUnit,int> unit in data.UnitsMovedDistance.OrderByDescending
                (key => key.Value))
            {
                if (unit.Value > curFarthestUnit)
                {
                    curFarthestUnit = unit.Value;
                    tempUnitsMovedFarthest.Clear();
                    tempUnitsMovedFarthest.Add(unit.Key);
                }
                else if (unit.Value == curFarthestUnit)
                {
                    tempUnitsMovedFarthest.Add(unit.Key);
                }
            }

            farthestMovedUnits.Units = tempUnitsMoved;
            farthestMovedUnits.amount = curFarthestUnit;
            
            foreach (KeyValuePair<IUnit,int> unit in data.UnitsMoved.OrderByDescending
                (key => key.Value))
            {
                if (unit.Value < curLeastUnitMoved)
                {
                    curLeastUnitMoved = unit.Value;
                    tempUnitsMovedLeast.Clear();
                    tempUnitsMovedLeast.Add(unit.Key);
                }
                else if (unit.Value == curLeastUnitMoved)
                {
                    tempUnitsMovedLeast.Add(unit.Key);
                }
            }

            leastTimesMovedUnits.Units = tempUnitsMovedLeast;
            leastTimesMovedUnits.amount = curLeastUnitMoved;

            #endregion
            
            #region BattleOutcome

            float percentage = 0;

            if (playerWin)
            {
                data.BattleOutcome = " Victory";
                percentage = (float)EndHealthPool / data.PlayerHealthPool * 100;
            }
            else
            {
                data.BattleOutcome = " Defeat";
                percentage = (float)EndHealthPool / data.EnemyHealthPool * 100;
            }

            if (percentage > 75)
                data.BattleOutcome = "Total" + data.BattleOutcome;
            else if (percentage >= 30 && percentage <= 74)
                data.BattleOutcome = "Decisive" + data.BattleOutcome;
            else if (percentage >= 10 && percentage <= 29)
                data.BattleOutcome = "Close" + data.BattleOutcome;
            else if (percentage < 9)
                data.BattleOutcome = "Pyrrhic" + data.BattleOutcome;

            #endregion

            #region TimeCalculation

            float averageTimeForRounds = 0;
            float averageTimeForTurns = 0;

            string strAverageTimeForRounds = "";
            string strAverageTimesForTurns = "";
            string strOverallTime = "";
            
            averageTimeForRounds = Queryable.Average(data.TimeForRounds.Values.AsQueryable());
            averageTimeForTurns = Queryable.Average(data.TimeForTurns.Values.AsQueryable());

            var tsOne = TimeSpan.FromSeconds(averageTimeForRounds);
            strAverageTimeForRounds =
                string.Format("{0:00}:{1:00}", tsOne.TotalMinutes, tsOne.Seconds);

            var tsTwo = TimeSpan.FromSeconds(averageTimeForTurns);
            strAverageTimesForTurns =
                string.Format("{0:00}:{1:00}", tsTwo.TotalMinutes, tsTwo.Seconds);
            
            var tsThree = TimeSpan.FromSeconds(data.OverallTime);
            strOverallTime =
                string.Format("{0:00}:{1:00}", tsThree.TotalMinutes, tsThree.Seconds);
            
            #endregion

            #region Entries

            data.Entries.Add(new Tuple<string, string>(strOverallTime,
            totalPlaytimeField));
                     
            data.Entries.Add(new Tuple<string, string>(strAverageTimeForRounds,
                averageTimeTakenPerRoundField));
            
            data.Entries.Add(new Tuple<string, string>(strAverageTimesForTurns,
                averageTimeTakenPerTurnField));
            
            data.Entries.Add(new Tuple<string, string>(mostTurnManipulatedUnits + " times",
                unitsThatTurnManipulatedTheMostField));
            
            data.Entries.Add(new Tuple<string, string>(mostTimesMovedUnits + " times",
                unitsThatMovedTheMostField));
            
            data.Entries.Add(new Tuple<string, string>(farthestMovedUnits + " cells",
                unitsThatMovedTheFarthestField));
            
            data.Entries.Add(new Tuple<string, string>(leastTimesMovedUnits + " times",
                unitsThatMovedTheLeastField));
            
            data.Entries.Add(new Tuple<string, string>(data.BattleOutcome,
                whichTeamWonField));
            
            data.Entries.Add(new Tuple<string,string>(data.EndStateUnits,endUnitStatField));
            data.Entries.Add(new Tuple<string,string>(data.RoundCount.ToString(),initialTimelineField));
            data.Entries.Add(new Tuple<string, string>(data.AmountOfTurnsManipulated.ToString(),
            amountOfTimesTurnManipulatedField));
            
            data.Entries.Add(new Tuple<string, string>(data.MeditateAmount.ToString(),totalTimesMeditatedField));
            
            UpdateAbilityUsage();
            
            #endregion

        }

        #region MonoBehaviour
        
        private void Awake()
        {
            if (!Application.isPlaying || !canRecordPlaytestData)
                return;

          
            
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
        }

        private void Update()
        {
            if (canTimeOverall)
                data.OverallTime += Time.deltaTime;

            if (canTimeRounds)
                data.RoundTimer += Time.deltaTime;

            if (canTimeTurns)
                data.TurnTimer += Time.deltaTime;
        }
        
        private void Start()
        {
            
            if (!canRecordPlaytestData)
                return;

            if (!Application.isPlaying)
            {
                PostAll(data.Entries);
                return;
            }

            
            
            commandManager.ListenCommand<TurnQueueCreatedCommand>(cmd => InitialiseStats());
            commandManager.ListenCommand<GameEndedCommand>(cmd => EndGame(cmd.DidPlayerWin));
            commandManager.ListenCommand<TurnManipulatedCommand>(cmd => data.AmountOfTurnsManipulated++);
            
            commandManager.ListenCommand<MeditatedCommand>(cmd =>
            {
                data.MeditateAmount++;
                data.RoundEntry += $"{cmd.Unit} meditated" + Environment.NewLine;
            });
            
            commandManager.ListenCommand<TurnManipulatedCommand>(cmd => TurnManipulated(cmd.Unit,
            cmd.TargetUnit));

            commandManager.ListenCommand<AbilityCommand>(cmd =>
                UpdateAbility(cmd.Ability, cmd.OriginCoordinate, cmd.TargetVector, cmd.AbilityUser));

            commandManager.ListenCommand<PrepareRoundCommand>(cmd => UpdateRound());

            commandManager.ListenCommand<StartRoundCommand>(cmd =>
            {
                data.Entries.Add(new Tuple<string, string>(data.RoundEntry,roundFields[data.RoundCount]));
                data.RoundEntry = "";
            });

            commandManager.ListenCommand<StartMoveCommand>(cmd => UpdateMoveUsage(cmd.Unit,cmd
            .StartCoords,cmd.TargetCoords));
            
            commandManager.ListenCommand<EndTurnCommand>(cmd => UpdateTurn());
        }
        
        #endregion

        #region Process Data

        private void UpdateMoveUsage(IUnit unit, Vector2Int startCoord, Vector2Int targetCoord)
        {
            int distance = ManhattanDistance.GetManhattanDistance(startCoord, targetCoord);
            
            data.RoundEntry =
                $"{data.RoundEntry} {unit.Name} moved from {startCoord} " +
                $"to {targetCoord}" + Environment.NewLine;

            if (!data.UnitsMoved.ContainsKey(unit))
                data.UnitsMoved.Add(unit, 1);
            else
                data.UnitsMoved[unit]++;
            
            if (!data.UnitsMovedDistance.ContainsKey(unit))
                data.UnitsMovedDistance.Add(unit, distance);
            else
                data.UnitsMovedDistance[unit] += distance;
        }

        private void UpdateTurn()
        {
            data.TurnCount++;
            
            canTimeTurns = false;
            data.TimeForTurns.Add(data.TurnCount,data.TurnTimer);
            data.TurnTimer = 0;
            canTimeTurns = true;

        }

        private void UpdateAbility(Ability ability, Vector2Int originCoord, Vector2 targetVector,
         IAbilityUser abilityUser)
        {
            string targetNames = "";
            bool flag = true;
                
            data.AbilitiesUsedInARound.Add(ability);
            
            if (data.Abilities.ContainsKey(ability))
                data.Abilities[ability]++;
            else
                data.Abilities.Add(ability,1);
         
            GridObject[] targets = ability.Shape.
                GetTargets(originCoord, targetVector).
                AsEnumerable().
                ToArray();

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] is IUnit unit)
                {
                    if (i == targets.Length - 1 || targets.Length == 1)
                        targetNames += unit.Name;
                    else
                        targetNames += unit.Name + " and ";
                }
            }

            data.RoundEntry +=
                $"{abilityUser.Name} casted {ability.name} at {targetNames}";

            //TODO: Add the effect of the ability to each affected unit here.
                
            data.RoundEntry += Environment.NewLine;
        }
        
        private void TurnManipulated(IUnit unit, IUnit targetUnit)
        {
            data.RoundEntry += $"{unit} turn manipulated with {targetUnit}" + 
                               Environment.NewLine;

            if (!data.UnitsTurnManipulated.ContainsKey(unit))
                data.UnitsTurnManipulated.Add(unit, 1);
            else
                data.UnitsTurnManipulated[unit]++;
            
        }

        private void UpdateRound()
        {
               data.RoundEntry = Environment.NewLine + "CURRENT INSIGHT: " +  playerManager.Insight
                .Value + 
                Environment.NewLine + data.RoundEntry;

                data.RoundCount++;
                
                foreach (IUnit unit in unitManager.AllUnits)
                {
                    string tenet1 = "";
                    string tenet2 = "";

                    if (unit.TenetStatuses.AsEnumerable().ToArray().Length > 1)
                    {
                        tenet1 = unit.TenetStatuses.AsEnumerable().ToArray()[0].TenetType+ " "
                                 + unit.TenetStatuses.AsEnumerable().ToArray()[0].StackCount;
                        
                        tenet2 = unit.TenetStatuses.AsEnumerable().ToArray()[1].TenetType + " "
                                 + unit.TenetStatuses.AsEnumerable().ToArray()[1].StackCount;
                    }
                    else if (unit.TenetStatuses.AsEnumerable().ToArray().Length == 1)
                    {
                        tenet1 = unit.TenetStatuses.AsEnumerable().ToArray()[0].TenetType + " "
                                 + unit.TenetStatuses.AsEnumerable().ToArray()[0].StackCount;
                    }

                    data.RoundEntry = unit.Name + " HP: " + unit.HealthStat.Value + " ATK: " +
                                      unit.AttackStat.Value + " DEF: " + unit.DefenceStat.Value +
                                      " MP: " + unit.MovementPoints.Value + " SPD: " +
                                      unit.SpeedStat.Value +
                                      $" {tenet1} {tenet2} {Environment.NewLine} {data.RoundEntry} ";
                }


                canTimeRounds = false;
                data.TimeForRounds.Add(data.RoundCount,data.RoundTimer);
                data.RoundTimer = 0;
                canTimeRounds = true;

                data.RoundEntry += Environment.NewLine + $"Abilities used in this round were: ";

                foreach (Ability ability in data.AbilitiesUsedInARound)
                    data.RoundEntry += Environment.NewLine + ability.name;
                
                data.AbilitiesUsedInARound.Clear();
        }
        
        private void UpdateAbilityUsage()
        {

            int max = data.Abilities.Max(x => x.Value);
            KeyValuePair<Ability, int> favouriteAbility = new KeyValuePair<Ability, int>();
            
            foreach (KeyValuePair<Ability, int> ability in data.Abilities.OrderByDescending(key => key
            .Value))
            {
                
                if (ability.Value == max && favouriteAbility.Key is null)
                {
                    favouriteAbility = ability;
                    data.AbilityUsage =
                        $"Most used ability is {favouriteAbility.Key.name} | {favouriteAbility.Value}" +
                        Environment.NewLine;
                    
                    continue;
                }

                data.AbilityUsage +=
                    $"{ability.Key.name} | {ability.Value}" + Environment.NewLine;
            }  
            
            data.Entries.Add(new Tuple<string, string>(data.AbilityUsage,endAbilityUsagefield));
        }

        #endregion
      
        #region PostData

        private async void PostAll(List<Tuple<string,string>> entries)
        {
            WWWForm form = new WWWForm();
            
            foreach (Tuple<string, string> entry in entries)
                form.AddField(entry.Item2, entry.Item1);
            
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            await www.SendWebRequest();
        }
        
        private async void Post(string entryName, string response)
        {
            WWWForm form = new WWWForm();
            form.AddField(entryName, response);
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            await www.SendWebRequest();
        }
        
        #endregion
    }
}
