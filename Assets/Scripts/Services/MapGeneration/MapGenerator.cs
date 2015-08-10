using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	private int width;
	private int height;
	private int randomFillPercent;
	
	private FillType[,] map;

	public MapGenerator(int width, int height, int randomFillPercent) {
		this.width = width;
		this.height = height;
		this.randomFillPercent = randomFillPercent;
	}
	
	public FillType[,] GenerateMap() {
		map = new FillType[width,height];
		RandomFillMap();
		
		for (int i = 0; i < 5; i ++) {
			SmoothMap();
		}

		return map;
	}
	
	
	void RandomFillMap() {
		string seed = Time.time.ToString();
		
		System.Random pseudoRandom = new System.Random(seed.GetHashCode());
		
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (x == 0 || x == width-1 || y == 0 || y == height -1) {
					map[x,y] = FillType.SOLID;
				}
				else {
					map[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent) ? FillType.SOLID : FillType.EMPTY;
				}
			}
		}
	}
	
	void SmoothMap() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = GetSurroundingWallCount(x,y);
				
				if (neighbourWallTiles > 4)
					map[x,y] = FillType.SOLID;
				else if (neighbourWallTiles < 4)
					map[x,y] = FillType.EMPTY;
				
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
}
