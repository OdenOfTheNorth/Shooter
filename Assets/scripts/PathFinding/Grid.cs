using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour 
{
	public enum GridDirection
	{
		XZ,
		XY,
		ZY
	}

	public GridDirection Direction;
	private GridDirection OldDirection;
	
	public bool displayGridGizmos;
	[Range(0, 1)] public float alpth = 0.2f;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	private float oldNodeRadius;
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;
	int oldGridSizeX, oldGridSizeY;

	void Awake() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	private void Update()
	{
		if (OldDirection != Direction || oldGridSizeX != gridSizeX || oldGridSizeY != gridSizeY || nodeRadius != oldNodeRadius)
		{
			CreateGrid();
		}
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid() {
		grid = new Node[gridSizeX,gridSizeY];

		Vector3 worldBottomLeft = Vector3.zero;
		Vector3 FirstDir = Vector3.zero;
		Vector3 SecondDir = Vector3.zero;

		switch (Direction)
		{
			case GridDirection.XZ:
			worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
			FirstDir = Vector3.right;
			SecondDir = Vector3.forward;
				break;
			case GridDirection.XY:
			worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.up * gridWorldSize.y/2;
			FirstDir = Vector3.right;
			SecondDir = Vector3.up;
				break;
			case GridDirection.ZY:
			worldBottomLeft = transform.position - Vector3.forward * gridWorldSize.x/2 - Vector3.up * gridWorldSize.y/2;
			FirstDir = Vector3.forward;
			SecondDir = Vector3.up;	
				break;
		}

		OldDirection = Direction;
		oldGridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		oldGridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		oldNodeRadius = nodeRadius;
		nodeDiameter = nodeRadius*2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + FirstDir * (x * nodeDiameter + nodeRadius) + SecondDir * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				int gridIndex = (gridSizeX * x) + y;
				grid[x,y] = new Node(walkable,worldPoint, x,y, 0 , gridIndex);
			}
		}
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	

	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = 0;
		float percentY = 0;
		
		switch (Direction)
		{
			case GridDirection.XZ:
				percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
				percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
				break;
			case GridDirection.XY:
				percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
				percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
				break;
			case GridDirection.ZY:
				percentX = (worldPosition.z + gridWorldSize.x/2) / gridWorldSize.x;
				percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
				break;
			default:
				break;
		}

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}
	
	public int NodeIndexFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = 0;
		float percentY = 0;
		
		switch (Direction)
		{
			case GridDirection.XZ:
				percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
				percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
				break;
			case GridDirection.XY:
				percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
				percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
				break;
			case GridDirection.ZY:
				percentX = (worldPosition.z + gridWorldSize.x/2) / gridWorldSize.x;
				percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
				break;
			default:
				break;
		}

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return x * y;
	}
	
	void OnDrawGizmos() {
		
		if (OldDirection != Direction || oldGridSizeX != gridSizeX || oldGridSizeY != gridSizeY || nodeRadius != oldNodeRadius)
		{
			CreateGrid();
		}
		
		//Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid)
			{
				Color white = new Color(1, 1, 1, alpth);
				Color red = new Color(1, 0, 0, alpth);
				
				Gizmos.color = (n.walkable) ? white : red;
				Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
}