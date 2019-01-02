using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMapGenerator : MonoBehaviour {

	public const int defaultXDimension = 100;  // Default size of the dungeon floor in the X direction
	public const int defaultZDimension = 100;  // Default size of the dungeon floor in the Z direction

	public GameObject floorTile;	// Prefab of a floor tile model to generate.  

	public Material darkTexture;	
	public Material lightTexture;

	public bool _____________________;

	// This function will generate a map given some dimensions
	public Map GenerateMap(int xSize = defaultXDimension, int ySize = defaultZDimension){
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
				tileScript.curTileState = Tile.TileState.Open;

				// This if block checkerboards the textures so we can clearly see tiles.  Mostly for testing purposes.
				if ((i + j) % 2 == 0) {
					curTileObject.GetComponent<Renderer>().material = darkTexture;
				} else {
					curTileObject.GetComponent<Renderer>().material = lightTexture;
				}

				// Add the generated tile to the tiles array and set the object's parent to the map GameObject
				tiles[i, j] = tileScript;
				curTileObject.transform.SetParent(map.transform);
			}
		} 

		mapScript.tileMap = tiles;
		return mapScript;
	}
}
