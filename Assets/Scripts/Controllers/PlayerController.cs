using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour {
	
	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	public float accelerationTimeAirborne = .2f;
	public float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;

	public LayerMask collisionMask;
	
	float gravityAcceleration;
	float jumpVelocity;

	Vector2 velocity;

	float velocityXSmoothing;
	
	CollisionHandler collisionHandler;
	
	void Start() {

		collisionHandler = new CollisionHandler(GetComponent<BoxCollider2D>(), collisionMask);

		// Kinematic equation: accerlation = (2 x distance) / (time^2)
		// Initial velocity is zero.
		gravityAcceleration = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);

		// Kinematic equation: final_velocity = acceleration * time
		// Initial velocity is zero.
		jumpVelocity = Mathf.Abs(gravityAcceleration) * timeToJumpApex;
	}
	
	void Update() {

		// Handle y direction.
		if (collisionHandler.collisions.above || collisionHandler.collisions.below) {
			velocity.y = 0;
		}

		if (InputHandler.Instance.IsJumpPressed() && collisionHandler.collisions.below) {
			velocity.y = jumpVelocity;
		}

		velocity.y += gravityAcceleration * Time.deltaTime;

		// Handle x direction.
		float targetVelocityX = InputHandler.Instance.GetMovementVector().x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (collisionHandler.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

		// Handle collisions.
		Vector2 finalVelocity = collisionHandler.CalculateCollisions (velocity * Time.deltaTime);

		// Move player
		transform.Translate (finalVelocity);
	}
}