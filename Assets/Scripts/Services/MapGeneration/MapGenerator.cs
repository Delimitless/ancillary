using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public static Map GenerateMap(int width, int height, int borderSize, int randomFillPercent, CellularAutomator cellularAutomator) {

		Map map = new Map(width, height);

		RandomFillMap(map, randomFillPercent);

		cellularAutomator.CellularAutomate(map);

		Map borderedMap = new Map(width + borderSize * 2, height + borderSize * 2);
		for(int x = 0; x < borderedMap.Width; x++) {
			for(int y = 0; y < borderedMap.Height; y++) {



				if (x >= borderSize && y >= borderSize && x < borderedMap.Width - borderSize && y < borderedMap.Height - borderSize) {
					borderedMap.Cells[x,y] = map.Cells[x-borderSize,y-borderSize];
				}
				else {
					borderedMap.Cells[x,y] = Cell.SOLID;
				}
			}
		}

		return borderedMap;
	}
	
	static void RandomFillMap(Map map, int randomFillPercent) {

		System.Random pseudoRandom = new System.Random();
		
		for (int x = 0; x < map.Width; x++) {
			for (int y = 0; y < map.Height; y++) {
				if (x == 0 || x == map.Width-1 || y == 0 || y == map.Height -1) {
					map.Cells[x,y] = Cell.SOLID;
				}
				else {
					map.Cells[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent) ? Cell.SOLID : Cell.EMPTY;
				}
			}
		}
	}
}
