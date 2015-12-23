using UnityEngine;
using System.Collections;

public class GroundedAnimation : StateAnimation
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    /*override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

    }*/

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        playerNavigation.handleGroundMovement();
        if (playerNavigation.FrameMove.magnitude > 0)
        {
            animator.speed = playerNavigation.AnimSpeedMultiplier;
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + playerNavigation.RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < playerNavigation.K_Half ? 1 : -1) * playerNavigation.ForwardAmount;
        animator.SetFloat("JumpLeg", jumpLeg);
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	/*override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

    }*/

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	/*override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Time.deltaTime > 0)
        {
            Vector3 v = (animator.deltaPosition * playerNavigation.MoveSpeedMultiplier) / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = rigidbody.velocity.y;
            rigidbody.velocity = v;
        }
    }*/

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	/*override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateIK(animator, stateInfo, layerIndex);

    }*/
}
