using UnityEngine;

public class VectorUtil {

	public static bool IsMovingInXDirection(Vector2 vector) {
		if (Mathf.Approximately(vector.x, 0)) {
			return false;
		}
		else {
			return true;
		}
	}

	public static bool IsMovingInYDirection(Vector2 vector) {
		if (Mathf.Approximately(vector.y, 0)) {
			return false;
		}
		else {
			return true;
		}
	}

	public static bool IsPositiveXDirection(Vector2 vector) {
		if (Mathf.Approximately(Mathf.Sign(vector.x), 1)) {
			return true;
		}
		else {
			return false;
		}
	}
}
