using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public static FillType[,] GenerateMap(int width, int height, int randomFillPercent, CellularAutomator cellularAutomator) {

		FillType[,] map = new FillType[width,height];

		RandomFillMap(map, randomFillPercent);

		cellularAutomator.CellularAutomate(map);

		return map;
	}
	
	static void RandomFillMap(FillType[,] map, int randomFillPercent) {

		string seed = Time.time.ToString();
		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		int width = map.GetLength(0);
		int height = map.GetLength(1);
		
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
}
