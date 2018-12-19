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

	public Vector2Int location;
	public TileState curTileState = TileState.Open;
}