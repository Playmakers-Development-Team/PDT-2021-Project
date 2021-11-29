using System.Collections.Generic;
using UI.Core;
using Units.Players;

namespace UI.CombatEndUI
{
    public class HealRewardDialogue : Dialogue
    {
        protected readonly List<LoadoutUnitInfo> units = new List<LoadoutUnitInfo>();
        internal readonly Event<LoadoutUnitInfo> unitSpawned = new Event<LoadoutUnitInfo>();
    
        protected override void OnDialogueAwake()
        {
            base.OnDialogueAwake();
        
            unitSpawned.AddListener(info =>
            {
                if (info.Unit is PlayerUnit)
                    units.Add(info);
            });
        }

        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}
    }
}
