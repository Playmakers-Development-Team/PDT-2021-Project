using UI.Core;
using UnityEngine;

public class HealRewardDialogue : Dialogue
{
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
