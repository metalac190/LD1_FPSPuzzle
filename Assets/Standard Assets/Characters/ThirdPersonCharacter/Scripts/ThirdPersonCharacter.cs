using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class ThirdPersonCharacter : MonoBehaviour
{
    #region Variables and Classes
    [SerializeField] float movingTurnSpeed = 2000;
	[SerializeField] float stionaryTurnSpeed = 1000;
	[SerializeField] float jumpPower = 6f;
	[Range(1f, 4f)][SerializeField] float gravityMultiplier = 2f;
	[SerializeField] float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float moveSpeedMultiplier = 1f;
	[SerializeField] float animSpeedMultiplier = 1f;
	[SerializeField] float groundCheckDistance = 0.3f;

    const float k_Half = 0.5f;

    // states
    Vector3 groundNormal;
    float turnAmount;
    float forwardAmount;
    bool isGrounded;
    bool isCrouching;

    // book-keeping
    float capsuleHeight;
    Vector3 capsuleCenter;

    // caching
    Rigidbody rigidbody;
	Animator animator;
	float originalCheckDistance;

    // caching
    CapsuleCollider capsuleCollider;
    #endregion

    #region Setup
    void Start()
	{
        // caching
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		capsuleHeight = capsuleCollider.height;
		capsuleCenter = capsuleCollider.center;

        // prevent rigidbody from rotating by locking rotate constraints. This would make our character model twist unrealistically
		rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        // ??
        originalCheckDistance = groundCheckDistance;
	}
    #endregion

    public void Move(Vector3 _move, bool _crouch, bool _jump)
	{

		// convert the world relative moveInput vector into a local-relative
		// turn amount and forward amount required to head in the desired
		// direction.
		if (_move.magnitude > 1f) _move.Normalize();
		_move = transform.InverseTransformDirection(_move);
		CheckGroundStatus();
		_move = Vector3.ProjectOnPlane(_move, groundNormal);
		turnAmount = Mathf.Atan2(_move.x, _move.z);
		forwardAmount = _move.z;

		ApplyExtraTurnRotation();

		// control and velocity handling is different when grounded and airborne:
		if (isGrounded)
		{
			HandleGroundedMovement(_crouch, _jump);
		}
		else
		{
			HandleAirborneMovement();
		}

		ScaleCapsuleForCrouching(_crouch);
		PreventStandingInLowHeadroom();

		// send input and other state parameters to the animator
		UpdateAnimator(_move);
	}


	void ScaleCapsuleForCrouching(bool _crouch)
	{
		if (isGrounded && _crouch)
		{
			if (isCrouching) return;
			capsuleCollider.height = capsuleCollider.height / 2f;
			capsuleCollider.center = capsuleCollider.center / 2f;
			isCrouching = true;
		}
		else
		{
			Ray crouchRay = new Ray(rigidbody.position + Vector3.up * capsuleCollider.radius * k_Half, Vector3.up);
			float crouchRayLength = capsuleHeight - capsuleCollider.radius * k_Half;
			if (Physics.SphereCast(crouchRay, capsuleCollider.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				isCrouching = true;
				return;
			}
			capsuleCollider.height = capsuleHeight;
			capsuleCollider.center = capsuleCenter;
			isCrouching = false;
		}
	}

	void PreventStandingInLowHeadroom()
	{
		// prevent standing up in crouch-only zones
		if (!isCrouching)
		{
			Ray crouchRay = new Ray(rigidbody.position + Vector3.up * capsuleCollider.radius * k_Half, Vector3.up);
			float crouchRayLength = capsuleHeight - capsuleCollider.radius * k_Half;
			if (Physics.SphereCast(crouchRay, capsuleCollider.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				isCrouching = true;
			}
		}
	}


	void UpdateAnimator(Vector3 _move)
	{
		// update the animator parameters
		animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
		animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
		animator.SetBool("Crouch", isCrouching);
		animator.SetBool("OnGround", isGrounded);
		if (!isGrounded)
		{
			animator.SetFloat("Jump", rigidbody.velocity.y);
		}

		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		// (This code is reliant on the specific run cycle offset in our animations,
		// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		float runCycle =
			Mathf.Repeat(
				animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);
		float jumpLeg = (runCycle < k_Half ? 1 : -1) * forwardAmount;
		if (isGrounded)
		{
			animator.SetFloat("JumpLeg", jumpLeg);
		}

		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		// which affects the movement speed because of the root motion.
		if (isGrounded && _move.magnitude > 0)
		{
			animator.speed = animSpeedMultiplier;
		}
		else
		{
			// don't use that while airborne
			animator.speed = 1;
		}
	}


	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
		rigidbody.AddForce(extraGravityForce);

		groundCheckDistance = rigidbody.velocity.y < 0 ? originalCheckDistance : 0.01f;
	}


	void HandleGroundedMovement(bool _crouch, bool _jump)
	{
		// check whether conditions are right to allow a jump:
		if (_jump && !_crouch && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
		{
			// jump!
			rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpPower, rigidbody.velocity.z);
			isGrounded = false;
			animator.applyRootMotion = false;
			groundCheckDistance = 0.1f;
		}
	}

	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(stionaryTurnSpeed, movingTurnSpeed, forwardAmount);
		transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
	}


	public void OnAnimatorMove()
	{
		// we implement this function to override the default root motion.
		// this allows us to modify the positional speed before it's applied.
		if (isGrounded && Time.deltaTime > 0)
		{
			Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

			// we preserve the existing y part of the current velocity.
			v.y = rigidbody.velocity.y;
			rigidbody.velocity = v;
		}
	}


	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
		{
			groundNormal = hitInfo.normal;
			isGrounded = true;
			animator.applyRootMotion = true;
		}
		else
		{
			isGrounded = false;
			groundNormal = Vector3.up;
			animator.applyRootMotion = false;
		}
	}
}
