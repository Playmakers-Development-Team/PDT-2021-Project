using Commands;
using Game.Commands;
using Managers;

namespace UI.PauseScreen
{
    public class RestartEncounterButton : PauseScreenButton
    {
        private CommandManager commandManager;
        
        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        
        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            commandManager.ExecuteCommand(new RestartEncounterCommand());
        }
    }
}
