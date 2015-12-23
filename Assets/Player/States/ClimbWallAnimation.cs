using UnityEngine;
using System.Collections;

public class ClimbWallAnimation : StateAnimation
{
    [SerializeField]
    private float distanceBetweenHands;

    Vector3 targetLeft;
    Vector3 targetRight;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, playerNavigation.JumpPower / 2, rigidbody.velocity.z);
        Collider collider = playerNavigation.getFrontCollider();
        if(collider)
        {
            Bounds bounds = collider.bounds;
            Vector3 target = bounds.ClosestPoint(player.transform.position);
            Vector3 playerPosition = player.transform.position;
            player.transform.LookAt(target);

            playerNavigation.findHandsPositions(ref targetLeft, ref targetRight, bounds);
        }
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetIKPosition(AvatarIKGoal.LeftHand, targetLeft);
        animator.SetIKPosition(AvatarIKGoal.RightHand, targetRight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
    }
}
