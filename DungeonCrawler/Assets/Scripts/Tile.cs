using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : IHeapItem<Tile> {

	public enum TileState {
		Open,
		Obstructed,
		Ally,
		Enemy
	};

	public int heapIndex = 0;
	public int HeapIndex{
		get{ return heapIndex; }
		set{ heapIndex = value; }
	}
		
	public bool ________________;
	public Vector2Int location;
	public TileState curTileState = TileState.Open;

	// Pathfinding variables
	public Tile parent;
	public int pathWeight = 0;

	// Constructors
	public Tile(Vector2Int loc, TileState state = TileState.Open){
		location = loc;
	}

	public void PrintTile(){
		Debug.Log ("TileState: " + curTileState 
			+ ", Location: (" + location.x + ", " + location.y + ")");
	}

	public int CompareTo(Tile tileToCompare){
		int compare = pathWeight.CompareTo (tileToCompare.pathWeight);
		if (compare == 0) {
			// TODO - Insert tiebreaker code here
		} 
		return -compare;
	}
}