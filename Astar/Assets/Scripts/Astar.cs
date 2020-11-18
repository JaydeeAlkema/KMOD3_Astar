using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
	Node[,] nodeGrid;
	int gridSizeX = 0;
	int gridSizeY = 0;
	private bool nodeGridCreated = false;

	/// <summary>
	/// Creates a grid from nodes. These nodes are essentially an overlay of the existing cell grid.
	/// </summary>
	/// <param name="endPos"> The end position of the node (a.k.a. target node). </param>
	/// <param name="grid"> Reference to the cell array. </param>
	private void CreateNodeGrid(Vector2Int endPos, Cell[,] grid)
	{
		gridSizeX = grid.GetLength(0);
		gridSizeY = grid.GetLength(1);

		nodeGrid = new Node[gridSizeX, gridSizeY];

		for(int x = 0; x < gridSizeX; x++)
		{
			for(int y = 0; y < gridSizeY; y++)
			{
				Vector2Int pos = new Vector2Int(x, y);
				Node node = new Node
				{
					position = pos,
					GScore = int.MaxValue,
					HScore = GetDistance(pos, endPos),
					parent = null
				};

				nodeGrid[x, y] = node;
			}
		}

		nodeGridCreated = true;
	}

	/// <summary>
	/// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
	/// Note that you will probably need to add some helper functions
	/// from the startPos to the endPos
	/// </summary>
	/// <param name="startPos"></param>
	/// <param name="endPos"></param>
	/// <param name="grid"></param>
	/// <returns></returns>
	public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
	{
		if(!nodeGridCreated) CreateNodeGrid(endPos, grid);

		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();

		Node startNode = nodeGrid[startPos.x, startPos.y];
		Node targetNode = nodeGrid[endPos.x, endPos.y];

		openList.Add(startNode);

		while(openList.Count > 0)
		{
			Node currentNode = openList[0];
			for(int i = 1; i < openList.Count; i++)
			{
				if(openList[i].FScore < currentNode.FScore || openList[i].FScore == currentNode.FScore && openList[i].HScore < currentNode.HScore)
				{
					currentNode = openList[i];
				}
			}

			openList.Remove(currentNode);
			closedList.Add(currentNode);

			if(currentNode == targetNode)
			{
				return RetracePath(startNode.position, targetNode.position);
			}

			foreach(Node neighbour in GetNeighbours(currentNode.position, grid))
			{
				if(closedList.Contains(neighbour))
				{
					continue;
				}

				int newMovementCostToNeighbour = currentNode.GScore + GetDistance(currentNode.position, neighbour.position);
				if(newMovementCostToNeighbour < neighbour.GScore || !openList.Contains(neighbour))
				{
					neighbour.GScore = newMovementCostToNeighbour;
					neighbour.parent = currentNode;

					if(!openList.Contains(neighbour))
					{
						openList.Add(neighbour);
					}
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Retraces the path from the end node, to the start node.
	/// This path is reversed before return.
	/// </summary>
	/// <param name="startPos"> From where we start tracing the path. </param>
	/// <param name="endPos"> Where the path end is. </param>
	/// <returns></returns>
	private List<Vector2Int> RetracePath(Vector2Int startPos, Vector2Int endPos)
	{
		List<Vector2Int> path = new List<Vector2Int>();
		Node currentNode = nodeGrid[endPos.x, endPos.y];

		while(currentNode != nodeGrid[startPos.x, startPos.y])
		{
			path.Add(currentNode.position);
			currentNode = currentNode.parent;
		}

		path.Reverse();
		return path;
	}

	/// <summary>
	/// Get the distance between nodeA and nodeB.
	/// </summary>
	/// <param name="nodeA"> Position of Node A. </param>
	/// <param name="nodeB"> Position of Node B. </param>
	/// <returns></returns>
	private int GetDistance(Vector2Int nodeA, Vector2Int nodeB)
	{
		int dstX = Mathf.Abs(nodeA.x - nodeB.x);
		int dstY = Mathf.Abs(nodeA.y - nodeB.y);

		if(dstX > dstY)
		{
			return 14 * dstY + 10 * (dstX - dstY);
		}
		else
		{
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}

	/// <summary>
	/// Get all neighbours of the currentnode.
	/// Check for walls.
	/// </summary>
	/// <param name="currentNode"> Position of the current node. </param>
	/// <param name="grid"> Reference to the Cell Grid. </param>
	/// <returns></returns>
	private List<Node> GetNeighbours(Vector2Int currentNode, Cell[,] grid)
	{
		List<Node> neighbours = new List<Node>();
		Cell cell = grid[currentNode.x, currentNode.y];

		if(!cell.HasWall(Wall.UP))
		{
			neighbours.Add(nodeGrid[currentNode.x, currentNode.y + 1]);
		}
		if(!cell.HasWall(Wall.DOWN))
		{
			neighbours.Add(nodeGrid[currentNode.x, currentNode.y - 1]);
		}
		if(!cell.HasWall(Wall.LEFT))
		{
			neighbours.Add(nodeGrid[currentNode.x - 1, currentNode.y]);
		}
		if(!cell.HasWall(Wall.RIGHT))
		{
			neighbours.Add(nodeGrid[currentNode.x + 1, currentNode.y]);
		}

		return neighbours;
	}

	/// <summary>
	/// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
	/// </summary>
	public class Node
	{
		public Vector2Int position; //Position on the grid
		public Node parent; //Parent Node of this node

		public int FScore
		{ //GScore + HScore
			get { return GScore + HScore; }
		}
		public int GScore; //Current Travelled Distance
		public int HScore; //Distance estimated based on Heuristic

		public Node() { }
		public Node(Vector2Int position, Node parent, int GScore, int HScore)
		{
			this.position = position;
			this.parent = parent;
			this.GScore = GScore;
			this.HScore = HScore;
		}
	}
}
