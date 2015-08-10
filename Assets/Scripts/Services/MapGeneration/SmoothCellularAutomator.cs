
public class SmoothCellularAutomator : CellularAutomator {

	protected override void SetCell(Map map, int x, int y, int neighborWallCount) {
		if (neighborWallCount > 4)
			map.Cells[x,y] = Cell.SOLID;
		else if (neighborWallCount < 4)
			map.Cells[x,y] = Cell.EMPTY;
	}

	protected override int GetNumberOfIterations() {
		return 5;
	}
}
