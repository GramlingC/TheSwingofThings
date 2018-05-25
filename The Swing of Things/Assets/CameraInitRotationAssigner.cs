using UnityEngine;

public class CameraInitRotationAssigner : MonoBehaviour {
    public float verticalSensitivity;
    void Start(){
        transform.rotation = Quaternion.LookRotation((transform.parent.position - transform.position));
    }

    void Update()
    {
        transform.rotation = (Input.GetAxisRaw("Mouse Y") == 0) ? transform.rotation : (Input.GetAxisRaw("Mouse Y") > 0) ? Quaternion.LookRotation(Vector3.RotateTowards(transform.forward.normalized, Vector3.up, verticalSensitivity * Mathf.Deg2Rad, 0f)) : Quaternion.LookRotation(Vector3.RotateTowards(transform.forward.normalized, Vector3.down, verticalSensitivity * Mathf.Deg2Rad, 0f));
    }
}