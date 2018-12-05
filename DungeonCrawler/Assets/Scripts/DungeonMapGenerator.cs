﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMapGenerator : MonoBehaviour {

	public int maxXDimension = 10;  // Max size of the dungeon floor in the X direction
	public int maxZDimension = 10;  // Max size of the dungeon floor in the Z direction
	public int maxHeight = 1;       // Max height of the stackable floor tiles

	public GameObject floorTile;

	public bool _____________________;

	public enum DungeonType {Square};

	void Start(){
		//This line is for testing purposes only
		//GenerateMap(DungeonType.Square);
	}

	// Default map generator uses default values for dimensions and height
	public GameObject[,] GenerateMap(DungeonType layout){
		switch (layout){
		// Square layouts will have equal dimensions in the X and Z directions
		case (DungeonType.Square):
			return GenerateSquareMap ();
		default:
			Debug.LogError ("Invalid DungeonType passed into map generator");
			return new GameObject[0, 0];
		}
	}

	GameObject[,] GenerateSquareMap(){
		GameObject[,] tiles = new GameObject[maxXDimension, maxXDimension];

		// I will use maxXDimension here exclusively due to both dimensions being equal on a square map
		for (int i = 0; i < maxXDimension; ++i) {
			for (int j = 0; j < maxXDimension; ++j) {
				GameObject curTile = Instantiate (floorTile, new Vector3 (i, 0, j), Quaternion.identity) as GameObject;
				curTile.name = "(" + i + ", 0, " + j + ")";
				tiles[i, j] = curTile;
			}
		} 

		return tiles;
	}
}
