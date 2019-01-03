using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All generated rooms will be an odd number of tiles in dimensions to allow for a central door
public class DungeonMapGenerator : MonoBehaviour {
	public GameObject floorTile;	// Prefab of a floor tile model to generate.  

	// Generation variables to mess with
	[Tooltip("Toggles debug logs for map generation")]
	public bool generationDebugLogs = true;

	[Tooltip("Smallest dimension a room can have.  Lower than 3 can generate a room that has no walkable space")]
	public int minRoomDiameter = 3;
	[Tooltip("Largest dimension a room can have")]
	public int maxRoomDiameter = 20;

	[Tooltip("Number of failed attempts to place a major feature until generator believes it is done")]
	public int maxAttempts = 100;

	public bool _____________________;

	public const int XDIMDEFAULT = 100;  // Default size of the dungeon floor in the X direction
	public const int YDIMDEFAULT = 100;  // Default size of the dungeon floor in the Z direction

	public int curRoomAttempts = 0;

	Tile[,] tiles;

	//.N.E.
	//..O..
	//.W.S.
	enum Direction{
		North,	// +z(y)
		South,  // -z(y)
		East, 	// +x
		West	// -x
	}

	// This function will generate a map given some dimensions
	public Map GenerateMap(int xSize = XDIMDEFAULT, int ySize = YDIMDEFAULT){
		// Create an empty parent object for the map
		GameObject map = new GameObject("Map");	
		map.transform.position = new Vector3(0f, 0f, 0f);
		Map mapScript = map.AddComponent<Map> ();
		mapScript.SetMapSize (xSize, ySize);

		tiles = new Tile[xSize, ySize];

		for (int i = 0; i < xSize; ++i) {
			for (int j = 0; j < ySize; ++j) {

				// Create a tile and give it a name based on its location
				GameObject curTileObject = Instantiate (floorTile, new Vector3(i, 0, j), Quaternion.identity) as GameObject;
				curTileObject.name = "(" + i + ", " + j + ")";

				// When instantiating a Tile, attach a Tile script to it and set variables
				Tile tileScript = curTileObject.AddComponent<Tile>();
				tileScript.location = new Vector2Int (i, j);
				tileScript.curTileState = Tile.TileState.Ungenerated;
				curTileObject.GetComponent<Renderer> ().material.color = Color.yellow;	// Testing visual for an ungenerated tile

				// Add the generated tile to the tiles array and set the object's parent to the map GameObject
				tiles[i, j] = tileScript;
				curTileObject.transform.SetParent(map.transform);
			}
		} 

		// Now that all the tiles are created, assign them
		mapScript.tileMap = tiles;

		// Create the initial room in the center (ish)
		int xLength = Random.Range(minRoomDiameter, maxRoomDiameter + 1) / 2;
		int yLength = Random.Range (minRoomDiameter, maxRoomDiameter + 1);
		GenerateRoom((xSize / 2) - (xLength / 2), (ySize / 2) - (yLength / 2), xLength, yLength, Direction.North);

		// TODO - Keep generating stuff!

		return mapScript;
	}

	// xLocation, yLocation: 	Coordinates to a door of this room.
	// xLength: 				How far perpendicular to the door normal the room extends from the door tile
	// yLength:					How far from the door the room extends in depth towards direction
	// Direction:				In what direction the room extends (in the y direction)
	void GenerateRoom(int xLocation, int yLocation, int xLength, int yLength, Direction direction){
		if (generationDebugLogs){ 
			Debug.Log ("Attempting to create room at (" + xLocation + "," + yLocation + ")...\n" +
				"Dimensions of " + (xLength * 2 + 1) + "x" + yLength + "...\n" +
				"Direction: " + direction);
		}

		// Check if we can fit a room here
		if (CheckTiles (xLocation, yLocation, xLength, yLength, direction)) {
			// Good, lets generate it!
			for (int i = -xLength; i <= xLength; ++i) {
				for (int j = 0; j < yLength; ++j) {
					Tile curTile;

					switch (direction) {
					case Direction.North:
						curTile = tiles [xLocation + i, yLocation + j];
						if (Mathf.Abs (i) == xLength || j == 0 || j == (yLength - 1)) {
							curTile.curTileState = Tile.TileState.Wall;
							curTile.gameObject.GetComponent<Renderer> ().material.color = Color.grey;
						} else {
							curTile.curTileState = Tile.TileState.Open;
							curTile.gameObject.GetComponent<Renderer> ().material.color = Color.white;
						}
						break;
					case Direction.South:

						break;
					case Direction.East:

						break;
					case Direction.West:

						break;
					}
				}
			}
		} else {
			++curRoomAttempts;
		}
	}

