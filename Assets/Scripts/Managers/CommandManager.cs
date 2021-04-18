using System.Collections.Generic;
using Commands;
using Managers;

public class CommandManager : IManager
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
    
}
