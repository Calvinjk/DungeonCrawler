using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The GameManager tracks and stores gamedata other classes may need to access
public class GameManager : MonoBehaviour {

	public enum MovementType{ Grid, Free };
	public enum GameState{ 
		InputLocked, 
		AwaitingInput, 
		InputStalling 
	};

	public float stallTime = 0.5f;
	public Material overlayMaterial;

	public bool _______________;

	public float curStallTimer = 0f;
	public GameState curGameState = GameState.AwaitingInput;
	public Map map;
	public Player curSelectedPlayer = null;

	public CameraController cameraScript;
	public DungeonMapGenerator generatorScript;

	public HashSet<Tile.TileState> nonWalkableTiles;
	public HashSet<Tile.TileState> passThroughOnlyTiles;

	void Start(){
		// Define which set of tiles cannot be stepped on
		nonWalkableTiles = new HashSet<Tile.TileState> ();
		nonWalkableTiles.Add (Tile.TileState.Wall);
		nonWalkableTiles.Add (Tile.TileState.Enemy);
		nonWalkableTiles.Add (Tile.TileState.Ungenerated);
		nonWalkableTiles.Add (Tile.TileState.Obstructed);

		// Define which set of tiles can be pathed through, but not ended on
		passThroughOnlyTiles = new HashSet<Tile.TileState>();
		passThroughOnlyTiles.Add (Tile.TileState.Ally);

		// Start the game off by generating a map
		map = generatorScript.GenerateMap(50, 50);
	}

	void Update(){
		// Deselect the currently selected player upon esc keypress
		if (Input.GetKeyDown(KeyCode.Escape) && curSelectedPlayer) {
			DeselectCurrentPlayer ();
		}

		if (curGameState == GameState.InputStalling) {
			curStallTimer += Time.deltaTime;
			if (curStallTimer > stallTime) {
				curGameState = GameState.AwaitingInput;
				curStallTimer = 0f;
			}
		}
	}

	void DeselectCurrentPlayer(){
		curSelectedPlayer.UpdateSelectedPlayer (false);
		cameraScript.removeCameraTarget ();
		curGameState = GameState.AwaitingInput;
	}
}