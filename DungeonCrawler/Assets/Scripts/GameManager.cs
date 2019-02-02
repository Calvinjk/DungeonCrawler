using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The GameManager tracks and stores gamedata other classes may need to access
public class GameManager : MonoBehaviour {

	public enum GameState{ 
		InputLocked, 
		AwaitingInput, 
		InputStalling 
	};

	public float stallTime = 0.5f;
	public Material overlayMaterial;
	public GameObject samplePlayerPrefab;

	public bool _______________;

	public float curStallTimer = 0f;
	public GameState curGameState = GameState.AwaitingInput;
	public Map map;
	public Player curSelectedPlayer = null;
	public HashSet<Unit> unitList;

	public CameraController cameraScript;
	public DungeonMapGenerator generatorScript;

	void Start(){
		unitList = new HashSet<Unit> ();
	}

	public void GenerateNewMap(){
		if (unitList.Count != 0) {
			foreach (Unit unit in unitList) {
				Destroy (unit.gameObject);
			}
			unitList.Clear ();
		}

		curSelectedPlayer = null;

		if (map != null) { Destroy (map.gameObject); }

		cameraScript.transform.position = new Vector3 (5f, 0f, 5f);

		map = generatorScript.GenerateMap (50, 50);
	}

	public void SpawnPlayerUnit(GameObject spawnButton){
		int xLoc = Convert.ToInt32(spawnButton.transform.Find ("X").gameObject.GetComponent<InputField> ().text);
		int yLoc = Convert.ToInt32(spawnButton.transform.Find ("Y").gameObject.GetComponent<InputField> ().text);

		GameObject unit = Instantiate(samplePlayerPrefab, new Vector3 (xLoc, 0.5f, yLoc), Quaternion.identity) as GameObject;
		Player unitScript = unit.GetComponent<Player> ();
		unitScript.SetLocation (map.tileMap[xLoc, yLoc]);
		map.tileMap[xLoc, yLoc].curTileState = Tile.TileState.Ally;

		unitList.Add (unitScript);
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