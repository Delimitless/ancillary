
public class SmoothCellularAutomator : CellularAutomator {

	protected override void FillAlgorithm(int x, int y, int neighborWallCount) {
		if (neighborWallCount > 4)
			map[x,y] = FillType.SOLID;
		else if (neighborWallCount < 4)
			map[x,y] = FillType.EMPTY;
	}

	protected override int GetNumberOfIterations() {
		return 5;
	}
}
