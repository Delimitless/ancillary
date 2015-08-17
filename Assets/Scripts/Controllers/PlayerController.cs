using UnityEngine;

[RequireComponent (typeof(CollisionHandler))]
public class PlayerController : MonoBehaviour {
	
	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	public float accelerationTimeAirborne = .2f;
	public float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;
	
	float gravityAcceleration;
	float jumpVelocity;

	Vector2 velocity;

	float velocityXSmoothing;
	
	CollisionHandler collisionHandler;
	
	void Start() {

		collisionHandler = GetComponent<CollisionHandler>();

		// Kinematic equation: accerlation = (2 x distance) / (time^2)
		// Initial velocity is zero.
		gravityAcceleration = (2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);

		// Kinematic equation: final_velocity = acceleration * time
		// Initial velocity is zero.
		jumpVelocity = Mathf.Abs(gravityAcceleration) * timeToJumpApex;
	}
	
	void Update() {

		// Handle y direction.
		if (collisionHandler.HasCollisionAbove() || collisionHandler.HasCollisionBelow()) {
			velocity.y = 0;
		}

		if (InputHandler.Instance.IsJumpPressed() && collisionHandler.HasCollisionBelow()) {
			velocity.y = jumpVelocity;
		}

		velocity.y -= gravityAcceleration * Time.deltaTime;

		// Handle x direction.
		float targetVelocityX = InputHandler.Instance.GetMovementVector().x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (collisionHandler.HasCollisionBelow())?accelerationTimeGrounded:accelerationTimeAirborne);

		// Handle collisions.
		Vector2 finalVelocity = collisionHandler.AdjustVelocityForCollisions(velocity * Time.deltaTime);

		// Move player
		transform.Translate(finalVelocity);
	}
}