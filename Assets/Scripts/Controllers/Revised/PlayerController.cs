using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 6f;

	public float jumpHeight = 4f;
	public float timeToJumpApex = 0.4f;

	public float timeToMaxVelocityOnGround = 0.1f;
	public float timeToMaxVelocityInAir = 0.2f;

	// These are read-only after awake.
	float gravityAcceleration;
	float jumpVelocity;
	
	Vector2 velocity;
	float targetVelocityX;
	float velocityXSmoothing;

	void Awake() {

		// Kinematic equation: accerlation = (2 x distance) / (time^2)
		// Initial velocity is zero.
		gravityAcceleration = (2 * jumpHeight) / (Mathf.Pow(timeToJumpApex, 2));

		// Kinematic equation: final_velocity = acceleration * time
		// Initial velocity is zero.
		jumpVelocity = (gravityAcceleration * timeToJumpApex);
	}

	void Update() {

		Vector2 input = InputHandler.Instance.GetMovementVector();
		targetVelocityX = input.x * moveSpeed;
	}

	void FixedUpdate() {
		velocity.x = SmoothedTargetVelocity();
		velocity.y += gravityAcceleration;

		// TODO Move player (velocity)
	}


	/**
	 *  Smooth towards the target velocity, this gives the appearance of accelerating to the target velocity.
	 */
	float SmoothedTargetVelocity() {

		float smoothTime;
		if (velocity.y.Equals(0f)) { // TODO Rewrite to detect if collision below feet, then rewrite to just see if in air.
			smoothTime = timeToMaxVelocityOnGround;
		}
		else {
			smoothTime = timeToMaxVelocityInAir;
		}

		return Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
	}

}
