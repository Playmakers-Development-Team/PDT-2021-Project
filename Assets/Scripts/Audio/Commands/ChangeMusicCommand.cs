using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;

public class ChangeMusicCommand : Command
{
    
    public string StateGroup { get; }
    
    public string StateName { get; }
    
    public ChangeMusicCommand(string StateGroup, string StateName)
    {
        this.StateGroup = StateGroup;
        this.StateName = StateName;
    }
    
    
}
