﻿using UnityEngine;
using System.Collections.Generic;

public class MapMeshGenerator : MonoBehaviour {

	SquareGrid squareGrid;
	List<Vector3> vertices;
	List<int> triangles;

	Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
	List<List<int>> outlines = new List<List<int>>();
	HashSet<int> checkedVertices = new HashSet<int>();

	public Mesh GenerateMesh(Map map, float squareSize) {

		outlines.Clear();
		checkedVertices.Clear();

		squareGrid = new SquareGrid(map, squareSize);
		
		vertices = new List<Vector3>();
		triangles = new List<int>();
		
		for (int x = 0; x < squareGrid.squares.GetLength(0); x ++) {
			for (int y = 0; y < squareGrid.squares.GetLength(1); y ++) {
				TriangulateSquare(squareGrid.squares[x,y]);
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		Generate2DColliders();

		return mesh;
	}

	
	void TriangulateSquare(Square square) {
		switch (square.configuration) {
		case 0:
			break;
			
			// 1 points:
		case 1:
			MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
			break;
		case 2:
			MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
			break;
		case 4:
			MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
			break;
		case 8:
			MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
			break;
			
			// 2 points:
		case 3:
			MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
			break;
		case 6:
			MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
			break;
		case 9:
			MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
			break;
		case 12:
			MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
			break;
		case 5:
			MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
			break;
		case 10:
			MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
			break;
			
			// 3 point:
		case 7:
			MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
			break;
		case 11:
			MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
			break;
		case 13:
			MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
			break;
		case 14:
			MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
			break;
			
			// 4 point:
		case 15:
			MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
			checkedVertices.Add(square.topLeft.vertexIndex);
			checkedVertices.Add(square.topRight.vertexIndex);
			checkedVertices.Add(square.bottomRight.vertexIndex);
			checkedVertices.Add(square.bottomLeft.vertexIndex);
			break;
		}
		
	}
	
	void MeshFromPoints(params Node[] points) {
		AssignVertices(points);
		
		if (points.Length >= 3)
			CreateTriangle(points[0], points[1], points[2]);
		if (points.Length >= 4)
			CreateTriangle(points[0], points[2], points[3]);
		if (points.Length >= 5) 
			CreateTriangle(points[0], points[3], points[4]);
		if (points.Length >= 6)
			CreateTriangle(points[0], points[4], points[5]);
		
	}
	
	void AssignVertices(Node[] points) {
		for (int i = 0; i < points.Length; i ++) {
			if (points[i].vertexIndex == -1) {
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}
	
	void CreateTriangle(Node a, Node b, Node c) {
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);

		Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
		AddTriangleToDictionary(triangle.vertexIndexA, triangle);
		AddTriangleToDictionary(triangle.vertexIndexB, triangle);
		AddTriangleToDictionary(triangle.vertexIndexC, triangle);
	}

	void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle) {
		if (triangleDictionary.ContainsKey(vertexIndexKey)) {
			triangleDictionary[vertexIndexKey].Add (triangle);
		}
		else {
			List<Triangle> triangleList = new List<Triangle>();
			triangleList.Add(triangle);
			triangleDictionary.Add(vertexIndexKey, triangleList);
		}
	}

	void Generate2DColliders() {
		
		EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D> ();
		for (int i = 0; i < currentColliders.Length; i++) {
			Destroy(currentColliders[i]);
		}
		
		CalculateMeshOutlines();
		
		foreach (List<int> outline in outlines) {
			EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
			Vector2[] edgePoints = new Vector2[outline.Count];
			
			for (int i =0; i < outline.Count; i ++) {
				edgePoints[i] = new Vector2(vertices[outline[i]].x,vertices[outline[i]].y);
			}
			edgeCollider.points = edgePoints;
		}
		
	}

	void CalculateMeshOutlines() {

		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
			if (!checkedVertices.Contains(vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);

				if (newOutlineVertex != -1) {
					checkedVertices.Add(vertexIndex);

					List<int> newOutline = new List<int>();
					newOutline.Add(vertexIndex);
					outlines.Add (newOutline);
					FollowOutline(newOutlineVertex, outlines.Count-1);
					outlines[outlines.Count-1].Add(vertexIndex);
				}
			}
		}
	}

	void FollowOutline(int vertexIndex, int outlineIndex) {
		outlines[outlineIndex].Add(vertexIndex);
		checkedVertices.Add(vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

		if (nextVertexIndex != -1) {
			FollowOutline(nextVertexIndex, outlineIndex);
		}
	}

	int GetConnectedOutlineVertex(int vertex) {
		List<Triangle> trianglesContainingVertex = triangleDictionary[vertex];

		for (int i = 0; i < trianglesContainingVertex.Count; i++) {
			Triangle triangle = trianglesContainingVertex [i];

			if (IsOutlineEdge (vertex, triangle.vertexIndexA)) {
				return triangle.vertexIndexA;
			}
			if (IsOutlineEdge (vertex, triangle.vertexIndexB)) {
				return triangle.vertexIndexB;
			}
			if (IsOutlineEdge (vertex, triangle.vertexIndexC)) {
				return triangle.vertexIndexC;
			}
		}

		return -1;
	}

	bool IsOutlineEdge(int vertex, int otherVertex) {

		// No point in comparing vertex to itself, of if it's already been checked.
		if (vertex == otherVertex || checkedVertices.Contains(otherVertex)) {
			return false;
		}

		List<Triangle> trianglesContainingVertex = triangleDictionary[vertex];
		int sharedTriangleCount = 0;

		for (int i = 0; i < trianglesContainingVertex.Count; i++) {
			if (trianglesContainingVertex[i].Contains(otherVertex)) {
				sharedTriangleCount += 1;

				if (sharedTriangleCount > 1) {
					break;
				}
			}
		}

		return sharedTriangleCount == 1;
	}

	struct Triangle {
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;

		public Triangle(int a, int b, int c) {
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;
		}

		public bool Contains(int vertexIndex) {
			return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
		}
	}

	public class SquareGrid {
		public Square[,] squares;
		
		public SquareGrid(Map map, float squareSize) {
			int nodeCountX = map.Cells.GetLength(0);
			int nodeCountY = map.Cells.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;
			
			ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];
			
			for (int x = 0; x < nodeCountX; x ++) {
				for (int y = 0; y < nodeCountY; y ++) {
					Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2, -mapHeight/2 + y * squareSize + squareSize/2, 0);
					controlNodes[x,y] = new ControlNode(pos, map.Cells[x,y] == Cell.SOLID, squareSize);
				}
			}
			
			squares = new Square[nodeCountX -1,nodeCountY -1];
			for (int x = 0; x < nodeCountX-1; x ++) {
				for (int y = 0; y < nodeCountY-1; y ++) {
					squares[x,y] = new Square(controlNodes[x,y+1], controlNodes[x+1,y+1], controlNodes[x+1,y], controlNodes[x,y]);
				}
			}
			
		}
	}
	
	public class Square {
		
		public ControlNode topLeft, topRight, bottomRight, bottomLeft;
		public Node centerTop, centerRight, centerBottom, centerLeft;
		public int configuration;
		
		public Square (ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft) {
			this.topLeft = topLeft;
			this.topRight = topRight;
			this.bottomRight = bottomRight;
			this.bottomLeft = bottomLeft;
			
			centerTop = topLeft.right;
			centerRight = bottomRight.above;
			centerBottom = bottomLeft.right;
			centerLeft = bottomLeft.above;
			
			if (topLeft.active)
				configuration += 8;
			if (topRight.active)
				configuration += 4;
			if (bottomRight.active)
				configuration += 2;
			if (bottomLeft.active)
				configuration += 1;
		}
		
	}
	
	public class Node {
		public Vector3 position;
		public int vertexIndex = -1;
		
		public Node(Vector3 pos) {
			position = pos;
		}
	}
	
	public class ControlNode : Node {
		
		public bool active;
		public Node above, right;
		
		public ControlNode(Vector3 pos, bool active, float squareSize) : base(pos) {
			this.active = active;
			above = new Node(position + Vector3.up * squareSize/2f);
			right = new Node(position + Vector3.right * squareSize/2f);
		}
	}
}
