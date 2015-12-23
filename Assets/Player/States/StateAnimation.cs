using System;
using UnityEngine;

public class StateAnimation : StateMachineBehaviour
{
    protected GameObject player;
    protected Rigidbody rigidbody;
    protected PlayerNavigation playerNavigation;
    protected bool jumpPressed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.gameObject;
        playerNavigation = player.GetComponent<PlayerNavigation>();
        rigidbody = playerNavigation.Rigidbody;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /*public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

    }*/

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /*public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }*/

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    /*public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }*/

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    /*public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateIK(animator, stateInfo, layerIndex);
    }*/
}