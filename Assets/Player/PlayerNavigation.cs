using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerNavigation : MonoBehaviour
{
    [SerializeField]
    float m_MovingTurnSpeed = 360;
    [SerializeField]
    float m_StationaryTurnSpeed = 180;
    [SerializeField]
    float jumpPower = 12f;
    public float JumpPower
    { get { return jumpPower; } }
    [Range(1f, 4f)]
    [SerializeField]
    float gravityMultiplier = 2f;
    public float GravityMultiplier
    { get { return gravityMultiplier; } }
    [SerializeField]
    float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    public float RunCycleLegOffset
    { get { return runCycleLegOffset; } }
    [SerializeField]
    float moveSpeedMultiplier = 1f;
    public float MoveSpeedMultiplier
    { get { return moveSpeedMultiplier; } }
    [SerializeField]
    float animSpeedMultiplier = 1f;
    public float AnimSpeedMultiplier
    { get { return animSpeedMultiplier; } }
    [SerializeField]
    float groundCheckDistance = 0.15f;
    public float GroundCheckDistance
    {
        set { groundCheckDistance = value;}
        get { return groundCheckDistance; }
    }

    [SerializeField]
    private Transform rightHandCastOrigin;
    [SerializeField]
    private Transform leftHandCastOrigin;

    Vector3 frameMove;
    public Vector3 FrameMove
    {
        get { return frameMove; }
    }
    bool frameJump;
    public bool FrameJump
    {
        get { return frameJump; }
    }
    bool frameCrouch;
    public bool FrameCrouch
    {
        get { return frameCrouch; }
    }
    float turnAmount;
    public float TurnAmount
    {
        get { return turnAmount; }
    }
    float forwardAmount;
    public float ForwardAmount
    {
        get { return forwardAmount; }
    }
    const float k_Half = 0.5f;
    public float K_Half
    {
        get { return k_Half; }
    }

    Rigidbody myRigidbody;
    public Rigidbody Rigidbody
    { get { return myRigidbody; } }

    Animator animator;
    bool isGrounded;
    Vector3 groundNormal;
    float capsuleHeight;
    Vector3 capsuleCenter;
    CapsuleCollider capsule;
    bool crouching;

    void Start()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        capsuleHeight = capsule.height;
        capsuleCenter = capsule.center;
        isGrounded = true;


        myRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    public void move(Vector3 move, bool crouch, bool jump)
    {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        checkGroundStatus();
        move = Vector3.ProjectOnPlane(move, groundNormal);
        turnAmount = Mathf.Atan2(move.x, move.z);
        forwardAmount = move.z;

        frameMove = move;
        frameJump = jump;
        frameCrouch = crouch;

        applyExtraTurnRotation();
        // control and velocity handling is different when grounded and airborne:

        //scaleCapsuleForCrouching(crouch);
        //preventStandingInLowHeadroom();

        // send input and other state parameters to the animator
        updateAnimator(move);
    }


    void scaleCapsuleForCrouching(bool crouch)
    {
        if (isGrounded && crouch)
        {
            if (crouching) return;
            capsule.height = capsule.height / 2f;
            capsule.center = capsule.center / 2f;
            crouching = true;
        }
        else
        {
            Ray crouchRay = new Ray(myRigidbody.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = capsuleHeight - capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
            {
                crouching = true;
                return;
            }
            capsule.height = capsuleHeight;
            capsule.center = capsuleCenter;
            crouching = false;
        }
    }

    void preventStandingInLowHeadroom()
    {
        // prevent standing up in crouch-only zones
        if (!crouching)
        {
            Ray crouchRay = new Ray(myRigidbody.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = capsuleHeight - capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
            {
                crouching = true;
            }
        }
    }


    void updateAnimator(Vector3 move)
    {
        // update the animator parameters
        animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        animator.SetBool("Crouch", crouching);
        animator.SetBool("OnGround", isGrounded);
    }

    void doHandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch)
        {
            // jump!
            isGrounded = false;
            groundCheckDistance = 0.15f;
        }
    }

    /*public void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (isGrounded && Time.deltaTime > 0)
        {
            Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = myRigidbody.velocity.y;
            myRigidbody.velocity = v;
        }
    }*/

    public void handleGroundMovement()
    {
        doHandleGroundedMovement(frameCrouch, frameJump);
    }

    void applyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void checkGroundStatus()
    {
        if (frameJump)
            return;
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance), Color.red);
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down * groundCheckDistance, out hitInfo, groundCheckDistance))
        {
            groundNormal = hitInfo.normal;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }
        Debug.Log(isGrounded);
    }

    public Collider getFrontCollider()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position, transform.position + (transform.forward * 1f));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 1))
        {
            return hitInfo.collider;
        }
        else
        {
            return null;
        }
    }

    public void findHandsPositions(ref Vector3 leftPosition, ref Vector3 rightPosition, Bounds bounds)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(rightHandCastOrigin.position, transform.forward, out hitInfo, 1))
        {
            rightPosition = hitInfo.point;
        }
        else
        {
            rightPosition = bounds.ClosestPoint(rightHandCastOrigin.position);
        }
        if (Physics.Raycast(leftHandCastOrigin.position, transform.forward, out hitInfo, 1))
        {
            leftPosition = hitInfo.point;
        }
        else
        {
            leftPosition = bounds.ClosestPoint(leftHandCastOrigin.position);
        }
        leftPosition.y = bounds.max.y;
        rightPosition.y = bounds.max.y;
    }
}
