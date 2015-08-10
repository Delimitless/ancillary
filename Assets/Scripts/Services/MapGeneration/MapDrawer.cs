using UnityEngine;

public class MapDrawer : MonoBehaviour {

	public int width;
	public int height;
	[Range(0,100)] 
	public int randomFillPercent;
	public int squareSize;
	
	CellularAutomator cellularAutomator;
	FillType[,] map;

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
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					Gizmos.color = (map[x,y] == FillType.SOLID)?Color.black:Color.white;
					Vector3 pos = new Vector3(-width/2 + x + .5f,-height/2 + y+.5f, 0);
					Gizmos.DrawCube(pos,Vector3.one);
				}
			}
		}
	}
}
