
public class Map {

	int width;
	int height;

	Cell[,] cells;

	public Map(int width, int height) {
		this.width = width;
		this.height = height;

		cells = new Cell[width, height];
	}

	public int Width {
		get {
			return width;
		}
	}

	public int Height {
		get {
			return height;
		}
	}

	public Cell[,] Cells {
		get {
			return cells;
		}
	}

	public bool IsWithinBounds(int x, int y) {
		return (x >= 0 && x < width && y >= 0 && y < height);
	}
}
