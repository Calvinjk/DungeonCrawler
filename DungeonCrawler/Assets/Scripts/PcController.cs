﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PcController : MonoBehaviour {

	public float heightOffset = 0.5f;		// This number accounts for the difference in player model height so it sits nicely on the floor
	public float lerpSpeed = 0.1f;			// How fast the character will move between squares (0-1 only)
	public float minSnapDistance = 0.05f;	// How close the character must get to its destination before it snaps into position

	public bool ________________;

	public bool isSelected = false;			// True if this character is selected
	public bool isMoving = false;			// True if this character is moving
	public Vector3 movementDestination;
	public GameManager.MovementType curMovementType;

	GameManager gameManager;
	CameraController cameraScript;

	void Start(){
		// We need to find and remember the GameManager and CameraController
		gameManager = (GameManager)(GameObject.Find("GameManager").GetComponent<GameManager>());
		cameraScript = (CameraController)(GameObject.Find ("IsometricCameraTarget").GetComponent<CameraController> ());
	}

	void Update(){
		// Deal with moving the character if selected and not moving
		if (isSelected && !isMoving && Input.GetKey(KeyCode.Mouse0) 
			&& (gameManager.curGameState == GameManager.GameState.AwaitingInput)) {
			// Figure out what tile I just clicked on
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit) 
				&& hit.transform.gameObject.tag != "PlayerCharacter") {
				isMoving = true;
				gameManager.curGameState = GameManager.GameState.InputLocked;

				// Now that I know what was it, figure out where I want to move to
				movementDestination = new Vector3 (hit.transform.position.x, hit.transform.position.y + heightOffset, hit.transform.position.z);
			}
		}

		// Go there!
		if (isMoving) {
			MoveTo(movementDestination);
		}
	}

	// If this character is clicked on, select them for movement and such.
	void OnMouseDown(){
		if (!isSelected) {
			UpdateSelectedChar (true);
		}
	}

	void MoveTo(Vector3 destination){
		// TODO - Change based off of movement type?
		transform.position = Vector3.Lerp(transform.position, destination, lerpSpeed);

		// Move until you get within minSnapDistance and then snap to position and stop moving
		if (Vector3.Distance (transform.position, destination) < minSnapDistance) {
			transform.position = destination;
			isMoving = false;
			UpdateSelectedChar (false);
			gameManager.curGameState = GameManager.GameState.AwaitingInput;
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
		}
	}
}