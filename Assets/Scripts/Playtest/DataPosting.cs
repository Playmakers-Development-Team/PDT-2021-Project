using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers;
using Units;
using UnityEngine;
using UnityEngine.Networking;

namespace Playtest
{
    public static class DataPosting
    {
        #region UnitIntialise
        
        // TODO: Implement.
        // private TemplateUnit mostDamageDealtUnits = new TemplateUnit();
        // private TemplateUnit leastDamageDealtUnits = new TemplateUnit();
        // private TemplateUnit mostDamageTakenUnits = new TemplateUnit();
        // private TemplateUnit leastDamageTakenUnits = new TemplateUnit();
        
        #endregion

        private class Entry
        {
            public string Field { get; set; }
            public string Data { get; set; }

            public Entry(string field, string data)
            {
                Field = field;
                Data = data;
            }
        }
        
        private static List<Entry> Entries { get; } = new List<Entry>();

        #region EntryFields

        private const string levelPlayedField = "entry.295363220";
        private const string initialUnitStatField = "entry.512851580";
        private const string initialTimelineField = "entry.887732368";
        private const string endUnitStatField = "entry.682653089";
        private const string endAbilityUsageField = "entry.225796238";
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
        
        private const string url = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdnv_MhoRAG5l7yFEVhFOvBLpIgKGynzoiHUhjP7f19L-99Fw/formResponse";
        
        #region PostData

        public static async void PostAll()
        {
            WWWForm form = new WWWForm();
            
            foreach (Entry entry in Entries)
                form.AddField(entry.Field, entry.Data);
            
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

        public static void InitialiseStatsEntries(PlaytestData data)
        {
            AddEntry(levelPlayedField, data.ActiveScene);
            AddEntry(initialUnitStatField, data.InitialUnits);
            AddEntry(initialTimelineField, data.InitialUnitOrder);
        }

        public static void EndGameEntries(PlaytestData data)
        {
            AddEntry(totalPlaytimeField, DataProcessing.TotalPlaytime(data));
            AddEntry(averageTimeTakenPerRoundField, DataProcessing.StrAverageTimeForRounds(data));
            AddEntry(averageTimeTakenPerTurnField, DataProcessing.StrAverageTimesForTurns(data));
            AddEntry(unitsThatTurnManipulatedTheMostField, DataProcessing.MostTurnManipulatedUnits(data));
            AddEntry(unitsThatMovedTheMostField, DataProcessing.MostTimesMovedUnits(data));
            AddEntry(unitsThatMovedTheFarthestField, DataProcessing.FarthestMovedUnits(data));
            AddEntry(unitsThatMovedTheLeastField, DataProcessing.LeastTimesMovedUnits(data));
            AddEntry(whichTeamWonField, DataProcessing.BattleOutcome(data));
            AddEntry(endUnitStatField, data.EndStateUnits);
            AddEntry(initialTimelineField, data.RoundCount.ToString());
            AddEntry(amountOfTimesTurnManipulatedField, data.AmountOfTurnsManipulated.ToString());
            AddEntry(totalTimesMeditatedField, data.MeditateAmount.ToString());
            AddEntry(endAbilityUsageField, DataProcessing.UpdateAbilityUsage(data));

            foreach (var roundData in data.Rounds)
            {
                AddEntry(roundFields[data.RoundCount], DataProcessing.EndRoundEntry(roundData));
            }
        }
        
        private static void AddEntry(string field, string data)
        {
            Entries.Add(new Entry(field, data));
        }
    }
}
