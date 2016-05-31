using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ETouch;

// 第三人称摄像机跟随（如鬼泣 战神 龙之谷）
public class ARPGFollowCamera : CameraBase
{
	public float rotationDamping = 3.0f; // How fast it should rotate to target angles.
	
	void Start () 
    {
		myTransform = transform;		
		myTransform.position = target.position;			
		
		if(target == null)
		{			
			Debug.LogWarning("No taget added, please add target Game object ");
		}
		
	}

    protected override void OnPinch(Gesture gesture)
    {
        if (Device.IsMobilePlatform()) {
            // Zoom Camera and keep the distance between [minDistance, maxDistance].
            if (Input.touchCount == 2 && pinchZoom) {
                Vector2 touch0 = Input.GetTouch(0).position;
                Vector2 touch1 = Input.GetTouch(1).position;

                float distance = Vector2.Distance(touch0, touch1);

                // Check prev distance and zoom in or out.
                if (prevDistance > 0) {
                    if (prevDistance - distance > 0) {
                        startingDistance += Time.deltaTime * zoomSpeed;
                        if (startingDistance > maxDistance)
                            startingDistance = maxDistance;
                    } else if (prevDistance - distance < 0) {
                        startingDistance -= Time.deltaTime * zoomSpeed;
                        if (startingDistance < minDistance)
                            startingDistance = minDistance;
                    }
                }

                prevDistance = distance;

            } else {
                prevDistance = 0;
            }
        } else {
            // Zoom Camera and keep the distance between [minDistance, maxDistance].
            float mw = Input.GetAxis("Mouse ScrollWheel");
            if (mw > 0) {
                startingDistance -= Time.deltaTime * zoomSpeed;
                if (startingDistance < minDistance)
                    startingDistance = minDistance;
            } else if (mw < 0) {
                startingDistance += Time.deltaTime * zoomSpeed;
                if (startingDistance > maxDistance)
                    startingDistance = maxDistance;
            }

            // Rotate Camera around character.
            if (Input.GetButton("Fire3")) { // 0 is left, 1 is right, 2 is middle mouse button.						
                float v = Input.GetAxis("Mouse Y"); // The vertical movement of the mouse.
                if (v > 0) {
                    camXAngle += camRotationSpeed * Time.deltaTime;
                    if (camXAngle > maxCameraAngle) {
                        camXAngle = maxCameraAngle;
                    }
                } else if (v < 0) {
                    camXAngle += -camRotationSpeed * Time.deltaTime;
                    if (camXAngle < minCameraAngle) {
                        camXAngle = minCameraAngle;
                    }
                }
            }
        }
    }

    void LateUpdate  ()
    {
		
		if(target == null)
		{			
			return;
		}
		
		// Calculate the current rotation angles
		float wantedRotationAngle = target.eulerAngles.y;			
		float currentRotationAngle = myTransform.eulerAngles.y;		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);	
		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (camXAngle, currentRotationAngle, 0);
		
		// Position Camera.
		myTransform.position = target.position;
		myTransform.position -= currentRotation * Vector3.forward * startingDistance + new Vector3(0, -1 * targetHeight, 0);
		
		Vector3 targetToLookAt = target.position;
		targetToLookAt.y += targetHeight;
		myTransform.LookAt (targetToLookAt);

        CheckFadeObjects();
	}
}
