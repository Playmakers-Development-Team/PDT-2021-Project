using Managers;
using UnityEngine;

namespace UI
{
    //TODO DELETE THIS CLASS 
    public class TurnManipulationUI : MonoBehaviour
    {


        public void Manipulate()
        {
            ManagerLocator.Get<TurnManager>().ShiftTurnQueue(5, 1);
        }
        
        
        
    }
}
