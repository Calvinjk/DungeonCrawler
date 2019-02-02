using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {
	
	public bool ________________;

	public bool isSelected = false;
	public Tile movementDestination = null;
	public Dictionary<Tile, int> pathDict = new Dictionary<Tile, int>();
	public GameObject movementOverlay;

	void Start(){
		// Set up the connections to the camera and the GameManager
		InitializeConnections ();

		// Define this unit to be of type ally
		unitType = UnitType.Ally;

		// Define which tiles this unit cannot path across
		nonWalkableTiles = new HashSet<Tile.TileState> ();
		nonWalkableTiles.Add (Tile.TileState.Wall);
		nonWalkableTiles.Add (Tile.TileState.Enemy);
		nonWalkableTiles.Add (Tile.TileState.Ungenerated);
		nonWalkableTiles.Add (Tile.TileState.Obstructed);

		// Define which tiles this unit can path across but not end on
		passThroughOnlyTiles = new HashSet<Tile.TileState>();
		passThroughOnlyTiles.Add (Tile.TileState.Ally);
		passThroughOnlyTiles.Add (Tile.TileState.Door);
	}

	// TODO - Delete this public function when we actually SPAWN or CREATE units
	public void SetLocation(Tile location){
		curLocation = location;
	}

	void Update(){
		// Deal with moving the character if selected and not moving
		if (isSelected && !isMoving && Input.GetKeyDown (KeyCode.Mouse0)
		    && (gameManager.curGameState == GameManager.GameState.AwaitingInput)) {
			// Figure out what tile I just clicked on
			RaycastHit hit;

			// Bit shift the layermask to have a 1 in the places that correspond to layers we want ignored
			// Currently layers 9 and 10 (3 = 0011, shifted 9 places left)
			int layerMask = 3 << 9;
			layerMask = ~layerMask; // Invert the layermask so we AVOID the layer specificed

			if (Physics.Raycast (Camera.main.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, layerMask)
			    && hit.transform.gameObject.tag == "Tile") {

				// Now that I know that I hit a tile, figure out what tile it was
				Tile hitTile = gameManager.map.tileMap [(int)hit.transform.position.x, (int)hit.transform.position.z];

				if (movementDestination == null) {	// Need to do confirmation step
					if (!nonWalkableTiles.Contains(hitTile.curTileState)
						&& !passThroughOnlyTiles.Contains(hitTile.curTileState)
						&& gameManager.map.FindPath (curLocation, hitTile, unitSpeed, nonWalkableTiles) != null) { // We are still confirming movement

						movementDestination = hitTile;
						gameManager.map.UpdateSelectedOverlayTile (movementDestination, movementDestination);
					}
				} else if (movementDestination == hitTile) { // Actually move (user clicked same tile)
					MoveToTile (movementDestination);
					UpdateSelectedPlayer (false);
				} else { // User clicked on a different tile
					if (!nonWalkableTiles.Contains(hitTile.curTileState)
						&& !passThroughOnlyTiles.Contains(hitTile.curTileState)
						&& gameManager.map.FindPath (curLocation, movementDestination, unitSpeed, nonWalkableTiles) != null) {

						gameManager.map.UpdateSelectedOverlayTile (movementDestination, hitTile);
						movementDestination = hitTile;
					}
				}
			}
		}
	}

	// If this character is clicked on, select them for movement and such.
	void OnMouseDown(){
		if (!isSelected && gameManager.curGameState == GameManager.GameState.AwaitingInput) {
			UpdateSelectedPlayer (true);
		}
	}

	public void UpdateSelectedPlayer(bool shouldBeSelected){
		if (shouldBeSelected) {
			// De-select old character if it exists
			if (gameManager.curSelectedPlayer) {
				gameManager.curSelectedPlayer.isSelected = false;
				gameManager.curSelectedPlayer.gameObject.GetComponent<Renderer> ().material.color = Color.gray;
			}

			// Select this character
			gameManager.curSelectedPlayer = this;
			isSelected = true;
			this.gameObject.GetComponent<Renderer> ().material.color = Color.green;

			// Highlight movement range
			movementOverlay = gameManager.map.HighlightMovementRange(curLocation, unitSpeed, nonWalkableTiles, passThroughOnlyTiles);

			// Deal with the camera
			isoCamera.removeCameraTarget();
			isoCamera.setCameraTarget (this.gameObject, true);

			// Update game state
			gameManager.curGameState = GameManager.GameState.InputStalling;
		} else {
			// De-select this charcter
			gameManager.curSelectedPlayer = null;
			isSelected = false;
			this.gameObject.GetComponent<Renderer> ().material.color = Color.gray;
			movementDestination = null;
			Destroy (movementOverlay);

			// Deal with the camera
			isoCamera.removeCameraTarget();
		}
	}
}