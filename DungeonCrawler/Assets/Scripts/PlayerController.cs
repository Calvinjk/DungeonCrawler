using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public enum MovementType{ Grid, Free };
	public MovementType curMovementType = MovementType.Grid;

	public float heightOffset = 0.5f;
	public float lerpSpeed = 0.1f;

	public bool ________________;

	public bool isSelected = false;
	public bool isMoving = false;
	public Vector3 movementDestination;

	GameManager gameManager;

	void Start(){
		// We need to find and remember the GameManager and MainCamera
		gameManager = (GameManager)(GameObject.Find("GameManager").GetComponent<GameManager>());
	}

	void Update(){
		// Deal with moving the character if selected
		if (isSelected && Input.GetKey(KeyCode.Mouse0)) {
			// Figure out what tile I just clicked on
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.gameObject.tag != "PlayerCharacter") {
				isMoving = true;

				movementDestination = new Vector3 (hit.transform.position.x, hit.transform.position.y + heightOffset, hit.transform.position.z);
			}
		}

		if (isMoving) {
			MoveTo(movementDestination);
		}
	}

	// If this character is clicked on, select them for movement and such.
	void OnMouseDown(){
		isSelected = true;

		// Make sure to update the GameManager and previously selected object!
		if (gameManager.curSelectedCharacter) { gameManager.curSelectedCharacter.GetComponent<PlayerController> ().isSelected = false; }
		gameManager.curSelectedCharacter = this.gameObject;
	}

	void MoveTo(Vector3 destination){
		// TODO - Change based off of movement type?
		transform.position = Vector3.Lerp(transform.position, destination, lerpSpeed);

		if (Vector3.Distance (transform.position, destination) < 0.1f) {
			transform.position = destination;
			isMoving = false;
		}
	}
}
