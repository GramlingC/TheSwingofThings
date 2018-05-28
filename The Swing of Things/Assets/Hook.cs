using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {



	public Color stringColor = Color.cyan;
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
		line.startColor = stringColor;
		linePositions = new List<Vector3>();
	}
	
	public void triggerHook(Vector3 from, Vector3 to)
	{
		if (grapplePoint == Vector3.zero)
		{
			castGrapple(from, to, Mathf.Infinity);
		}
		else
		{
			grapplePoint = Vector3.zero;
			line.enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
		
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


	void castGrapple(Vector3 from, Vector3 to, float distance)
	{
		RaycastHit hit;
		Debug.DrawRay(from,to, Color.cyan);
		if (Physics.Raycast(from, to, out hit, distance, mask))
		{
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
			rb.AddForce((grapplePoint-transform.position)*springForce * Time.deltaTime);
		}
		//*/
	}

	 void OnGUI(){
     GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), "");
  }
}
