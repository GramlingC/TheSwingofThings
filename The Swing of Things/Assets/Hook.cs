using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {



	public Color stringColor = Color.cyan;
	public LayerMask mask;

	public float maxSpeed = 300f;

	public float springConstant = 100f;

	public float springRatio = .5f;

	public Camera camera;

    [HideInInspector]public bool lineIsActive = false;

    private Rigidbody rb;

	private Vector3 velocity;

	private Vector3 inputVector;

	private Vector3 grapplePoint = Vector3.zero;

	private List<Vector3> linePositions;

	private LineRenderer line;

	private float grappleDistance = 0f;

	//private Vector2 reticlePos;

    

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		line = GetComponentInChildren<LineRenderer>();
		line.startColor = stringColor;
		linePositions = new List<Vector3>();
	}
	//returned value represents whether or not the hook hit something
	public bool triggerHook(Vector3 from, Vector3 to)
	{
		if (grapplePoint == Vector3.zero)
		{
			return castGrapple(from, to, Mathf.Infinity);
		}
		else
		{
            Debug.Log("disconnecting line");
			grapplePoint = Vector3.zero;
            lineIsActive = false;
			line.enabled = false;
            return false;
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
	}

	void applyGrapple()
	{
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
     GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), "");
  }
}
