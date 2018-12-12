using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float heightOffset = 0.5f;
	public float lerpSpeed = 0.1f;
	public float minSnapDistance = 0.05f;

	public bool ________________;

	public bool isSelected = false;
	public bool isMoving = false;
	public Vector3 movementDestination;
	public GameManager.MovementType curMovementType;

	GameManager gameManager;

	void Start(){
		// We need to find and remember the GameManager and MainCamera
		gameManager = (GameManager)(GameObject.Find("GameManager").GetComponent<GameManager>());
	}

	void Update(){
		// Deal with moving the character if selected and not moving
		if (isSelected && Input.GetKey(KeyCode.Mouse0) && !isMoving) {
			// Figure out what tile I just clicked on
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit) 
				&& hit.transform.gameObject.tag != "PlayerCharacter") {
				isMoving = true;

				// Now that I know what was it, figure out where I want to move to
				movementDestination = new Vector3 (hit.transform.position.x, hit.transform.position.y + heightOffset, hit.transform.position.z);
			}
		}

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

		if (Vector3.Distance (transform.position, destination) < minSnapDistance) {
			transform.position = destination;
			isMoving = false;
			UpdateSelectedChar (false);
		}
	}

	void UpdateSelectedChar(bool shouldBeSelected){
		if (shouldBeSelected) {
			// De-select old character if it exists
			if (gameManager.curSelectedCharacter) {
				gameManager.curSelectedCharacter.GetComponent<PlayerController> ().isSelected = false;
				gameManager.curSelectedCharacter.GetComponent<Renderer> ().material.color = Color.gray;
			}

			// Select this character
			gameManager.curSelectedCharacter = this.gameObject;
			isSelected = true;
			this.gameObject.GetComponent<Renderer> ().material.color = Color.green;
		} else {
			// De-select this charcter
			gameManager.curSelectedCharacter = null;
			isSelected = false;
			this.gameObject.GetComponent<Renderer> ().material.color = Color.gray;
		}
	}
}