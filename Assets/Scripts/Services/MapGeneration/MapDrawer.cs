using UnityEngine;

public class MapDrawer : MonoBehaviour {

	public int width;
	public int height;
	[Range(0,100)] 
	public int randomFillPercent;
	public int squareSize;
	
	CellularAutomator cellularAutomator;
	Map map;

	void Start() {
		cellularAutomator = GetComponent<CellularAutomator>();

		DrawMap();
	}
	
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			DrawMap();
		}
	}

	void DrawMap() {
		map = MapGenerator.GenerateMap(width, height, randomFillPercent, cellularAutomator);
		GenerateMesh();

	}

	void GenerateMesh() {

	}

	void OnDrawGizmos() {
		if (map != null) {
			for (int x = 0; x < map.Width; x ++) {
				for (int y = 0; y < map.Height; y ++) {
					Gizmos.color = (map.Cells[x,y] == Cell.SOLID) ? Color.black : Color.white;
					Vector3 pos = new Vector3(-map.Width/2 + x + .5f,-map.Height/2 + y+.5f, 0);
					Gizmos.DrawCube(pos,Vector3.one);
				}
			}
		}
	}
}
