using UnityEngine;

public class CollisionHandler {
	
	const float SKIN_WIDTH = .015f;
	const int HORIZONTAL_RAY_COUNT = 4;
	const int VERTICAL_RAY_COUNT = 4;
	const float MAX_CLIMB_ANGLE = 80;
	const float MAX_DESCEND_ANGLE = 80;
	
	float horizontalRaySpacing;
	float verticalRaySpacing;

	Vector2 originalVelocity;

	BoxCollider2D boxCollider;
	LayerMask collisionMask;
	CollisionInfo collisions;
	RaycastOrigins raycastOrigins;

	public CollisionHandler(BoxCollider2D boxCollider, LayerMask collisionMask) {
		this.boxCollider = boxCollider;
		this.collisionMask = collisionMask;

		CalculateRaySpacing ();
	}

	public Vector2 CalculateCollisions(Vector2 velocity) {

		UpdateRaycastOrigins ();
		collisions.Reset ();
		originalVelocity = velocity;
		
		if (velocity.y < 0) {
			DescendSlope(ref velocity);
		}
		if (!Mathf.Approximately(velocity.x, 0)) {
			HorizontalCollisions (ref velocity);
		}
		if (!Mathf.Approximately(velocity.y, 0)) {
			VerticalCollisions (ref velocity);
		}
		
		return velocity;
	}

	public bool HasCollisionBelow() {
		return collisions.below;
	}

	public bool HasCollisionAbove() {
		return collisions.above;
	}
	
	void HorizontalCollisions(ref Vector2 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + SKIN_WIDTH;
		
		for (int i = 0; i < HORIZONTAL_RAY_COUNT; i ++) {
			Vector2 rayOrigin = (Mathf.Approximately(directionX, -1))?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);
			
			if (hit) {
				
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				
				if (i == 0 && slopeAngle <= MAX_CLIMB_ANGLE) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						velocity = originalVelocity;
					}
					float distanceToSlopeStart = 0;
					if (!Mathf.Approximately(slopeAngle, collisions.prevSlopeAngle)) {
						distanceToSlopeStart = hit.distance-SKIN_WIDTH;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}
				
				if (!collisions.climbingSlope || slopeAngle > MAX_CLIMB_ANGLE) {
					velocity.x = (hit.distance - SKIN_WIDTH) * directionX;
					rayLength = hit.distance;
					
					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}
					
					collisions.left = Mathf.Approximately(directionX, -1);
					collisions.right = Mathf.Approximately(directionX, 1);
				}
			}
		}
	}
	
	void VerticalCollisions(ref Vector2 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + SKIN_WIDTH;
		
		for (int i = 0; i < VERTICAL_RAY_COUNT; i ++) {
			Vector2 rayOrigin = (Mathf.Approximately(directionY, -1))?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);
			
			if (hit) {
				velocity.y = (hit.distance - SKIN_WIDTH) * directionY;
				rayLength = hit.distance;
				
				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}
				
				collisions.below = Mathf.Approximately(directionY, -1);
				collisions.above = Mathf.Approximately(directionY, 1);
			}
		}
		
		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + SKIN_WIDTH;
			Vector2 rayOrigin = ((Mathf.Approximately(directionX, -1))?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength,collisionMask);
			
			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
				if (!Mathf.Approximately(slopeAngle, collisions.slopeAngle)) {
					velocity.x = (hit.distance - SKIN_WIDTH) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}
	
	void ClimbSlope(ref Vector2 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
		
		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}
	
	void DescendSlope(ref Vector2 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (Mathf.Approximately(directionX, -1)) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
		
		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (!Mathf.Approximately(slopeAngle, 0) && slopeAngle <= MAX_DESCEND_ANGLE) {
				if (Mathf.Approximately(Mathf.Sign(hit.normal.x), directionX)) {
					if (hit.distance - SKIN_WIDTH <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;
						
						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}
	
	void UpdateRaycastOrigins() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (SKIN_WIDTH * -2);
		
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}
	
	void CalculateRaySpacing() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (SKIN_WIDTH * -2);
		
		horizontalRaySpacing = bounds.size.y / (HORIZONTAL_RAY_COUNT - 1);
		verticalRaySpacing = bounds.size.x / (VERTICAL_RAY_COUNT - 1);
	}
	
	struct CollisionInfo {
		public bool above, below;
		public bool left, right;
		
		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, prevSlopeAngle;
		
		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			
			prevSlopeAngle = slopeAngle;
			slopeAngle = 0;
		}
	}

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}