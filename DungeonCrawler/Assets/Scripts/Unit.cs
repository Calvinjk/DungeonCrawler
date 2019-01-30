using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

	////////// Gameplay variables //////////
	protected int unitSpeed = 5;				// How many tiles this unit can move in a single turn

	////////// Mechanics variables //////////
	protected float lerpSpeed = 0.1f;			// How fast the unit will move between squares
	protected float heightOffset = 0.5f;		// This number accounts for the difference in player model height so it sits nicely on a Tile
	protected float minSnapDistance = 0.05f;	// How close the character must get to its destination before it snaps into position

	////////// Code Variables //////////
	public enum UnitType {
		Ally,
		Enemy
	};	
	protected UnitType unitType;				// Describes what type of unit this is
	protected Tile curLocation;					// The current tile this unit is occupying
	protected bool isMoving = false;			// Describes if this character is moving or not

	////////// Dynamically linked variables //////////
	protected GameManager gameManager;
	protected CameraController isoCamera;



	//////////////////////// METHODS BELOW ////////////////////////

	/// <summary>
	/// Call this in order to set up connections to the GameManager and camera
	/// Not necessary to call if you do not need these connections
	/// </summary>
	protected void InitializeConnections() {
		gameManager = (GameManager)(GameObject.Find("GameManager").GetComponent<GameManager>());
		isoCamera = (CameraController)(GameObject.Find ("IsometricCameraTarget").GetComponent<CameraController> ());
	}

	/// <summary>
	/// Updates the states of your start and end tiles for movement.  
	/// Start tile becomes open, end tile gains the state of the unit type
	/// </summary>
	void UpdateMovementTiles(Tile start, Tile end){
		start.curTileState = Tile.TileState.Open;

		switch (unitType) {
		case UnitType.Ally:
			end.curTileState = Tile.TileState.Ally;
			break;
		case UnitType.Enemy:
			end.curTileState = Tile.TileState.Enemy;
			break;
		}
	} 

	/// <summary>
	/// This takes a destination tile and attempts to move to it
	/// Returns false if it cannot make it to the tile, true otherwise
	/// </summary>
	protected bool MoveToTile(Tile destination){
		// If the destination tile is not open, we cannot move to it.
		if (destination.curTileState != Tile.TileState.Open) { return false; }

		// First thing to do is find the path.  This requires InitializeConnections() to be called.
		List<Tile> path = gameManager.map.FindPath (curLocation, destination, unitSpeed);

		// Check if a path was even found
		if (path != null) { isMoving = true; } 
		else { return false; }

		// Once the path has been found, update the tile and gameManager states
		UpdateMovementTiles(curLocation, destination);
		gameManager.curGameState = GameManager.GameState.InputLocked;

		// Start that baby moving!
		StartCoroutine (Move (path));
	
		return true;
	}

	/// <summary>
	/// Helper coroutine for MoveToTile()
	/// This handles the actual moving of the Unit object
	/// </summary>
	IEnumerator Move(List<Tile> path){
		Tile finalDestination = path [path.Count - 1];

		while (path.Count > 0) {
			Vector3 destinationPosition = new Vector3 (path[0].location.x, heightOffset, path[0].location.y);

			// Big 'ol loop to get there!
			while (transform.position != destinationPosition) {
				// Move a little bit towards the destination
				transform.position = Vector3.Lerp (transform.position, destinationPosition, lerpSpeed);

				// Check if we are within minSnapDistance
				if (Vector3.Distance (transform.position, destinationPosition) < minSnapDistance) {
					transform.position = destinationPosition;
					path.Remove (path [0]);
				}

				// Pause execution of this function until next frame
				yield return null;
			}
				
		}

		// At this point, we have finished moving along the entire path.
		isMoving = false;
		curLocation = finalDestination;
		gameManager.curGameState = GameManager.GameState.AwaitingInput;
	}
}
