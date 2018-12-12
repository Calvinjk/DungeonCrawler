using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The GameManager tracks and stores gamedata other classes may need to access
public class GameManager : MonoBehaviour {
		
	public GameObject curSelectedCharacter = null;

	void Update(){
		// Deselct the currently selected character upon esc keypress
		if (Input.GetKeyDown(KeyCode.Escape) && curSelectedCharacter) {
			curSelectedCharacter.GetComponent<PlayerController>().isSelected = false;
			curSelectedCharacter = null;
		}
	}
}
