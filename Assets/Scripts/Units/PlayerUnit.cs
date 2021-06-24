using Abilities;
using Commands;
using GridObjects;
using Managers;
using UnityEngine;

namespace Units
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public enum AnimationStates
        {
            Idle,
            Forward,
            Backward,
            Left,
            Right,
            Casting, }

        public AnimationStates animationStates;
        public Animator animator;
        public Ability CurrentlySelectedAbility;
        private SpriteRenderer spriteRenderer;

        protected override void Start()
        {
            base.Start();
            ManagerLocator.Get<PlayerManager>().Spawn(this);
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void ChangeAnimation(AnimationStates animationStates) // this stuff is temporary, should probably be done in a better way
        {
            this.animationStates = animationStates;
            Debug.Log("AnimationStates" + animationStates);

            switch (this.animationStates)
            {
                case AnimationStates.Idle:
                    animator.SetInteger("Movement", 0);
                    spriteRenderer.flipX = false;
                    animator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Backward:
                    animator.SetInteger("Movement", 2);
                    spriteRenderer.flipX = false;
                    animator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Forward:
                    animator.SetInteger("Movement", 1);
                    spriteRenderer.flipX = true;
                    animator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Left:
                    animator.SetInteger("Movement", 1);
                    spriteRenderer.flipX = true;
                    animator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Right:
                    animator.SetInteger("Movement", 1);
                    spriteRenderer.flipX = false;
                    animator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Casting:
                    animator.SetInteger("Movement", 0);
                    spriteRenderer.flipX = false;
                    animator.SetBool("isCasting", true);
                    break;
            }
        }
    }
}
