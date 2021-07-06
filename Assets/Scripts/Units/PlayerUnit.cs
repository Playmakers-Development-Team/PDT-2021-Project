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
        
        public Animator UnitAnimator { get; private set; }
        public Ability CurrentlySelectedAbility { get; set; }

        private AnimationStates UnitAnimationStates;
        private SpriteRenderer spriteRenderer;
        
        
        protected override void Start()
        {
            base.Start();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            ManagerLocator.Get<PlayerManager>().Spawn(this);
            UnitAnimator = GetComponentInChildren<Animator>();
        }

        public void ChangeAnimation(AnimationStates animationStates) // this stuff is temporary, should probably be done in a better way
        {
            this.UnitAnimationStates = animationStates;
            Debug.Log("AnimationStates" + animationStates);

            switch (this.UnitAnimationStates)
            {
                case AnimationStates.Idle:
                    UnitAnimator.SetInteger("Movement", 0);
                    spriteRenderer.flipX = false;
                    UnitAnimator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Backward:
                    UnitAnimator.SetInteger("Movement", 2);
                    spriteRenderer.flipX = false;
                    UnitAnimator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Forward:
                    UnitAnimator.SetInteger("Movement", 1);
                    spriteRenderer.flipX = true;
                    UnitAnimator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Left:
                    UnitAnimator.SetInteger("Movement", 1);
                    spriteRenderer.flipX = true;
                    UnitAnimator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Right:
                    UnitAnimator.SetInteger("Movement", 1);
                     spriteRenderer.flipX = false;
                    UnitAnimator.SetBool("isCasting", false);
                    break;
                case AnimationStates.Casting:
                    UnitAnimator.SetInteger("Movement", 0);
                    spriteRenderer.flipX = false;
                    UnitAnimator.SetBool("isCasting", true);
                    break;
            }
        }
    }
}
