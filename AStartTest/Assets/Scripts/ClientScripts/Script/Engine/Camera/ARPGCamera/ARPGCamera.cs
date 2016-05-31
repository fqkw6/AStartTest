using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ETouch;

// ARPG模式摄像机跟随，参考暗黑2 火炬之光 POE
public class ARPGCamera : CameraBase {	
	void Start () {
		myTransform = transform;		
		Vector3 angles = myTransform.eulerAngles ; 
		// Set default y angle.
	    y = angles.y;		
	}

    public void AttachToPlayer(Transform obj)
    {
        target = obj;
        myTransform.position = target.position;	
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

            // 摄像机旋转
            if (Input.GetButton("Fire3")) { // 0 is left, 1 is right, 2 is middle mouse button.
                float h = Input.GetAxis("Mouse X"); // The horizontal movement of the mouse.						
                float v = Input.GetAxis("Mouse Y"); // The vertical movement of the mouse.
                if (h > 0 && h > Math.Abs(v)) {
                    myTransform.RotateAround(target.transform.position, new Vector3(0, 1, 0), camRotationSpeed * Time.deltaTime);
                    y = myTransform.eulerAngles.y;
                } else if (h < 0 && h < -Math.Abs(v)) {
                    myTransform.RotateAround(target.transform.position, new Vector3(0, 1, 0), -camRotationSpeed * Time.deltaTime);
                    y = myTransform.eulerAngles.y;
                } else if (v > 0 && v > Math.Abs(h)) {
                    camXAngle += camRotationSpeed * Time.deltaTime;
                    if (camXAngle > maxCameraAngle) {
                        camXAngle = maxCameraAngle;
                    }
                } else if (v < 0 && v < -Math.Abs(h)) {
                    camXAngle += -camRotationSpeed * Time.deltaTime;
                    if (camXAngle < minCameraAngle) {
                        camXAngle = minCameraAngle;
                    }
                }
            }
        }
    }
	
    // 使用LateUpdate防止出现穿帮
	void LateUpdate  () 
    {
		if(target == null) {
			return;
		}
		
		// Set camera angles.
		Quaternion rotation = Quaternion.Euler (camXAngle, y, 0); 	
	   	myTransform.rotation  = rotation ;
		
		// Position Camera.
		Vector3 trm = rotation * Vector3.forward * startingDistance + new Vector3(0, -1 * targetHeight, 0);
		Vector3 position = target.position  - trm; 
		myTransform.position = position ;

        CheckFadeObjects();
	}
}
