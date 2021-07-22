using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Cysharp.Threading.Tasks;
using Managers;
using Turn;
using Turn.Commands;
using Units;
using Units.Commands;
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

        #region EntryFields

        private const string levelPlayedField = "entry.295363220";
        private const string initialUnitStatField = "entry.512851580";
        private const string initialTimelineField = "entry.887732368";
        private const string inGameStatField = "entry.1697534877";
        private const string endUnitStatField = "entry.682653089";
        #endregion
        
        private const string url = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdnv_MhoRAG5l7yFEVhFOvBLpIgKGynzoiHUhjP7f19L-99Fw/formResponse";
        
        private void InitialiseStats()
        {
            #region DataInitialise
            
            data.InitialUnits = "";
            data.TimesMoved = 0;
            data.activeScene = SceneManager.GetActiveScene().name;
        
            foreach (var unit in unitManager.AllUnits)
            {
                data.InitialUnits = data.InitialUnits +  unit.Name + " HP: " +unit.HealthStat
                .Value + " ATK: " + unit.AttackStat.Value + " DEF: " + unit.DefenceStat.Value + " MP: " +
                                    unit.MovementPoints.Value + " SPD: " + unit.SpeedStat.Value +
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

        private void UpdateRound()
        {
            
        }

        private void EndGame()
        {
            
        }
    
        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
        }

        private void Start()
        {
            commandManager.ListenCommand<TurnQueueCreatedCommand>(cmd => InitialiseStats());
            commandManager.ListenCommand<GameEndedCommand>(cmd => EndGame());

            commandManager.ListenCommand<StartMoveCommand>(cmd =>
            {
                data.TimesMoved++;
            });

            if (!Application.isPlaying)
                PostAll(data.Entries);
            else
                InitialiseStats();
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
