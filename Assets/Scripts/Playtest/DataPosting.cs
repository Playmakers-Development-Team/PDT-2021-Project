using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Playtest
{
    public static class DataPosting
    {
        private class Field
        {
            public string Name { get; }
            public string ID { get; }
            public Func<PlaytestData, string> Process { get; }

            public Field(string name, string id, Func<PlaytestData, string> process)
            {
                Name = name;
                ID = id;
                Process = process;
            }
        }

        #region EntryFields

        private static readonly List<Field> fields = new List<Field>
        {
            new Field("levelPlayed", "entry.295363220", data => data.ActiveScene),
            new Field("initialUnitStat", "entry.512851580", data => data.InitialUnits),
            new Field("initialTimeline", "entry.887732368", data => data.InitialUnitOrder),
            new Field("endUnitStat", "entry.682653089", data => data.EndStateUnits),
            new Field("endAbilityUsage", "entry.225796238", DataProcessing.UpdateAbilityUsage),
            new Field("amountOfTimesTurnManipulated", "entry.1916883487", data => data.AmountOfTurnsManipulated.ToString()),
            new Field("totalTimesMeditated", "entry.1342463486", data => data.MeditateAmount.ToString()),
            new Field("whichTeamWon", "entry.1541678721", DataProcessing.BattleOutcome),
            new Field("unitsThatTookTheMostDamage", "entry.1698034145", data => "Not yet implemented."),
            new Field("unitsThatTookTheLeastDamage", "entry.987243088", data => "Not yet implemented."),
            new Field("unitsThatDealtTheMostDamage", "entry.2144379506", data => "Not yet implemented."),
            new Field("unitsThatDealtTheLeastDamage", "entry.2086514507", data => "Not yet implemented."),
            new Field("unitsThatTurnManipulatedTheMost", "entry.1814874951", DataProcessing.MostTurnManipulatedUnits),
            new Field("maxUnitTurnManipulations", "entry.1032794462", DataProcessing.MaxUnitTurnManipulations),
            new Field("unitsThatMovedTheMost", "entry.892339909", DataProcessing.MostTimesMovedUnits),
            new Field("maxUnitMovements", "entry.1458945870", DataProcessing.MaxUnitMovements),
            new Field("unitsThatMovedTheLeast", "entry.92297117", DataProcessing.LeastTimesMovedUnits),
            new Field("minUnitMovements", "entry.1645540812", DataProcessing.MinUnitMovements),
            new Field("unitsThatMovedTheFarthest", "entry.601566501", DataProcessing.FarthestMovedUnits),
            new Field("maxUnitMovementDistance", "entry.1604345157", DataProcessing.MaxUnitMovementDistance),
            new Field("averageTimeTakenPerRound", "entry.216162399", DataProcessing.StrAverageTimeForRounds),
            new Field("averageTimeTakenPerTurn", "entry.102012017", DataProcessing.StrAverageTimesForTurns),
            new Field("totalPlaytime", "entry.13297372", DataProcessing.TotalPlaytime),
            new Field("totalInsightGained", "entry.521626075", data => "Not yet implemented."),
        };

        private static readonly string[] roundFieldsIDs =
        {
            "entry.1697534877","entry.1217979400","entry.1049741033","entry.957301180","entry.225039077",
            "entry.225796238","entry.1733593626","entry.1822572691","entry.1416126283","entry.1255273599",
            "entry.1343072204","entry.1950527946","entry.11679908","entry.1792358652","entry.1020408910",
            "entry.375944825", "entry.772404417", "entry.139966911", "entry.205423091", "entry.599673676",
            "entry.2108334703","entry.1887166720","entry.952498432","entry.827097642","entry.1867678434",
            "entry.675949053","entry.1519987537", "entry.2005715946", "entry.535465027", "entry.386028226"
        };

        #endregion
        
        private const string url = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdnv_MhoRAG5l7yFEVhFOvBLpIgKGynzoiHUhjP7f19L-99Fw/formResponse";
        
        #region PostData

        public static async void PostAll(PlaytestData data)
        {
            WWWForm form = new WWWForm();
            
            foreach (Field field in fields)
                form.AddField(field.ID, field.Process(data));

            for (var i = 0; i < data.Rounds.Count; i++)
            {
                var roundField = roundFieldsIDs[i];
                form.AddField(roundField, DataProcessing.EndRoundEntry(data.Rounds[i]));
            }

            UnityWebRequest www = UnityWebRequest.Post(url, form);
            await www.SendWebRequest();
        }
        
        private static async void Post(string entryName, string response)
        {
            WWWForm form = new WWWForm();
            form.AddField(entryName, response);
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            await www.SendWebRequest();
        }
        
        #endregion
    }
}
