using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour {

	public float moveSpeed = 6f;

	public float jumpHeight = 4f;
	public float timeToJumpApex = 0.4f;

	public float timeToMaxVelocityOnGround = 0.1f;
	public float timeToMaxVelocityInAir = 0.2f;

	BoxCollider2D playerCollider;
	RaycastOrigins raycastOrigins;
	
	float gravityAcceleration;
	float jumpVelocity;
	
	Vector2 velocity;
	float targetVelocityX;
	float velocityXSmoothing;

	void Awake() {

		playerCollider = GetComponent<BoxCollider2D>();
		raycastOrigins = new RaycastOrigins(playerCollider.bounds);

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

		// Update y direction with gravity, if no collision below.
		if (Collision.below) {
			velocity.y = 0;
		}
		else {
			velocity.y += gravityAcceleration;
		}

		MovePlayer();
	}

	void MovePlayer() {

		raycastOrigins.UpdateRaycastOrigins();
		Collision.Reset();
	}


	/**
	 *  Smooth towards the target velocity, this gives the appearance of accelerating to the target velocity.
	 */
	float SmoothedTargetVelocity() {

		float smoothTime = (Collision.below) ? timeToMaxVelocityOnGround : timeToMaxVelocityInAir;

		return Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
	}

	struct Collision {
		public static bool above, below;
		public static bool left, right;
		
		public static void Reset() {
			above = below = false;
			left = right = false;
		}
	}

	class RaycastOrigins {

		public const float SKIN_WIDTH = 0.015f;
		public const float HORIZONTAL_RAY_COUNT = 4;
		public const float VERTICAL_RAY_COUNT = 4;

		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;

		public float horizontalRaySpacing, verticalRaySpacing;

		Bounds bounds;

		public RaycastOrigins(Bounds playerBounds) {

			bounds = playerBounds;

			// Shrink bounds by skin width. NOTE the negative.
			// Multiplied by 2 to cover both sides.
			bounds.Expand (SKIN_WIDTH * -2);

			SetupRaySpacing();
			UpdateRaycastOrigins();
		}
		
		public void UpdateRaycastOrigins() {

			topLeft = new Vector2(bounds.min.x, bounds.max.y);
			topRight = new Vector2(bounds.max.x, bounds.max.y);
			bottomRight = new Vector2(bounds.max.x, bounds.min.y);
			bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		}

		void SetupRaySpacing() {
			
			horizontalRaySpacing = (bounds.size.x) / (HORIZONTAL_RAY_COUNT - 1);
			verticalRaySpacing = (bounds.size.y) / (VERTICAL_RAY_COUNT - 1);
		}
	}
}
