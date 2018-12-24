using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMapGenerator : MonoBehaviour {

	public int maxXDimension = 10;  // Max size of the dungeon floor in the X direction
	public int maxZDimension = 10;  // Max size of the dungeon floor in the Z direction
	public int maxHeight = 1;       // Max height of the stackable floor tiles

	public GameObject floorTile;	// Prefab of a floor tile model to generate.  

	public Material darkTexture;	
	public Material lightTexture;

	public bool _____________________;

	public enum DungeonType {Square};

	// Default map generator uses default values for dimensions and height
	public Map GenerateMap(DungeonType layout){
		// Create an empty parent object for the map
		GameObject map = new GameObject("Map");	
		map.transform.position = new Vector3(0f, 0f, 0f);
		map.AddComponent<Map> ();

		// Switch case based on the layout of map desired
		switch (layout){
		// Square layouts will have equal dimensions in the X and Z directions
		case (DungeonType.Square):
			map.GetComponent<Map> ().tileMap = GenerateSquareMap (map);
			return map.GetComponent<Map>();
		default:  // If the code reaches this default block, something went wrong and we should show an error while returning an empty map
			Debug.LogError ("Invalid DungeonType passed into map generator");
			return null;
		}
	}

	// This function will generate a square map given a parent object to put all the pieces under
	Tile[,] GenerateSquareMap(GameObject map){
		Tile[,] tiles = new Tile[maxXDimension, maxXDimension];

		// I will use maxXDimension here exclusively due to both dimensions being equal on a square map
		for (int i = 0; i < maxXDimension; ++i) {
			for (int j = 0; j < maxXDimension; ++j) {

				// Create a tile and give it a name based on its location
				GameObject curTileObject = Instantiate (floorTile, new Vector3(i, 0, j), Quaternion.identity) as GameObject;
				curTileObject.name = "(" + i + ", " + j + ")";

				// When instantiating a Tile, set its location.  Currently we do not need to mess with any of the other member variables, but that may change.
				Tile newTile = new Tile();
				newTile.location = new Vector2Int (i, j);

				// This if block checkerboards the textures so we can clearly see tiles.  Mostly for testing purposes.
				if ((i + j) % 2 == 0) {
					curTileObject.GetComponent<Renderer>().material = darkTexture;
				} else {
					curTileObject.GetComponent<Renderer>().material = lightTexture;
				}

				// Add the generated tile to the tiles array and set the object's parent to the passed-in parameter.
				tiles[i, j] = newTile;
				curTileObject.transform.SetParent(map.transform);
			}
		} 
			
		return tiles;
	}
}
