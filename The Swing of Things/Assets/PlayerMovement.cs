using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Hook))]
public class PlayerMovement : MonoBehaviour {



	public Transform cameraTransform;

	public float moveMultiplier = 30f;
	public float boostForce = 50f;
	public float turnSpeed = 1f;
    public KeyCode boostKey, grappleKey;

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

		bool boost = Input.GetKeyDown(boostKey);

		inputVector = transform.forward*vinput *moveMultiplier;//new Vector3(hinput,0f,vinput).normalized * 10f;


		movePlayer(inputVector, boost);
        //get key down will release an existing grapple
        if (hook.lineIsActive)
        {
            if (Input.GetKeyUp(grappleKey))
            {
                hook.triggerHook(cameraTransform.position, cameraTransform.forward);
            }
        }
        else
        {
            //but if there is not yet an existing grapple, get key will try to grapple every frame until a connection is made
            if (Input.GetKey(grappleKey))
            {
                hook.triggerHook(cameraTransform.position, cameraTransform.forward);
            }
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
