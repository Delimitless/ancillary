using UnityEngine;

public abstract class CellularAutomator : MonoBehaviour {

	protected FillType[,] map;

	int width;
	int height;

	public void CellularAutomate(FillType[,] map) {
		this.map = map;
		this.width = map.GetLength(0);
		this.height = map.GetLength(1);

		for(int i = 1; i < GetNumberOfIterations(); i++) {
			CelluarAutomateIteration();
		}
	}

	void CelluarAutomateIteration() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighborWallCount = GetSurroundingWallCount(x,y);
				
				FillAlgorithm(x, y, neighborWallCount);
			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY) {

		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}
		
		return wallCount;
	}

	protected abstract void FillAlgorithm(int x, int y, int neighborWallCount);

	protected abstract int GetNumberOfIterations();
}
