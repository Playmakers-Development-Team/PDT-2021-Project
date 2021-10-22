using System;
using System.Collections;
using Commands;
using Game.Commands;
using Managers;
using Turn.Commands;
using UI.Core;
using UI.Game;
using UnityEngine;

namespace UI
{
    public class SceneCloser : MonoBehaviour
    {
        [SerializeField] private float delay;
        
        private void Start()
        {
            StartCoroutine(CloseRoutine());
        }

        private IEnumerator CloseRoutine()
        {
            yield return new WaitForSeconds(delay);
            
            ManagerLocator.Get<CommandManager>().ExecuteCommand(new NoRemainingPlayerUnitsCommand());
            ManagerLocator.Get<CommandManager>().ExecuteCommand(new NoRemainingEnemyUnitsCommand());
            ManagerLocator.Get<UIManager>().GetDialogue<GameDialogue>().Demote();
        }
    }
}
