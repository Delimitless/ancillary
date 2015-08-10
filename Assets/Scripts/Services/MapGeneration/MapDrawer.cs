using UnityEngine;
using System.Collections;

public class MapDrawer : MonoBehaviour {

	public int width;
	public int height;
	[Range(0,100)] 
	public int randomFillPercent;

	private MapGenerator generator;
	private FillType[,] map;

	void Start() {
		generator = new MapGenerator(width, height, randomFillPercent);
		map = generator.GenerateMap();
	}
	
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			map = generator.GenerateMap();
		}
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
