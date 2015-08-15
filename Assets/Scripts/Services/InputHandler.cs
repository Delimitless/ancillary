using UnityEngine;

public class InputHandler : Singleton<InputHandler> {

	protected InputHandler() {}

	public Vector2 GetMovementVector() {
		return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
	}

	public bool IsJumpPressed() {
		return Input.GetKeyDown(KeyCode.Space);
	}

	public bool IsJumpReleased() {
		return Input.GetKeyUp(KeyCode.Space);
	}
}
