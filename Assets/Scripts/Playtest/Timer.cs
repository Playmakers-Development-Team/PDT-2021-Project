using UnityEngine;

namespace Playtest
{
    public class Timer : MonoBehaviour
    {
        private bool active;
        public float Elapsed { get; private set; }

        private void Update()
        {
            if (active)
                Elapsed += Time.deltaTime;
        }

        public void StartTimer() => active = true;

        public void StopTimer() => active = false;

        public void Reset() => Elapsed = 0;
    }
}
