using UnityEngine;

public abstract class CellularAutomator : MonoBehaviour {

	public void CellularAutomate(Map map) {
		if (map == null) {
			throw new System.ArgumentNullException ("map cannot be null");
		}

		for (int i = 1; i < GetNumberOfIterations(); i++) {
			CelluarAutomateIteration (map);
		}
	}

	void CelluarAutomateIteration(Map map) {
		for (int x = 0; x < map.Width; x ++) {
			for (int y = 0; y < map.Height; y ++) {
				int neighborWallCount = GetSurroundingWallCount(map, x, y);
				
				SetCell(map, x, y, neighborWallCount);
			}
		}
	}

	int GetSurroundingWallCount(Map map, int x, int y) {

		int wallCount = 0;
		for (int neighborX = x - 1; neighborX <= x + 1; neighborX ++) {
			for (int neighborY = y - 1; neighborY <= y + 1; neighborY ++) {
				if (map.IsWithinBounds(neighborX, neighborY)) {
					if (neighborX != x || neighborY != y) {
						Cell cell = map.Cells[neighborX, neighborY];

						if (cell == Cell.SOLID) {
							wallCount += 1;
						}
					}
				}
				else {
					wallCount += 1;
				}
			}
		}
		
		return wallCount;
	}

	protected abstract void SetCell(Map map, int x, int y, int neighborWallCount);

	protected abstract int GetNumberOfIterations();
}
