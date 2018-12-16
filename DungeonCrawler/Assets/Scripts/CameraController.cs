using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float freeCameraSpeed = .1f;
	public float lerpSpeed = 0.1f;
	public float minSnapDistance = 0.025f;

	public bool ________________;

	public enum State { Idle, VectorTarget, ObjectTarget };

	public State cameraState = State.Idle;
	public bool followTarget = false;

	public Vector3 vectorTarget;
	public GameObject objectTarget;

	public void setCameraTarget(Vector3 target){
		vectorTarget = target;
		cameraState = State.VectorTarget;

		objectTarget = null;
		followTarget = false;
	}

	public void setCameraTarget(GameObject target, bool follow){
		objectTarget = target;
		cameraState = State.ObjectTarget;
		followTarget = follow;

		vectorTarget = Vector3.zero;
	}

	public void removeCameraTarget(){
		vectorTarget = Vector3.zero;
		objectTarget = null;
		followTarget = false;
		cameraState = State.Idle;
	}

	// Update is called once per frame
	void Update () {
		switch (cameraState){
		// Move towards target vector
		case State.VectorTarget:	
			transform.position = Vector3.Lerp (transform.position, vectorTarget, lerpSpeed);

			// Snap into place if close enough to target position
			if (Vector3.Distance(transform.position, vectorTarget) < minSnapDistance){
				transform.position = vectorTarget;

				// No reason to keep this location saved if we have reached it already
				vectorTarget = Vector3.zero;
				cameraState = State.Idle;
			}
			break;
		// Move towards target object
		case State.ObjectTarget:
			Vector3 targetPosition = objectTarget.transform.position;
			transform.position = Vector3.Lerp (transform.position, targetPosition, lerpSpeed);

			if (!followTarget) {
				// Snap into place if close enough to target object
				if (Vector3.Distance (transform.position, targetPosition) < minSnapDistance) {
					transform.position = vectorTarget;

					// No reason to keep this location saved if we have reached it already
					objectTarget = null;
					cameraState = State.Idle;
				}
			}
			break;
		// Free target the camera
		case State.Idle:
			if (Input.GetKey (KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
				transform.position = new Vector3 (transform.position.x - freeCameraSpeed, transform.position.y, transform.position.z + freeCameraSpeed);
			}
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
				transform.position = new Vector3 (transform.position.x + freeCameraSpeed, transform.position.y, transform.position.z - freeCameraSpeed);
			}
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
				transform.position = new Vector3 (transform.position.x + freeCameraSpeed, transform.position.y, transform.position.z + freeCameraSpeed);
			}
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
				transform.position = new Vector3 (transform.position.x - freeCameraSpeed, transform.position.y, transform.position.z - freeCameraSpeed);
			}
			break;
		default:
			Debug.LogError ("Invalid CameraState");
			break;
		}
	}
}
