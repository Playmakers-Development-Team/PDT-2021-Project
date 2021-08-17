using Managers;

namespace Units.Players
{
    public class PlayerController : UnitController<PlayerUnitData>
    {
        protected override void Awake()
        {
            base.Awake();
            
            #region GetManagers

            unitManagerT = ManagerLocator.Get<PlayerManager>();

            #endregion
        }
    }
}
