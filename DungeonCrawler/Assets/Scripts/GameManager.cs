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

	public bool _______________;

	public float curStallTimer = 0f;
	public GameState curGameState = GameState.AwaitingInput;
	public GameObject curSelectedCharacter = null;
	public CameraController cameraScript;


	void Update(){
		// Deselect the currently selected character upon esc keypress
		if (Input.GetKeyDown(KeyCode.Escape) && curSelectedCharacter) {
			curSelectedCharacter.GetComponent<PcController> ().UpdateSelectedChar (false);
			cameraScript.removeCameraTarget ();
			curGameState = GameState.AwaitingInput;
		}

		if (curGameState == GameState.InputStalling) {
			curStallTimer += Time.deltaTime;
			if (curStallTimer > stallTime) {
				curGameState = GameState.AwaitingInput;
				curStallTimer = 0f;
			}
		}
	}
}
