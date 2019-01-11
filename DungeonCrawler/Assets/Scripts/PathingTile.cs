using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingTile : IHeapItem<PathingTile> {

	public Vector2Int location;
	public PathingTile parent;

	public int pathWeight = 0;
	public int heapIndex = 0;
	public int HeapIndex{
		get{ return heapIndex; }
		set{ heapIndex = value; }
	}

	// We only need a constructor to make a PathingTile based on an existing Tile
	public PathingTile(Tile tile){
		location = tile.location;
	}

	public int CompareTo(PathingTile tileToCompare){
		int compare = pathWeight.CompareTo (tileToCompare.pathWeight);
		if (compare == 0) {
			// TODO - Insert tiebreaker code here
		} 
		return -compare;
	}
}
