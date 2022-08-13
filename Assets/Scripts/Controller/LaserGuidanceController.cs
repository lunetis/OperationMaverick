using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaserGuidanceController : MonoBehaviour
{
    [HideInInspector]
    public LaserGuidedBomb lgb;
    RaycastHit hit;
    int layerMask;
    public float raycastDistance = 1000;

    bool enableGuidance;

    // UI
    public GameObject scanUI;
    public FollowTransformUI guidanceUI;
    GameObject guideObject;
    
    [Header("Line Properties")]
	[SerializeField]
    int lineWidth = 3;
    [SerializeField]
	Material lineMaterial;
    
    Vector3[] vertexPositions;

    public float dashedLineLength = 10;
    
    void Awake()
    {
        scanUI.SetActive(false);
    }

    void OnEnable()
    {
        scanUI?.SetActive(true);
    }

    void OnDisable()
    {
        if(scanUI == null)
            return;

        scanUI.SetActive(false);
    }

    public void ShowGuidanceUI()
    {
        guideObject = new GameObject();
        guidanceUI.gameObject.SetActive(true);
        guidanceUI.targetTransform = guideObject.transform;
    }

    public void HideGuidanceUI()
    {
        lgb = null;
        guidanceUI.gameObject.SetActive(false);
        Destroy(guideObject);
    }

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Ground");
        layerMask += 1 << LayerMask.NameToLayer("Object");
        enableGuidance = true;

        InputAction fireAction = GameManager.PlayerInput.actions.FindAction("Fire");

        fireAction.started += (InputAction.CallbackContext context) => { enableGuidance = true; };
        fireAction.canceled += (InputAction.CallbackContext context) => { enableGuidance = false; };

        vertexPositions = new Vector3[2];
        Camera.onPostRender += OnPostRenderCallback;
    }

    // Update is called once per frame
    void Update()
    {
        if(lgb == null || enableGuidance == false)
            return;
        
        Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, layerMask);

        if(hit.collider == null)
        {
            lgb.guidedPosition = transform.position + transform.forward * raycastDistance;
        }
        else
        {
            lgb.guidedPosition = hit.point;
        }

        guideObject.transform.position = lgb.guidedPosition;
    }


    // Draw Line between target and bomb
    void OnPostRenderCallback(Camera cam)
	{
        if(lgb == null)
            return;

		if (vertexPositions == null || vertexPositions.Length < 2)
			return;
        
        // Init
        vertexPositions[0] = guideObject.transform.position;
        vertexPositions[1] = lgb.gameObject.transform.position;
        Vector3 directionVector = vertexPositions[1] - vertexPositions[0];

        float distance = Vector3.Distance(vertexPositions[0], vertexPositions[1]);
        directionVector = directionVector.normalized * dashedLineLength;

		int end = vertexPositions.Length - 1;
        lineMaterial.SetPass(0);
 
        // Draw
        GL.Begin(GL.LINES);
        while(distance > 0)
        {
            distance -= dashedLineLength * 2;
            if(distance > 0)
            {
                vertexPositions[1] = vertexPositions[0] + directionVector;
            }
            else
            {
                vertexPositions[1] = lgb.gameObject.transform.position;
            }
            

            GL.Vertex(vertexPositions[0]);
            GL.Vertex(vertexPositions[1]);

            vertexPositions[0] += directionVector * 2;
        }

        /*
        // Please somebody solve the issue
        // when the object is on back of the main camera, WorldToViewportPoint doesn't return correct value (maybe)
		
		float nearClip = cam.nearClipPlane + 0.00001f;
		float thisWidth = 1f/Screen.width * lineWidth * 0.5f;
 
        if (lineWidth == 1)
		{
	        GL.Begin(GL.LINES);
	        for (int i = 0; i < end; ++i)
			{
	            GL.Vertex(vertexPositions[i]);
	            GL.Vertex(vertexPositions[i + 1]);
        	}
    	}
    	else
		{
	        GL.Begin(GL.QUADS);
	        for (int i = 0; i < end; ++i)
			{
                Vector3 linePoint = cam.WorldToViewportPoint(vertexPositions[i]);
                Vector3 nextlinePoint = cam.WorldToViewportPoint(vertexPositions[i + 1]);

	            Vector3 perpendicular = (new Vector3(nextlinePoint.y, linePoint.x, nearClip) -
	                                 new Vector3(linePoint.y, nextlinePoint.x, nearClip)).normalized * thisWidth;
	            Vector3 v1 = new Vector3(linePoint.x, linePoint.y, nearClip);
	            Vector3 v2 = new Vector3(nextlinePoint.x, nextlinePoint.y, nearClip);

	            GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
	            GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
        	}
    	}
        */
    	GL.End();
	}
}
