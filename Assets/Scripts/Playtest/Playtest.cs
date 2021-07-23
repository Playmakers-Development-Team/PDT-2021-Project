using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Abilities.Commands;
using Commands;
using Cysharp.Threading.Tasks;
using Grid.GridObjects;
using Managers;
using Turn;
using Turn.Commands;
using Units;
using Units.Commands;
using Units.Players;
using Units.Stats;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Playtest
{
    [ExecuteAlways]
    public class Playtest : MonoBehaviour
    {
        [SerializeField] private PlaytestData data;

        private CommandManager commandManager;
        private TurnManager turnManager;
        private UnitManager unitManager;
        private PlayerManager playerManager;

        #region EntryFields

        private const string levelPlayedField = "entry.295363220";
        private const string initialUnitStatField = "entry.512851580";
        private const string initialTimelineField = "entry.887732368";
        private const string inGameStatField = "entry.1697534877";
        private const string endUnitStatField = "entry.682653089";
        private const string endTimelineField = "entry.521626075";

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
            data.activeScene = SceneManager.GetActiveScene().name;
        
            foreach (var unit in unitManager.AllUnits)
            {
                data.InitialUnits = data.InitialUnits +  unit.Name + " HP: " +unit.HealthStat
                .Value + " ATK: " + unit.AttackStat.Value + " DEF: " + unit.DefenceStat.Value + " MP: " +
                                    unit.MovementPoints.Value + " SPD: " + unit.SpeedStat.Value 
                                     + " CORD: " + unit.Coordinate +
                                    Environment.NewLine;
            }
            
            foreach (var unit in turnManager.CurrentTurnQueue)
                data.InitialUnitOrder = data.InitialUnitOrder +  unit.Name + Environment.NewLine;
            
            #endregion

            #region EntriesInitialisation
            
            if(data.Entries.Count != 0)
                 data.Entries.Clear();
            
            data.Entries.Add(new Tuple<string,string>(data.activeScene,levelPlayedField));
            data.Entries.Add(new Tuple<string,string>(data.InitialUnits,initialUnitStatField));
            data.Entries.Add(new Tuple<string,string>(data.InitialUnitOrder,initialTimelineField));

            #endregion
            
        }
        

        /// <summary>
        /// All data that is processed during the game will be calculated in this function
        /// </summary>
        private void EndGame()
        {
            foreach (IUnit unit in unitManager.AllUnits)
            {
                data.EndStateUnits = data.EndStateUnits +  unit.Name + " HP: " +unit.HealthStat
                                        .Value + " ATK: " + unit.AttackStat.Value + " DEF: " + unit.DefenceStat.Value + " MP: " +
                                    unit.MovementPoints.Value + " SPD: " + unit.SpeedStat.Value +
                                    Environment.NewLine;
            }
            
            data.Entries.Add(new Tuple<string,string>(data.EndStateUnits,endUnitStatField));
            data.Entries.Add(new Tuple<string,string>(data.RoundCount.ToString(),initialTimelineField));
        }
    
        private void Awake()
        {
            if (!Application.isPlaying)
                return;
            
            
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
        }

        private void Start()
        {

            if (!Application.isPlaying)
            {
                PostAll(data.Entries);
                return;
            }
            
            commandManager.ListenCommand<TurnQueueCreatedCommand>(cmd => InitialiseStats());
            commandManager.ListenCommand<GameEndedCommand>(cmd => EndGame());
            commandManager.ListenCommand<TurnManipulatedCommand>(cmd => data.AmountOfTurnsManipulated++);
            
            commandManager.ListenCommand<MeditatedCommand>(cmd =>
            {
                data.RoundEntry += $"{cmd.Unit} meditated" + Environment.NewLine;
            });

            commandManager.ListenCommand<AbilityCommand>(cmd =>
            {
                string targetNames = "";

                GridObject[] targets = cmd.Ability.Shape.
                    GetTargets(cmd.OriginCoordinate, cmd.TargetVector).
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
                    $"{cmd.AbilityUser.Name} casted {cmd.Ability.name} at {targetNames}";
                
                data.RoundEntry += Environment.NewLine;
            });
            
            commandManager.ListenCommand<PrepareRoundCommand>(cmd =>
            {
                data.RoundEntry =  "CURRENT INSIGHT: " +  playerManager.Insight.Value + Environment.NewLine + data.RoundEntry + 
                                    
                data.RoundCount++;
            });
            
            commandManager.ListenCommand<StartRoundCommand>(cmd =>
            {
                data.Entries.Add(new Tuple<string, string>(data.RoundEntry,roundFields[data.RoundCount]));
                data.RoundEntry = "";
            });
           
            
            commandManager.ListenCommand<StartMoveCommand>(cmd =>
            {
                data.RoundEntry =
                    $"{data.RoundEntry} {cmd.Unit.Name} moved from {cmd.StartCoords} " +
                    $"to {cmd.TargetCoords}" + Environment.NewLine;
            });

           
        }
        
        private async void Post(string entryName, string response)
        {
            WWWForm form = new WWWForm();
            form.AddField(entryName, response);
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            await www.SendWebRequest();
        }

        #region PostData

        private async void PostAll(List<Tuple<string,string>> entries)
        {
            WWWForm form = new WWWForm();
            
            foreach (Tuple<string, string> entry in entries)
                form.AddField(entry.Item2, entry.Item1);
            
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            await www.SendWebRequest();
        }
        
        #endregion
    }
}
