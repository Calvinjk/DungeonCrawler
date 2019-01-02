using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMapGenerator : MonoBehaviour {

	public const int XDIMDEFAULT = 100;  // Default size of the dungeon floor in the X direction
	public const int YDIMDEFAULT = 100;  // Default size of the dungeon floor in the Z direction

	public GameObject floorTile;	// Prefab of a floor tile model to generate.  

	public Material darkTexture;	
	public Material lightTexture;

	public bool _____________________;

	// This function will generate a map given some dimensions
	public Map GenerateMap(int xSize = XDIMDEFAULT, int ySize = YDIMDEFAULT){
		// Create an empty parent object for the map
		GameObject map = new GameObject("Map");	
		map.transform.position = new Vector3(0f, 0f, 0f);
		Map mapScript = map.AddComponent<Map> ();
		mapScript.SetMapSize (xSize, ySize);

		Tile[,] tiles = new Tile[xSize, ySize];

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

		return mapScript;
	}
}
