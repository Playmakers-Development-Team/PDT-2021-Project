using System.Collections;
using E7.Minefield;
using UnityEngine.TestTools;


namespace Tests.AutomatedTests
{
    public class SimpleTests : SceneTest
    {
        protected override string Scene => "MainTest";

        [UnityTest]
        public IEnumerator TestGameRun()
        {
            yield return ActivateScene();
        }
    }
}
