using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.VFX
{
    [RequireComponent(typeof(Animator))]
    public class UnitFade : MonoBehaviour
    {
        [SerializeField] private float minTime;
        [SerializeField] private float maxTime;

        [SerializeField] private Animator animator;

        private float startTime;
        private float time;

        private bool isIn;
        
        private static readonly int inId = Animator.StringToHash("in");
        private static readonly int outId = Animator.StringToHash("out");

        private void Awake()
        {
            time = Random.Range(minTime, maxTime);
        }

        private void Start()
        {
            startTime = Time.time;
        }

        private void Update()
        {
            if (Time.time < startTime + time || isIn)
                return;
            
            In();
        }

        private void In()
        {
            animator.SetTrigger(inId);
            isIn = true;
        }

        private void Out()
        {
            animator.SetTrigger(outId);
        }
    }
}
