using Managers;
using UnityEngine;

namespace Testing
{
    public class TestGridCoordinateConversion : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            ManagerLocator.Get<GridManager>().ConvertWorldSpaceToGridSpace(transform.position);
        }
    }
}
