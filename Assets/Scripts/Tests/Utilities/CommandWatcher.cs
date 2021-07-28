using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Commands;
using Managers;
using UnityEngine;

namespace Tests.Utilities
{
    public class CommandWatcher
    {
        private readonly Task task;
        private readonly Type[] types;

        private CommandWatcher(Task task, Type[] types)
        {
            this.task = task;
            this.types = types;
        }

        public void Assert(string failMessage = null)
        {
            if (string.IsNullOrEmpty(failMessage))
                failMessage = $"Expected Command {string.Join(", ", types.ToArray<object>())}, but didn't get any!";
            
            NUnit.Framework.Assert.True(task.IsCompleted, failMessage);
        }

        public IEnumerator Wait()
        {
            yield return new WaitUntil(() => task.IsCompleted);
        }
        
        private static CommandManager CommandManager => ManagerLocator.Get<CommandManager>();
        
        public static CommandWatcher BeginWatching<T>() where T : Command => 
            new CommandWatcher(CommandManager.WaitForCommand<T>(), new [] {typeof(T)});
        
        public static CommandWatcher BeginWatching<T>(Predicate<T> filter) where T : Command => 
            new CommandWatcher(CommandManager.WaitForCommand(filter), new [] {typeof(T)});
        
        public static CommandWatcher BeginWatching<T1, T2>(Func<T1, T2, bool> filter) 
            where T1 : Command 
            where T2 : Command => 
            new CommandWatcher(CommandManager.WaitForCommand(filter), new [] {typeof(T1), typeof(T2)});
        
        public static CommandWatcher BeginWatching<T1, T2, T3>(Func<T1, T2, T3, bool> filter) 
            where T1 : Command
            where T2 : Command 
            where T3 : Command => 
            new CommandWatcher(CommandManager.WaitForCommand(filter), new [] {typeof(T1), typeof(T2), typeof(T3)});
        
        public static CommandWatcher BeginWatching<T1, T2, T3, T4>(Func<T1, T2, T3, T4, bool> filter) 
            where T1 : Command
            where T2 : Command 
            where T3 : Command 
            where T4 : Command => 
            new CommandWatcher(CommandManager.WaitForCommand(filter), new [] {typeof(T1), typeof(T2), typeof(T3), typeof(T4)});
        
        public static CommandWatcher BeginWatching<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, bool> filter) 
            where T1 : Command
            where T2 : Command 
            where T3 : Command 
            where T4 : Command 
            where T5 : Command => 
            new CommandWatcher(CommandManager.WaitForCommand(filter), new [] {typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)});
    }
}
