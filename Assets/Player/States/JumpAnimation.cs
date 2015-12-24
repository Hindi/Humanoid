using UnityEngine;
using System.Collections;

public class JumpAnimation : StateAnimation
{
    [SerializeField]
    private float groundCheckDistance;
    private bool shouldLand;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.applyRootMotion = false;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, playerNavigation.JumpPower, rigidbody.velocity.z);
        shouldLand = false;
        animator.ResetTrigger("Land");
        animator.speed = 1;
        playerNavigation.GroundCheckDistance = groundCheckDistance;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 extraGravityForce = (Physics.gravity * playerNavigation.GravityMultiplier) - Physics.gravity;
        rigidbody.AddForce(extraGravityForce);
        animator.SetFloat("Jump", rigidbody.velocity.y);
        if (rigidbody.velocity.y < -2)
            shouldLand = true; ;
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = true;

        if (shouldLand)
            animator.SetTrigger("Land");
    }

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
