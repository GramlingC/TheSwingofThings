using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnchorRotator : MonoBehaviour {
    public float horizontalSensitivity;
    public float verticalSensitivity;
    public float angleFromTrueTop;

    private float xAxis, yAxis;
	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        /*if(Input.GetKey(KeyCode.Escape)) {
          Cursor.lockState = CursorLockMode.None;
          Cursor.visible=true;
        }*/
        xAxis = Input.GetAxisRaw("Mouse X");
        yAxis = Input.GetAxisRaw("Mouse Y");
        Vector3 rot = transform.rotation.eulerAngles;
        rot.x -= verticalSensitivity*yAxis;
        rot.y += horizontalSensitivity*xAxis;
        rot.z = 0;
        
        Vector3 currentRot = Quaternion.Euler(rot.x, rot.y, rot.z) * new Vector3(0, 0, 1);
        //if change just applied to rot is deemed to be too close to true top or true bottom, undo that invalid change
        if ((Vector3.Angle(currentRot, new Vector3(0, 1, 0)) < angleFromTrueTop) || (Vector3.Angle(currentRot, new Vector3(0, -1, 0)) < angleFromTrueTop))
        {
            rot.x += verticalSensitivity * yAxis;
        }
        transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
    }
}
