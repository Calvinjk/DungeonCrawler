using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float lerpSpeed = 0.1f;
	public float minSnapDistance = 0.025f;

	public bool ________________;

	public bool hasVectorTarget = false;
	public bool hasObjectTarget = false;
	public bool followObject = false;

	public Vector3 vectorTarget;
	public GameObject objectTarget;

	public void setCameraTarget(Vector3 target){
		vectorTarget = target;
		hasVectorTarget = true;

		objectTarget = null;
		hasObjectTarget = false;
		followObject = false;
	}

	public void setCameraTarget(GameObject target, bool follow){
		objectTarget = target;
		hasObjectTarget = true;
		followObject = follow;

		vectorTarget = Vector3.zero;
		hasVectorTarget = false;
	}

	public void removeCameraTarget(){
		vectorTarget = Vector3.zero;
		objectTarget = null;
		followObject = false;
		hasVectorTarget = false;
		hasObjectTarget = false;
	}

	// Update is called once per frame
	void Update () {
		// Move towards target vector
		if (hasVectorTarget) {
			transform.position = Vector3.Lerp (transform.position, vectorTarget, lerpSpeed);

			// Snap into place if close enough to target position
			if (Vector3.Distance(transform.position, vectorTarget) < minSnapDistance){
				transform.position = vectorTarget;

				// No reason to keep this location saved if we have reached it already
				vectorTarget = Vector3.zero;
				hasVectorTarget = false;
			}
		}

		// Move towards target object
		if (hasObjectTarget){
			Vector3 targetPosition = objectTarget.transform.position;
			transform.position = Vector3.Lerp (transform.position, targetPosition, lerpSpeed);

			if (!followObject) {
				// Snap into place if close enough to target object
				if (Vector3.Distance (transform.position, targetPosition) < minSnapDistance) {
					transform.position = vectorTarget;

					// No reason to keep this location saved if we have reached it already
					objectTarget = null;
					hasObjectTarget = false;
				}
			}
		}
	}
}
