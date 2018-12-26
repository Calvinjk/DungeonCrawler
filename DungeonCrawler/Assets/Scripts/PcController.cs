using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PcController : MonoBehaviour {

	public float heightOffset = 0.5f;		// This number accounts for the difference in player model height so it sits nicely on the floor
	public float lerpSpeed = 0.1f;			// How fast the character will move between squares. [0, 1] only.  
	public float minSnapDistance = 0.05f;	// How close the character must get to its destination before it snaps into position

	public bool ________________;

	public Tile curLocation;
	public bool isSelected = false;			// True if this character is selected
	public bool isMoving = false;			// True if this character is moving
	public Tile movementDestination;
	public GameManager.MovementType curMovementType;
	public Dictionary<Tile, int> pathDict = new Dictionary<Tile, int>();
	public List<Tile> movePath;

	GameManager gameManager;
	CameraController cameraScript;

	void Start(){
		// We need to find and remember the GameManager and CameraController
		gameManager = (GameManager)(GameObject.Find("GameManager").GetComponent<GameManager>());
		cameraScript = (CameraController)(GameObject.Find ("IsometricCameraTarget").GetComponent<CameraController> ());
	}

	void Update(){
		// I don't like that I am doing this, but gotta always know my location
		if (!isMoving){
			curLocation = gameManager.map.tileMap [(int)transform.position.x, (int)transform.position.z];
			curLocation.curTileState = Tile.TileState.Ally;
		}

		// Deal with moving the character if selected and not moving
		if (isSelected && !isMoving && Input.GetKey(KeyCode.Mouse0) 
			&& (gameManager.curGameState == GameManager.GameState.AwaitingInput)) {
			// Figure out what tile I just clicked on
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit) 
				&& hit.transform.gameObject.tag == "Tile") {

				// Now that I know what it was, figure out what tile I want to move to
				movementDestination = gameManager.map.tileMap[(int)hit.transform.position.x, (int)hit.transform.position.z];

				// Determine if that tile is valid to move to
				if (movementDestination.curTileState == Tile.TileState.Open) {
					isMoving = true;
					gameManager.curGameState = GameManager.GameState.InputLocked;

					// Reset the state of the tile I used to be on
					curLocation.curTileState = Tile.TileState.Open;

					// Determine the shortest path using the map.FindPath algorithm
					movePath = gameManager.map.FindPath (curLocation, movementDestination);
				} else {
					UpdateSelectedChar (false);
				}
			}
		}

		// Go there!
		if (isMoving) {
			MoveTo(movePath);
		}
	}

	// If this character is clicked on, select them for movement and such.
	void OnMouseDown(){
		if (!isSelected && gameManager.curGameState == GameManager.GameState.AwaitingInput) {
			UpdateSelectedChar (true);
		}
	}
		
	// Moves closer to the destinationTile 
	// True if it reached the destination and snapped to it
	// False if it is still in transit
	bool MoveTo(Tile destinationTile){
		Vector3 destinationPosition = new Vector3 (destinationTile.location.x, heightOffset, destinationTile.location.y);
		transform.position = Vector3.Lerp(transform.position, destinationPosition, lerpSpeed);

		// Move until you get within minSnapDistance and then snap to position and stop moving
		if (Vector3.Distance (transform.position, destinationPosition) < minSnapDistance) {
			transform.position = destinationPosition;
			return true;
		}

		return false;
	}

	// Checks the path and starts moving along it
	void MoveTo(List<Tile> path){
		// Move to the next tile in the lineup and see if it was reached this frame
		if (MoveTo(path[0])){
			// If we reached this tile, remove it and check if there is still a path to follow
			path.Remove (path [0]);
			if (path.Count == 0) { // We finished traversing the path
				isMoving = false;
				UpdateSelectedChar (false);
				gameManager.curGameState = GameManager.GameState.AwaitingInput;
			}
		}
	}

	public void UpdateSelectedChar(bool shouldBeSelected){
		if (shouldBeSelected) {
			// De-select old character if it exists
			if (gameManager.curSelectedCharacter) {
				gameManager.curSelectedCharacter.GetComponent<PcController> ().isSelected = false;
				gameManager.curSelectedCharacter.GetComponent<Renderer> ().material.color = Color.gray;
			}

			// Select this character
			gameManager.curSelectedCharacter = this.gameObject;
			isSelected = true;
			this.gameObject.GetComponent<Renderer> ().material.color = Color.green;

			// Deal with the camera
			cameraScript.removeCameraTarget();
			cameraScript.setCameraTarget (this.gameObject, true);

			// Update game state
			gameManager.curGameState = GameManager.GameState.InputStalling;
		} else {
			// De-select this charcter
			gameManager.curSelectedCharacter = null;
			isSelected = false;
			this.gameObject.GetComponent<Renderer> ().material.color = Color.gray;

			// Deal with the camera
			cameraScript.removeCameraTarget();
		}
	}
}