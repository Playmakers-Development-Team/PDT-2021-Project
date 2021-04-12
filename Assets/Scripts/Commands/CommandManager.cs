using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;

public class CommandManager : MonoBehaviour
{
    private List<Command> commandQueue = new List<Command>();
    private List<Command> commandHistory = new List<Command>();

    // private List<Command> turnOrder = new List<Command>();

    private int currentCommandQueueIndex;
    private int currentCommandHistoryIndex;

    public void QueueCommand(Command command)
    {
        commandQueue.Add(command);
        command.Queue();

        currentCommandQueueIndex = commandQueue.Count - 1;
    }

    public void ExecuteCommand(Command command)
    {
        commandHistory.Add(command);
        command.Execute();
        currentCommandHistoryIndex = commandHistory.Count - 1;
    }

    void Start() {}

    // Update is called once per frame
    void Update() {}
}
