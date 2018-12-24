using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
	public Tile[,] tileMap;
	public int xSize = 10;
	public int ySize = 10;

	public List<Tile> GetNeighbors(Tile tile){
		List<Tile> neighbors = new List<Tile> ();

		for (int i = -1; i <= -1; ++i) {
			for (int j = -1; j <= -1; ++j) {
				if (i == 0 && j == 0) { continue; } // No need to check the original tile

				int checkX = tile.location.x + i;
				int checkY = tile.location.y + j;

				if (checkX >= 0 && checkX < xSize && checkY >= 0 && checkY < ySize) {
					neighbors.Add (tileMap [checkX, checkY]);
				}
			}
		}
		return neighbors;
	}
}
