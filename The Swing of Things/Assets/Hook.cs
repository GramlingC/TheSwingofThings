﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {



	public Color stringColor = Color.cyan;
	public LayerMask mask;

	public float maxSpeed = 300f;

	public float maxHookDistance = 300f;

	public float springConstant = 100f;

	public float springRatio = .5f;

	public Camera camera;

	public GameObject pointObject;

    [HideInInspector]public bool lineIsActive = false;

    private Rigidbody rb;

	private Vector3 velocity;

	private Vector3 inputVector;

	private Vector3 grapplePoint = Vector3.zero;

	private List<Vector3> linePositions;

	private List<GameObject> linePoints;

	private LineRenderer line;

	private float grappleDistance = 0f;

	private bool hookPossible;

	private GameObject lastTarget;

	private Vector3 lastTargetPosition;

    

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		line = GetComponentInChildren<LineRenderer>();
		line.startColor = stringColor;
		linePositions = new List<Vector3>();
		linePoints = new List<GameObject>();
	}
	//returned value represents whether or not the hook hit something
	public bool triggerHook(Vector3 from, Vector3 to)
	{
		if (grapplePoint == Vector3.zero)
		{
			return castGrapple(from, to, maxHookDistance);
		}
		else
		{
			grapplePoint = Vector3.zero;
            lineIsActive = false;
			line.enabled = false;
			linePoints.Clear();
            return false;
		}
	}

	public void getReticle(Vector3 from, Vector3 to)
	{
		RaycastHit hit;
		if (Physics.Raycast(from, to, out hit, maxHookDistance, mask))
		{
			hookPossible = true;
		}
		else
		{
			hookPossible = false;
		}
	}

	// Update is called once per frame
	void Update () {
		
		if (line.enabled)
		{
			linePositions.Clear();
			for (int i = 0; i < linePoints.Count; i++)
			{
				linePositions.Add(linePoints[i].transform.position);
			}
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
            lineIsActive = true;
			line.enabled = true;
			linePositions.Clear();
		}

		linePositions.Add(grapplePoint);
	}

    //bool represents raycast hit something or not
	bool castGrapple(Vector3 from, Vector3 to, float distance)
	{
		RaycastHit hit;
		Debug.DrawRay(from,to, Color.cyan);
		if (Physics.Raycast(from, to, out hit, distance, mask))
		{
			GameObject point = Instantiate(pointObject) as GameObject;
			point.transform.position = hit.point;
			point.transform.parent = hit.transform;
			if (!line.enabled)
				linePoints.Clear();
			linePoints.Add(point);

			lastTarget = hit.transform.gameObject;
			lastTargetPosition = lastTarget.transform.position;
			grapplePoint = hit.point;
			grappleDistance = hit.distance;

            //grapple hit! so call the setup fxn to mark the line as active and to start the lineRenderer showing the line.
			updateLinePositions();
            return true;
		}
        return false;

	}

	void FixedUpdate()
	{
		if (grapplePoint != Vector3.zero)
		{
			applyGrapple();
		}

		if (rb.velocity.magnitude > 50f)
		{
			float desiredFOV = 60f + 20f * (rb.velocity.magnitude - 50f) / 100f;
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView,desiredFOV,.25f);
		}
		else
		{
			camera.fieldOfView = 60f;
		}

		if (rb.velocity.magnitude > 300f)
		{
			rb.velocity = Vector3.ClampMagnitude(rb.velocity,300f); 
		}

		if (transform.position.y > 200f)
		{
			rb.drag = 0.1f * (transform.position.y - 200f) / 200f;
		}
		else
		{
			rb.drag = 0f;
		}
	}

	void applyGrapple()
	{
		grapplePoint = linePoints[linePoints.Count-1].transform.position;
		/*if (lastTarget.transform.position != lastTargetPosition)
		{
			Vector3 delta =  lastTarget.transform.position - lastTargetPosition;
			lastTargetPosition = lastTarget.transform.position;
			grapplePoint += delta;
			linePositions.Clear();
			linePositions.Add(grapplePoint); 
		}*/
        //check for and update grapplePoint if the line hits something new and closer while swinging
		castGrapple(transform.position,grapplePoint-transform.position,Vector3.Distance(transform.position,grapplePoint)-5f);

		float distanceMultiplier = springRatio + 1f;

		Vector3 springPull = 2f * (grapplePoint-transform.position).normalized + .5f * (grapplePoint-transform.position);
		 
		if (distanceMultiplier * Vector3.Distance(transform.position,grapplePoint) >= grappleDistance)
		{
			float diff = distanceMultiplier * Vector3.Distance(transform.position,grapplePoint) -grappleDistance;
			//transform.position = transform.position + diff * ((grapplePoint-transform.position).normalized);
			
			float springFunction = (diff)/((grappleDistance)*springRatio);
			float springForce = springFunction*springFunction;

			springPull *= springForce;
		}
		else if (Vector3.Distance(transform.position,grapplePoint) > 5f)
		{
			grappleDistance = distanceMultiplier * Vector3.Distance(transform.position,grapplePoint);
		}
		
		rb.AddForce(springPull * springConstant * Time.deltaTime);
	}

	 void OnGUI(){
		 //if (hookPossible)
    		// GUI.Box(new Rect(reticlePos.x,reticlePos.y, 10, 10), "");
		GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), "");
  }
}
