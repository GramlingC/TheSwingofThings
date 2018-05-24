using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {


	private Rigidbody rb;

	private Vector3 velocity;

	private Vector3 vchange;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		velocity = rb.velocity;
	}
	
	// Update is called once per frame
	void Update () {
		float vinput = Input.GetAxis("Vertical");
		float hinput = Input.GetAxis("Horizontal");

		vchange = new Vector3(hinput,0f,vinput) * 10f;

	}

	void FixedUpdate()
	{
		rb.AddForce(vchange * Time.deltaTime,ForceMode.VelocityChange);
	}
}
