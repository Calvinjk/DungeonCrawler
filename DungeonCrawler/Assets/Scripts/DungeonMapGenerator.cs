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

	void Start(){
		//This line is for testing purposes only
		//GenerateMap(DungeonType.Square);
	}

	// Default map generator uses default values for dimensions and height
	public GameObject[,] GenerateMap(DungeonType layout){
		// Create an empty parent object for the map
		GameObject map = new GameObject("Map");	
		map.transform.position = new Vector3(0f, 0f, 0f);

		// Switch case based on the layout of map desired
		switch (layout){
		// Square layouts will have equal dimensions in the X and Z directions
		case (DungeonType.Square):
			return GenerateSquareMap (map);
		default:  // If the code reaches this default block, something went wrong and we should show an error while returning an empty map
			Debug.LogError ("Invalid DungeonType passed into map generator");
			return new GameObject[0, 0];
		}
	}

	// This function will generate a square map given a parent object to put all the pieces under
	GameObject[,] GenerateSquareMap(GameObject map){
		GameObject[,] tiles = new GameObject[maxXDimension, maxXDimension];

		// I will use maxXDimension here exclusively due to both dimensions being equal on a square map
		for (int i = 0; i < maxXDimension; ++i) {
			for (int j = 0; j < maxXDimension; ++j) {

				// Create a tile and give it a name based on its location
				GameObject curTile = Instantiate (floorTile, new Vector3(i, 0, j), Quaternion.identity) as GameObject;
				curTile.name = "(" + i + ", 0, " + j + ")";

				// This if block checkerboards the textures so we can clearly see tiles.  Mostly for testing purposes.
				if ((i + j) % 2 == 0) {
					curTile.GetComponent<Renderer>().material = darkTexture;
				} else {
					curTile.GetComponent<Renderer>().material = lightTexture;
				}

				// Add the generated tile to the tiles array and set its parent to the passed-in parameter.
				tiles[i, j] = curTile;
				curTile.transform.SetParent(map.transform);
			}
		} 
			
		return tiles;
	}
}
