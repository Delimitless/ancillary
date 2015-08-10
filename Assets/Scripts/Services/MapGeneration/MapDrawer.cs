using UnityEngine;

public class MapDrawer : MonoBehaviour {

	public int width;
	public int height;
	[Range(0,100)] 
	public int randomFillPercent;
	public int borderSize;
	public float squareSize;
	
	CellularAutomator cellularAutomator;
	MapMeshGenerator meshGenerator;
	Map map;

	void Start() {
		cellularAutomator = GetComponent<CellularAutomator>();
		meshGenerator = GetComponent<MapMeshGenerator>();

		DrawMap();
	}
	
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			DrawMap();
		}
	}

	void DrawMap() {
		map = MapGenerator.GenerateMap(width, height, borderSize, randomFillPercent, cellularAutomator);

		GetComponent<MeshFilter>().mesh = meshGenerator.GenerateMesh(map, squareSize);
	}
}
