using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour {
	
	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	public float accelerationTimeAirborne = .2f;
	public float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;

	public LayerMask collisionMask;
	
	float gravity;
	float jumpVelocity;

	Vector2 velocity;

	float velocityXSmoothing;
	
	CollisionHandler collisionHandler;
	
	void Start() {

		collisionHandler = new CollisionHandler(GetComponent<BoxCollider2D>(), collisionMask);
		
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}
	
	void Update() {
		
		if (collisionHandler.collisions.above || collisionHandler.collisions.below) {
			velocity.y = 0;
		}
		
		Vector2 input = InputHandler.Instance.GetMovementVector();
		
		if (InputHandler.Instance.IsJumpPressed() && collisionHandler.collisions.below) {
			velocity.y = jumpVelocity;
		}
		
		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (collisionHandler.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;

		Vector2 finalVelocity = collisionHandler.CalculateCollisions (velocity * Time.deltaTime);
		transform.Translate (finalVelocity);
	}
}