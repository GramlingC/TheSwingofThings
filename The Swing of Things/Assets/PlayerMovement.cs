using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {



	public Transform cameraTransform;

	public float moveMultiplier = 30f;
	public float boostForce = 50f;
	public float turnSpeed = 1f;

	public LayerMask mask;

	public float springConstant = 300f;

	public float springRatio = .5f;
	private Rigidbody rb;

	private Vector3 velocity;

	private Vector3 inputVector;

	private Vector3 grapplePoint = Vector3.zero;

	private List<Vector3> linePositions;

	private LineRenderer line;

	private float grappleDistance = 0f;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		line = GetComponentInChildren<LineRenderer>();
		linePositions = new List<Vector3>();
		if (cameraTransform == null)
			cameraTransform = GameObject.Find("Main Camera").transform;
	}
	
	// Update is called once per frame
	void Update () {
		float vinput = Input.GetAxis("Vertical");
		float hinput = Input.GetAxis("Horizontal");

		bool boost = Input.GetKeyDown(KeyCode.Space);

		inputVector = transform.forward*vinput *moveMultiplier;//new Vector3(hinput,0f,vinput).normalized * 10f;


		movePlayer(inputVector, boost);

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			if (grapplePoint == Vector3.zero)
			{
				Vector3 dir = cameraTransform.forward;
				castGrapple(cameraTransform.position,dir,Mathf.Infinity);
			}
			else
			{
				grapplePoint = Vector3.zero;
				line.enabled = false;
			}
		}
		
		if (line.enabled)
		{
			linePositions.Add(transform.position);
			line.positionCount = linePositions.Count;
			line.SetPositions(linePositions.ToArray());
			linePositions.Remove(transform.position);
		}
	}

	void updateLinePositions()
	{

		if (!line.enabled)
		{
			line.enabled = true;
			linePositions.Clear();
		}

		linePositions.Add(grapplePoint);
	}

	void movePlayer(Vector3 input, bool boost)
	{
		rb.AddForce(input);
		if (boost)
		{
			rb.AddForce(transform.forward*boostForce,ForceMode.Impulse);
		}
	}

	void castGrapple(Vector3 from, Vector3 to, float distance)
	{
		RaycastHit hit;
		if (Physics.Raycast(from, to, out hit, distance, mask))
		{
			Debug.DrawRay(transform.position,hit.point, Color.cyan);
			Debug.Log(hit.transform.gameObject.name);
			grapplePoint = hit.point;
			grappleDistance = hit.distance;
			updateLinePositions();
		}
	}

	void FixedUpdate()
	{
		if (grapplePoint != Vector3.zero)
		{
			applyGrapple();
		}
	}

	void applyGrapple()
	{
		Debug.DrawRay(transform.position,grapplePoint - transform.position, Color.cyan);

		castGrapple(transform.position,grapplePoint-transform.position,Vector3.Distance(transform.position,grapplePoint)-5f);
	/*
		else if (Vector3.Distance(transform.position,grapplePoint) >= grappleDistance)
		{
			Vector3 cross1 = Vector3.Cross(rb.velocity,transform.position-grapplePoint).normalized;
			Vector3 cross2 = Vector3.Cross(cross1,transform.position-grapplePoint).normalized;
			Debug.Log((transform.position-grapplePoint).normalized);
			Debug.Log(cross1);
			Debug.Log(cross2);
			Debug.Log(Vector3.Dot(cross2,rb.velocity.normalized));
			Debug.Log(rb.velocity.normalized);
			rb.velocity = cross2 * -rb.velocity.magnitude;
		}
	*/


		
		
		/*if (Vector3.Distance(transform.position,grapplePoint) >= (springRatio*grappleDistance)+grappleDistance)
		{
			Vector3 cross1 = Vector3.Cross(rb.velocity,transform.position-grapplePoint).normalized;
			Vector3 cross2 = Vector3.Cross(cross1,transform.position-grapplePoint).normalized;
			Vector3 tangentVelocity = cross2 * -rb.velocity.magnitude;
			rb.velocity = tangentVelocity;
			//float centripetalForce = tangentVelocity.magnitude*tangentVelocity.magnitude*rb.mass;
			//centripetalForce /= grappleDistance;

		}*/
		if (Vector3.Distance(transform.position,grapplePoint) >= grappleDistance)
		{
			float diff = Vector3.Distance(transform.position,grapplePoint) - grappleDistance;
			//transform.position = transform.position + diff * ((grapplePoint-transform.position).normalized);
			
			float springFunction = (diff)/(grappleDistance*springRatio);
			float springForce = springFunction*springFunction*springConstant;
			Debug.Log(springForce);
			rb.AddForce((grapplePoint-transform.position)*springForce * Time.deltaTime);
		}
		//*/
	}
}
