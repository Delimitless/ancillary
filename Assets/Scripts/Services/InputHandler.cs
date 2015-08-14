using UnityEngine;

public class InputHandler : Singleton<InputHandler> {

	Vector2 movementVector;

	protected InputHandler() {}

	public Vector2 GetMovementVector() {

		movementVector.x = Input.GetAxisRaw("Horizontal");
		movementVector.y = Input.GetAxisRaw("Vertical");

		return movementVector;
	}
}
