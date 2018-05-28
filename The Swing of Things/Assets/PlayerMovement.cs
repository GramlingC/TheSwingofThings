using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {



	public Transform cameraTransform;

	public float moveMultiplier = 30f;
	public float boostForce = 50f;
	public float turnSpeed = 1f;

	public LayerMask mask;

	private Hook hook;

	private Rigidbody rb;

	private Vector3 velocity;

	private Vector3 inputVector;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		if (GetComponent<Hook>() != null)
		hook = GetComponent<Hook>();

		if (cameraTransform == null)
			cameraTransform = GameObject.Find("Main Camera").transform;
	}
	
	// Update is called once per frame
	void Update () {
		float vinput = Input.GetAxisRaw("Vertical");
		float hinput = Input.GetAxisRaw("Horizontal");

		bool boost = Input.GetKeyDown(KeyCode.Space);

		inputVector = transform.forward*vinput *moveMultiplier;//new Vector3(hinput,0f,vinput).normalized * 10f;


		movePlayer(inputVector, boost);

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			hook.triggerHook(cameraTransform.position, cameraTransform.forward);
		}
	}

	void movePlayer(Vector3 input, bool boost)
	{
		rb.AddForce(input);
		if (boost)
		{
			rb.AddForce(transform.forward*boostForce,ForceMode.Impulse);
		}
	}
}
