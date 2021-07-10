using Abilities;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public enum AnimationStates
        {
            Idle,
            Up,
            Down,
            Left,
            Right,
            Casting
        }
        
        public Animator UnitAnimator { get; private set; }
        public Ability CurrentlySelectedAbility { get; set; }

        private AnimationStates unitAnimationStates;
        private SpriteRenderer spriteRenderer;

        private CommandManager commandManager;
        
        protected override void Start()
        {
            base.Start();
            commandManager = ManagerLocator.Get<CommandManager>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            ManagerLocator.Get<PlayerManager>().Spawn(this);
            UnitAnimator = GetComponentInChildren<Animator>();
            
            commandManager.ListenCommand<AbilityCommand>(cmd =>
            {
                if (!ReferenceEquals(cmd.Unit, this))
                    return;
                
                ChangeAnimation(AnimationStates.Casting);
            });
        }

        public void ChangeAnimation(AnimationStates animationStates) // this stuff is temporary, should probably be done in a better way
        {
            unitAnimationStates = animationStates;

            switch (unitAnimationStates)
            {
                case AnimationStates.Idle:
                    UnitAnimator.SetBool("moving", false);
                    UnitAnimator.SetBool("front", true);
                    
                    spriteRenderer.flipX = false;
                    break;
                
                case AnimationStates.Down:
                    UnitAnimator.SetBool("moving", true);
                    UnitAnimator.SetBool("front", true);
                    
                    spriteRenderer.flipX = false;
                    break;
                
                case AnimationStates.Up:
                    UnitAnimator.SetBool("moving", true);
                    UnitAnimator.SetBool("front", false);
                    
                    spriteRenderer.flipX = true;
                    break;
                
                case AnimationStates.Left:
                    UnitAnimator.SetBool("moving", true);
                    UnitAnimator.SetBool("front", true);
                    
                    spriteRenderer.flipX = true;
                    break;
                
                case AnimationStates.Right:
                    UnitAnimator.SetBool("moving", true);
                    UnitAnimator.SetBool("front", false);
                    
                    spriteRenderer.flipX = false;
                    break;
                
                case AnimationStates.Casting:
                    UnitAnimator.SetBool("moving", false);
                    // UnitAnimator.SetBool("front", true);
                    
                    UnitAnimator.SetTrigger("attack");
                    break;
            }
        }
    }
}
