using Tests.Beacons.Base;
using UnityEngine;

namespace Tests.AutomatedTests
{
    public class TestObject : MonoBehaviour
    {
        public InputBeacon inputBeacon { get; set; }
        private void OnDestroy()
        {
            inputBeacon.RestoreRegularDevices();
        }
    }
}
