using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public enum TileState {
		Open,
		Obstructed,
		Ally,
		Enemy
	};
		
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
}