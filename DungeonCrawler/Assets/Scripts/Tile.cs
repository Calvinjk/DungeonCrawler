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

	public int HeapIndex{
		get{ return HeapIndex; }
		set{ HeapIndex = value; }
	}
		
	public bool ________________;
	public Vector2Int location;
	public TileState curTileState = TileState.Open;
	public int pathWeight = 0;

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