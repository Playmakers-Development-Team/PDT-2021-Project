using System;
using Commands;
using Cysharp.Threading.Tasks;
using Managers;
using Units.Commands;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Playtest
{
    [ExecuteAlways]
    public class Playtest : MonoBehaviour
    {
        [SerializeField] private PlaytestData data;
        
        private CommandManager commandManager;

        private const string url = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdnv_MhoRAG5l7yFEVhFOvBLpIgKGynzoiHUhjP7f19L-99Fw/formResponse";

        private void InitialiseStats()
        {
            data.TimesMoved = 0;
            data.activeScene = SceneManager.GetActiveScene();
        }
    
        private void Awake()
        {
            //InitialiseStats();
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        private void Start()
        {
            commandManager.ListenCommand<StartMoveCommand>(cmd =>
            {
                data.TimesMoved++;
            });

            if (Application.isPlaying)
                InitialiseStats();
            else
                PostAll();
        }

        
        private async void Post(string entryName, string response)
        {
            WWWForm form = new WWWForm();
            form.AddField(entryName, response);
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            await www.SendWebRequest();

        }

        #region PostData

        private void PostAll()
        {
            Post("entry.290157031","Level Playing: " + data.activeScene);
            Post("entry.290157031","Level Playing: " + data.activeScene);
            Post("entry.290157031","Level Playing: " + data.activeScene);
            Post("entry.290157031","Level Playing: " + data.activeScene);
            Post("entry.290157031","Level Playing: " + data.activeScene);

            
        }
        #endregion
    }
}
