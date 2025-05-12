using UnityEngine;

namespace Assets.Game.Data.GameObjects.NonPlayerCharacters.Animations
{
    public class WavingBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            animator.SetBool("IsWaving", false);
        }
    }
}
