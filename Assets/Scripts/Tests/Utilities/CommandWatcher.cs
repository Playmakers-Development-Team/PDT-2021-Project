using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Commands;
using Managers;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Tests.Utilities
{
    public class CommandWatcher
    {
        private readonly Func<UniTask> taskCreator;
        private readonly Type[] types;
        private UniTask? task;
        
        private string CommandsDescription => string.Join(", ", types.ToArray<object>());

        private CommandWatcher(Func<UniTask> task, Type[] types)
        {
            this.taskCreator = task;
            this.types = types;
        }

        private CommandWatcher StartWatching()
        {
            task = taskCreator();
            return this;
        }

        public void Rewatch() => StartWatching();

        public void Assert(string failMessage = null)
        {
            if (!task.HasValue)
            {
                throw new InvalidOperationException(
                    $"Cannot Assert when we haven't even start watching {CommandsDescription}");
            }

            if (string.IsNullOrEmpty(failMessage))
                failMessage = $"Expected {CommandsDescription}, but didn't get any!";
            
            NUnit.Framework.Assert.True(task.Value.GetAwaiter().IsCompleted, failMessage);
        }

        public IEnumerator Wait()
        {
            if (!task.HasValue)
            {
                throw new InvalidOperationException(
                    $"Cannot Wait when we haven't even start watching {CommandsDescription}");
            }
            
            yield return new WaitUntil(() => task.Value.Status.IsCompleted());
        }

        private static CommandManager CommandManager => ManagerLocator.Get<CommandManager>();

        public static CommandWatcher BeginWatching<T>() where T : Command =>
            new CommandWatcher(() => CommandManager.WaitForCommand<T>(), new[] {typeof(T)})
                .StartWatching();

        public static CommandWatcher BeginWatching<T>(Predicate<T> filter) where T : Command =>
            new CommandWatcher(() => CommandManager.WaitForCommand(filter), new[] {typeof(T)})
                .StartWatching();

        public static CommandWatcher BeginWatching<T1, T2>(Func<T1, T2, bool> filter)
            where T1 : Command
            where T2 : Command =>
            new CommandWatcher(() => CommandManager.WaitForCommand(filter), new[] {typeof(T1), typeof(T2)});

        public static CommandWatcher BeginWatching<T1, T2, T3>(Func<T1, T2, T3, bool> filter)
            where T1 : Command
            where T2 : Command
            where T3 : Command =>
            new CommandWatcher(() => CommandManager.WaitForCommand(filter),
                new[] {typeof(T1), typeof(T2), typeof(T3)}).StartWatching();

        public static CommandWatcher BeginWatching<T1, T2, T3, T4>(Func<T1, T2, T3, T4, bool> filter)
            where T1 : Command
            where T2 : Command
            where T3 : Command
            where T4 : Command =>
            new CommandWatcher(() => CommandManager.WaitForCommand(filter),
                new[] {typeof(T1), typeof(T2), typeof(T3), typeof(T4)}).StartWatching();

        public static CommandWatcher BeginWatching<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, bool> filter)
            where T1 : Command
            where T2 : Command
            where T3 : Command
            where T4 : Command
            where T5 : Command =>
            new CommandWatcher(() => CommandManager.WaitForCommand(filter),
                new[] {typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)}).StartWatching();
    }
}
