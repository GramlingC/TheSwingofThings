using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnchorRotator : MonoBehaviour {
    public float horizontalSensitivity;
    public float verticalSensitivity;
    private Quaternion minStep = new Quaternion();
    private float xAxis, yAxis;
	// Use this for initialization
	void Start () {
        minStep.eulerAngles = new Vector3(0, horizontalSensitivity, 0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)) {
          Cursor.lockState = CursorLockMode.None;
          Cursor.visible=true;
        }
        xAxis = Input.GetAxisRaw("Mouse X");
        yAxis = Input.GetAxisRaw("Mouse Y");
        Vector3 rot = transform.rotation.eulerAngles;
        rot.x -= verticalSensitivity*yAxis;
        rot.y += horizontalSensitivity*xAxis;
        float minAngle = 180;
        float maxAngle = 180;
        if (rot.x > minAngle && rot.x < 90) rot.x = minAngle;
        if (rot.x > 360 - 90 && rot.x < 360 - maxAngle) rot.x = 360 - maxAngle;
        transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
	}
}
