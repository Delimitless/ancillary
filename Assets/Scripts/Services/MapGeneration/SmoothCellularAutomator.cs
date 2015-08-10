using UnityEngine;
using System.Collections;

public class SmoothCellularAutomator : CellularAutomator {

	protected override void fillAlgorithm(int x, int y, int neighborWallCount) {
		if (neighborWallCount > 4)
			map[x,y] = FillType.SOLID;
		else if (neighborWallCount < 4)
			map[x,y] = FillType.EMPTY;
	}

	protected override int numberOfIterations() {
		return 5;
	}
}