	bool CheckTiles(int xLocation, int yLocation, int xLength, int yLength, Direction direction){
		if (generationDebugLogs) { Debug.Log ("Checking attempted room placement..."); }

		/*
		This code will split for each of the 4 directions.  Therefore, in order to not duplicate
		each of my comments 4 times, I will write the general logic up here and only comment the north direction:

		1: 		Iterate through each tile that would be involved in the room we are making, walls included.
		2: 		If even one tile that we check is NOT a ungenerated tile, return false as it overlaps with an existing room
		2.1: 	If we find an existing wall tile in a place we WOULD put a wall tile, allow it as we are ok with "shared" walls
		2.2:	If we find an existing wall tile in a place we WOULD NOT put a wall, return false
		3: 		If the loop makes it all the way through without returning false it has checked every tile!
		*/
		for (int i = -xLength; i <= xLength; ++i) {
			for (int j = 0; j < yLength; ++j) {
				switch (direction) {
				case Direction.North:
					// Check if this tile is ungenerated
					if (tiles[xLocation + i, yLocation + j].curTileState != Tile.TileState.Ungenerated){
						// If this tile as already been generated, check if it is a wall
						if (tiles [xLocation +i, yLocation + j].curTileState != Tile.TileState.Wall) {

							// If the tile is no a wall, we have overlap and should return false
							if (generationDebugLogs) {
								Debug.Log ("Result: False");
							}
							return false;
						} else {
							// If we have reached this point, we found a wall.  Now check if our current room would also put a wall there
							if (Mathf.Abs(i) == xLength || j == 0 || j == (yLength - 1)){
								// Cool, this would also be a wall!  Allow it!
							} else {
								// Bad.  We wouldnt put a wall here.  That means something will be overlapping!
								if (generationDebugLogs) {
									Debug.Log ("Result: False");
								}
								return false;
							}
						}
					}
					break;
				case Direction.South:
					if (tiles [xLocation + i, yLocation - j].curTileState != Tile.TileState.Ungenerated) {
						if (tiles [xLocation + i, yLocation - j].curTileState != Tile.TileState.Wall) {
							if (generationDebugLogs) {
								Debug.Log ("Result: False");
							}
							return false;
						} else {
							if (Mathf.Abs(i) == xLength || j == 0 || j == (yLength - 1)){
							} else {
								if (generationDebugLogs) {
									Debug.Log ("Result: False");
								}
								return false;
							}
						}
					}
					break;
				case Direction.East:
					if (tiles [yLocation + j, xLocation + i].curTileState != Tile.TileState.Ungenerated) {
						if (tiles [yLocation + j, xLocation + i].curTileState != Tile.TileState.Wall) {
							if (generationDebugLogs) {
								Debug.Log ("Result: False");
							}	
							return false;
						} else {
							if (Mathf.Abs(i) == xLength || j == 0 || j == (yLength - 1)){
							} else {
								if (generationDebugLogs) {
									Debug.Log ("Result: False");
								}
								return false;
							}
						}
					}
					break;

				case Direction.West:
					if (tiles [yLocation - j, xLocation + i].curTileState != Tile.TileState.Ungenerated) {
						if (tiles [yLocation - j, xLocation + i].curTileState != Tile.TileState.Wall) {
							if (generationDebugLogs) {
								Debug.Log ("Result: False");
							}
							return false;
						} else {
							if (Mathf.Abs(i) == xLength || j == 0 || j == (yLength - 1)){
							} else {
								if (generationDebugLogs) {
									Debug.Log ("Result: False");
								}
								return false;
							}
						}	
					}
					break;
				}
			}
		}

		if (generationDebugLogs) { Debug.Log ("Result: True"); }
		return true;
	}
}