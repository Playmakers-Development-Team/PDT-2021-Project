using System.Collections;
using UnityEngine.TestTools;

namespace Tests.AutomatedTests
{
    public class SimpleTests : BaseTest
    {
        [UnityTest]
        public IEnumerator TestGameRun()
        {
            yield return ActivateScene();
        }
    }
}
