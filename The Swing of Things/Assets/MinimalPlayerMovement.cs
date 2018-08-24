using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(GameObject))]
public class MinimalPlayerMovement : MonoBehaviour {
    [HideInInspector]
    public Vector3 velocity;
    public float movementSpeed;
    public float maxWalkingSpeed;
    public float friction;
    public GameObject debug;
	// Use this for initialization
	void Start () {
        velocity = new Vector3();
        friction = Mathf.Clamp(friction, 0, 1);
	}
	
	// Update is called once per frame
	void Update () {
        //apply loss of velocity due to friction
        if (velocity.magnitude < friction)
        {
            velocity = new Vector3();
        }
        else
        {
            velocity = velocity.normalized * (velocity.magnitude - friction);
        }
        //get movement input this frame
        Vector3 thisFramesInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //edit to be relative to default rotation, not current view
        thisFramesInput = transform.rotation * thisFramesInput;
        //rotation could have rotated a 2d plane input into y component, dont want walking input to allow one to move up or down, so setting that back to 0
        thisFramesInput.y = 0;
        //changing magnitude of input to match movementSpeed so that the entire vector represents movement from walking this frame
        thisFramesInput = thisFramesInput.normalized * movementSpeed;
        debug.transform.position = transform.position + thisFramesInput;

        //attempt to apply movementSpeed velocity change
        velocity += thisFramesInput;
        if (velocity.magnitude > maxWalkingSpeed)
        {
            //beyond max therefore set to max
            velocity = velocity.normalized * maxWalkingSpeed;
        }

        //apply positional change
        //gameObject.transform.Translate(Time.deltaTime * velocity);
        transform.position = transform.position + Time.deltaTime * velocity;
	}
}
