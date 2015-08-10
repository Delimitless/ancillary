using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public static Map GenerateMap(int width, int height, int randomFillPercent, CellularAutomator cellularAutomator) {

		Map map = new Map(width, height);

		RandomFillMap(map, randomFillPercent);

		cellularAutomator.CellularAutomate(map);

		return map;
	}
	
	static void RandomFillMap(Map map, int randomFillPercent) {

		string seed = Time.time.ToString();
		System.Random pseudoRandom = new System.Random(seed.GetHashCode());
		
		for (int x = 0; x < map.Width; x ++) {
			for (int y = 0; y < map.Height; y ++) {
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
