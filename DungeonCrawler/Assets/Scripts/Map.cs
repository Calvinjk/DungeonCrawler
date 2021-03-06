﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
	public bool debugLogs = false;
	public Tile[,] tileMap;
	public int xSize = 10;
	public int ySize = 10;
	public float overlayVerticalOffset = 0.051f;

	public bool ______________;

	// TODO - Sterilize inputs here
	public void SetMapSize(int x, int y){
		xSize = x;
		ySize = y;
	}

	public List<Tile> FindPath(Tile startTile, Tile targetTile, int maxDistance, HashSet<Tile.TileState> nonWalkableTiles){
		// Create the two containers necessary for the pathing algorithm
		Heap<PathingTile> openSet = new Heap<PathingTile> (xSize * ySize);
		HashSet<PathingTile> closedSet = new HashSet<PathingTile> ();

		// Create PathingTile equivalent of the startTile
		PathingTile startPath = new PathingTile(startTile);

		// Start off by adding the starting location to the openSet
		openSet.Add (startPath);

		// Loop until the openSet is empty or a path has been found
		while (openSet.Count > 0) {
			// Due to our beautiful heap, currentTile is the tile with the current shortest path
			PathingTile currentTile = openSet.RemoveFirst();
			closedSet.Add (currentTile);

			// Wow we found the path!  killer.
			if (currentTile.location == targetTile.location) {
				return RetracePath(startPath, currentTile);
			}
				
			foreach (Tile neighbor in GetNeighbors(tileMap [currentTile.location.x, currentTile.location.y])) {
				PathingTile neighborTile = new PathingTile (neighbor);

				// If we cant walk on it or if we have already checked it, dont add it again to the openSet
				if (nonWalkableTiles.Contains(neighbor.curTileState) || closedSet.Contains(neighborTile)) {
					continue;
				}

				int newCostToNeighbor = currentTile.pathWeight + GetTravelCost (tileMap [currentTile.location.x, currentTile.location.y], neighbor);
				if (newCostToNeighbor <= maxDistance && (newCostToNeighbor < neighborTile.pathWeight || !openSet.Contains (neighborTile))) {
					neighborTile.pathWeight = newCostToNeighbor;
					neighborTile.parent = currentTile;
					openSet.Add (neighborTile);
				}
			}
		}
		// If we hit this point in the code, no path was found to the target tile
		if (debugLogs) { Debug.Log ("No path found"); }
		return null;
	}

	List<Tile> RetracePath(PathingTile start, PathingTile end){
		List<Tile> path = new List<Tile> ();
		PathingTile currentTile = end;

		// Trace backwards and add all the tiles to path
		while (currentTile != start) {
			path.Add (tileMap[currentTile.location.x, currentTile.location.y]);
			currentTile = currentTile.parent;
		}

		// Now path is actually the reversed version of how we want to move, so fix that
		path.Reverse();
		return path;
	}

	int GetTravelCost(Tile tileA, Tile tileB){
		int xDiff = Mathf.Abs (tileA.location.x - tileB.location.x);
		int yDiff = Mathf.Abs (tileA.location.y - tileB.location.y);

		return xDiff + yDiff;
	}

	// Returns true if the location is in the map.   False otherwise.
	// NOTE: Does NOT check if it is a walkable square within generated map
	public bool IsWithinMapBounds(int x, int y){
		return x >= 0 && x < xSize && y >= 0 && y < ySize;
	}

	public List<Tile> GetNeighbors(Tile tile){
		List<Tile> neighbors = new List<Tile> ();

		for (int i = -1; i <= 1; ++i) {
			for (int j = -1; j <= 1; ++j) {
				if (i == 0 && j == 0) { continue; } // No need to check the original tile

				int checkX = tile.location.x + i;
				int checkY = tile.location.y + j;

				if (IsWithinMapBounds(checkX, checkY)) {
					neighbors.Add (tileMap [checkX, checkY]);
				}
			}
		}
		return neighbors;
	}

	public GameObject HighlightMovementRange(Tile center, int moveSpeed, HashSet<Tile.TileState> nonWalkableTiles, HashSet<Tile.TileState> passThroughOnlyTiles){
		GameObject movementOverlay = new GameObject ("MovementOverlay");
		// Cycle through every tile that might be in our range
		for (int i = -moveSpeed; i <= moveSpeed; ++i) {
			for (int j = -moveSpeed; j <= moveSpeed; ++j) {
				// If it is actually outside of our range, disregard it
				if ((Mathf.Abs (i) + Mathf.Abs (j)) > moveSpeed) { continue; }
				// If it would be outside the current map, disregard it
				if (!IsWithinMapBounds(center.location.x + i, center.location.y + j)) { continue; }


				Tile curTarget = tileMap [center.location.x + i, center.location.y + j];

				// Check if the tile is walkable and we can get to it
				if (!nonWalkableTiles.Contains(curTarget.curTileState) 
					&& !passThroughOnlyTiles.Contains(curTarget.curTileState)
					&& FindPath(center, curTarget, moveSpeed, nonWalkableTiles) != null){

					// Highlight that sucker!
					GameObject movementOverlayTile = Instantiate(Resources.Load("Prefabs/MovementOverlay") as GameObject);
					movementOverlayTile.name = "MoveOverlay(" + curTarget.location.x + "," + curTarget.location.y + ")";
					movementOverlayTile.transform.position = new Vector3 (curTarget.location.x, overlayVerticalOffset, curTarget.location.y);
					movementOverlayTile.transform.SetParent (movementOverlay.transform);
				}
			}
		}
		return movementOverlay;
	}

	public void UpdateSelectedOverlayTile(Tile prev, Tile next){
		GameObject overlayTile = GameObject.Find ("MoveOverlay(" + prev.location.x + "," + prev.location.y + ")");
		overlayTile.GetComponent<Renderer> ().material = Resources.Load ("Materials/MovementOverlayMaterial") as Material;

		overlayTile = GameObject.Find ("MoveOverlay(" + next.location.x + "," + next.location.y + ")");
		overlayTile.GetComponent<Renderer> ().material = Resources.Load ("Materials/MovementOverlayConfirmMaterial") as Material;
	}
}