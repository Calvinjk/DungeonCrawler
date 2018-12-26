using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
	public Tile[,] tileMap;
	public int xSize = 10;
	public int ySize = 10;

	public List<Tile> GetNeighbors(Tile tile){
		List<Tile> neighbors = new List<Tile> ();

		for (int i = -1; i <= 1; ++i) {
			for (int j = -1; j <= 1; ++j) {
				if (i == 0 && j == 0) { continue; } // No need to check the original tile

				int checkX = tile.location.x + i;
				int checkY = tile.location.y + j;

				if (checkX >= 0 && checkX < xSize && checkY >= 0 && checkY < ySize) {
					neighbors.Add (tileMap [checkX, checkY]);
				}
			}
		}
		return neighbors;
	}

	public List<Tile> FindPath(Tile startTile, Tile targetTile){
		// Create the two containers necessary for the pathing algorithm
		Heap<Tile> openSet = new Heap<Tile> (xSize * ySize);
		HashSet<Tile> closedSet = new HashSet<Tile> ();

		// Start off by adding the current location to the openSet
		startTile.pathWeight = 0;
		openSet.Add (startTile);

		// Loop until the openSet is empty or a path has been found
		while (openSet.Count > 0) {
			// Due to our beautiful heap, currentTile is the tile with the current shortest path
			Tile currentTile = openSet.RemoveFirst();
			closedSet.Add (currentTile);

			// Wow we found the path!  killer.
			if (currentTile == targetTile) {
				return RetracePath(startTile, targetTile);
			}

			foreach (Tile neighbor in GetNeighbors(currentTile)) {
				// If we cant walk on it or if we have already checked it, dont add it again to the openSet
				if (neighbor.curTileState == Tile.TileState.Enemy || neighbor.curTileState == Tile.TileState.Obstructed || closedSet.Contains(neighbor)) {
					continue;
				}

				int newCostToNeighbor = currentTile.pathWeight + GetTravelCost (currentTile, neighbor);
				if (newCostToNeighbor < neighbor.pathWeight || !openSet.Contains (neighbor)) {
					neighbor.pathWeight = newCostToNeighbor;
					neighbor.parent = currentTile;

					if (!openSet.Contains (neighbor)) {
						openSet.Add (neighbor);
					}
				}
			}
		}
		// If we hit this point in the code, no path was found to the target tile
		Debug.Log("No path found");
		return null;
	}

	List<Tile> RetracePath(Tile start, Tile end){
		List<Tile> path = new List<Tile> ();
		Tile currentTile = end;

		// Trace backwards and add all the tiles to path
		while (currentTile != start) {
			path.Add (currentTile);
			currentTile = currentTile.parent;
		}

		// Now path is actually the reversed version of how we want to move, so fix that
		path.Reverse();
		return path;
	}

	int GetTravelCost(Tile tileA, Tile tileB){
		int xDiff = Mathf.Abs (tileA.location.x - tileB.location.x);
		int yDiff = Mathf.Abs (tileA.location.y - tileB.location.y);

		if (xDiff == 1 && yDiff == 1) { //Diagonal
			return 2;
		} else {	// Not diagonal
			return 1;
		}
	}
}
